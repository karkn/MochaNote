/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Handles {
    public class MoveNodeHandle: AbstractMoveHandle, IAuxiliaryHandle {
        private IFigure _figure;
        private HandleStickyKind _stickyKind;

        public HandleStickyKind StickyKind {
            get { return _stickyKind; }
            set { _stickyKind = value; }
        }

        public IFigure Figure {
            get {
                if (_figure == null) {
                    var fig = new SimpleRect();
                    fig.Foreground = Color.Black;
                    fig.Background = new SolidBrushDescription(Color.AliceBlue);
                    _figure = fig;
                }
                return _figure;
            }
        }

        public virtual bool HideOnFocus {
            get { return true; }
        }

        protected override IFigure _Figure {
            get { return Figure; }
        }

        public void Relocate(IFigure hostFigure) {
            Figure.Bounds = hostFigure.Bounds;
        }


    }
}
