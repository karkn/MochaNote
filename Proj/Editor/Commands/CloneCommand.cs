/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Common.Externalize;
using Mkamo.Editor.Internal.Core;
using System.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Common.Command;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Mkamo.Figure.Figures;

namespace Mkamo.Editor.Commands {
    public class CloneCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IEnumerable<IEditor> _cloningEditors;
        private Size _moveDelta;

        private IEnumerable<IEditor> _clonedEditors;
        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public CloneCommand(IEditor target, IEnumerable<IEditor> cloning, Size moveDelta) {
            _target = target;
            _cloningEditors = cloning;
            _moveDelta = moveDelta;
        }


        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _cloningEditors != null && _target.IsContainer; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public IEnumerable<IEditor> ClonedEditors {
            get { return _clonedEditors; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var containerCtrl = _target.Controller as IContainerController;

            var clonings = new List<IEditor>();
            var copyReq = new CopyRequest(_cloningEditors);
            foreach (var cloning in _cloningEditors) {
                if (cloning.CanUnderstand(copyReq)) {
                    clonings.Add(cloning);
                }
            }

            var data = EditorFactory.CreateDataObject(clonings);
            _clonedEditors = EditorFactory.RestoreDataObject(data, _target.Site.ControllerFactory);

            _command = new AddEditorsCommand(_target, _clonedEditors, _moveDelta);
            _command.Execute();
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
        }

        public override void Redo() {
            if (_command != null) {
                _command.Redo();
            }
        }
    }
}
