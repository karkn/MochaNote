/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class MemoTableUIProvider: AbstractMemoContentUIProvider {
        // ========================================
        // field
        // ========================================
        private MemoTableController _owner;

        private ToolStripMenuItem _export;

        // ========================================
        // constructor
        // ========================================
        public MemoTableUIProvider(MemoTableController owner): base(owner, false) {
            _owner = owner;
        }

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_export == null) {
                InitItems();
            }

            _ContextMenu.Items.Clear();

            var selectionCount = _owner.Host.Site.SelectionManager.SelectedEditors.Count();

            if (selectionCount == 1) {
                _ContextMenu.Items.Add(_export);
            }

            return _ContextMenu;
        }

        private void InitItems() {
            _export = new ToolStripMenuItem("取り出す(&E)");
            var exportCsv = new ToolStripMenuItem("CSV(&C)...");
            exportCsv.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var dialog = new SaveFileDialog();
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.FileName = "Export.csv";
                dialog.Filter = "Csv Files(*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK) {
                    var outputPath = dialog.FileName;
                    _owner.Host.RequestExport("Csv", outputPath);
                }
            };
            _export.DropDownItems.Add(exportCsv);
        }
    }
}
