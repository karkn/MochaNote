/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Common.DataType;
using System.Windows.Forms;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.Common.Forms.Clipboard;
    using Mkamo.StyledText.Core;
    using Mkamo.StyledText.Writer;

    public class CopyCommand: AbstractCommand {
        // ========================================
        // static field
        // ========================================
        public static bool SupportHtml = true;

        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private Range _range;

        // ========================================
        // constructor
        // ========================================
        public CopyCommand(StyledText target, Range range) {
            _target = target;
            _range = range;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && !_range.IsEmpty; }
        }

        public override bool CanUndo {
            get { return false; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {

            var data = new DataObject();
            var copy = _target.CopyBlocksAndInlines(_range);

            data.SetData(StyledTextConsts.BlocksAndInlinesFormat.Name, false, copy);

            /// plain text
            {
                var writer = new PlainTextWriter();
                var text = writer.ToPlainText(copy);
                data.SetText(text, TextDataFormat.UnicodeText);
            }

            /// html
            if (SupportHtml) {
                var writer = new HtmlWriter();
                var html = writer.ToHtml(copy);
                data.SetData(DataFormats.Html, ClipboardUtil.GetCFHtmlMemoryStream(html));
            }

            Clipboard.SetDataObject(data, true);
        }

        public override void Undo() {
        }
    }
}
