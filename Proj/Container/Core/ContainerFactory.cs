/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Container.Internal.Core;
using Mkamo.Common.Diagnostics;
using System.Data.SqlServerCe;

namespace Mkamo.Container.Core {
    public static class ContainerFactory {
        private static EntityContainer _container;

        public static IEntityContainer GetContainer() {
            Contract.Requires(_container != null);
            return _container;
        }

        public static IEntityContainer GetContainerSurely() {
            try
            {
                return _container = _container ?? new EntityContainer();
            }
            catch (Exception)
            {
                // デザイナで表示するとProgram Files以下にファイルを作ろうとして例外になるので適当なフォルダのstoreを作る
                return _container = _container ?? new EntityContainer(new XmlFileEntityStore("."));
            }
        }

        //public static IEntityContainer CreateContainer(string storeRoot) {
        //    Contract.Requires(storeRoot != null);
        //    Contract.Requires(_container == null);
        //    return _container = new EntityContainer(new XmlFileEntityStore(storeRoot));
        //}

        //public static IEntityContainer CreateContainer(string storeRoot, string proxyAssemblyName, string proxyAssemblyPath) {
        //    Contract.Requires(storeRoot != null && proxyAssemblyName != null && proxyAssemblyPath != null);
        //    Contract.Requires(_container == null);
        //    return _container = new EntityContainer(new XmlFileEntityStore(storeRoot), proxyAssemblyName, proxyAssemblyPath);
        //    //return _container = new EntityContainer(new XmlSqlServerEntityStore(storeRoot), proxyAssemblyName, proxyAssemblyPath);
        //}

        public static IEntityContainer CreateContainer(IEntityStore store, string proxyAssemblyName, string proxyAssemblyPath) {
            Contract.Requires(store != null && proxyAssemblyName != null && proxyAssemblyPath != null);
            Contract.Requires(_container == null);
            return _container = new EntityContainer(store, proxyAssemblyName, proxyAssemblyPath);
        }

        public static IEntityStore CreateXmlFileStore(string storeRoot) {
            return new XmlFileEntityStore(storeRoot);
        }

        public static IEntityStore CreateXmlSqlServerStore(SqlCeConnection memoConn, SqlCeConnection exDataConn) {
            return new XmlSqlServerEntityStore(memoConn, exDataConn);
        }
    }
}
