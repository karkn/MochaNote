namespace Mkamo.Memopad.Internal.Controls {
    partial class WorkspaceView {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this._folderContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._activateFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._deactivateFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._openAllMemosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this._addAllOpenMemoAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._clearMemosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this._createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._createFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._createAndAddMemoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._removeFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._commonContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._commonSmartFolderCreateSmartFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._commonCreateTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._commonFolderCreateFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._commonEmptyTrashBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._workspaceTreePanel = new System.Windows.Forms.Panel();
            this._workspaceTree = new Mkamo.Memopad.Internal.Controls.WorkspaceTree();
            this._tagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._tagCreateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tagCreateTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tagCreateMemoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tagRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tagRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._smartFolderContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._smartFolderRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._smartFolderRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._editSmartFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._folderContextMenuStrip.SuspendLayout();
            this._commonContextMenuStrip.SuspendLayout();
            this._workspaceTreePanel.SuspendLayout();
            this._tagContextMenuStrip.SuspendLayout();
            this._smartFolderContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _folderContextMenuStrip
            // 
            this._folderContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._folderContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._activateFolderToolStripMenuItem,
            this._deactivateFolderToolStripMenuItem,
            this._openAllMemosToolStripMenuItem,
            this.toolStripMenuItem1,
            this._addAllOpenMemoAToolStripMenuItem,
            this._clearMemosToolStripMenuItem,
            this.toolStripMenuItem3,
            this._createToolStripMenuItem,
            this._removeFolderToolStripMenuItem,
            this._renameToolStripMenuItem});
            this._folderContextMenuStrip.Name = "_folderTreeViewContextMenuStrip";
            this._folderContextMenuStrip.Size = new System.Drawing.Size(251, 214);
            this._folderContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._memoFolderContextMenuStrip_Opening);
            // 
            // _activateFolderToolStripMenuItem
            // 
            this._activateFolderToolStripMenuItem.Name = "_activateFolderToolStripMenuItem";
            this._activateFolderToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this._activateFolderToolStripMenuItem.Text = "アクティブにする(&A)";
            this._activateFolderToolStripMenuItem.Click += new System.EventHandler(this._activateFolderToolStripMenuItem_Click);
            // 
            // _deactivateFolderToolStripMenuItem
            // 
            this._deactivateFolderToolStripMenuItem.Name = "_deactivateFolderToolStripMenuItem";
            this._deactivateFolderToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this._deactivateFolderToolStripMenuItem.Text = "非アクティブにする(&D)";
            this._deactivateFolderToolStripMenuItem.Click += new System.EventHandler(this._deactivateFolderToolStripMenuItem_Click);
            // 
            // _openAllMemosToolStripMenuItem
            // 
            this._openAllMemosToolStripMenuItem.Name = "_openAllMemosToolStripMenuItem";
            this._openAllMemosToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this._openAllMemosToolStripMenuItem.Text = "すべてのノートを開く(&O)";
            this._openAllMemosToolStripMenuItem.Click += new System.EventHandler(this._openAllMemosToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(247, 6);
            // 
            // _addAllOpenMemoAToolStripMenuItem
            // 
            this._addAllOpenMemoAToolStripMenuItem.Name = "_addAllOpenMemoAToolStripMenuItem";
            this._addAllOpenMemoAToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this._addAllOpenMemoAToolStripMenuItem.Text = "開いているノートをすべて追加(&A)";
            this._addAllOpenMemoAToolStripMenuItem.Click += new System.EventHandler(this._addAllOpenMemoAToolStripMenuItem_Click);
            // 
            // _clearMemosToolStripMenuItem
            // 
            this._clearMemosToolStripMenuItem.Name = "_clearMemosToolStripMenuItem";
            this._clearMemosToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this._clearMemosToolStripMenuItem.Text = "すべてのノートをクリアファイルから削除(&C)";
            this._clearMemosToolStripMenuItem.Click += new System.EventHandler(this._clearMemosToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(247, 6);
            // 
            // _createToolStripMenuItem
            // 
            this._createToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._createFolderToolStripMenuItem,
            this._createAndAddMemoToolStripMenuItem});
            this._createToolStripMenuItem.Name = "_createToolStripMenuItem";
            this._createToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this._createToolStripMenuItem.Text = "新規作成(&N)";
            // 
            // _createFolderToolStripMenuItem
            // 
            this._createFolderToolStripMenuItem.Name = "_createFolderToolStripMenuItem";
            this._createFolderToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this._createFolderToolStripMenuItem.Text = "クリアファイルを作成(&C)";
            this._createFolderToolStripMenuItem.Click += new System.EventHandler(this._createFolderToolStripMenuItem_Click);
            // 
            // _createAndAddMemoToolStripMenuItem
            // 
            this._createAndAddMemoToolStripMenuItem.Name = "_createAndAddMemoToolStripMenuItem";
            this._createAndAddMemoToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this._createAndAddMemoToolStripMenuItem.Text = "ノートを作成(&M)";
            this._createAndAddMemoToolStripMenuItem.Click += new System.EventHandler(this._createAndAddMemoToolStripMenuItem_Click);
            // 
            // _removeFolderToolStripMenuItem
            // 
            this._removeFolderToolStripMenuItem.Name = "_removeFolderToolStripMenuItem";
            this._removeFolderToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this._removeFolderToolStripMenuItem.Text = "削除(&D)";
            this._removeFolderToolStripMenuItem.Click += new System.EventHandler(this._removeFolderToolStripMenuItem_Click);
            // 
            // _renameToolStripMenuItem
            // 
            this._renameToolStripMenuItem.Name = "_renameToolStripMenuItem";
            this._renameToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this._renameToolStripMenuItem.Text = "名前変更(&R)";
            this._renameToolStripMenuItem.Click += new System.EventHandler(this._renameToolStripMenuItem_Click);
            // 
            // _commonContextMenuStrip
            // 
            this._commonContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._commonContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._commonSmartFolderCreateSmartFolderToolStripMenuItem,
            this._commonCreateTagToolStripMenuItem,
            this._commonFolderCreateFolderToolStripMenuItem,
            this._commonEmptyTrashBoxToolStripMenuItem});
            this._commonContextMenuStrip.Name = "contextMenuStrip1";
            this._commonContextMenuStrip.Size = new System.Drawing.Size(195, 92);
            this._commonContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._commonContextMenuStrip_Opening);
            // 
            // _commonSmartFolderCreateSmartFolderToolStripMenuItem
            // 
            this._commonSmartFolderCreateSmartFolderToolStripMenuItem.Name = "_commonSmartFolderCreateSmartFolderToolStripMenuItem";
            this._commonSmartFolderCreateSmartFolderToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this._commonSmartFolderCreateSmartFolderToolStripMenuItem.Text = "スマートフォルダを作成(&C)";
            this._commonSmartFolderCreateSmartFolderToolStripMenuItem.Click += new System.EventHandler(this._commonSmartFolderCreateSmartFolderToolStripMenuItem_Click);
            // 
            // _commonCreateTagToolStripMenuItem
            // 
            this._commonCreateTagToolStripMenuItem.Name = "_commonCreateTagToolStripMenuItem";
            this._commonCreateTagToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this._commonCreateTagToolStripMenuItem.Text = "タグを作成(&C)";
            this._commonCreateTagToolStripMenuItem.Click += new System.EventHandler(this._commonCreateTagToolStripMenuItem_Click);
            // 
            // _commonFolderCreateFolderToolStripMenuItem
            // 
            this._commonFolderCreateFolderToolStripMenuItem.Name = "_commonFolderCreateFolderToolStripMenuItem";
            this._commonFolderCreateFolderToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this._commonFolderCreateFolderToolStripMenuItem.Text = "クリアファイルを作成(&C)";
            this._commonFolderCreateFolderToolStripMenuItem.Click += new System.EventHandler(this._workspaceCreateFolderToolStripMenuItem_Click);
            // 
            // _commonEmptyTrashBoxToolStripMenuItem
            // 
            this._commonEmptyTrashBoxToolStripMenuItem.Name = "_commonEmptyTrashBoxToolStripMenuItem";
            this._commonEmptyTrashBoxToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this._commonEmptyTrashBoxToolStripMenuItem.Text = "ごみ箱を空にする(&E)";
            this._commonEmptyTrashBoxToolStripMenuItem.Click += new System.EventHandler(this._commonEmptyTrashBoxToolStripMenuItem_Click);
            // 
            // _workspaceTreePanel
            // 
            this._workspaceTreePanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this._workspaceTreePanel.Controls.Add(this._workspaceTree);
            this._workspaceTreePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workspaceTreePanel.Location = new System.Drawing.Point(0, 0);
            this._workspaceTreePanel.Name = "_workspaceTreePanel";
            this._workspaceTreePanel.Padding = new System.Windows.Forms.Padding(5);
            this._workspaceTreePanel.Size = new System.Drawing.Size(201, 401);
            this._workspaceTreePanel.TabIndex = 0;
            // 
            // _workspaceTree
            // 
            this._workspaceTree.AllowDrop = true;
            this._workspaceTree.BackColor = System.Drawing.Color.WhiteSmoke;
            this._workspaceTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._workspaceTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workspaceTree.FullRowSelect = true;
            this._workspaceTree.HideSelection = false;
            this._workspaceTree.ImageIndex = 0;
            this._workspaceTree.ItemHeight = 18;
            this._workspaceTree.LabelEdit = true;
            this._workspaceTree.Location = new System.Drawing.Point(5, 5);
            this._workspaceTree.Name = "_workspaceTree";
            this._workspaceTree.SelectedImageIndex = 0;
            this._workspaceTree.ShowLines = false;
            this._workspaceTree.Size = new System.Drawing.Size(191, 391);
            this._workspaceTree.Sorted = true;
            this._workspaceTree.TabIndex = 0;
            // 
            // _tagContextMenuStrip
            // 
            this._tagContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tagCreateToolStripMenuItem,
            this._tagRemoveToolStripMenuItem,
            this._tagRenameToolStripMenuItem});
            this._tagContextMenuStrip.Name = "_tagContextMenuStrip";
            this._tagContextMenuStrip.Size = new System.Drawing.Size(139, 70);
            this._tagContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._tagContextMenuStrip_Opening);
            // 
            // _tagCreateToolStripMenuItem
            // 
            this._tagCreateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tagCreateTagToolStripMenuItem,
            this._tagCreateMemoToolStripMenuItem});
            this._tagCreateToolStripMenuItem.Name = "_tagCreateToolStripMenuItem";
            this._tagCreateToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this._tagCreateToolStripMenuItem.Text = "新規作成(&N)";
            // 
            // _tagCreateTagToolStripMenuItem
            // 
            this._tagCreateTagToolStripMenuItem.Name = "_tagCreateTagToolStripMenuItem";
            this._tagCreateTagToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this._tagCreateTagToolStripMenuItem.Text = "タグの新規作成(&T)";
            this._tagCreateTagToolStripMenuItem.Click += new System.EventHandler(this._tagCreateTagToolStripMenuItem_Click);
            // 
            // _tagCreateMemoToolStripMenuItem
            // 
            this._tagCreateMemoToolStripMenuItem.Name = "_tagCreateMemoToolStripMenuItem";
            this._tagCreateMemoToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this._tagCreateMemoToolStripMenuItem.Text = "ノートの新規作成(&M)";
            this._tagCreateMemoToolStripMenuItem.Click += new System.EventHandler(this._tagCreateMemoToolStripMenuItem_Click);
            // 
            // _tagRemoveToolStripMenuItem
            // 
            this._tagRemoveToolStripMenuItem.Name = "_tagRemoveToolStripMenuItem";
            this._tagRemoveToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this._tagRemoveToolStripMenuItem.Text = "削除(&D)";
            this._tagRemoveToolStripMenuItem.Click += new System.EventHandler(this._tagRemoveToolStripMenuItem_Click);
            // 
            // _tagRenameToolStripMenuItem
            // 
            this._tagRenameToolStripMenuItem.Name = "_tagRenameToolStripMenuItem";
            this._tagRenameToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this._tagRenameToolStripMenuItem.Text = "名前変更(&R)";
            this._tagRenameToolStripMenuItem.Click += new System.EventHandler(this._tagRenameToolStripMenuItem_Click);
            // 
            // _smartFolderContextMenuStrip
            // 
            this._smartFolderContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._smartFolderRemoveToolStripMenuItem,
            this._smartFolderRenameToolStripMenuItem,
            this._editSmartFolderToolStripMenuItem});
            this._smartFolderContextMenuStrip.Name = "_tagContextMenuStrip";
            this._smartFolderContextMenuStrip.Size = new System.Drawing.Size(139, 70);
            // 
            // _smartFolderRemoveToolStripMenuItem
            // 
            this._smartFolderRemoveToolStripMenuItem.Name = "_smartFolderRemoveToolStripMenuItem";
            this._smartFolderRemoveToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this._smartFolderRemoveToolStripMenuItem.Text = "削除(&D)";
            this._smartFolderRemoveToolStripMenuItem.Click += new System.EventHandler(this._smartFolderRemoveToolStripMenuItem_Click);
            // 
            // _smartFolderRenameToolStripMenuItem
            // 
            this._smartFolderRenameToolStripMenuItem.Name = "_smartFolderRenameToolStripMenuItem";
            this._smartFolderRenameToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this._smartFolderRenameToolStripMenuItem.Text = "名前変更(&R)";
            this._smartFolderRenameToolStripMenuItem.Click += new System.EventHandler(this._smartFolderRenameToolStripMenuItem_Click);
            // 
            // _editSmartFolderToolStripMenuItem
            // 
            this._editSmartFolderToolStripMenuItem.Name = "_editSmartFolderToolStripMenuItem";
            this._editSmartFolderToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this._editSmartFolderToolStripMenuItem.Text = "編集(&E)...";
            this._editSmartFolderToolStripMenuItem.Click += new System.EventHandler(this._editSmartFolderToolStripMenuItem_Click);
            // 
            // WorkspaceView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._workspaceTreePanel);
            this.Name = "WorkspaceView";
            this.Size = new System.Drawing.Size(201, 401);
            this._folderContextMenuStrip.ResumeLayout(false);
            this._commonContextMenuStrip.ResumeLayout(false);
            this._workspaceTreePanel.ResumeLayout(false);
            this._tagContextMenuStrip.ResumeLayout(false);
            this._smartFolderContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip _folderContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _removeFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _openAllMemosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _addAllOpenMemoAToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip _commonContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _commonFolderCreateFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem _clearMemosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _createFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _createAndAddMemoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _activateFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _deactivateFolderToolStripMenuItem;
        private WorkspaceTree _workspaceTree;
        private System.Windows.Forms.Panel _workspaceTreePanel;
        private System.Windows.Forms.ContextMenuStrip _tagContextMenuStrip;
        private System.Windows.Forms.ContextMenuStrip _smartFolderContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _tagCreateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _tagCreateTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _tagCreateMemoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _tagRemoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _tagRenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _commonEmptyTrashBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _commonCreateTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _commonSmartFolderCreateSmartFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _smartFolderRemoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _smartFolderRenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _editSmartFolderToolStripMenuItem;
    }
}
