/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Core;
using System.IO;
using Mkamo.Memopad.Properties;
using Mkamo.Memopad.Core;
using System.Runtime.Serialization.Formatters.Binary;
using Mkamo.Common.Externalize;
using Mkamo.Container.Core;
using Mkamo.Control.Progress;
using System.ComponentModel;
using System.Windows.Forms;
using Mkamo.Common.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using Mkamo.Common.Externalize.Internal;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class MemoSerializeUtil {
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static IEnumerable<MemoInfo> RecoverMemoInfosFromMemento(Action<int> reportProgress) {
            Contract.Requires(reportProgress != null);

            var ret = new List<MemoInfo>();

            /// mementoの取得
            var mementoIds = MemopadApplication.Instance.MemoAccessor.GetMementoIds();
            var len = mementoIds.Count();
            var i = 0;
            foreach (var mementoId in mementoIds) {
                var canvas = new EditorCanvas();

                try {
                    LoadEditor(canvas, mementoId);

                    var memo = canvas.EditorContent as Memo;

                    if (memo != null) {
                        var title = memo.Title;
                        var memoId = MemopadApplication.Instance.Container.GetId(memo);

                        var info = new MemoInfo();
                        info.Title = title;
                        info.MemoId = memoId;
                        info.MementoId = mementoId;

                        ret.Add(info);
                    }

                } catch (Exception e) {
                    Logger.Warn("Restore failed mementoId=" + mementoId, e);

                } finally {
                    canvas.Dispose();
                }

                ++i;
                if (reportProgress != null) {
                    reportProgress(i * 100 / len);
                }
            }

            reportProgress(100);

            return ret;
        }

    
        internal static void LoadEditor(EditorCanvas canvas, string id) {
            var bytes = MemopadApplication.Instance.MemoAccessor.LoadMemento(id);

            //using (var stream = new MemoryStream(bytes))
            //using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max)) {

            //    var knownTypes = new [] {
            //        typeof(Memo),
            //        typeof(Dictionary<string, object>),
            //        typeof(List<IMemento>),
            //        typeof(System.Drawing.Rectangle),
            //        typeof(Mkamo.Common.Forms.Descriptions.FontDescription),
            //    };

            //    var ser = new DataContractSerializer(typeof(Memento), knownTypes);
            //    var mem = (IMemento) ser.ReadObject(reader);
            //    canvas.LoadContent(mem, new MemoModelSerializer());
            //}
                
            using (var stream = new MemoryStream(bytes)) {
                var formatter = new BinaryFormatter();
                //formatter.Binder = new DelegatingSerializationBinder(
                //    (assemName, typeName) => {
                //        typeName = typeName.Replace("PublicKeyToken=null", "PublicKeyToken=89184b2d63ba68a6");
                //        var name = new AssemblyName(assemName);
                //        var assem = default(Assembly);
                //        try {
                //            assem = Assembly.Load(name.Name);
                //        } catch {
                //            assem = Assembly.Load(name);
                //        }
                //        return assem.GetType(typeName);
                //    }
                //);
                var mem = formatter.Deserialize(stream) as IMemento;

                //var serializer = new NetDataContractSerializer();
                //var mem = serializer.Deserialize(stream) as IMemento;

                canvas.LoadContent(mem, new MemoModelSerializer());
            }
        }


        internal static void SaveEditor(string id, EditorCanvas canvas) {
            if (canvas.FocusManager.IsEditorFocused) {
                canvas.FocusManager.FocusedEditor.RequestFocusCommit(true);
            }
            var mem = canvas.SaveContent(new MemoModelSerializer());

            //using (var stream = new MemoryStream())
            //using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream)) {

            //    var knownTypes = new [] {
            //        typeof(Memo),
            //        typeof(Dictionary<string, object>),
            //        typeof(List<IMemento>),
            //        typeof(System.Drawing.Rectangle),
            //        typeof(Mkamo.Common.Forms.Descriptions.FontDescription),
            //    };

            //    var ser = new DataContractSerializer(typeof(Memento), knownTypes, int.MaxValue, false, true, null);
            //    ser.WriteObject(writer, mem);

            //    var acc = MemopadApplication.Instance.MemoAccessor;
            //    if (acc.IsMementoExists(id)) {
            //        acc.UpdateMemento(id, stream.GetBuffer());
            //    } else {
            //        acc.InsertMemento(id, stream.GetBuffer());
            //    }
                
            //}

            using (var stream = new MemoryStream()) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, mem);

                //var serializer = new NetDataContractSerializer();
                //serializer.Serialize(stream, mem);

                var acc = MemopadApplication.Instance.MemoAccessor;
                if (acc.IsMementoExists(id)) {
                    acc.UpdateMemento(id, stream.GetBuffer());
                } else {
                    acc.InsertMemento(id, stream.GetBuffer());
                }
            }
        }

    }

}
