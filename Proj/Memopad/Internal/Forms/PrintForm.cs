/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class PrintForm: Form {
        private Form _owner;
        private EditorCanvas _targetCanvas;
        private int _currentPrintPage = 0;

        private PrintDocument _document;

        public PrintForm(Form owner, EditorCanvas targetCanvas) {
            InitializeComponent();

            _owner = owner;
            _targetCanvas = targetCanvas;

            _document = new PrintDocument();
            _document.PrintPage += HandlePrintDocumentPrintPage;

            _fitNoneRadioButton.Checked = true;
        }

        private void CleanUp() {
            if (_document != null) {
                _document.Dispose();
            }
        }

        private PrintFitKind FitKind {
            get {
                if (_fitHorizontalRadioButton.Checked) {
                    return PrintFitKind.Horizontal;
                } else if (_fitVerticalRadioButton.Checked) {
                    return PrintFitKind.Vertical;
                } else if (_fitBothRadioButton.Checked) {
                    return PrintFitKind.Both;
                } else {
                    return PrintFitKind.None;
                }
            }
        }

        private void HandlePrintDocumentPrintPage(object sender, PrintPageEventArgs e) {
            if (_targetCanvas == null) {
                return;
            }

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.Default;

            var pages = _targetCanvas.CalcPrintPages(g, e.MarginBounds, 30, FitKind);
            if (_currentPrintPage < pages) {
                _targetCanvas.PrintPages(g, e.MarginBounds, _currentPrintPage, 30, FitKind);

                ++_currentPrintPage;
                if (_currentPrintPage < pages) {
                    e.HasMorePages = true;
                } else {
                    e.HasMorePages = false;

                    /// これがないとPrintPreviewDialogの印刷ボタンを押されたときに
                     /// _currentPrintPageがpagesと同じ値になって白紙が印刷される
                    _currentPrintPage = 0;
                }
            }
        }

        private void _printButton_Click(object sender, EventArgs e) {
            if (_targetCanvas  == null) {
                return;
            }

            _currentPrintPage = 0;
            _document.Print();
        }

        private void _previewButton_Click(object sender, EventArgs e) {
            if (_targetCanvas  == null) {
                return;
            }

            _currentPrintPage = 0;

            using (var preview = new PrintPreviewDialog()) {
                var bounds = _owner.Bounds;
                preview.StartPosition = FormStartPosition.Manual;
                preview.SetBounds(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
                //preview.PrintPreviewControl.Zoom = 1;

                preview.Document = _document;
                preview.ShowDialog(this);
            }
        }

        private void _pageSetupButton_Click(object sender, EventArgs e) {
            using (var dialog = new PageSetupDialog()) {
                dialog.ShowHelp = true;
                dialog.EnableMetric = true;
                dialog.Document = _document;
                dialog.ShowDialog(this);
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e) {
            Close();
        }

    }
}
