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
using Mkamo.Editor.Requests;
using System.Drawing;
using Mkamo.Editor.Commands;
using Mkamo.Common.Command;
using System.Windows.Forms;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.DataType;

namespace Mkamo.Editor.Roles.Container {
    using FormatAndPaster = Tuple<string, EditorPaster>;

    public class PasteRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private List<FormatAndPaster> _formatAndPasters;

        // ========================================
        // constructor
        // ========================================
        public PasteRole() {
            _formatAndPasters = new List<FormatAndPaster>();
        }

        // ========================================
        // property
        // ========================================
        protected List<FormatAndPaster> _FormatAndPasters {
            get { return _formatAndPasters; }
        }

        // ========================================
        // method
        // ========================================

        // === IRole ==========
        public override bool CanUnderstand(IRequest request) {
            if (request.Id != RequestIds.Paste) {
                return false;
            }

            var data = Clipboard.GetDataObject();

            foreach (var provider in _FormatAndPasters) {
                var format = DataFormats.GetFormat(provider.Item1);
                if (data.GetDataPresent(format.Name)) {
                    return true;
                }
            }

            return EditorFactory.CanRestoreDataObject(data, _Host);
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as PasteRequest;
            if (req == null) {
                return null;
            }

            var data = Clipboard.GetDataObject();

            var found = Tuple.Create((string) null, (EditorPaster) null);
            foreach (var fmtAndPaster in _FormatAndPasters) {
                var format = DataFormats.GetFormat(fmtAndPaster.Item1);
                if (data.GetDataPresent(format.Name)) {
                    found = fmtAndPaster;
                    break;
                }
            }

            return new PasteCommand(
                _Host, GetGridAdjustedPoint(req.Location), req.Description, found.Item1, found.Item2
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            return null;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
        }


        // === PasteRole ==========
        public virtual void RegisterPaster(string format, EditorPaster paster) {
            _FormatAndPasters.Add(Tuple.Create(format, paster));
        }
    }
}
