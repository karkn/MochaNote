/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using System.Windows.Forms;
using Mkamo.Figure.Core;
using Mkamo.StyledText.Core;
using System.Drawing;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using Mkamo.Common.Forms.Input;
using Mkamo.Memopad.Internal.Utils;
using System.Diagnostics;
using Mkamo.Editor.Core;
using Mkamo.Common.Command;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.MouseOperatable;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Control.Core;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoTextEditorHandle: DefaultEditorHandle {
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;

        private bool _isSelectedOnMouseDown;
        private bool _toggle;

        private Run _lastHovered;
        private Popup _previewPopup;

        // ========================================
        // constructor
        // ========================================
        public MemoTextEditorHandle() {
            _app = MemopadApplication.Instance;

            _isSelectedOnMouseDown = false;
            _toggle = false;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override Cursor GetMouseCursor(MouseEventArgs e) {
            var run = GetRun(e.Location);
            if (run != null && run.HasLink) {
                return Cursors.Hand;
            } else {

                var node = _Figure as INode;
                if (node != null) {
                    if (node.IsInBullet(e.Location, ListKind.CheckBox | ListKind.TriStateCheckBox)) {
                        return Cursors.Default;
                    }
                }

                return Cursors.IBeam;
            }
        }

        protected override void OnFigureMouseDown(MouseEventArgs e) {
            var handled = false;

            if (
                (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle) &&
                !KeyUtil.IsAltPressed() &&
                !KeyUtil.IsControlPressed() &&
                !KeyUtil.IsShiftPressed()
            ) {
                var run = GetRun(e.Location);
                if (run != null && run.HasLink) {
                    LinkUtil.GoLink(run.Link);
                    handled = true;
                }
            }

            /// checkbox
            if (e.Button == MouseButtons.Left) {
                var node = _Figure as INode;
                if (node != null) {
                    var bulcmd = node.GetProcessCheckBoxBulletCommand(e.Location);
                    if (bulcmd != null) {
                        /// bulcmdはFigureのStyledTextを変更するだけ
                        /// ModelのStyledTextを変えないといけない
                        bulcmd.Execute();

                        var model = Host.Model as MemoText;
                        var oldStext = model.StyledText;
                        var newStext = node.StyledText.CloneDeeply() as StyledText.Core.StyledText;
                        var cmd = new DelegatingCommand(
                            () => {
                                model.StyledText = newStext;
                            },
                            () => {
                                model.StyledText = oldStext;
                            }
                        );
                        Host.Site.CommandExecutor.Execute(cmd);
                        handled = true;
                    }
                }
            }


            if (!handled) {
                _isSelectedOnMouseDown = Host.IsSelected;
                _toggle = KeyUtil.IsControlPressed();

                /// EditorがFocusされていたらCommitしておく
                var focused = Host.Site.FocusManager.FocusedEditor;
                if (focused != null) {
                    focused.RequestFocusCommit(true);
                }

                Host.RequestSelect(SelectKind.True, !_toggle && !Host.IsSelected);
                if (e.Button == MouseButtons.Left && !KeyUtil.IsControlPressed()) {
                    Host.RequestFocus(FocusKind.Begin, e.Location);
                    Host.Site.EditorCanvas.EventDispatcher.SetDnDTarget(Host.Focus.Figure);
                }
                base.OnFigureMouseDown(e);
            }
        }

        protected override void OnFigureMouseUp(MouseEventArgs e) {
            if (_toggle && _isSelectedOnMouseDown) {
                Host.RequestSelect(SelectKind.False, false);
            }
            base.OnFigureMouseUp(e);
        }

        protected override void OnFigureMouseHover(MouseHoverEventArgs e) {
            base.OnFigureMouseHover(e);

            if (Form.ActiveForm != _app.MainForm || !Host.Site.EditorCanvas.Visible) {
                return;
            }

            var run = GetRun(e.Location);
            if (run != _lastHovered) {
                if (run != null && run.HasLink) {
                    ShowPreview(run.Link, e.Location);
                }
            }

            e.ResetHover = true;
            _lastHovered = run;
        }

        protected override void OnFigureMouseMove(MouseEventArgs e) {
            base.OnFigureMouseMove(e);

            var run = GetRun(e.Location);
            if (run != _lastHovered) {
                DisposePreviewPopup();
            }
        }

        protected override void OnFigureMouseLeave() {
            _lastHovered = null;
            DisposePreviewPopup();
            base.OnFigureMouseLeave();
        }

        // ------------------------------
        // private
        // ------------------------------
        private void ShowPreview(Link link, Point loc) {
            if (link != null && UriUtil.IsMemoUri(link.Uri)) {
                var info = UriUtil.GetMemoInfo(link.Uri);

                if (info != null) {
                    var bmp = default(Bitmap);
                    if (_app.IsLoadedMemo(info)) {
                        /// Loadされていればそのcanvasからbitmapを作る
                        var content = _app.FindPageContent(info);
                        bmp = MemoOutlineUtil.CreateOutline(content.EditorCanvas);

                    } else {
                        bmp = MemoOutlineUtil.LoadOrSaveAndLoadOutline(info);
                    }

                    DisposePreviewPopup();

                    var canvas = Host.Site.EditorCanvas;
                    var bounds = _Figure.Bounds;
                    var screen = Screen.FromRectangle(bounds);
                    var outline = new OutlinePreviewControl(screen, info.Title, bmp, 1f);
                    _previewPopup = new Popup(outline);
                    /// AutoClose = falseにしておかないとPopupにフォーカスを取られてしまう
                    _previewPopup.AutoClose = false;
                    _previewPopup.Tag = bmp;

                    var locDelta = 20;
                    var locOnScreen = canvas.PointToScreen(canvas.TranslateToControlPoint(loc));
                    var preferredSize = outline.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));

                    var isEnoughLowerSpace = preferredSize.Height < screen.WorkingArea.Height - (locOnScreen.Y + locDelta) - 2;
                    var isLocUpper = locOnScreen.Y <= screen.WorkingArea.Height / 2;
                    var y = 0;
                    if (isEnoughLowerSpace || isLocUpper) {
                        outline.MaxHeight = screen.WorkingArea.Height - (locOnScreen.Y + locDelta) - 2;
                        y = locOnScreen.Y + locDelta;
                    } else {
                        outline.MaxHeight = locOnScreen.Y - locDelta - 2;
                        y = locOnScreen.Y - locDelta - Math.Min(preferredSize.Height, outline.MaxHeight);
                    }

                    var isEnoughRighterSpace = preferredSize.Width < screen.WorkingArea.Width - (locOnScreen.X + locDelta) - 2;
                    var isLocLefter = locOnScreen.X <= screen.WorkingArea.Width / 2;
                    var x = 0;
                    if (isEnoughRighterSpace || isLocLefter) {
                        outline.MaxWidth = screen.WorkingArea.Width - (locOnScreen.X + locDelta) - 2;
                        x = locOnScreen.X + locDelta;
                    } else {
                        outline.MaxWidth = locOnScreen.X - locDelta - 2;
                        x = locOnScreen.X - locDelta - Math.Min(preferredSize.Width, outline.MaxWidth);
                    }

                    _previewPopup.Show(new Point(x, y));
                }
            }
        }

        private void DisposePreviewPopup() {
            if (_previewPopup != null) {
                var bmp = _previewPopup.Tag as Bitmap;
                if (bmp != null) {
                    bmp.Dispose();
                }
            }

            if (_previewPopup != null && !_previewPopup.IsDisposed) {
                _previewPopup.Close();
                _previewPopup.Dispose();
            }
        }

        private Run GetRun(Point loc) {
            var fig = _Figure as INode;
            return fig.GetInlineAt(loc) as Run;
        }
    }
}
