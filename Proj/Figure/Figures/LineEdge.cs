/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using System.Drawing.Drawing2D;
using Mkamo.Common.Collection;
using Mkamo.Common.Externalize;
using Mkamo.Figure.Figures.EdgeDecorations;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Figure.Figures {
    public class LineEdge: AbstractEdge {
        // ========================================
        // field
        // ========================================
        private IEdgeDecoration _sourceDecoration;
        private IEdgeDecoration _targetDecoration;


        // ========================================
        // property
        // ========================================
        public override Color LineColor {
            get { return base.LineColor; }
            set {
                if (_sourceDecoration != null) {
                    _sourceDecoration.Foreground = value;
                }
                if (_targetDecoration != null) {
                    _targetDecoration.Foreground = value;
                }
                base.LineColor = value;
            }
        }

        public override int LineWidth {
            get { return base.LineWidth; }
            set {
                if (_sourceDecoration != null) {
                    _sourceDecoration.LineWidth = value;
                }
                if (_targetDecoration != null) {
                    _targetDecoration.LineWidth = value;
                }
                base.LineWidth = value;
            }
        }

        public override Rectangle PaintBounds {
            get {
                // todo: DecorationからBoundsをとってきてUnionする
                //       LineWidth分もInflateする(
                //       AbstractEdgeでもこれはoverrideしてやっておく)
                var ret = Bounds;
                ret.Inflate(8, 8);
                return ret;
            }
        }

        public IEdgeDecoration SourceDecoration {
            get { return _sourceDecoration; }
            set {
                if (value == _sourceDecoration) {
                    return;
                }

                if (_sourceDecoration != null) {
                    _sourceDecoration.Deactivate();
                    _sourceDecoration.ForegroundChanged -= HandleDecorationChanged;
                    _sourceDecoration.BackgroundChanged -= HandleDecorationChanged;
                    _sourceDecoration.LineWidthChanged -= HandleDecorationChanged;
                }
                _sourceDecoration = value;
                if (_sourceDecoration != null) {
                    _sourceDecoration.Foreground = LineColor;
                    _sourceDecoration.LineWidth = LineWidth;
                    _sourceDecoration.ForegroundChanged += HandleDecorationChanged;
                    _sourceDecoration.BackgroundChanged += HandleDecorationChanged;
                    _sourceDecoration.LineWidthChanged += HandleDecorationChanged;
                    _sourceDecoration.Activate();
                }

                InvalidatePaint();
            }
        }

        public IEdgeDecoration TargetDecoration {
            get { return _targetDecoration; }
            set {
                if (value == _targetDecoration) {
                    return;
                }

                if (_targetDecoration != null) {
                    _targetDecoration.Deactivate();
                    _targetDecoration.ForegroundChanged -= HandleDecorationChanged;
                    _targetDecoration.BackgroundChanged -= HandleDecorationChanged;
                    _targetDecoration.LineWidthChanged -= HandleDecorationChanged;
                }
                _targetDecoration = value;
                if (_targetDecoration != null) {
                    _targetDecoration.Foreground = LineColor;
                    _targetDecoration.LineWidth = LineWidth;
                    _targetDecoration.ForegroundChanged += HandleDecorationChanged;
                    _targetDecoration.BackgroundChanged += HandleDecorationChanged;
                    _targetDecoration.LineWidthChanged += HandleDecorationChanged;
                    _targetDecoration.Activate();
                }

                InvalidatePaint();
            }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteExternalizable("SourceDecoration", _sourceDecoration);
            memento.WriteExternalizable("TargetDecoration", _targetDecoration);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            SourceDecoration = memento.ReadExternalizable("SourceDecoration") as IEdgeDecoration;
            TargetDecoration = memento.ReadExternalizable("TargetDecoration") as IEdgeDecoration;
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void PaintSelf(Graphics g) {
            // todo: GraphicsPathのリソースキャッシュ
            using (_ResourceCache.UseResource())
            using (var path = new GraphicsPath()) {
                path.AddLines(EdgePoints.ToArray());
                g.DrawPath(_PenResource, path);
            }

            if (_sourceDecoration != null) {
                _sourceDecoration.Target = new Line(First, FirstRef.Next.EdgePoint);
                _sourceDecoration.Paint(g);
            }
            if (_targetDecoration != null) {
                _targetDecoration.Target = new Line(Last, LastRef.Prev.EdgePoint);
                _targetDecoration.Paint(g);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void HandleDecorationChanged(object sender, EventArgs e) {
            InvalidatePaint();
        }

    }
}
