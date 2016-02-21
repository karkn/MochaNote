/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Controllers;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Roles;
using Mkamo.Editor.Handles;
using Mkamo.Editor.Internal.Core;
using Mkamo.Figure.Layouts;
using System.IO;
using System.Reflection;

namespace Mkamo.Editor.Internal.Editors {
    internal sealed class RootEditor: Editor, IRootEditor {
        // ========================================
        // field
        // ========================================
        private EditorCanvas _owner;

        private IEditor _content;

        // ========================================
        // constructor
        // ========================================
        internal RootEditor(EditorCanvas owner): base() {
            _owner = owner;

            _Controller = new RootController();
            _Figure = _owner._PrimaryLayer;
            Model = new object(); /// dummy

            Figure.Layout = new StackLayout();
        }

        // ========================================
        // property
        // ========================================
        // === IEditor ==========
        public override IRootEditor Root {
            get { return this; }
        }

        public override IEditorSite Site {
            get { return _owner._EditorSite; }
        }

        public override bool IsRoot {
            get { return true; }
        }

        // === RootEditor ==========
        public IEditor Content {
            get { return _content; }
            internal set {
                if (value == _content) {
                    return;
                }
                if (_content != null) {
                    var editor = _content as Editor;
                    editor.Disable();
                    RemoveChildEditor(editor);
                }
                _content = value;
                if (_content != null) {
                    var editor = _content as Editor;
                    AddChildEditor(editor);
                    editor.Enable();
                }
            }
        }

    }
}
