/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Drawing;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Editor.Commands {
    public class CreateNodeCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IModelFactory _modelFactory;
        private Rectangle _bounds;

        private IEditor _createdEditor;

        // ========================================
        // constructor
        // ========================================
        public CreateNodeCommand(IEditor target, IModelFactory modelFactory, Rectangle bounds) {
            _target = target;
            _modelFactory = modelFactory;
            _bounds = bounds;
        }


        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                var containerCtl = _target.Controller as IContainerController;
                return
                    _target != null &&
                    _target is Mkamo.Editor.Internal.Editors.Editor &&
                    _modelFactory != null &&
                    containerCtl != null &&
                    containerCtl.CanContainChild(_modelFactory.ModelDescriptor);
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public IEditor Target {
            get { return _target; }
        }

        public IModelFactory ModelFactory {
            get { return _modelFactory; }
        }

        public Rectangle Bounds {
            get { return _bounds; }
        }

        public IEditor CreatedEditor {
            get { return _createdEditor; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var model = _modelFactory.CreateModel();

            _createdEditor = _target.AddChild(model);
            _createdEditor.Figure.Bounds = _bounds;
            _createdEditor.Enable();

            var select = new SelectRequest();
            select.DeselectOthers = true;
            select.Value = SelectKind.True;
            _createdEditor.PerformRequest(select);
        }

        public override void Undo() {
            var model = _createdEditor.Model;
            var containerCtl = _target.Controller as IContainerController;
            if (containerCtl != null) {
                _target.RemoveChildEditor(_createdEditor);
                containerCtl.RemoveChild(model);
                _createdEditor.Disable();
                _createdEditor.Controller.DisposeModel(model);
            }
        }

        public override void Redo() {
            var containerCtrl = _target.Controller as IContainerController;

            _createdEditor.Controller.RestoreModel(_createdEditor.Model);
            _createdEditor.Figure.Bounds = _bounds;
            _target.AddChildEditor(_createdEditor);
            containerCtrl.InsertChild(_createdEditor.Model, containerCtrl.ChildCount);
            _createdEditor.Enable();
            
            var select = new SelectRequest();
            select.DeselectOthers = true;
            select.Value = SelectKind.True;
            _createdEditor.PerformRequest(select);
        }
    }
}
