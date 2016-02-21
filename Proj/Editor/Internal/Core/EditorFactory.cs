/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Common.Externalize;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.DataType;
using System.Windows.Forms;
using Mkamo.Figure.Core;
using Mkamo.Common.Structure;


namespace Mkamo.Editor.Internal.Core {
    using Editor = Mkamo.Editor.Internal.Editors.Editor;
    //using TransferContentTree = Tree<Tuple<IModelDescriptor, IMemento, IMemento>>;
    //using TransferContentTreeNode = TreeNode<Tuple<IModelDescriptor, IMemento, IMemento>>;
    //using TransferContent = Tuple<IModelDescriptor, IMemento, IMemento>;

    using TransferContentTreeNode = TreeNode<TransferContent>;
    using Mkamo.Editor.Commands;

    [Serializable]
    internal class TransferContentTree: Tree<TransferContent> {
        public TransferContentTree(TransferContentTreeNode root): base(root) {
        }
    }

    [Serializable]
    internal struct TransferContent {
        public string Id;
        public IModelDescriptor ModelDescriptor;
        public IMemento ModelMemento;
        public IMemento FigureMemento;
        public TransferInitializer Initializer;
        public object TransferInitArgs;

        public TransferContent(
            string id,
            IModelDescriptor modelDescriptor,
            IMemento modelMemento,
            IMemento figureMemento,
            TransferInitializer initializer,
            object transferInitArgs
        ) {
            Id = id;
            ModelDescriptor = modelDescriptor;
            ModelMemento = modelMemento;
            FigureMemento = figureMemento;
            Initializer = initializer;
            TransferInitArgs = transferInitArgs;
        }
    }

    [Serializable]
    internal struct TransferConnection {
        public string ConnectionId;
        public string SourceId;
        public string TargetId;

        public TransferConnection(
            string connectionId,
            string sourceId,
            string targetId
        ) {
            ConnectionId = connectionId;
            SourceId = sourceId;
            TargetId = targetId;
        }
    }

    internal static class EditorFactory {
        // ========================================
        // method
        // ========================================
        /// <summary>
        /// modelのためのEditorを生成する．
        /// 生成するEditorはController，Figure，Modelがnullでないことが保証される．
        /// </summary>
        public static Editor CreateEditor(object model, IControllerFactory controllerFactory) {
            Contract.Requires(model != null);

            var editor = new Editor();

            var controller = controllerFactory.CreateController(model);
            Contract.Ensures(controller != null);

            var figure = controller.CreateFigure(model);
            Contract.Ensures(figure != null);

            editor.Model = model;
            editor._Figure = figure;
            editor._Controller = controller;

            controller.ConfigureEditor(editor);

            return editor;
        }

        /// <summary>
        /// modelとfigureを使ってEditorを生成する．
        /// </summary>
        public static Editor CreateEditorWithFigure(object model, IFigure figure, IControllerFactory controllerFactory) {
            Contract.Requires(model != null);
            Contract.Requires(figure != null);

            var editor = new Editor();

            var controller = controllerFactory.CreateController(model);
            Contract.Ensures(controller != null);

            editor.Model = model;
            editor._Figure = figure;
            editor._Controller = controller;

            controller.ConfigureEditor(editor);

            return editor;
        }

    
        /// <summary>
        /// prototypeのFigureを複製してmodelCloneをModelとする複製を返す．
        /// 子孫も含めて複製する．
        /// </summary>
        //public static IEditor CloneEditorStructure(IEditor prototype, object modelClone) {
        //    /// 自分のclone作成
        //    var clone = CloneEditorOnly(prototype, modelClone);

        //    /// 子のclone作成
        //    var container = clone.Controller as IContainerController;
        //    if (container != null) {
        //        var modelCloneChildren = container.Children;
        //        Ensure.State(
        //            prototype.Children.Count() == container.ChildCount,
        //            "Controller.Clone() returns invalid model clone"
        //        );

        //        var i = 0;
        //        foreach (var modelCloneChild in modelCloneChildren) {
        //            var childClone = CloneEditorStructure(prototype.Children.ElementAt(i), modelCloneChild);
        //            clone.AddChildEditor(childClone);
        //            ++i;
        //        }
        //    }
        //    return clone;
        //}

        /// <summary>
        /// クリップボードやDnDのためのDataObjectを取得する．
        /// </summary>
        public static IDataObject CreateDataObject(IEnumerable<IEditor> editors) {
            Contract.Requires(editors != null);

            var trees = new List<TransferContentTree>();
            var editorToId= new Dictionary<IEditor, string>();
            var connections = new List<TransferConnection>();

            var externalizer = new Externalizer();
            foreach (var editor in editors) {
                var root = CreateTransferContentNode(editor, externalizer);
                var tree = new TransferContentTree(root);
                trees.Add(tree);
                editorToId.Add(editor, root.Value.Id);
            }

            /// editor間の接続情報
            foreach (var editor in editors) {
                if (editor.IsConnection) {
                    if (!editorToId.ContainsKey(editor)) {
                        break;
                    }

                    var edge = editor.Figure as IEdge;
                    if (edge != null) {
                        var conn = new TransferConnection();
                        conn.ConnectionId = editorToId[editor];

                        if (edge.Source != null) {
                            var srcEditor = edge.Source.GetEditor();
                            if (srcEditor != null && editorToId.ContainsKey(srcEditor)) {
                                conn.SourceId = editorToId[srcEditor];
                            }
                        }
                        if (edge.Target != null) {
                            var tgtEditor = edge.Target.GetEditor();
                            if (tgtEditor != null && editorToId.ContainsKey(tgtEditor)) {
                                conn.TargetId = editorToId[tgtEditor];
                            }
                        }

                        connections.Add(conn);
                    }
                }
            }

            var descs = new List<IModelDescriptor>();
            foreach (var content in trees) {
                descs.Add(content.Root.Value.ModelDescriptor);
            }

            var descFormat = EditorConsts.DataDescriptorFormat;
            var treeFormat = EditorConsts.DataEditorFormat;
            var connFormat = EditorConsts.DataConnectionFormat;

            var ret = new DataObject();
            ret.SetData(descFormat.Name, descs);
            ret.SetData(treeFormat.Name, trees);
            ret.SetData(connFormat.Name, connections);

            return ret;
        }

        public static bool CanRestoreDataObject(IDataObject dataObject) {
            var editorFormat = EditorConsts.DataEditorFormat;
            return dataObject.GetDataPresent(editorFormat.Name);
        }

        /// <summary>
        /// dataObjectに格納されたデータを復元したeditorのリストを返す．
        /// </summary>
        public static IEnumerable<IEditor> RestoreDataObject(IDataObject dataObject, IControllerFactory factory) {
            Contract.Requires(dataObject != null);

            var ret = new List<IEditor>();

            var editorFormat = EditorConsts.DataEditorFormat;
            if (!dataObject.GetDataPresent(editorFormat.Name)) {
                return ret;
            }

            var idToEditor = new Dictionary<string, IEditor>();
            var trees = dataObject.GetData(editorFormat.Name) as List<TransferContentTree>;
            if (trees != null) {
                var externalizer = new Externalizer();

                foreach (var tree in trees) {
                    var editor = RestoreEditorStructure(tree.Root, externalizer, factory);
                    ret.Add(editor);
                    idToEditor.Add(tree.Root.Value.Id, editor);
                }

            }

            /// editor間の接続
            var connFormat = EditorConsts.DataConnectionFormat;
            if (!dataObject.GetDataPresent(editorFormat.Name)) {
                return ret;
            }

            var conns = dataObject.GetData(connFormat.Name) as List<TransferConnection>;
            if (conns != null) {
                foreach (var conn in conns) {
                    if (idToEditor.ContainsKey(conn.ConnectionId)) {
                        var connEditor = idToEditor[conn.ConnectionId];
                        var connFig = connEditor.Figure as IEdge;
                        if (connEditor != null && connFig != null) {
                            if (!string.IsNullOrEmpty(conn.SourceId) && idToEditor.ContainsKey(conn.SourceId)) {
                                var srcEditor = idToEditor[conn.SourceId];
                                if (srcEditor != null) {
                                    /// command executorは通さない
                                    var cmd = new ConnectCommand(
                                        connEditor,
                                        connFig.SourceAnchor,
                                        srcEditor,
                                        connFig.SourcePoint
                                    );
                                    cmd.Execute();
                                }
                            }
                            if (!string.IsNullOrEmpty(conn.TargetId) && idToEditor.ContainsKey(conn.TargetId)) {
                                var tgtEditor = idToEditor[conn.TargetId];
                                if (tgtEditor != null) {
                                    /// command executorは通さない
                                    var cmd = new ConnectCommand(
                                        connEditor,
                                        connFig.TargetAnchor,
                                        tgtEditor,
                                        connFig.TargetPoint
                                    );
                                    cmd.Execute();
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// dataObjectに格納されたデータをtargetに復元できるかを返す．
        /// </summary>
        public static bool CanRestoreDataObject(IDataObject dataObject, IEditor target) {
            Contract.Requires(dataObject != null);
            Contract.Requires(target != null && target.IsContainer);

            var descFormat = EditorConsts.DataDescriptorFormat;
            if (!dataObject.GetDataPresent(descFormat.Name)) {
                return false;
            }

            var data = dataObject.GetData(descFormat.Name) as List<IModelDescriptor>;
            if (data == null) {
                return false;
            }

            var container = target.Controller as IContainerController;
            foreach (var desc in data) {
                if (!container.CanContainChild(desc)) {
                    return false;
                }
            }

            return true;
        }

        // ------------------------------
        // private
        // ------------------------------
        /// <summary>
        /// prototypeのFigureを複製してmodelCloneをModelとする複製を返す．
        /// 子孫は複製しない．
        /// </summary>
        //private static Editor CloneEditorOnly(IEditor prototype, object modelClone) {
        //    var clone = new Editor();

        //    var ctrlClone = prototype.Site.ControllerFactory.CreateController(modelClone);

        //    var figClone = prototype.Figure.CloneFigureOnly();
        //    foreach (var childfig in prototype.Figure.Children) {
        //        /// editor parts figure (= editor figureでないfigure)のみ追加
        //        if (childfig.GetRole() != EditorConsts.EditorFigureFigureRole) {
        //            figClone.Children.Add(childfig.CloneFigure(null));
        //        }
        //    }

        //    clone.Model = modelClone;
        //    clone._Figure = figClone;
        //    clone._Controller = ctrlClone;
        //    ctrlClone.ConfigureEditor(clone);

        //    return clone;
        //}

        private static TransferContentTreeNode CreateTransferContentNode(IEditor editor, Externalizer externalizer) {
            var content = CreateTransferContentOnly(editor, externalizer);
            var ret = new TransferContentTreeNode(content);

            foreach (var child in editor.Children) {
                ret.Children.Add(CreateTransferContentNode(child, externalizer));
            }

            return ret;
        }

        /// <summary>
        /// modelとfigureのmementoを格納したtupleを返す．
        /// 子孫は格納しない．
        /// </summary>
        private static TransferContent CreateTransferContentOnly(IEditor editor, Externalizer externalizer) {
            var modelMem = editor.Controller.GetModelMemento();

            var figClone = editor.Figure.CloneFigureOnly();
            foreach (var childfig in editor.Figure.Children) {
                /// editor parts figure (= editor figureでないfigure)のみ追加
                if (childfig.GetRole() != EditorConsts.EditorFigureFigureRole) {
                    var childClone = childfig.CloneFigure(null);
                    figClone.Children.Add(childClone);
                    var constraint = editor.Figure.GetLayoutConstraint(childfig);
                    if (constraint != null) {
                        figClone.SetLayoutConstraint(childClone, constraint);
                    }
                }
            }
            var figMem = externalizer.Save(figClone);

            var id = Guid.NewGuid().ToString();
            return new TransferContent(
                id,
                editor.Controller.ModelDescriptor,
                modelMem,
                figMem,
                editor.Controller.GetTransferInitializer(),
                editor.Controller.GetTranserInitArgs()
            );
        }

        private static IEditor RestoreEditorStructure(
            TransferContentTreeNode content, Externalizer externalizer, IControllerFactory factory
        ) {
            var editor = RestoreEditorOnly(content, externalizer, factory);
            var container = editor.Controller as IContainerController;
            if (container == null) {
                return editor;
            }

            var tmpParent = default(object);
            if (externalizer.ExtendedData.ContainsKey(EditorConsts.RestoreEditorStructureParentModel)) {
                tmpParent = externalizer.ExtendedData[EditorConsts.RestoreEditorStructureParentModel];
            }
            externalizer.ExtendedData[EditorConsts.RestoreEditorStructureParentModel] = editor.Model;
            foreach (var childContent in content.Children) {
                var childEditor = RestoreEditorStructure(childContent, externalizer, factory);
                if (container.CanContainChild(childEditor.Controller.ModelDescriptor)) {
                    editor.AddChildEditor(childEditor);
                    container.InsertChild(childEditor.Model, container.ChildCount);
                }
            }
            if (tmpParent != null) {
                externalizer.ExtendedData[EditorConsts.RestoreEditorStructureParentModel] = tmpParent;
            }

            return editor;
        }

        private static IEditor RestoreEditorOnly(
            TransferContentTreeNode content, Externalizer externalizer, IControllerFactory factory
        ) {
            /// var type = content.Value.Item1; // restoreには使わない
            var modelMem = content.Value.ModelMemento;
            var figMem = content.Value.FigureMemento;

            var model = externalizer.Load(modelMem);
            var fig = externalizer.Load(figMem) as IFigure;

            var ret = CreateEditorWithFigure(model, fig, factory);
            if (content.Value.Initializer != null) {
                content.Value.Initializer(ret, content.Value.TransferInitArgs);
            }
            return ret;
        }
    }
}
