/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using System.Drawing;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Handles;
using Mkamo.Memopad.Internal.Commands;
using Mkamo.Memopad.Internal.Requests;

namespace Mkamo.Memopad.Internal.Handles {
    internal class EdgeCommentAnchorHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private IAnchor _anchor;
        private ConnectRequest _connectRequest;
        private ConnectRequest _tmpConnectRequest; /// targetを探すためのテンポラリ

        private IFigure _figure;
        private Size _figureSize;

        private IEditor _target;

        // ========================================
        // constructor
        // ========================================
        public EdgeCommentAnchorHandle(IAnchor anchor) {
            _anchor = anchor;

            _connectRequest = new ConnectRequest();
            _connectRequest.ConnectingAnchor = anchor;

            _tmpConnectRequest = new ConnectRequest();
            _tmpConnectRequest.ConnectingAnchor = anchor;

            _figure = new SimpleRect();
            _figureSize = new Size(8, 8);
        }

        // ========================================
        // property
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        public Size FigureSize {
            get { return _figureSize; }
            set { _figureSize = value; }
        }

        // ========================================
        // method
        // ========================================
        public override void Install(IEditor host) {
            if (!(host.Controller is IConnectionController)) {
                throw new ArgumentException("host.Controller must be IConnectionController");
            }
            if (!(host.Figure is IConnection)) {
                throw new ArgumentException("host.Figure must be IConnection");
            }

            base.Install(host);
        }

        public override void Relocate(IFigure hostFigure) {
            _figure.Size = _figureSize;
            _figure.Location = new Point(
                _connectRequest.ConnectingAnchor.Location.X - _figureSize.Width / 2,
                _connectRequest.ConnectingAnchor.Location.Y - _figureSize.Height / 2
            );
        }

        protected override void OnFigureDragStart(MouseEventArgs e) {
            base.OnFigureDragStart(e);

            var edge = Host.Figure as IEdge;
            _tmpConnectRequest.NewLocation = e.Location;
            _target = GetTargetEditor(e.Location);

            _connectRequest.NewConnectableEditor = _target;
            _connectRequest.NewLocation = e.Location;

            Host.ShowFeedback(_connectRequest);
            if (_target != null) {
                ShowDropTextFeedback(_target, new Point(e.X, e.Y));
            }
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            base.OnFigureDragMove(e);

            _tmpConnectRequest.NewLocation = e.Location;
            var oldTarget = _target;
            _target = GetTargetEditor(e.Location);

            if (_target != _connectRequest.NewConnectableEditor) {
                Host.HideFeedback(_connectRequest);
                _connectRequest.NewConnectableEditor = _target;
            }
            _connectRequest.NewLocation = e.Location;
            Host.ShowFeedback(_connectRequest);

            if (oldTarget != null) {
                HideDropTextFeedback(oldTarget);
            }
            if (_target != null) {
                ShowDropTextFeedback(_target, new Point(e.X, e.Y));
            }
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            base.OnFigureDragFinish(e);

            _tmpConnectRequest.NewLocation = e.Location;
            var oldTarget = _target;
            _target = GetTargetEditor(e.Location);

            if (_target != _connectRequest.NewConnectableEditor) {
                Host.HideFeedback(_connectRequest);
                _connectRequest.NewConnectableEditor = _target;
            }
            _connectRequest.NewLocation = e.Location;
            Host.HideFeedback(_connectRequest);

            Host.PerformRequest(_connectRequest);
            _connectRequest.NewConnectableEditor = null;

            if (oldTarget != null) {
                HideDropTextFeedback(oldTarget);
            }
            if (_target != null) {
                HideDropTextFeedback(_target);
            }
            _target = null;
        }

        protected override void OnFigureDragCancel() {
            base.OnFigureDragCancel();

            Host.HideFeedback(_connectRequest);
            _connectRequest.NewConnectableEditor = null;
            if (_target != null) {
                HideDropTextFeedback(_target);
            }
            _target = null;
        }

        // ------------------------------
        // private
        // ------------------------------
        private IEditor GetTargetEditor(Point location) {
            return Host.Root.FindEditor(
                location,
                editor => {
                    _tmpConnectRequest.NewConnectableEditor = editor;
                    return Host.CanUnderstand(_tmpConnectRequest);
                }
            );
        }


        private void ShowDropTextFeedback(IEditor target, Point loc) {
            if (_anchor.Kind == ConnectionAnchorKind.Source) {
                return;
            }
            var node = target.Figure as INode;
            if (node != null && target.Figure.Root != null) {
                var charIndex = node.GetCharIndexAt(loc);
                var charRect = node.GetCharRect(charIndex);
                var feedbackRect = new Rectangle(charRect.Location, new Size(2, charRect.Height));
                target.ShowFeedback(new DropTextRequest(feedbackRect));
            }
        }

        private void HideDropTextFeedback(IEditor target) {
            if (_anchor.Kind == ConnectionAnchorKind.Source) {
                return;
            }
            target.HideFeedback(new DropTextRequest(Rectangle.Empty));
        }
    }
}
