/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Drawing;
using Mkamo.Common.DataType;

namespace Mkamo.Editor.Requests {
    public class ChangeBoundsRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        private Size _moveDelta;
        private Size _sizeDelta;
        private Directions _resizeDirection;
        public bool AdjustSize;

        public IEnumerable<IEditor> MovingEditors;

        // ========================================
        // constructor
        // ========================================
        public ChangeBoundsRequest() {
            AdjustSize = true;
        }

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.ChangeBounds; }
        }

        public Size MoveDelta {
            get { return _moveDelta; }
            set { _moveDelta = value; }
        }

        public Size SizeDelta {
            get { return _sizeDelta; }
            set { _sizeDelta = value; }
        }

        public Directions ResizeDirection {
            get { return _resizeDirection; }
            set { _resizeDirection = value; }
        }

    }
}
