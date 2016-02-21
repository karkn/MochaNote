/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Control.SelectorDropDown;
using System.Drawing;
using Mkamo.Memopad.Properties;
using Mkamo.Common.Command;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Core;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing.Drawing2D;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Model.Memo;

namespace Mkamo.Memopad.Internal.Controls {
    internal class LineStyleSelectorDropDown: SelectorDropDown {
        // ========================================
        // field
        // ========================================
        private MemopadFormBase _form;
        private bool _isPrepared;

        // ========================================
        // constructor
        // ========================================
        public LineStyleSelectorDropDown(MemopadFormBase form) {
            _form = form;
            _isPrepared = false;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public void Prepare() {
            if (_isPrepared) {
                return;
            }

            SuspendLayout();

            var style = new SelectorCategory("スタイル");
            style.BackColor = Color.White;
            style.LabelSize = new Size(20, 20);
            style.MaxCols = 9;

            var black = Color.Black; /// 0, 0, 0
            var gray = Color.DimGray; /// 105, 105, 105
            var blue = Color.FromArgb(75, 125, 190);
            var red = Color.FromArgb(190, 75, 70);
            var green = Color.FromArgb(150, 185, 85);
            var purple = Color.FromArgb(125, 95, 160);
            var aqua = Color.FromArgb(70, 170, 200);
            var orange = Color.FromArgb(245, 145, 65);
            var yellow = Color.FromArgb(250, 210, 80);

            /// 細，実線
            RegisterSylte(
                style,
                Resources.line_style_black_thin,
                "細黒実線",
                black,
                1,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_gray_thin,
                "細灰実線",
                gray,
                1,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_blue_thin,
                "細青実線",
                blue,
                1,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_red_thin,
                "細赤実線",
                red,
                1,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_green_thin,
                "細緑実線",
                green,
                1,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_purple_thin,
                "細紫実線",
                purple,
                1,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_aqua_thin,
                "細水色実線",
                aqua,
                1,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_orange_thin,
                "細オレンジ実線",
                orange,
                1,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_yellow_thin,
                "細黄実線",
                yellow,
                1,
                DashStyle.Solid
            );


            /// 中実線
            RegisterSylte(
                style,
                Resources.line_style_black_middle,
                "中黒実線",
                black,
                2,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_gray_middle,
                "中灰実線",
                gray,
                2,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_blue_middle,
                "中青実線",
                blue,
                2,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_red_middle,
                "中赤実線",
                red,
                2,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_green_middle,
                "中緑実線",
                green,
                2,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_purple_middle,
                "中紫実線",
                purple,
                2,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_aqua_middle,
                "中水色実線",
                aqua,
                2,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_orange_middle,
                "中オレンジ実線",
                orange,
                2,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_yellow_middle,
                "中黄実線",
                yellow,
                2,
                DashStyle.Solid
            );

            
            /// 太実線
            RegisterSylte(
                style,
                Resources.line_style_black_thick,
                "太黒実線",
                black,
                3,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_gray_thick,
                "太灰実線",
                gray,
                3,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_blue_thick,
                "太青実線",
                blue,
                3,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_red_thick,
                "太赤実線",
                red,
                3,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_green_thick,
                "太緑実線",
                green,
                3,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_purple_thick,
                "太紫実線",
                purple,
                3,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_aqua_thick,
                "太水色実線",
                aqua,
                3,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_orange_thick,
                "太オレンジ実線",
                orange,
                3,
                DashStyle.Solid
            );
            RegisterSylte(
                style,
                Resources.line_style_yellow_thick,
                "太黄実線",
                yellow,
                3,
                DashStyle.Solid
            );

            
            /// 細，点線
            RegisterSylte(
                style,
                Resources.line_style_black_thin_dash,
                "細黒点線",
                black,
                1,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_gray_thin_dash,
                "細灰点線",
                gray,
                1,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_blue_thin_dash,
                "細青点線",
                blue,
                1,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_red_thin_dash,
                "細赤点線",
                red,
                1,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_green_thin_dash,
                "細緑点線",
                green,
                1,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_purple_thin_dash,
                "細紫点線",
                purple,
                1,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_aqua_thin_dash,
                "細水色点線",
                aqua,
                1,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_orange_thin_dash,
                "細オレンジ点線",
                orange,
                1,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_yellow_thin_dash,
                "細黄点線",
                yellow,
                1,
                DashStyle.Dash
            );


            /// 中点線
            RegisterSylte(
                style,
                Resources.line_style_black_middle_dash,
                "中黒点線",
                black,
                2,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_gray_middle_dash,
                "中灰点線",
                gray,
                2,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_blue_middle_dash,
                "中青点線",
                blue,
                2,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_red_middle_dash,
                "中赤点線",
                red,
                2,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_green_middle_dash,
                "中緑点線",
                green,
                2,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_purple_middle_dash,
                "中紫点線",
                purple,
                2,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_aqua_middle_dash,
                "中水色点線",
                aqua,
                2,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_orange_middle_dash,
                "中オレンジ点線",
                orange,
                2,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_yellow_middle_dash,
                "中黄点線",
                yellow,
                2,
                DashStyle.Dash
            );

            
            /// 太点線
            RegisterSylte(
                style,
                Resources.line_style_black_thick_dash,
                "太黒点線",
                black,
                3,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_gray_thick_dash,
                "太灰点線",
                gray,
                3,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_blue_thick_dash,
                "太青点線",
                blue,
                3,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_red_thick_dash,
                "太赤点線",
                red,
                3,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_green_thick_dash,
                "太緑点線",
                green,
                3,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_purple_thick_dash,
                "太紫点線",
                purple,
                3,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_aqua_thick_dash,
                "太水色点線",
                aqua,
                3,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_orange_thick_dash,
                "太オレンジ点線",
                orange,
                3,
                DashStyle.Dash
            );
            RegisterSylte(
                style,
                Resources.line_style_yellow_thick_dash,
                "太黄点線",
                yellow,
                3,
                DashStyle.Dash
            );

            
            AddCategory(style);

            ResumeLayout();

            _isPrepared = true;
        }

        private void RegisterSylte(
            SelectorCategory cate,
            Image image,
            string text,
            Color lineColor,
            int lineWidth,
            DashStyle lineDashStyle
        ) {
            cate.AddLabel(
                image,
                text,
                () => SetLineStyle(lineColor, lineWidth, lineDashStyle)
            );
        }


        private void SetLineStyle(
            Color lineColor,
            int lineWidth,
            DashStyle lineDashStyle
        ) {
            var canvas = _form.EditorCanvas;
            if (canvas == null) {
                return;
            }

            var cmd = new CompositeCommand();
            var selecteds = canvas.SelectionManager.SelectedEditors.ToArray();
            foreach (var target in selecteds) {
                var edge = target.Model as MemoEdge;
                if (edge != null) {
                    cmd.Chain(
                        new SetEdgeLineCommand(target, lineColor, lineWidth, lineDashStyle)
                    );
                }
            }

            using (canvas.RootFigure.DirtManager.BeginDirty()) {
                canvas.CommandExecutor.Execute(cmd);
            }
        }
    }
}
