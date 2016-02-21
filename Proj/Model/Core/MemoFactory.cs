/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using Mkamo.Container.Core;

namespace Mkamo.Model.Core {
    using Memo = Mkamo.Model.Memo.Memo;

    public static class MemoFactory {
        // ========================================
        // static field
        // ========================================
        public static void Remove(object entity) {
            GetContainer().Remove(entity);
        }

        public static Memo CreateMemo() {
            return GetContainer().Create<Memo>();
        }

        public static MemoCollection CreateMemoCollection() {
            return GetContainer().Create<MemoCollection>();
        }

        public static MemoElementCollection CreateMemoElementCollection() {
            return GetContainer().Create<MemoElementCollection>();
            //return new MemoElementCollection();
        }

        public static MemoTag CreateTag() {
            return GetContainer().Create<MemoTag>();
        }

        public static MemoFolder CreateFolder() {
            return GetContainer().Create<MemoFolder>();
            //return new MemoFolder();
        }

        public static MemoEdge CreateEdge() {
            return GetContainer().Create<MemoEdge>();
            //return new MemoEdge();
        }

        public static MemoShape CreateShape() {
            return GetContainer().Create<MemoShape>();
            //return new MemoShape();
        }

        public static MemoText CreateText() {
            return GetContainer().Create<MemoText>();
            //return new MemoText();
        }

        public static MemoImage CreateImage() {
            return GetContainer().Create<MemoImage>();
            //return new MemoImage();
        }

        public static MemoFile CreateFile() {
            return GetContainer().Create<MemoFile>();
            //return new MemoFile();
        }

        public static MemoFreehand CreateFreehand() {
            return GetContainer().Create<MemoFreehand>();
            //return new MemoFreehand();
        }

        public static MemoTagCollection CreateMemoTagCollection() {
            return GetContainer().Create<MemoTagCollection>();
        }

        public static MemoSmartFolder CreateSmartFolder() {
            return GetContainer().Create<MemoSmartFolder>();
        }

        public static MemoSmartFolder CreateTransientSmartFolder() {
            return GetContainer().CreateTransient<MemoSmartFolder>();
        }

        public static MemoSmartFilter CreateSmartFilter() {
            return GetContainer().Create<MemoSmartFilter>();
        }

        public static MemoSmartFilter CreateTransientSmartFilter() {
            return GetContainer().CreateTransient<MemoSmartFilter>();
        }

        //public static MemoMarkDefinition CreateMarkDefinition() {
        //    return GetContainer().Create<MemoMarkDefinition>();
        //}

        public static MemoMark CreateMark() {
            return GetContainer().Create<MemoMark>();
            //return new MemoMark();
        }

        public static MemoQuery CreateQuery() {
            return GetContainer().Create<MemoQuery>();
        }

        public static MemoQuery CreateTransientQuery() {
            return GetContainer().CreateTransient<MemoQuery>();
        }

        public static MemoTable CreateTable() {
            return GetContainer().Create<MemoTable>();
            //return new MemoTable();
        }

        public static MemoTableRow CreateTableRow() {
            return GetContainer().Create<MemoTableRow>();
            //return new MemoTableRow();
        }

        public static MemoTableCell CreateTableCell() {
            return GetContainer().Create<MemoTableCell>();
            //return new MemoTableCell();
        }

        public static MemoTableLine CreateTableLine() {
            return GetContainer().Create<MemoTableLine>();
            //return new MemoTableLine();
        }

        public static MemoAnchorReference CreateAnchorReference() {
            return GetContainer().Create<MemoAnchorReference>();
            //return new MemoAnchorReference();
        }

        // ------------------------------
        // private
        // ------------------------------
        private static IEntityContainer GetContainer() {
            return ContainerFactory.GetContainer();
        }
    }
}
