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
using Mkamo.Common.Forms.Drawing;
using System.Drawing.Drawing2D;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Disposable;
using System.Runtime.Serialization;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures.EdgeDecorations {
    [Externalizable]
    public abstract class AbstractPathEdgeDecoration: AbstractEdgeDecoration {
        // ========================================
        // static field
        // ========================================
        public static readonly string PathResourceKey = "AbstractPathEdgeDecoration.Path";

        // ========================================
        // field
        // ========================================
        private GraphicsPathDescription _path;

        // ========================================
        // constructor
        // ========================================
        public AbstractPathEdgeDecoration() {
            _path = CreatePath();
        }

        // ========================================
        // property
        // ========================================
        protected GraphicsPath _PathResource {
            get { return (GraphicsPath) _ResourceCache.GetResource(PathResourceKey); }
        }

        protected abstract bool _IsPathClosed { get; }

        // ========================================
        // method
        // ========================================
        public override void Paint(Graphics g) {
            using (_ResourceCache.UseResource()) {
                if (_IsPathClosed) {
                    g.FillPath(_BrushResource, _PathResource);
                }
                g.DrawPath(_PenResource, _PathResource);
            }
        }

        protected override void InitResourceCache() {
            base.InitResourceCache();

            _ResourceCache.RegisterResourceCreator(
                PathResourceKey,
                () => {
                    var ret = _path.CreateGraphicsPath(_IsPathClosed);
                    var mat = new Matrix();
                    var r = Math.Atan2(Target.End.Y - Target.Start.Y, Target.End.X - Target.Start.X);
                    var d = r * 180 / Math.PI;
                    mat.Translate(Target.Start.X, Target.Start.Y);
                    mat.Rotate((float) d);
                    ret.Transform(mat);
                    return ret;
                },
                ResourceDisposingPolicy.Explicit
            );
        }

        protected override void DisposeResourceCache() {
            base.DisposeResourceCache();
            _ResourceCache.DisposeResource(PathResourceKey);
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected abstract GraphicsPathDescription CreatePath();

        protected override void OnTargetChanged() {
            base.OnTargetChanged();
            _ResourceCache.DisposeResource(PathResourceKey);
        }
    }
}
