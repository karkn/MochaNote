namespace Mkamo.Memopad.Internal.Forms {
    partial class PrintForm {
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
                CleanUp();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this._printButton = new System.Windows.Forms.Button();
            this._pageSetupButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._previewButton = new System.Windows.Forms.Button();
            this._optionGroupBox = new System.Windows.Forms.GroupBox();
            this._fitBothRadioButton = new System.Windows.Forms.RadioButton();
            this._fitVerticalRadioButton = new System.Windows.Forms.RadioButton();
            this._fitHorizontalRadioButton = new System.Windows.Forms.RadioButton();
            this._fitNoneRadioButton = new System.Windows.Forms.RadioButton();
            this._optionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _printButton
            // 
            this._printButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._printButton.Location = new System.Drawing.Point(36, 135);
            this._printButton.Name = "_printButton";
            this._printButton.Size = new System.Drawing.Size(75, 23);
            this._printButton.TabIndex = 1;
            this._printButton.Text = "印刷(&P)";
            this._printButton.UseVisualStyleBackColor = true;
            this._printButton.Click += new System.EventHandler(this._printButton_Click);
            // 
            // _pageSetupButton
            // 
            this._pageSetupButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._pageSetupButton.Location = new System.Drawing.Point(228, 135);
            this._pageSetupButton.Name = "_pageSetupButton";
            this._pageSetupButton.Size = new System.Drawing.Size(90, 23);
            this._pageSetupButton.TabIndex = 3;
            this._pageSetupButton.Text = "ページ設定(&S)...";
            this._pageSetupButton.UseVisualStyleBackColor = true;
            this._pageSetupButton.Click += new System.EventHandler(this._pageSetupButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(324, 135);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(78, 23);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "閉じる(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _previewButton
            // 
            this._previewButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._previewButton.Location = new System.Drawing.Point(117, 135);
            this._previewButton.Name = "_previewButton";
            this._previewButton.Size = new System.Drawing.Size(105, 23);
            this._previewButton.TabIndex = 2;
            this._previewButton.Text = "印刷プレビュー(&V)...";
            this._previewButton.UseVisualStyleBackColor = true;
            this._previewButton.Click += new System.EventHandler(this._previewButton_Click);
            // 
            // _optionGroupBox
            // 
            this._optionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._optionGroupBox.Controls.Add(this._fitBothRadioButton);
            this._optionGroupBox.Controls.Add(this._fitVerticalRadioButton);
            this._optionGroupBox.Controls.Add(this._fitHorizontalRadioButton);
            this._optionGroupBox.Controls.Add(this._fitNoneRadioButton);
            this._optionGroupBox.Location = new System.Drawing.Point(12, 12);
            this._optionGroupBox.Name = "_optionGroupBox";
            this._optionGroupBox.Size = new System.Drawing.Size(390, 117);
            this._optionGroupBox.TabIndex = 0;
            this._optionGroupBox.TabStop = false;
            this._optionGroupBox.Text = "縮小オプション(&O)";
            // 
            // _fitBothRadioButton
            // 
            this._fitBothRadioButton.AutoSize = true;
            this._fitBothRadioButton.Location = new System.Drawing.Point(6, 84);
            this._fitBothRadioButton.Name = "_fitBothRadioButton";
            this._fitBothRadioButton.Size = new System.Drawing.Size(133, 16);
            this._fitBothRadioButton.TabIndex = 3;
            this._fitBothRadioButton.TabStop = true;
            this._fitBothRadioButton.Text = "すべてを1枚に収める(&4)";
            this._fitBothRadioButton.UseVisualStyleBackColor = true;
            // 
            // _fitVerticalRadioButton
            // 
            this._fitVerticalRadioButton.AutoSize = true;
            this._fitVerticalRadioButton.Location = new System.Drawing.Point(6, 62);
            this._fitVerticalRadioButton.Name = "_fitVerticalRadioButton";
            this._fitVerticalRadioButton.Size = new System.Drawing.Size(124, 16);
            this._fitVerticalRadioButton.TabIndex = 2;
            this._fitVerticalRadioButton.TabStop = true;
            this._fitVerticalRadioButton.Text = "高さを1枚に収める(&3)";
            this._fitVerticalRadioButton.UseVisualStyleBackColor = true;
            // 
            // _fitHorizontalRadioButton
            // 
            this._fitHorizontalRadioButton.AutoSize = true;
            this._fitHorizontalRadioButton.Location = new System.Drawing.Point(6, 40);
            this._fitHorizontalRadioButton.Name = "_fitHorizontalRadioButton";
            this._fitHorizontalRadioButton.Size = new System.Drawing.Size(116, 16);
            this._fitHorizontalRadioButton.TabIndex = 1;
            this._fitHorizontalRadioButton.TabStop = true;
            this._fitHorizontalRadioButton.Text = "幅を1枚に収める(&2)";
            this._fitHorizontalRadioButton.UseVisualStyleBackColor = true;
            // 
            // _fitNoneRadioButton
            // 
            this._fitNoneRadioButton.AutoSize = true;
            this._fitNoneRadioButton.Location = new System.Drawing.Point(6, 18);
            this._fitNoneRadioButton.Name = "_fitNoneRadioButton";
            this._fitNoneRadioButton.Size = new System.Drawing.Size(80, 16);
            this._fitNoneRadioButton.TabIndex = 0;
            this._fitNoneRadioButton.TabStop = true;
            this._fitNoneRadioButton.Text = "縮小なし(&1)";
            this._fitNoneRadioButton.UseVisualStyleBackColor = true;
            // 
            // PrintForm
            // 
            this.AcceptButton = this._printButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(414, 170);
            this.Controls.Add(this._optionGroupBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._pageSetupButton);
            this.Controls.Add(this._previewButton);
            this.Controls.Add(this._printButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ノートの印刷";
            this._optionGroupBox.ResumeLayout(false);
            this._optionGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _printButton;
        private System.Windows.Forms.Button _pageSetupButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _previewButton;
        private System.Windows.Forms.GroupBox _optionGroupBox;
        private System.Windows.Forms.RadioButton _fitNoneRadioButton;
        private System.Windows.Forms.RadioButton _fitHorizontalRadioButton;
        private System.Windows.Forms.RadioButton _fitBothRadioButton;
        private System.Windows.Forms.RadioButton _fitVerticalRadioButton;
    }
}
