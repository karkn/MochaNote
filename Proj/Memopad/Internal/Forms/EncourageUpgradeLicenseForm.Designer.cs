namespace Mkamo.Memopad.Internal.Forms {
    partial class EncourageUpgradeLicenseForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EncourageUpgradeLicenseForm));
            this._premiumLicenseButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this._messageLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // _premiumLicenseButton
            // 
            this._premiumLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._premiumLicenseButton.Location = new System.Drawing.Point(318, 182);
            this._premiumLicenseButton.Name = "_premiumLicenseButton";
            this._premiumLicenseButton.Size = new System.Drawing.Size(112, 23);
            this._premiumLicenseButton.TabIndex = 0;
            this._premiumLicenseButton.Text = "アップグレードする(&U)";
            this._premiumLicenseButton.UseVisualStyleBackColor = true;
            this._premiumLicenseButton.Click += new System.EventHandler(this._premiumLicenseButton_Click);
            // 
            // _closeButton
            // 
            this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._closeButton.Location = new System.Drawing.Point(437, 182);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(82, 23);
            this._closeButton.TabIndex = 1;
            this._closeButton.Text = "閉じる(&C)";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this._messageLabel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(531, 173);
            this.panel1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Mkamo.Memopad.Properties.Resources.confidante48;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // _messageLabel
            // 
            this._messageLabel.AutoSize = true;
            this._messageLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this._messageLabel.Location = new System.Drawing.Point(85, 12);
            this._messageLabel.Name = "_messageLabel";
            this._messageLabel.Size = new System.Drawing.Size(50, 12);
            this._messageLabel.TabIndex = 1;
            this._messageLabel.Text = "メッセージ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(408, 96);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.UseMnemonic = false;
            // 
            // EncourageUpgradeLicenseForm
            // 
            this.AcceptButton = this._premiumLicenseButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._closeButton;
            this.ClientSize = new System.Drawing.Size(531, 217);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._closeButton);
            this.Controls.Add(this._premiumLicenseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EncourageUpgradeLicenseForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "プレミアムライセンスのご紹介";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _premiumLicenseButton;
        private System.Windows.Forms.Button _closeButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _messageLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
