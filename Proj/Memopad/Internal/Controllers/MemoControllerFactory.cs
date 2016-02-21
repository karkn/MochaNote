/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Controllers;
using Mkamo.Model.Memo;
using Mkamo.Model.Uml;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class MemoControllerFactory: IControllerFactory {

        public IController CreateController(object model) {
            if (model is Memo) {
                return new MemoController();

            } else if (model is UmlClass) {
                return new UmlClassController();

            } else if (model is UmlInterface) {
                return new UmlInterfaceController();

            } else if (model is UmlAssociation) {
                return new UmlAssociationController();

            } else if (model is UmlGeneralization) {
                return new UmlGeneralizationController();

            } else if (model is UmlInterfaceRealization) {
                return new UmlInterfaceRealizationController();

            } else if (model is UmlDependency) {
                return new UmlDependencyController();

            } else if (model is UmlPropertyCollection) {
                return new UmlAttributeCollectionController();

            } else if (model is UmlOperationCollection) {
                return new UmlOperationCollectionController();

            } else if (model is UmlProperty) {
                return new UmlPropertyController();

            } else if (model is UmlOperation) {
                return new UmlOperationController();

            } else if (model is MemoShape) {
                return new MemoShapeController();

            } else if (model is MemoText) {
                return new MemoTextController();

            } else if (model is MemoImage) {
                return new MemoImageController();

            } else if (model is MemoFile) {
                return new MemoFileController();

            } else if (model is MemoTable) {
                return new MemoTableController();

            } else if (model is MemoTableCell) {
                return new MemoTableCellController();

            } else if (model is MemoAnchorReference) {
                return new MemoAnchorReferenceController();

            } else if (model is MemoEdge) {
                return new MemoEdgeController();

            } else if (model is MemoFreehand) {
                return new MemoFreehandController();

            }
            return null;
        }

    }
}
