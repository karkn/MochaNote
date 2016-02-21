/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Event;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;

namespace Mkamo.Model.Memo {
    using StyledText = Mkamo.StyledText.Core.StyledText;
using System.Runtime.Serialization;

    [Entity, Externalizable]
    [DataContract, Serializable]
    public abstract class MemoStyledText: MemoContent {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "StyledText")]
        private StyledText _styledText;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoStyledText() {
            StyledText = new StyledText();
        }

        // ========================================
        // property
        // ========================================
        [Persist, External(Clone = "CloneStyledText")]
        public virtual StyledText StyledText {
            get { return _styledText; }
            set {
                var old  = _styledText;
                if (old != null) {
                    old.ContentsChanged -= HandleStyledTextContentsChanged;
                }
                _styledText = value;
                if (value != null) {
                    value.ContentsChanged += HandleStyledTextContentsChanged;
                }

                OnPropertySet(this, "StyledText", old, value);
            }
        }

        // ========================================
        // method
        // ========================================
        [Dirty]
        protected virtual void HandleStyledTextContentsChanged(object sender, EventArgs e) {
            OnPropertySet(this, "StyledText", _styledText, _styledText);
        }

        protected virtual StyledText CloneStyledText() {
            return _styledText == null? null: _styledText.CloneDeeply() as StyledText;
        }
    }
}
