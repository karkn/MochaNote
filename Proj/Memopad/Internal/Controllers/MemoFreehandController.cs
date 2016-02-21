/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using Mkamo.Figure.Core;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.StyledText.Core;
using Mkamo.Figure.Layouts;
using Mkamo.Figure.Layouts.Locators;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Common.Externalize;
using Mkamo.Editor.Handles;
using System.Windows.Forms;
using Mkamo.Editor.Roles;
using Mkamo.Editor.Focuses;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Editor.Utils;
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Controllers.UIProviders;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class MemoFreehandController: AbstractMemoContentController<MemoFreehand, FreehandFigure> {
        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        // ========================================
        // constructor
        // ========================================
        internal MemoFreehandController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoFreehandUIProvider(this));
        }

        // ========================================
        // property
        // ========================================
        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }

        // ========================================
        // method
        // ========================================
        public override void ConfigureEditor(IEditor editor) {
            var editorHandle = new MoveEditorHandle() {
                Cursor = Cursors.SizeAll
            };

            var facade = MemopadApplication.Instance;
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(new SelectionIndicatingHandle());

            editor.InstallRole(new SelectRole());
            editor.InstallRole(new MoveRole(false));
            editor.InstallRole(new RemoveRole());
            editor.InstallRole(new CopyRole());
            editor.InstallRole(new ReorderRole());

        }

        protected override FreehandFigure CreateFigure(MemoFreehand model) {
            var ret = new FreehandFigure();
            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, FreehandFigure figure, MemoFreehand model) {
            figure.AdjustSize();
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            return externalizer.Save(Model, (key, obj) => false);
        }

        public override string GetText() {
            var ret = "";
            if (Model.Keywords != null) {
                ret = Model.Keywords;
            }
            return ret;
        }
    }
}
