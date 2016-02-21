/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Container.Core;
using Mkamo.Common.IO;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Properties;
using System.Drawing;
using Mkamo.Model.Utils;
using Mkamo.Memopad.Internal.Forms;
using System.IO;
using log4net.Config;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Crypto;
using System.Reflection;
using System.Data.SqlServerCe;
using System.Diagnostics;
using Mkamo.Common.Win32.Psapi;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.Memopad.Core {
    public class MemopadAppContext: ApplicationContext {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private SqlCeConnection _memoConnection;
        private SqlCeConnection _exDataConnection;

        private MemopadApplication _facade;

#if !DEBUG
        private MemopadServer _server;
#endif

        // ========================================
        // constructor
        // ========================================
        public MemopadAppContext() {
            try {
                PathUtil.EnsureDirectoryExists(MemopadConsts.EmbeddedRoot);
                PathUtil.EnsureDirectoryExists(MemopadConsts.EmbeddedFileRoot);
                PathUtil.EnsureDirectoryExists(MemopadConsts.EmbeddedImageRoot);

                PathUtil.EnsureDirectoryExists(MemopadConsts.AutoFileImportDirPath);
                PathUtil.EnsureDirectoryExists(MemopadConsts.MyDocumentsRootPath);

            } catch (Exception ex) {
                MessageBox.Show(
                    "Confidanteが使用するフォルダを作成できません。" + Environment.NewLine +
                    ex.Message,
                    "起動エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                
                return;
            }

            FileImageDescription.RootPath = MemopadConsts.EmbeddedImageRoot;

            InitConnections();
            var store = ContainerFactory.CreateXmlSqlServerStore(_memoConnection, _exDataConnection);

            ContainerFactory.CreateContainer(
                store,
                MemopadConsts.ProxyAssemblyName,
                MemopadConsts.ProxyAssemblyFilePath
            );

            _facade = MemopadApplication.Instance;
            _facade.Init(this);
            if (_facade.IllegalSettings) {
                throw new ArgumentException("Illegal settings");
            }
            if (_facade.NeedRecoverMemoInfos) {
                /// Application.Restart()される
                _facade.RecoverMemoInfos();
            }

            //Application.ApplicationExit += HandleApplicationExit;
            
            _facade.LoadFusenForms();
            var isMin = _facade.WindowSettings.MinimizeToTaskTray && _facade.WindowSettings.MinimizeOnStartUp;
            if (isMin) {
                using (var proc = Process.GetCurrentProcess()) {
                    PsapiPI.EmptyWorkingSet(proc.Handle);
                }
            } else {
                _facade.ShowMainForm();
            }

#if !DEBUG
            _server = new MemopadServer();
            _server.Listen();
#endif
        }

        private void InitConnections() {
            var memoConnStr = string.Format(
                MemopadConsts.ConnectionString,
                MemopadConsts.MemoSdfFilePath
            );
            var exDataConnStr = string.Format(
                MemopadConsts.ConnectionString,
                MemopadConsts.ExtendedDataSdfFilePath
            );

            if (!File.Exists(MemopadConsts.MemoSdfFilePath)) {
                var engine = new SqlCeEngine(memoConnStr);
                engine.CreateDatabase();
            }
            if (!File.Exists(MemopadConsts.ExtendedDataSdfFilePath)) {
                var engine = new SqlCeEngine(exDataConnStr);
                engine.CreateDatabase();
            }

            _memoConnection = default(SqlCeConnection);
            {
                _memoConnection = new SqlCeConnection(memoConnStr);
                _memoConnection.Open();
            }

            _exDataConnection = default(SqlCeConnection);
            {
                _exDataConnection = new SqlCeConnection(exDataConnStr);
                _exDataConnection.Open();
            }
        }

        internal void OpenConnections() {
            if (_memoConnection != null) {
                _memoConnection.Open();
            }
            if (_exDataConnection != null) {
                _exDataConnection.Open();
            }
        }

        internal void CloseConnections() {
            if (_memoConnection != null) {
                _memoConnection.Close();
            }
            if (_exDataConnection != null) {
                _exDataConnection.Close();
            }
        }

        // ========================================
        // property
        // ========================================
        internal SqlCeConnection MemoConnection {
            get { return _memoConnection; }
        }

        internal SqlCeConnection ExtendedDataConnection {
            get { return _exDataConnection; }
        }

        // ========================================
        // method
        // ========================================
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (_memoConnection != null) {
                _memoConnection.Close();
                _memoConnection.Dispose();
            }
            if (_exDataConnection != null) {
                _exDataConnection.Close();
                _exDataConnection.Dispose();
            }
            
#if !DEBUG
            if (_server != null) {
                _server.StopListen();
            }
#endif
        }

//        private void HandleApplicationExit(object sender, EventArgs e) {
//            if (_memoConnection != null) {
//                _memoConnection.Close();
//                _memoConnection.Dispose();
//            }
//            if (_exDataConnection != null) {
//                _exDataConnection.Close();
//                _exDataConnection.Dispose();
//            }
            
//#if !DEBUG
//            if (_server != null) {
//                _server.StopListen();
//            }
//#endif
//        }
    }
}
