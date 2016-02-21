namespace Mkamo.Memopad.Internal.Forms {
    partial class ConfirmDataConversionForm {
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
            this._messageLabel = new System.Windows.Forms.Label();
            this._dontShowCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Location = new System.Drawing.Point(261, 114);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 0;
            this._okButton.Text = "&OK";
            this._okButton.UseVisualStyleBackColor = true;
            // 
            // _messageLabel
            // 
            this._messageLabel.AutoSize = true;
            this._messageLabel.Location = new System.Drawing.Point(12, 9);
            this._messageLabel.Name = "_messageLabel";
            this._messageLabel.Size = new System.Drawing.Size(228, 60);
            this._messageLabel.TabIndex = 1;
            this._messageLabel.Text = "Confidante Ver 1のデータが見つかりました。\r\nConfidante Ver 2のデータに変換するには\r\nConfidanteを終了してからスタート" +
                "メニューの\r\n「プログラム」>「Confidante」>「V1データ変換」を\r\n実行してください。";
            // 
            // _dontShowCheckBox
            // 
            this._dontShowCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._dontShowCheckBox.AutoSize = true;
            this._dontShowCheckBox.Location = new System.Drawing.Point(12, 92);
            this._dontShowCheckBox.Name = "_dontShowCheckBox";
            this._dontShowCheckBox.Size = new System.Drawing.Size(187, 16);
            this._dontShowCheckBox.TabIndex = 2;
            this._dontShowCheckBox.Text = "今後このウィンドウを表示しない(&C)";
            this._dontShowCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConfirmDataConversionForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 149);
            this.Controls.Add(this._dontShowCheckBox);
            this.Controls.Add(this._messageLabel);
            this.Controls.Add(this._okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConfirmDataConversionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ノートデータ変換の確認";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Label _messageLabel;
        private System.Windows.Forms.CheckBox _dontShowCheckBox;
    }
}
