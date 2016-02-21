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
using Mkamo.Common.DataType;

namespace Mkamo.Editor.Roles {
    using FormatAndExporter = Tuple<string, EditorExporter>;
    using Mkamo.Editor.Requests;
    using Mkamo.Common.Command;
    using Mkamo.Editor.Commands;

    public class ExportRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private List<FormatAndExporter> _formatAndExporter;

        // ========================================
        // constructor
        // ========================================
        public ExportRole() {
            _formatAndExporter = new List<FormatAndExporter>();
        }

        // ========================================
        // property
        // ========================================
        protected List<FormatAndExporter> _FormatAndExporter {
            get { return _formatAndExporter; }
        }

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            if (request.Id != RequestIds.Export) {
                return false;
            }

            var req = request as ExportRequest;
            if (req == null) {
                return false;
            }

            foreach (var registered in _formatAndExporter) {
                if (req.Format == registered.Item1) {
                    return true;
                }
            }

            return false;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as ExportRequest;
            if (req == null) {
                return null;
            }

            var exporter = default(EditorExporter);
            foreach (var registered in _formatAndExporter) {
                if (req.Format == registered.Item1) {
                    exporter = registered.Item2;
                }
            }
            if (exporter == null) {
                return null;
            }
            
            return new ExportCommand(
                _Host, exporter, req.OutputPath
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            return null;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
        }

        // === ExportRole ==========
        public virtual void RegisterExporter(string format, EditorExporter exporter) {
            _formatAndExporter.Add(Tuple.Create(format, exporter));
        }
    }
}
