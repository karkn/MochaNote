namespace Mkamo.Memopad.Internal.Forms {
    partial class TagManageForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagManageForm));
            this._mainToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this._closeButton = new System.Windows.Forms.Button();
            this._tagTreeView = new Mkamo.Memopad.Internal.Controls.TagTree();
            this._tagTreeViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._mainToolStrip = new System.Windows.Forms.ToolStrip();
            this._newTagToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._removeTagToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._renameTagToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._tagTreeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this._mainToolStripContainer.ContentPanel.SuspendLayout();
            this._mainToolStripContainer.TopToolStripPanel.SuspendLayout();
            this._mainToolStripContainer.SuspendLayout();
            this._mainToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mainToolStripContainer
            // 
            this._mainToolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // _mainToolStripContainer.ContentPanel
            // 
            this._mainToolStripContainer.ContentPanel.Controls.Add(this._closeButton);
            this._mainToolStripContainer.ContentPanel.Controls.Add(this._tagTreeView);
            this._mainToolStripContainer.ContentPanel.Padding = new System.Windows.Forms.Padding(3);
            this._mainToolStripContainer.ContentPanel.Size = new System.Drawing.Size(267, 432);
            this._mainToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainToolStripContainer.LeftToolStripPanelVisible = false;
            this._mainToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this._mainToolStripContainer.Name = "_mainToolStripContainer";
            this._mainToolStripContainer.RightToolStripPanelVisible = false;
            this._mainToolStripContainer.Size = new System.Drawing.Size(267, 457);
            this._mainToolStripContainer.TabIndex = 2;
            this._mainToolStripContainer.Text = "toolStripContainer1";
            // 
            // _mainToolStripContainer.TopToolStripPanel
            // 
            this._mainToolStripContainer.TopToolStripPanel.Controls.Add(this._mainToolStrip);
            // 
            // _closeButton
            // 
            this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._closeButton.AutoSize = true;
            this._closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._closeButton.Location = new System.Drawing.Point(182, 401);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(75, 24);
            this._closeButton.TabIndex = 2;
            this._closeButton.Text = "閉じる(&C)";
            this._closeButton.UseVisualStyleBackColor = true;
            // 
            // _tagTreeView
            // 
            this._tagTreeView.AllowDrop = true;
            this._tagTreeView.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._tagTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tagTreeView.ContextMenuStrip = this._tagTreeViewContextMenuStrip;
            this._tagTreeView.ImageIndex = 0;
            this._tagTreeView.LabelEdit = true;
            this._tagTreeView.Location = new System.Drawing.Point(3, 3);
            this._tagTreeView.Name = "_tagTreeView";
            this._tagTreeView.SelectedImageIndex = 0;
            this._tagTreeView.Size = new System.Drawing.Size(261, 392);
            this._tagTreeView.Sorted = true;
            this._tagTreeView.TabIndex = 1;
            // 
            // _tagTreeViewContextMenuStrip
            // 
            this._tagTreeViewContextMenuStrip.Name = "_tagTreeViewContextMenuStrip";
            this._tagTreeViewContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // _mainToolStrip
            // 
            this._mainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._mainToolStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._mainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._newTagToolStripButton,
            this._removeTagToolStripButton,
            this._renameTagToolStripButton});
            this._mainToolStrip.Location = new System.Drawing.Point(3, 0);
            this._mainToolStrip.Name = "_mainToolStrip";
            this._mainToolStrip.Size = new System.Drawing.Size(72, 25);
            this._mainToolStrip.TabIndex = 0;
            // 
            // _newTagToolStripButton
            // 
            this._newTagToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._newTagToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.tag_blue_add;
            this._newTagToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._newTagToolStripButton.Name = "_newTagToolStripButton";
            this._newTagToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._newTagToolStripButton.Text = "新規作成";
            // 
            // _removeTagToolStripButton
            // 
            this._removeTagToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._removeTagToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.tag_blue_delete;
            this._removeTagToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._removeTagToolStripButton.Name = "_removeTagToolStripButton";
            this._removeTagToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._removeTagToolStripButton.Text = "削除";
            // 
            // _renameTagToolStripButton
            // 
            this._renameTagToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._renameTagToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.tag_blue_edit;
            this._renameTagToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._renameTagToolStripButton.Name = "_renameTagToolStripButton";
            this._renameTagToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._renameTagToolStripButton.Text = "名前変更";
            // 
            // _tagTreeViewImageList
            // 
            this._tagTreeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("_tagTreeViewImageList.ImageStream")));
            this._tagTreeViewImageList.TransparentColor = System.Drawing.Color.Magenta;
            this._tagTreeViewImageList.Images.SetKeyName(0, "tag-blue.png");
            this._tagTreeViewImageList.Images.SetKeyName(1, "tags.png");
            this._tagTreeViewImageList.Images.SetKeyName(2, "tags-delete.png");
            // 
            // TagManageForm
            // 
            this.AcceptButton = this._closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._closeButton;
            this.ClientSize = new System.Drawing.Size(267, 457);
            this.Controls.Add(this._mainToolStripContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TagManageForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "タグの管理";
            this.Load += new System.EventHandler(this.TagEditForm_Load);
            this._mainToolStripContainer.ContentPanel.ResumeLayout(false);
            this._mainToolStripContainer.ContentPanel.PerformLayout();
            this._mainToolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this._mainToolStripContainer.TopToolStripPanel.PerformLayout();
            this._mainToolStripContainer.ResumeLayout(false);
            this._mainToolStripContainer.PerformLayout();
            this._mainToolStrip.ResumeLayout(false);
            this._mainToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer _mainToolStripContainer;
        private System.Windows.Forms.Button _closeButton;
        private Mkamo.Memopad.Internal.Controls.TagTree _tagTreeView;
        private System.Windows.Forms.ToolStrip _mainToolStrip;
        private System.Windows.Forms.ToolStripButton _newTagToolStripButton;
        private System.Windows.Forms.ImageList _tagTreeViewImageList;
        private System.Windows.Forms.ContextMenuStrip _tagTreeViewContextMenuStrip;
        private System.Windows.Forms.ToolStripButton _removeTagToolStripButton;
        private System.Windows.Forms.ToolStripButton _renameTagToolStripButton;

    }
}
