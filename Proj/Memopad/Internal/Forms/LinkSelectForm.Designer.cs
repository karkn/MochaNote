namespace Mkamo.Memopad.Internal.Forms {
    partial class LinkSelectForm {
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
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._linkTargetLabel = new System.Windows.Forms.Label();
            this._memoLinkPanel = new System.Windows.Forms.Panel();
            this._webLinkPanel = new System.Windows.Forms.Panel();
            this._addressTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._addressLabel = new System.Windows.Forms.Label();
            this._titleTextLabel = new System.Windows.Forms.Label();
            this._titleTextTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._linkTargetListBox = new System.Windows.Forms.ListBox();
            this._memoListView = new Mkamo.Memopad.Internal.Controls.MemoListView();
            this._workspaceView = new Mkamo.Memopad.Internal.Controls.WorkspaceView();
            this._memoLinkPanel.SuspendLayout();
            this._webLinkPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Location = new System.Drawing.Point(374, 383);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 2;
            this._okButton.Text = "OK(&O)";
            this._okButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(455, 383);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(87, 23);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _linkTargetLabel
            // 
            this._linkTargetLabel.AutoSize = true;
            this._linkTargetLabel.Location = new System.Drawing.Point(10, 14);
            this._linkTargetLabel.Name = "_linkTargetLabel";
            this._linkTargetLabel.Size = new System.Drawing.Size(57, 12);
            this._linkTargetLabel.TabIndex = 5;
            this._linkTargetLabel.Text = "リンク先(&L):";
            // 
            // _memoLinkPanel
            // 
            this._memoLinkPanel.Controls.Add(this._memoListView);
            this._memoLinkPanel.Controls.Add(this._workspaceView);
            this._memoLinkPanel.Location = new System.Drawing.Point(78, 58);
            this._memoLinkPanel.Name = "_memoLinkPanel";
            this._memoLinkPanel.Padding = new System.Windows.Forms.Padding(1);
            this._memoLinkPanel.Size = new System.Drawing.Size(465, 308);
            this._memoLinkPanel.TabIndex = 6;
            // 
            // _webLinkPanel
            // 
            this._webLinkPanel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._webLinkPanel.Controls.Add(this._addressTextBox);
            this._webLinkPanel.Controls.Add(this._addressLabel);
            this._webLinkPanel.Location = new System.Drawing.Point(78, 58);
            this._webLinkPanel.Name = "_webLinkPanel";
            this._webLinkPanel.Size = new System.Drawing.Size(464, 308);
            this._webLinkPanel.TabIndex = 7;
            // 
            // _addressTextBox
            // 
            this._addressTextBox.Location = new System.Drawing.Point(0, 15);
            this._addressTextBox.Name = "_addressTextBox";
            this._addressTextBox.Size = new System.Drawing.Size(464, 23);
            this._addressTextBox.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlDark;
            this._addressTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._addressTextBox.TabIndex = 6;
            this._addressTextBox.TextChanged += new System.EventHandler(this._addressTextBox_TextChanged);
            // 
            // _addressLabel
            // 
            this._addressLabel.AutoSize = true;
            this._addressLabel.Location = new System.Drawing.Point(3, 0);
            this._addressLabel.Name = "_addressLabel";
            this._addressLabel.Size = new System.Drawing.Size(59, 12);
            this._addressLabel.TabIndex = 5;
            this._addressLabel.Text = "アドレス(&A):";
            // 
            // _titleTextLabel
            // 
            this._titleTextLabel.AutoSize = true;
            this._titleTextLabel.Location = new System.Drawing.Point(76, 14);
            this._titleTextLabel.Name = "_titleTextLabel";
            this._titleTextLabel.Size = new System.Drawing.Size(46, 12);
            this._titleTextLabel.TabIndex = 5;
            this._titleTextLabel.Text = "表示(&T):";
            // 
            // _titleTextTextBox
            // 
            this._titleTextTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._titleTextTextBox.Location = new System.Drawing.Point(78, 29);
            this._titleTextTextBox.Name = "_titleTextTextBox";
            this._titleTextTextBox.Size = new System.Drawing.Size(465, 23);
            this._titleTextTextBox.StateCommon.Border.Color1 = System.Drawing.SystemColors.ControlDark;
            this._titleTextTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._titleTextTextBox.TabIndex = 6;
            this._titleTextTextBox.TextChanged += new System.EventHandler(this._titleTextTextBox_TextChanged);
            // 
            // _linkTargetListBox
            // 
            this._linkTargetListBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._linkTargetListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._linkTargetListBox.FormattingEnabled = true;
            this._linkTargetListBox.IntegralHeight = false;
            this._linkTargetListBox.ItemHeight = 12;
            this._linkTargetListBox.Items.AddRange(new object[] {
            "ノート",
            "Web"});
            this._linkTargetListBox.Location = new System.Drawing.Point(13, 30);
            this._linkTargetListBox.Name = "_linkTargetListBox";
            this._linkTargetListBox.Size = new System.Drawing.Size(53, 335);
            this._linkTargetListBox.TabIndex = 4;
            this._linkTargetListBox.SelectedIndexChanged += new System.EventHandler(this._linkTargetListBox_SelectedIndexChanged);
            // 
            // _memoListView
            // 
            this._memoListView.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._memoListView.Location = new System.Drawing.Point(219, 1);
            this._memoListView.Margin = new System.Windows.Forms.Padding(0);
            this._memoListView.Name = "_memoListView";
            this._memoListView.Size = new System.Drawing.Size(245, 306);
            this._memoListView.TabIndex = 2;
            // 
            // _workspaceView
            // 
            this._workspaceView.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._workspaceView.Location = new System.Drawing.Point(1, 1);
            this._workspaceView.Name = "_workspaceView";
            this._workspaceView.Size = new System.Drawing.Size(213, 306);
            this._workspaceView.TabIndex = 1;
            // 
            // LinkSelectForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(555, 418);
            this.Controls.Add(this._titleTextTextBox);
            this.Controls.Add(this._webLinkPanel);
            this.Controls.Add(this._memoLinkPanel);
            this.Controls.Add(this._titleTextLabel);
            this.Controls.Add(this._linkTargetLabel);
            this.Controls.Add(this._linkTargetListBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinkSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "リンクの設定";
            this._memoLinkPanel.ResumeLayout(false);
            this._webLinkPanel.ResumeLayout(false);
            this._webLinkPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label _linkTargetLabel;
        private System.Windows.Forms.Panel _memoLinkPanel;
        private Mkamo.Memopad.Internal.Controls.MemoListView _memoListView;
        private Mkamo.Memopad.Internal.Controls.WorkspaceView _workspaceView;
        private System.Windows.Forms.Panel _webLinkPanel;
        private System.Windows.Forms.Label _addressLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _addressTextBox;
        private System.Windows.Forms.Label _titleTextLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _titleTextTextBox;
        private System.Windows.Forms.ListBox _linkTargetListBox;

    }
}
