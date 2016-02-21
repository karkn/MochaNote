/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Remote;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemopadServer {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IpcServerChannel _channel;

        // ========================================
        // method
        // ========================================
        public void Listen() {
            try {
                _channel = new IpcServerChannel("mochanote");
                ChannelServices.RegisterChannel(_channel, true);
                var facade = new MemopadRemoteFacadeProxy(new MemopadRemoteFacade());
                RemotingServices.Marshal(facade, "facade", typeof(MemopadRemoteFacadeProxy));
            } catch (Exception e) {
                Logger.Warn("MemopadServerの起動に失敗しました。", e);
            }
        }

        public void StopListen() {
            if (_channel != null) {
                ChannelServices.UnregisterChannel(_channel);
            }
        }
    }
}
