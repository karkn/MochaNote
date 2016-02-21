/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.TreeView;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Forms.Input;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Collection;
using Mkamo.Common.String;
using Mkamo.Model.Core;
using Mkamo.Common.Core;
using Mkamo.Common.Forms.KeyMap;
using Mkamo.Memopad.Internal.KeyActions;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class TagSelector: UserControl {
        // ========================================
        // static field
        // ========================================
        private static readonly object NoSuperTagObj = new object();

        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        private Font _captionFont;
        private Font _inputFont;

        private KeyMap<TagSelector> _keyMap;
        private KeyMap<TreeView> _tagTreeViewKeyMap;
        private KeyMap<TextBox> _tagTextBoxKeyMap;
        private KeyMap<TextBox> _createTagTextBoxKeyMap;
        private KeyMap<ComboBox> _superTagComboBoxKeyMap;

        private Memo _targetMemo;

        // ========================================
        // constructor
        // ========================================
        public TagSelector(Memo memo) {
            InitializeComponent();

            _targetMemo = memo;

            _facade = MemopadApplication.Instance;

        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            InitKeyMaps();

            UpdateSuperTagComboBox();
            _superTagComboBox.SelectedItem = NoSuperTagObj;

            UpdateCreateTagButtonEnabled();

            _tagTextBox.LostFocus += HandleLostFocus;
            _tagTreeView.LostFocus += HandleLostFocus;
            _createTagTextBox.LostFocus += HandleLostFocus;
            _createTagButton.LostFocus += HandleLostFocus;
            _superTagComboBox.LostFocus += HandleLostFocus;
        }

        private void InitKeyMaps() {
            _keyMap = new KeyMap<TagSelector>();
            SetKeyMap(_keyMap);

            _tagTextBoxKeyMap = new KeyMap<TextBox>();
            _facade.KeySchema.TextBoxKeyBinder.Bind(_tagTextBoxKeyMap);
            _tagTextBoxKeyMap.SetAction(Keys.Down, textbox => _tagTreeView.Focus());
            _tagTextBoxKeyMap.SetAction(Keys.N | Keys.Control, textbox => _tagTreeView.Focus());

            _tagTreeViewKeyMap = new KeyMap<TreeView>();
            _facade.KeySchema.TreeViewKeyBinder.Bind(_tagTreeViewKeyMap);
            _tagTreeViewKeyMap.SetAction(Keys.Up, MovePreviousLineOrFocusTextBox);
            _tagTreeViewKeyMap.SetAction(Keys.P | Keys.Control, MovePreviousLineOrFocusTextBox);

            _createTagTextBoxKeyMap = new KeyMap<TextBox>();
            _facade.KeySchema.TextBoxKeyBinder.Bind(_createTagTextBoxKeyMap);

            _superTagComboBoxKeyMap = new KeyMap<ComboBox>();
            _facade.KeySchema.ComboBoxKeyBinder.Bind(_superTagComboBoxKeyMap);
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler RequireClose;

        // ========================================
        // property
        // ========================================
        public Font CaptionFont {
            set {
                _captionFont = value;
                Font = _captionFont;
            }
        }

        public Font InputFont {
            set {
                _inputFont = value;
                _tagTreeView.Font = _inputFont;
            }
        }

        public TextBox TagTextBox {
            get { return _tagTextBox.TextBox; }
        }

        public TreeView TagTreeView {
            get { return _tagTreeView; }
        }

        // ========================================
        // method
        // ========================================
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (KeyMapUtil.ProcessKeyMap(_keyMap, this, keyData)) {
                return true;
            }

            if (_tagTreeView.Focused) {
                if (KeyMapUtil.ProcessKeyMap(_tagTreeViewKeyMap, _tagTreeView, keyData)) {
                    return true;
                }
            }

            if (_tagTextBox.Focused) {
                if (KeyMapUtil.ProcessKeyMap(_tagTextBoxKeyMap, _tagTextBox.TextBox, keyData)) {
                    return true;
                }
            }

            if (_createTagTextBox.Focused) {
                if (KeyMapUtil.ProcessKeyMap(_createTagTextBoxKeyMap, _createTagTextBox.TextBox, keyData)) {
                    return true;
                }
            }

            if (_superTagComboBox.Focused) {
                if (KeyMapUtil.ProcessKeyMap(_superTagComboBoxKeyMap, _superTagComboBox.ComboBox, keyData)) {
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ------------------------------
        // public
        // ------------------------------
        public void InitUI() {
            InitUI(true);
        }

        public void InitUI(bool init) {
            _tagTextBox.Clear();
            _createTagTextBox.Clear();
            UpdateTagTreeView(init);
            if (_targetMemo != null) {
                UpdateTagTreeViewChecked(_targetMemo);
            }
            UpdateSuperTagComboBox();
            _superTagComboBox.SelectedItem = NoSuperTagObj;
        }

        public void ReflectChecksToMemo(Memo memo) {
            Contract.Requires(memo != null);

            var currents = memo.Tags.ToArray();
            var checkeds = GetCheckedTags().ToArray();

            var removings = currents.Except(checkeds);
            var addings = checkeds.Except(currents);

            foreach (var removing in removings) {
                memo.RemoveTag(removing);
            }
            foreach (var adding in addings) {
                memo.AddTag(adding);
            }
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnRequireClose() {
            var handler = RequireClose;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void SetKeyMap(IKeyMap<TagSelector> keyMap) {
            keyMap.SetAction(
                Keys.Enter,
                selector => {
                    if (_tagTextBox.Focused || _tagTreeView.Focused) {
                        OnRequireClose();
                    } else if (_createTagTextBox.Focused || _createTagButton.Focused) {
                        CreateTag();
                    } else if (_superTagComboBox.Focused) {
                        _superTagComboBox.DroppedDown = !_superTagComboBox.DroppedDown;
                    }
                }
            );

            keyMap.SetAction(
                Keys.C | Keys.Alt,
                selector => {
                    _createTagButton.Focus();
                    _createTagButton.PerformClick();
                }
            );

            keyMap.SetAction(
                Keys.S | Keys.Alt,
                selector => {
                    _superTagComboBox.Focus();
                    _superTagComboBox.DroppedDown = true;
                }
            );
        }

        private void UpdateTagTreeView(bool init) {
            var cond = _tagTextBox.Text;

            var oldChecked = GetCheckedTags().ToArray();

            Predicate<MemoTag> pred = null;
            if (!StringUtil.IsNullOrWhitespace(cond)) {
                var names = cond.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                pred = tag => {
                    //if (names.Any(name => !StringUtil.IsNullOrWhitespace(name) && tag.FullName.IndexOf(name.Trim(), StringComparison.OrdinalIgnoreCase) > -1)) {
                    if (
                        names.Where(
                            name => !StringUtil.IsNullOrWhitespace(name)
                        ).All(
                            name => tag.FullName.IndexOf(name.Trim(), StringComparison.OrdinalIgnoreCase) > -1
                        )
                    ) {
                        return true;
                    }
                    if (init) {
                        return _targetMemo != null && tag.Memos.Contains(_targetMemo);
                    } else {
                        return oldChecked.Contains(tag);
                    }
                };
            }

            _tagTreeView.BeginUpdate();
            TreeViewUtil.UpdateTagTreeView(_tagTreeView, _facade.Workspace, pred);
            _tagTreeView.Sort();
            _tagTreeView.ExpandAll();
            _tagTreeView.EndUpdate();

            if (_tagTreeView.Nodes.Count > 0) {
                _tagTreeView.SelectedNode = _tagTreeView.Nodes[0];
            }

            if (_targetMemo != null) {
                if (!init) {
                    UpdateTagTreeViewChecked(oldChecked);
                }
            }
        }

        private void UpdateTagTreeViewChecked(Memo memo) {
            UpdateTagTreeViewChecked(memo.Tags);
        }

        private void UpdateTagTreeViewChecked(IEnumerable<MemoTag> tags) {
            var ite = new TreeNodeIterator(_tagTreeView);
            foreach (var node in ite) {
                if (tags.Contains(node.Tag as MemoTag)) {
                    node.Checked = true;
                }
            }
        }

        private void UpdateCreateTagButtonEnabled() {
            _createTagButton.Enabled = !StringUtil.IsNullOrWhitespace(_createTagTextBox.Text);
        }

        private void UpdateSuperTagComboBox() {
            var tags = new List<MemoTag>(_facade.Workspace.Tags);
            tags.Sort(GetMemoTagFullNameComparer());

            _superTagComboBox.Items.Clear();
            _superTagComboBox.BeginUpdate();
            _superTagComboBox.Items.Add(NoSuperTagObj);
            _superTagComboBox.Items.AddRange(tags.ToArray());
            _superTagComboBox.EndUpdate();
        }

        private IComparer<MemoTag> GetMemoTagFullNameComparer() {
            return new DelegatingComparer<MemoTag>(
                (tag1, tag2) => tag1.FullName.CompareTo(tag2.FullName)
            );
        }

        private IEnumerable<MemoTag> GetCheckedTags() {
            var ite = new TreeNodeIterator(_tagTreeView);
            foreach (var node in ite) {
                if (node.Checked) {
                    yield return node.Tag as MemoTag;
                }
            }
        }

        private void SetTagChecked(MemoTag tag, bool value) {
            var ite = new TreeNodeIterator(_tagTreeView);
            foreach (var node in ite) {
                if (tag == node.Tag) {
                    node.Checked = value;
                }
            }
        }

        private void CreateTag() {
            if (StringUtil.IsNullOrWhitespace(_createTagButton.Text)) {
                return;
            }

            var created = _facade.Workspace.CreateTag();
            created.Name = _createTagTextBox.Text;
            if (_superTagComboBox.SelectedItem != NoSuperTagObj) {
                created.SuperTag = _superTagComboBox.SelectedItem as MemoTag;
            }

            InitUI(false);
            SetTagChecked(created, true);
        }


        // --- key action ---
        private void MovePreviousLineOrFocusTextBox(TreeView treeView) {
            if (treeView.Nodes.Count == 0) {
                _tagTextBox.Focus();
            } else if (treeView.SelectedNode == treeView.Nodes[0]) {
                _tagTextBox.Focus();
            } else {
                TreeViewKeyActions.MovePreviousLine(treeView);
            }
        }

        // --- handler ---
        private void HandleLostFocus(object sender, EventArgs e) {
            if (!ContainsFocus) {
                OnRequireClose();
            }
        }

        // --- generated handler ---
        private void _tagTextBox_TextChanged(object sender, EventArgs e) {
            UpdateTagTreeView(false);
            _createTagTextBox.Text = _tagTextBox.Text;
        }
        private void _createTagButton_Click(object sender, EventArgs e) {
            CreateTag();
        }

        private void _createTagTextBox_TextChanged(object sender, EventArgs e) {
            UpdateCreateTagButtonEnabled();
        }

        private void _superTagComboBox_Format(object sender, ListControlConvertEventArgs e) {
            if (e.ListItem is MemoTag) {
                e.Value = ((MemoTag) e.ListItem).GetIndentedName(4);
            } else {
                e.Value = "親タグなし (Alt+S)";
            }
        }
    }
}
