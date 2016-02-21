namespace Mkamo.Control.Configurator {
    partial class LineConfigurator {
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
            this._showColorDialogButton = new System.Windows.Forms.Button();
            this._colorPanel = new System.Windows.Forms.Panel();
            this._colorLabel = new System.Windows.Forms.Label();
            this._widthLabel = new System.Windows.Forms.Label();
            this._styleLabel = new System.Windows.Forms.Label();
            this._widthComboBox = new System.Windows.Forms.ComboBox();
            this._styleComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _showColorDialogButton
            // 
            this._showColorDialogButton.BackColor = System.Drawing.SystemColors.Control;
            this._showColorDialogButton.Location = new System.Drawing.Point(106, 3);
            this._showColorDialogButton.Name = "_showColorDialogButton";
            this._showColorDialogButton.Size = new System.Drawing.Size(60, 23);
            this._showColorDialogButton.TabIndex = 3;
            this._showColorDialogButton.Text = "選択(&S)";
            this._showColorDialogButton.UseVisualStyleBackColor = false;
            this._showColorDialogButton.Click += new System.EventHandler(this._showColorDialogButton_Click);
            // 
            // _colorPanel
            // 
            this._colorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._colorPanel.Location = new System.Drawing.Point(75, 5);
            this._colorPanel.Name = "_colorPanel";
            this._colorPanel.Size = new System.Drawing.Size(25, 20);
            this._colorPanel.TabIndex = 4;
            // 
            // _colorLabel
            // 
            this._colorLabel.AutoSize = true;
            this._colorLabel.Location = new System.Drawing.Point(3, 8);
            this._colorLabel.Name = "_colorLabel";
            this._colorLabel.Size = new System.Drawing.Size(35, 12);
            this._colorLabel.TabIndex = 2;
            this._colorLabel.Text = "色(&C):";
            // 
            // _widthLabel
            // 
            this._widthLabel.AutoSize = true;
            this._widthLabel.Location = new System.Drawing.Point(3, 35);
            this._widthLabel.Name = "_widthLabel";
            this._widthLabel.Size = new System.Drawing.Size(44, 12);
            this._widthLabel.TabIndex = 2;
            this._widthLabel.Text = "太さ(&W):";
            // 
            // _styleLabel
            // 
            this._styleLabel.AutoSize = true;
            this._styleLabel.Location = new System.Drawing.Point(3, 62);
            this._styleLabel.Name = "_styleLabel";
            this._styleLabel.Size = new System.Drawing.Size(58, 12);
            this._styleLabel.TabIndex = 2;
            this._styleLabel.Text = "スタイル(&S):";
            // 
            // _widthComboBox
            // 
            this._widthComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._widthComboBox.FormattingEnabled = true;
            this._widthComboBox.Items.AddRange(new object[] {
            "1pt",
            "2pt",
            "3pt",
            "4pt"});
            this._widthComboBox.Location = new System.Drawing.Point(75, 32);
            this._widthComboBox.Name = "_widthComboBox";
            this._widthComboBox.Size = new System.Drawing.Size(91, 20);
            this._widthComboBox.TabIndex = 5;
            this._widthComboBox.SelectedIndexChanged += new System.EventHandler(this._widthComboBox_SelectedIndexChanged);
            // 
            // _styleComboBox
            // 
            this._styleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._styleComboBox.FormattingEnabled = true;
            this._styleComboBox.Items.AddRange(new object[] {
            "実線",
            "ダッシュ",
            "ドット",
            "ダッシュドット",
            "ダッシュドットドット"});
            this._styleComboBox.Location = new System.Drawing.Point(75, 59);
            this._styleComboBox.Name = "_styleComboBox";
            this._styleComboBox.Size = new System.Drawing.Size(91, 20);
            this._styleComboBox.TabIndex = 5;
            this._styleComboBox.SelectedIndexChanged += new System.EventHandler(this._styleComboBox_SelectedIndexChanged);
            // 
            // LineConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._styleComboBox);
            this.Controls.Add(this._widthComboBox);
            this.Controls.Add(this._showColorDialogButton);
            this.Controls.Add(this._colorPanel);
            this.Controls.Add(this._styleLabel);
            this.Controls.Add(this._widthLabel);
            this.Controls.Add(this._colorLabel);
            this.Name = "LineConfigurator";
            this.Size = new System.Drawing.Size(213, 111);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _showColorDialogButton;
        private System.Windows.Forms.Panel _colorPanel;
        private System.Windows.Forms.Label _colorLabel;
        private System.Windows.Forms.Label _widthLabel;
        private System.Windows.Forms.Label _styleLabel;
        private System.Windows.Forms.ComboBox _widthComboBox;
        private System.Windows.Forms.ComboBox _styleComboBox;
    }
}
