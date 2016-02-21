/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using Mkamo.Figure.Core;
using Mkamo.Model.Memo;
using Mkamo.Figure.Figures;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Memopad.Properties;
using Mkamo.Memopad.Internal.Utils;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoMarkHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private ImageFigure _figure;
        private MemoMark _mark;
        MemoMarkLocation _location;

        // ========================================
        // constructor
        // ========================================
        public MemoMarkHandle() {
            _figure = new ImageFigure();
            _figure.IsForegroundEnabled = false;
            _location = MemoMarkLocation.LeftTop;
        }

        // ========================================
        // property
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        public MemoMark Mark {
            get { return _mark; }
            set {
                if (value == _mark) {
                    return;
                }
                _mark = value;
                var def = MemoMarkUtil.GetMarkDefinition(_mark);
                if (def != null) {
                    _figure.ImageDesc = new BytesImageDescription(def.Image);
                    _location = def.Location;
                }
            }
        }

        public MemoMarkLocation Location {
            get { return _location; }
        }

        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            _figure.AutoSizeKinds = AutoSizeKinds.GrowBoth;
            _figure.AdjustSize();
            _figure.AutoSizeKinds = AutoSizeKinds.None;
        }
    }
}
