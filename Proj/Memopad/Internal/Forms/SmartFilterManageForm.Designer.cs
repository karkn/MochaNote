namespace Mkamo.Memopad.Internal.Forms {
    partial class SmartFilterManageForm {
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
            this._mainToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this._smartFilterListBox = new ComponentFactory.Krypton.Toolkit.KryptonListBox();
            this._closeButton = new System.Windows.Forms.Button();
            this._mainToolStrip = new System.Windows.Forms.ToolStrip();
            this._createSmartFilterToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._removeSmartFilterToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._editSmartFilterToolStripButton = new System.Windows.Forms.ToolStripButton();
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
            this._mainToolStripContainer.ContentPanel.Controls.Add(this._smartFilterListBox);
            this._mainToolStripContainer.ContentPanel.Controls.Add(this._closeButton);
            this._mainToolStripContainer.ContentPanel.Size = new System.Drawing.Size(248, 428);
            this._mainToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainToolStripContainer.LeftToolStripPanelVisible = false;
            this._mainToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this._mainToolStripContainer.Name = "_mainToolStripContainer";
            this._mainToolStripContainer.RightToolStripPanelVisible = false;
            this._mainToolStripContainer.Size = new System.Drawing.Size(248, 453);
            this._mainToolStripContainer.TabIndex = 0;
            this._mainToolStripContainer.Text = "toolStripContainer1";
            // 
            // _mainToolStripContainer.TopToolStripPanel
            // 
            this._mainToolStripContainer.TopToolStripPanel.Controls.Add(this._mainToolStrip);
            // 
            // _smartFilterListBox
            // 
            this._smartFilterListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._smartFilterListBox.Location = new System.Drawing.Point(0, 0);
            this._smartFilterListBox.Name = "_smartFilterListBox";
            this._smartFilterListBox.Size = new System.Drawing.Size(248, 387);
            this._smartFilterListBox.TabIndex = 0;
            this._smartFilterListBox.SelectedIndexChanged += new System.EventHandler(this._smartFilterListBox_SelectedIndexChanged);
            // 
            // _closeButton
            // 
            this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._closeButton.Location = new System.Drawing.Point(161, 393);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(75, 23);
            this._closeButton.TabIndex = 1;
            this._closeButton.Text = "閉じる(&C)";
            this._closeButton.UseVisualStyleBackColor = true;
            // 
            // _mainToolStrip
            // 
            this._mainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._mainToolStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._mainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._createSmartFilterToolStripButton,
            this._removeSmartFilterToolStripButton,
            this._editSmartFilterToolStripButton});
            this._mainToolStrip.Location = new System.Drawing.Point(3, 0);
            this._mainToolStrip.Name = "_mainToolStrip";
            this._mainToolStrip.Size = new System.Drawing.Size(72, 25);
            this._mainToolStrip.TabIndex = 0;
            this._mainToolStrip.Text = "toolStrip1";
            // 
            // _createSmartFilterToolStripButton
            // 
            this._createSmartFilterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._createSmartFilterToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.filter_plus;
            this._createSmartFilterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._createSmartFilterToolStripButton.Name = "_createSmartFilterToolStripButton";
            this._createSmartFilterToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._createSmartFilterToolStripButton.Text = "新規作成";
            this._createSmartFilterToolStripButton.Click += new System.EventHandler(this._createSmartFilterToolStripButton_Click);
            // 
            // _removeSmartFilterToolStripButton
            // 
            this._removeSmartFilterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._removeSmartFilterToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.filter_minus;
            this._removeSmartFilterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._removeSmartFilterToolStripButton.Name = "_removeSmartFilterToolStripButton";
            this._removeSmartFilterToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._removeSmartFilterToolStripButton.Text = "削除";
            this._removeSmartFilterToolStripButton.Click += new System.EventHandler(this._removeSmartFilterToolStripButton_Click);
            // 
            // _editSmartFilterToolStripButton
            // 
            this._editSmartFilterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._editSmartFilterToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.filter_edit;
            this._editSmartFilterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._editSmartFilterToolStripButton.Name = "_editSmartFilterToolStripButton";
            this._editSmartFilterToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._editSmartFilterToolStripButton.Text = "編集";
            this._editSmartFilterToolStripButton.Click += new System.EventHandler(this._editToolStripButton_Click);
            // 
            // SmartFilterManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 453);
            this.Controls.Add(this._mainToolStripContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SmartFilterManageForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "スマートフィルタの管理";
            this._mainToolStripContainer.ContentPanel.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStrip _mainToolStrip;
        private System.Windows.Forms.ToolStripButton _createSmartFilterToolStripButton;
        private System.Windows.Forms.ToolStripButton _removeSmartFilterToolStripButton;
        private System.Windows.Forms.ToolStripButton _editSmartFilterToolStripButton;
        private ComponentFactory.Krypton.Toolkit.KryptonListBox _smartFilterListBox;

    }
}
