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
using System.Windows.Forms;
using Mkamo.Common.Externalize;
using Mkamo.Figure.Core;
using Mkamo.Common.DataType;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Editor.Commands {
    public class CopyCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEnumerable<IEditor> _targets;

        // ========================================
        // constructor
        // ========================================
        public CopyCommand(IEnumerable<IEditor> targets) {
            Contract.Requires(targets != null);
            _targets = targets.ToArray();
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _targets != null && _targets.Any(); }
        }

        public override bool CanUndo {
            get { return false; }
        }

        public IEnumerable<IEditor> Targets {
            get { return _targets; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var data = EditorFactory.CreateDataObject(_targets);

            var man = _targets.First().Site.EditorCopyExtenderManager;
            foreach (var ext in man.Extenders) {
                ext(_targets, data);
            }

            Clipboard.SetDataObject(data, true);
        }

        public override void Undo() {
            throw new NotImplementedException();
        }
    }
}
