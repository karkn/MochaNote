/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Commands;
using Mkamo.Model.Memo;
using System.Drawing;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal abstract class AbstractMemoContentUIProvider: AbstractUIProvider {
        // ========================================
        // field
        // ========================================
        private AbstractController _owner;

        private Lazy<ToolStripMenuItem> _cutInNewMemo;

        // ========================================
        // constructor
        // ========================================
        public AbstractMemoContentUIProvider(
            AbstractController owner,
            bool supportDetailForm
        ) : base(supportDetailForm) {
            _owner = owner;
            _cutInNewMemo = new Lazy<ToolStripMenuItem>(() => CreateCutInNewMemo());
        }

        // ========================================
        // property
        // ========================================
        protected AbstractController _Owner {
            get { return _owner; }
        }

        protected ToolStripMenuItem _CutInNewMemo {
            get { return _cutInNewMemo.Value; }
        }

        // ========================================
        // method
        // ========================================
        protected ToolStripMenuItem CreateCutInNewMemo() {
            var ret = new ToolStripMenuItem("切り出す(&C)");
            ret.Click += (sender, ev) => {
                using (var form = new CreateMemoForm()) {
                    var app = MemopadApplication.Instance;
                    form.Font = app.Theme.CaptionFont;
                    form.MemoTitle = "新しいノート";
                    form.ReplaceWithLinkRadioButton.Enabled = false;
                    if (form.ShowDialog() == DialogResult.OK) {
                        var site = _owner.Host.Site;
                        var targets = GetCutInNewMemoTargets();
                        if (targets != null && targets.Any()) {
                            var cloneds = site.EditorCanvas.CloneEditors(targets);

                            app.ActivateMainForm();
                            var info = app.CreateMemo(form.MemoTitle);

                            if (form.OriginalModification == CreateMemoForm.OriginalModificationKind.Remove) {
                                var bundle = new EditorBundle(targets);
                                var remove = bundle.GetCompositeCommand(new RemoveRequest());
                                site.CommandExecutor.Execute(remove);
                            }

                            var pageContent = app.MainForm.FindPageContent(info);
                            var targetCanvas = pageContent.EditorCanvas;
                            var target = targetCanvas.RootEditor.Children.First();
                            var command = new AddEditorsCommand(target, cloneds, Size.Empty);
                            targetCanvas.CommandExecutor.Execute(command);
                        }
                    }
                }
            };
            return ret;
        }

        protected virtual IEnumerable<IEditor> GetCutInNewMemoTargets() {
            return _owner.Host.Site.SelectionManager.SelectedEditors;
        }
    }
}
