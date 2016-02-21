/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Figure.Core;
using System.Drawing.Drawing2D;
using Mkamo.Common.Externalize;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.Figure.Figures {
    public class RoundedRect: AbstractPathBoundingNode {
        // ========================================
        // field
        // ========================================
        private float _cornerRatio;

        // ========================================
        // constructor
        // ========================================
        public RoundedRect() {
            _cornerRatio = 0.2f;
        }

        // ========================================
        // property
        // ========================================
        public float CornerRatio {
            get { return _cornerRatio; }
            set {
                _cornerRatio = value > 1? 1: value;
            }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);
            memento.WriteFloat("CornerRatio", _cornerRatio);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);
            _cornerRatio = memento.ReadFloat("CornerRatio");
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            var path = new GraphicsPathDescription();

            var roundWidth = (int) Math.Round(bounds.Width * CornerRatio, MidpointRounding.AwayFromZero);
            var roundHeight = (int) Math.Round(bounds.Height * CornerRatio, MidpointRounding.AwayFromZero);
            var roundSize = Math.Min(roundWidth, roundHeight);
            if (roundSize < 1) {
                roundSize = 1;
            }

            path.AddLine(
                new Point(bounds.Left  + roundSize, bounds.Top),
                new Point(bounds.Right - roundSize, bounds.Top)
            );
            path.AddArc(
                new Rectangle(
                    bounds.Right - roundSize,
                    bounds.Top,
                    roundSize,
                    roundSize
                ),
                270,
                90
            );
            path.AddLine(
                new Point(bounds.Right, bounds.Top + roundSize),
                new Point(bounds.Right, bounds.Bottom - roundSize)
            );
            path.AddArc(
                new Rectangle(
                    bounds.Right - roundSize,
                    bounds.Bottom - roundSize,
                    roundSize,
                    roundSize
                ),
                0,
                90
            );
            path.AddLine(
                new Point(bounds.Right - roundSize, bounds.Bottom),
                new Point(bounds.Left + roundSize, bounds.Bottom)
            );
            path.AddArc(
                new Rectangle(
                    bounds.Left,
                    bounds.Bottom - roundSize,
                    roundSize,
                    roundSize
                ),
                90,
                90
            );
            path.AddLine(
                new Point(bounds.Left, bounds.Bottom - roundSize),
                new Point(bounds.Left, bounds.Top + roundSize)
            );
            path.AddArc(
                new Rectangle(bounds.Left, bounds.Top, roundSize, roundSize),
                180,
                90
            );

            return path;
        }

    
    }
}
