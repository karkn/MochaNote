/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;

namespace Mkamo.Editor.Requests {
    public abstract class AbstractRequest: IRequest {
        // ========================================
        // field
        // ========================================
        private IFigure _customFeedback;

        // ========================================
        // constructor
        // ========================================
        protected AbstractRequest(IFigure customFeedback) {
            _customFeedback = customFeedback;
        }

        protected AbstractRequest(): this(null) {
        }

        // ========================================
        // property
        // ========================================
        public abstract string Id { get; }

        public virtual IFigure CustomFeedback {
            get { return _customFeedback; }
            set { _customFeedback = value; }
        }

    }
}
