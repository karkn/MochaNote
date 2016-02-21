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
    internal class NodeStyleSelectorDropDown: SelectorDropDown {
        // ========================================
        // field
        // ========================================
        private MemopadFormBase _form;
        private bool _isPrepared;

        // ========================================
        // constructor
        // ========================================
        public NodeStyleSelectorDropDown(MemopadFormBase form) {
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
            style.LabelSize = new Size(36, 36);

            var yellowBorder = Color.FromArgb(250, 210, 80);
            var yellowLight1 = Color.FromArgb(255, 250, 210);
            var yellowLight2 = Color.FromArgb(255, 230, 160);
            var yellowDeep1 = Color.FromArgb(240, 200, 30);
            var yellowDeep2 = Color.FromArgb(230, 170, 0);

            SetHollowStyle(
                style,
                Resources.style_hollow_black,
                "太枠中抜黒",
                Color.Black
            );
            SetHollowStyle(
                style,
                Resources.style_hollow_blue,
                "太枠中抜青",
                Color.FromArgb(75, 125, 190)
            );
            SetHollowStyle(
                style,
                Resources.style_hollow_red,
                "太枠中抜赤",
                Color.FromArgb(190, 75, 70)
            );
            SetHollowStyle(
                style,
                Resources.style_hollow_green,
                "太枠中抜緑",
                Color.FromArgb(150, 185, 85)
            );
            SetHollowStyle(
                style,
                Resources.style_hollow_violet,
                "太枠中抜紫",
                Color.FromArgb(125, 95, 160)
            );
            SetHollowStyle(
                style,
                Resources.style_hollow_sblue,
                "太枠中抜水色",
                Color.FromArgb(70, 170, 200)
            );
            SetHollowStyle(
                style,
                Resources.style_hollow_orange,
                "太枠中抜オレンジ",
                Color.FromArgb(245, 145, 65)
            );
            SetHollowStyle(
                style,
                Resources.style_hollow_yellow,
                "太枠中抜黄",
                yellowBorder
            );


            SetLightSolidStyle(
                style,
                Resources.style_light_solid_black,
                "太枠淡色単色黒",
                Color.FromArgb(240, 240, 240),
                Color.Black
            );
            SetLightSolidStyle(
                style,
                Resources.style_light_solid_blue,
                "太枠淡色単色青",
                Color.FromArgb(230, 240, 255),
                Color.FromArgb(75, 125, 190)
            );
            SetLightSolidStyle(
                style,
                Resources.style_light_solid_red,
                "太枠淡色単色赤",
                Color.FromArgb(255, 230, 230),
                Color.FromArgb(190, 75, 70)
            );
            SetLightSolidStyle(
                style,
                Resources.style_light_solid_green,
                "太枠淡色単色緑",
                Color.FromArgb(245, 255, 230),
                Color.FromArgb(150, 185, 85)
            );
            SetLightSolidStyle(
                style,
                Resources.style_light_solid_violet,
                "太枠淡色単色紫",
                Color.FromArgb(240, 235, 250),
                Color.FromArgb(125, 95, 160)
            );
            SetLightSolidStyle(
                style,
                Resources.style_light_solid_sblue,
                "太枠淡色単色水色",
                Color.FromArgb(230, 250, 255),
                Color.FromArgb(70, 170, 200)
            );
            SetLightSolidStyle(
                style,
                Resources.style_light_solid_orange,
                "太枠淡色単色オレンジ",
                Color.FromArgb(255, 235, 220),
                Color.FromArgb(245, 145, 65)
            );
            SetLightSolidStyle(
                style,
                Resources.style_light_solid_yellow,
                "太枠淡色単色黄",
                yellowLight1,
                yellowBorder
            );


            SetDeepSolidStyle(
                style,
                Resources.style_deep_solid_black,
                "太枠濃色単色黒",
                Color.Black,
                Color.Black
            );
            SetDeepSolidStyle(
                style,
                Resources.style_deep_solid_blue,
                "太枠濃色単色青",
                Color.FromArgb(45, 95, 150),
                Color.FromArgb(75, 125, 190)
            );
            SetDeepSolidStyle(
                style,
                Resources.style_deep_solid_red,
                "太枠濃色単色赤",
                Color.FromArgb(155, 45, 40),
                Color.FromArgb(190, 75, 70)
            );
            SetDeepSolidStyle(
                style,
                Resources.style_deep_solid_green,
                "太枠濃色単色緑",
                Color.FromArgb(120, 150, 55),
                Color.FromArgb(150, 185, 85)
            );
            SetDeepSolidStyle(
                style,
                Resources.style_deep_solid_violet,
                "太枠濃色単色紫",
                Color.FromArgb(95, 65, 125),
                Color.FromArgb(125, 95, 160)
            );
            SetDeepSolidStyle(
                style,
                Resources.style_deep_solid_sblue,
                "太枠濃色単色水色",
                Color.FromArgb(40, 135, 160),
                Color.FromArgb(70, 170, 200)
            );
            SetDeepSolidStyle(
                style,
                Resources.style_deep_solid_orange,
                "太枠濃色単色オレンジ",
                Color.FromArgb(200, 110, 30),
                Color.FromArgb(245, 145, 65)
            );
            SetDeepSolidStyle(
                style,
                Resources.style_deep_solid_yellow,
                "太枠濃色単色黄",
                yellowDeep2,
                yellowBorder
            );


            SetLightGradientStyle(
                style,
                Resources.style_light_grad_black,
                "細枠淡色グラデーション黒",
                Color.FromArgb(240, 240, 240),
                Color.FromArgb(190, 190, 190),
                Color.Black
            );
            SetLightGradientStyle(
                style,
                Resources.style_light_grad_blue,
                "細枠淡色グラデーション青",
                Color.FromArgb(230, 240, 255),
                Color.FromArgb(200, 220, 240),
                Color.FromArgb(75, 125, 190)
            );
            SetLightGradientStyle(
                style,
                Resources.style_light_grad_red,
                "細枠淡色グラデーション赤",
                Color.FromArgb(255, 230, 230),
                Color.FromArgb(255, 160, 160),
                Color.FromArgb(190, 75, 70)
            );
            SetLightGradientStyle(
                style,
                Resources.style_light_grad_green,
                "細枠淡色グラデーション緑",
                Color.FromArgb(245, 255, 230),
                Color.FromArgb(220, 255, 170),
                Color.FromArgb(150, 185, 85)
            );
            SetLightGradientStyle(
                style,
                Resources.style_light_grad_violet,
                "細枠淡色グラデーション紫",
                Color.FromArgb(240, 235, 250),
                Color.FromArgb(200, 180, 230),
                Color.FromArgb(125, 95, 160)
            );
            SetLightGradientStyle(
                style,
                Resources.style_light_grad_sblue,
                "細枠淡色グラデーション水色",
                Color.FromArgb(230, 250, 255),
                Color.FromArgb(160, 235, 255),
                Color.FromArgb(70, 170, 200)
            );
            SetLightGradientStyle(
                style,
                Resources.style_light_grad_orange,
                "細枠淡色グラデーションオレンジ",
                Color.FromArgb(255, 235, 220),
                Color.FromArgb(255, 190, 135),
                Color.FromArgb(245, 145, 65)
            );
            SetLightGradientStyle(
                style,
                Resources.style_light_grad_yellow,
                "細枠淡色グラデーション黄",
                yellowLight1,
                yellowLight2,
                yellowBorder
            );


            SetDeepGradientStyle(
                style,
                Resources.style_deep_grad_black,
                "細枠濃色グラデーション黒",
                Color.FromArgb(100, 100, 100),
                Color.Black,
                Color.Black
            );
            SetDeepGradientStyle(
                style,
                Resources.style_deep_grad_blue,
                "細枠濃色グラデーション青",
                Color.FromArgb(60, 125, 200),
                Color.FromArgb(45, 95, 150),
                Color.FromArgb(75, 125, 190)
            );
            SetDeepGradientStyle(
                style,
                Resources.style_deep_grad_red,
                "細枠濃色グラデーション赤",
                Color.FromArgb(205, 60, 55),
                Color.FromArgb(155, 45, 40),
                Color.FromArgb(190, 75, 70)
            );
            SetDeepGradientStyle(
                style,
                Resources.style_deep_grad_green,
                "細枠濃色グラデーション緑",
                Color.FromArgb(155, 200, 70),
                Color.FromArgb(120, 150, 55),
                Color.FromArgb(150, 185, 85)
            );
            SetDeepGradientStyle(
                style,
                Resources.style_deep_grad_violet,
                "細枠濃色グラデーション紫",
                Color.FromArgb(125, 90, 170),
                Color.FromArgb(95, 65, 125),
                Color.FromArgb(125, 95, 160)
            );
            SetDeepGradientStyle(
                style,
                Resources.style_deep_grad_sblue,
                "細枠濃色グラデーション水色",
                Color.FromArgb(50, 180, 215),
                Color.FromArgb(40, 135, 160),
                Color.FromArgb(70, 170, 200)
            );
            SetDeepGradientStyle(
                style,
                Resources.style_deep_grad_orange,
                "細枠濃色グラデーションオレンジ",
                Color.FromArgb(255, 140, 40),
                Color.FromArgb(200, 110, 30),
                Color.FromArgb(245, 145, 65)
            );
            SetDeepGradientStyle(
                style,
                Resources.style_deep_grad_yellow,
                "細枠濃色グラデーション黄",
                yellowDeep1,
                yellowDeep2,
                yellowBorder
            );

            AddCategory(style);

            ResumeLayout();

            _isPrepared = true;
        }

        private void SetLightGradientStyle(
            SelectorCategory cate,
            Image image, string text,
            Color color1, Color color2, Color borderColor
        ) {
            cate.AddLabel(
                image,
                text,
                () => {
                    SetStyle(
                        Color.Black,
                        new GradientBrushDescription(color1, color2, 90f),
                        true,
                        borderColor,
                        1,
                        DashStyle.Solid
                    );
                }
            );

        }

        private void SetDeepGradientStyle(
            SelectorCategory cate,
            Image image, string text,
            Color color1, Color color2, Color borderColor
        ) {
            cate.AddLabel(
                image,
                text,
                () => {
                    SetStyle(
                        Color.White,
                        new GradientBrushDescription(color1, color2, 90f),
                        true,
                        borderColor,
                        1,
                        DashStyle.Solid
                    );
                }
            );

        }

        private void SetLightSolidStyle(
            SelectorCategory cate,
            Image image, string text,
            Color color, Color borderColor
        ) {
            cate.AddLabel(
                image,
                text,
                () => {
                    SetStyle(
                        Color.Black,
                        new SolidBrushDescription(color),
                        true,
                        borderColor,
                        2,
                        DashStyle.Solid
                    );
                }
            );

        }

        private void SetDeepSolidStyle(
            SelectorCategory cate,
            Image image, string text,
            Color color, Color borderColor
        ) {
            cate.AddLabel(
                image,
                text,
                () => {
                    SetStyle(
                        Color.White,
                        new SolidBrushDescription(color),
                        true,
                        borderColor,
                        2,
                        DashStyle.Solid
                    );
                }
            );

        }

        private void SetHollowStyle(
            SelectorCategory cate,
            Image image, string text,
            Color borderColor
        ) {
            cate.AddLabel(
                image,
                text,
                () => {
                    SetStyle(
                        Color.Black,
                        null,
                        true,
                        borderColor,
                        2,
                        DashStyle.Solid
                    );
                }
            );

        }

        private void SetStyle(
            Color fontColor,
            IBrushDescription newBackground,
            bool newIsBorderEnabled,
            Color newBorderColor,
            int newBorderWidth,
            DashStyle newBorderDashStyle
        ) {
            var canvas = _form.EditorCanvas;
            if (canvas == null) {
                return;
            }

            var cmd = new CompositeCommand();
            var selecteds = canvas.SelectionManager.SelectedEditors.ToArray();
            foreach (var target in selecteds) {
                var shape = target.Model as MemoStyledText;
                if (shape != null) {
                    cmd.Chain(
                        new SetStyledTextColorCommand(
                            target,
                            () => shape.StyledText,
                            flow => fontColor
                        )
                    );

                    cmd.Chain(
                        new SetNodeBackgroundCommand(target, newBackground)
                    );

                    cmd.Chain(
                        new SetNodeBorderCommand(
                            target,
                            newIsBorderEnabled,
                            newBorderColor,
                            newBorderWidth,
                            newBorderDashStyle
                        )
                    );
                }
            }

            using (canvas.RootFigure.DirtManager.BeginDirty()) {
                canvas.CommandExecutor.Execute(cmd);
            }
        }
    }
}
