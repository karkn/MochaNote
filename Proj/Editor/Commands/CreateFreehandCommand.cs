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
using Mkamo.Editor.Internal.Core;
using Mkamo.Editor.Requests;
using Mkamo.Common.Command;
using Mkamo.Figure.Figures;

namespace Mkamo.Editor.Commands {
    public class CreateFreehandCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IModelFactory _modelFactory;
        private IEnumerable<Point> _points;
        private int _width;
        private Color _color;

        private IEditor _createdEditor;

        // ========================================
        // constructor
        // ========================================
        public CreateFreehandCommand(
            IEditor target, IModelFactory modelFactory,
            IEnumerable<Point> points, int width, Color color
        ) {
            _target = target;
            _modelFactory = modelFactory;
            _points = points;
            _width = width;
            _color = color;
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
                    _points != null &&
                    _points.Count() > 1 &&
                    _width > 0 &&
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

        public IEditor CreatedEditor {
            get { return _createdEditor; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var model = _modelFactory.CreateModel();

            _createdEditor = _target.AddChild(model);
            var freehand = _createdEditor.Figure as FreehandFigure;
            freehand.SetPoints(_points);
            freehand.BorderWidth = _width;
            freehand.Foreground = _color;
            _createdEditor.Enable();

            //var select = new SelectRequest();
            //select.DeselectOthers = true;
            //select.Value = SelectKind.True;
            //_createdEditor.PerformRequest(select);
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
             var freehand = _createdEditor.Figure as FreehandFigure;
            freehand.SetPoints(_points);
            freehand.BorderWidth = _width;
            freehand.Foreground = _color;
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
