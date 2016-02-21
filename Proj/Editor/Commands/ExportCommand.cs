/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using System.IO;

namespace Mkamo.Editor.Commands {
    public class ExportCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private EditorExporter _exporter;
        private string _outputPath;

        // ========================================
        // constructor
        // ========================================
        public ExportCommand(IEditor target, EditorExporter exporter, string outputPath) {
            _target = target;
            _exporter = exporter;
            _outputPath = outputPath;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                return
                    _target != null && _exporter != null &&
                    _outputPath != null && !Directory.Exists(_outputPath);
            }
        }

        public override bool CanUndo {
            get { return false; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _exporter(_target, _outputPath);
        }

        public override void Undo() {
        }
    }
}
