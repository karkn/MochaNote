/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using System.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Figure.Core;
using Mkamo.Common.Command;
using Mkamo.Editor.Commands;

namespace Mkamo.Editor.Core {
    public static class IEditorExt {
        // ========================================
        // method
        // ========================================
        public static ICommand RequestSelect(this IEditor editor, SelectKind value, bool deselectOthers) {
            var req = new SelectRequest();
            req.DeselectOthers = deselectOthers;
            req.Value = value;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestClone(this IEditor editor, IEnumerable<IEditor> cloningEditors, Size moveDelta) {
            var req = new CloneRequest(cloningEditors);
            req.MoveDelta = moveDelta;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestMove(this IEditor editor, Size moveDelta) {
            var req = new ChangeBoundsRequest();
            req.MoveDelta = moveDelta;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestResize(this IEditor editor, Size sizeDelta, Directions resizeDirection, bool adjustSize) {
            var req = new ChangeBoundsRequest();
            req.SizeDelta = sizeDelta;
            req.ResizeDirection = resizeDirection;
            req.AdjustSize = adjustSize;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestFocus(this IEditor editor, FocusKind value, Point? location) {
            var req = new FocusRequest();
            req.Value = value;
            req.Location = location;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestFocusCommit(this IEditor editor, bool orRollback) {
            var ret = editor.RequestFocus(FocusKind.Commit, null) as FocusCommand;
            if (ret != null && orRollback && ret.ResultKind == FocusCommitResultKind.Canceled) {
                return RequestFocus(editor, FocusKind.Rollback, null);
            }
            return ret;
        }

        public static ICommand RequestCreateNode(this IEditor editor, IModelFactory modelFactory, Rectangle bounds, bool fitGrid) {
            var req = new CreateNodeRequest();
            req.ModelFactory = modelFactory;
            req.Bounds = bounds;
            req.AdjustSizeToGrid = fitGrid;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestCreateNode(this IEditor editor, IModelFactory modelFactory, Rectangle bounds) {
            return RequestCreateNode(editor, modelFactory, bounds, true);
        }

        public static ICommand RequestConnect(
            this IEditor editor,
            IAnchor connectingAnchor, IEditor newConnectableEditor, Point location
        ) {
            var req = new ConnectRequest();
            req.ConnectingAnchor = connectingAnchor;
            req.NewConnectableEditor = newConnectableEditor;
            req.NewLocation = location;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestCreateEdge(
            this IEditor editor,
            IModelFactory modelFactory,
            IEditor sourceEditor,
            IEditor targetEditor,
            Point startPoint,
            Point endPoint
        ) {
            var req = new CreateEdgeRequest();
            req.ModelFactory = modelFactory;
            req.EdgeSourceEditor = sourceEditor;
            req.EdgeTargetEditor = targetEditor;
            req.StartPoint = startPoint;
            req.EndPoint = endPoint;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestGroup(this IEditor editor) {
            // todo
            throw new NotImplementedException();
        }

        public static ICommand RequestMoveEdgePoint(this IEditor editor, EdgePointRef edgePointRef, Point location) {
            var req = new MoveEdgePointRequest();
            req.EdgePointRef = edgePointRef;
            req.Location = location;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestNewEdgePoint(this IEditor editor, EdgePointRef prevEdgePointRef, Point location) {
            var req = new NewEdgePointRequest();
            req.PrevEdgePointRef = prevEdgePointRef;
            req.Location = location;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestRemove(this IEditor editor) {
            var req = new RemoveRequest();
            return editor.PerformRequest(req);
        }

        public static ICommand RequestReorder(this IEditor editor, ReorderKind reorderKind) {
            var req = new ReorderRequest();
            req.Kind = reorderKind;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestPaste(this IEditor editor, Point location, string description) {
            var req = new PasteRequest();
            req.Location = location;
            req.Description = description;
            return editor.PerformRequest(req);
        }

        public static ICommand RequestExport(this IEditor editor, string format, string outputPath) {
            var req = new ExportRequest();
            req.Format = format;
            req.OutputPath = outputPath;
            return editor.PerformRequest(req);
        }
    }
}
