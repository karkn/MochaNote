/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using System.Drawing;
using Mkamo.Editor.Core;
using Mkamo.Common.Forms.Input;
using System.Windows.Forms;
using Mkamo.Editor.Commands;

namespace Mkamo.Editor.Handles.Scenarios {
    public class MoveScenario: SelectScenario {
        // ========================================
        // field
        // ========================================
        private ChangeBoundsRequest _moveRequest;

        private Point _startPoint;

        private EditorBundle _targets;

        // ========================================
        // constructor
        // ========================================
        public MoveScenario(IHandle handle): base(handle) {
            _moveRequest = new ChangeBoundsRequest();
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void Apply() {
            base.Apply();
            Handle.DragStart += HandleDragStart;
            Handle.DragMove += HandleDragMove;
            Handle.DragFinish += HandleDragFinish;
            Handle.DragCancel += HandleDragCancel;
        }

        public virtual void HandleDragStart(object sender, MouseEventArgs e) {
            _startPoint = e.Location;

            _targets = new EditorBundle(Handle.Host.Site.SelectionManager.SelectedEditors);
            _moveRequest.MovingEditors = Handle.Host.Site.SelectionManager.SelectedEditors;

            _moveRequest.MoveDelta = Size.Empty;
            Handle.Host.ShowFeedback(_moveRequest);
        }

        public virtual void HandleDragMove(object sender, MouseEventArgs e) {
            using (Handle.Host.Figure.DirtManager.BeginDirty()) {
                _moveRequest.MoveDelta = (Size) e.Location - (Size) _startPoint;
                _targets.ShowFeedback(_moveRequest);
            }
        }

        public virtual void HandleDragFinish(object sender, MouseEventArgs e) {
            using (Handle.Host.Figure.DirtManager.BeginDirty()) {
                var moveDelta = (Size) e.Location - (Size) _startPoint;
                if (KeyUtil.IsControlPressed()) {
                    _targets.HideFeedback(_moveRequest);
                    if (_targets.Editors.Any()) {
                        var target = _targets.Editors.First().Parent;

                        var cloneRequest = new CloneRequest(_targets.Editors);
                        cloneRequest.MoveDelta = moveDelta;

                        var cmd = target.PerformRequest(cloneRequest) as CloneCommand;
                        if (cmd != null && cmd.ClonedEditors != null) {
                            var select = new SelectMultiCommand(cmd.ClonedEditors, SelectKind.True, true);
                            select.Execute();
                        }
                    }

                } else {
                    _moveRequest.MoveDelta = moveDelta;
                    _targets.HideFeedback(_moveRequest);
                    _targets.PerformCompositeRequest(_moveRequest, Handle.Host.Site.CommandExecutor);
                }
                Handle.Host.ShowFeedback(new HighlightRequest());
            }
        }

        public virtual void HandleDragCancel(object sender, EventArgs e) {
            using (Handle.Host.Figure.DirtManager.BeginDirty()) {
                _targets.HideFeedback(_moveRequest);
            }
        }

    }
}
