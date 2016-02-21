/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Editor.Core;
using Mkamo.Figure.Figures;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Editor.Handles;
using Mkamo.Editor.Roles;

namespace Mkamo.Editor.Controllers {
    public class ObjectNodeController: AbstractController {
        // ========================================
        // property
        // ========================================
        // ========================================
        // method
        // ========================================
        public override void Activate() {

        }

        public override void Deactivate() {

        }

        public override IFigure CreateFigure(object model) {
            //return new ImageFigure {
            //    ImageDesc = new FileImageDescriptor(@"c:\windows\web\wallpaper\Windows XP.jpg"),
            //    Opacity = 1.0f
            //};

            return new SimpleRect {
                Background = new GradientBrushDescription(Color.Blue, Color.White),
                Padding = new Insets(4),
                FontColor = Color.White,
            };
        }

        public override void RefreshEditor(RefreshContext context, IFigure figure, object model) {
            var label = figure as INode;
            var str = model == null? "": model.ToString();
            label.Text = str;
        }

        public override void ConfigureEditor(IEditor editor) {
            editor.InstallEditorHandle(new MoveEditorHandle());
            editor.InstallRole(new SelectRole());
            editor.InstallRole(new MoveRole());
            editor.InstallRole(new HighlightRole());
        }

        public override Mkamo.Common.Externalize.IMemento GetModelMemento() {
            throw new NotImplementedException();
        }

        public override string GetText() {
            return Model.ToString();
        }
    }
}
