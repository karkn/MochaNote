namespace Mkamo.Memopad.Internal.Forms {
    partial class AboutBox {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._productNameLabel = new System.Windows.Forms.Label();
            this._copyrightLabel = new System.Windows.Forms.Label();
            this._descriptionTextBox = new System.Windows.Forms.TextBox();
            this._okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this._productNameLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this._copyrightLabel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this._descriptionTextBox, 0, 2);
            this.tableLayoutPanel.Controls.Add(this._okButton, 0, 3);
            this.tableLayoutPanel.Location = new System.Drawing.Point(6, 11);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(401, 213);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // _productNameLabel
            // 
            this._productNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._productNameLabel.Location = new System.Drawing.Point(6, 0);
            this._productNameLabel.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this._productNameLabel.MaximumSize = new System.Drawing.Size(0, 16);
            this._productNameLabel.Name = "_productNameLabel";
            this._productNameLabel.Size = new System.Drawing.Size(392, 16);
            this._productNameLabel.TabIndex = 19;
            this._productNameLabel.Text = "MochaNote";
            this._productNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _copyrightLabel
            // 
            this._copyrightLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._copyrightLabel.Location = new System.Drawing.Point(6, 23);
            this._copyrightLabel.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this._copyrightLabel.MaximumSize = new System.Drawing.Size(0, 16);
            this._copyrightLabel.Name = "_copyrightLabel";
            this._copyrightLabel.Size = new System.Drawing.Size(392, 16);
            this._copyrightLabel.TabIndex = 21;
            this._copyrightLabel.Text = "Copyright (c) 2010-2015 mocha";
            this._copyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _descriptionTextBox
            // 
            this._descriptionTextBox.BackColor = System.Drawing.Color.White;
            this._descriptionTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this._descriptionTextBox.Location = new System.Drawing.Point(6, 49);
            this._descriptionTextBox.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this._descriptionTextBox.Multiline = true;
            this._descriptionTextBox.Name = "_descriptionTextBox";
            this._descriptionTextBox.ReadOnly = true;
            this._descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._descriptionTextBox.Size = new System.Drawing.Size(352, 124);
            this._descriptionTextBox.TabIndex = 23;
            this._descriptionTextBox.TabStop = false;
            this._descriptionTextBox.Text = "説明";
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._okButton.Location = new System.Drawing.Point(323, 189);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 21);
            this._okButton.TabIndex = 24;
            this._okButton.Text = "&OK";
            // 
            // AboutBox
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 237);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MochaNoteのバージョン情報";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label _productNameLabel;
        private System.Windows.Forms.Label _copyrightLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.TextBox _descriptionTextBox;
    }
}
