/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Editor.Controllers;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal abstract class AbstractUmlFeatureUIProvider: AbstractUIProvider {
        // ========================================
        // field
        // ========================================
        private AbstractController _owner;

        private ToolStripMenuItem _up;
        private ToolStripMenuItem _upMost;
        private ToolStripMenuItem _down;
        private ToolStripMenuItem _downMost;

        // ========================================
        // constructor
        // ========================================
        public AbstractUmlFeatureUIProvider(AbstractController owner): base(true) {
            _owner = owner;
        }


        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_up == null || _upMost == null || _down == null || _downMost == null) {
                InitItems();
            }
            UpdateItemEnabled();
            _ContextMenu.Items.Clear();
            _ContextMenu.Items.Add(_up);
            _ContextMenu.Items.Add(_down);
            _ContextMenu.Items.Add(_upMost);
            _ContextMenu.Items.Add(_downMost);
            return _ContextMenu;
        }
    
        private void InitItems() {
            _up = new ToolStripMenuItem();
            _up.Text = "上に移動(&U)";
            _up.Click += (sender, e) => {
                var target = _owner.Host;
                target.RequestReorder(ReorderKind.Back);
                target.RequestSelect(SelectKind.True, true);
            };

            _down = new ToolStripMenuItem();
            _down.Text = "下に移動(&D)";
            _down.Click += (sender, e) => {
                var target = _owner.Host;
                target.RequestReorder(ReorderKind.Front);
                target.RequestSelect(SelectKind.True, true);
            };

            _upMost = new ToolStripMenuItem();
            _upMost.Text = "一番上に移動(&U)";
            _upMost.Click += (sender, e) => {
                var target = _owner.Host;
                target.RequestReorder(ReorderKind.BackMost);
                target.RequestSelect(SelectKind.True, true);
            };

            _downMost = new ToolStripMenuItem();
            _downMost.Text = "一番下に移動(&D)";
            _downMost.Click += (sender, e) => {
                var target = _owner.Host;
                target.RequestReorder(ReorderKind.FrontMost);
                target.RequestSelect(SelectKind.True, true);
            };
        }


        private void UpdateItemEnabled() {
            var target = _owner.Host;
            var req = new ReorderRequest();
            var cmd = default(ICommand);

            req.Kind = ReorderKind.Back;
            cmd = target.GetCommand(req);
            _up.Enabled = cmd.CanExecute;

            req.Kind = ReorderKind.Front;
            cmd = target.GetCommand(req);
            _down.Enabled = cmd.CanExecute;

            req.Kind = ReorderKind.BackMost;
            cmd = target.GetCommand(req);
            _upMost.Enabled = cmd.CanExecute;

            req.Kind = ReorderKind.FrontMost;
            cmd = target.GetCommand(req);
            _downMost.Enabled = cmd.CanExecute;
        }
    }
}
