using Mkamo.Editor.Core;
namespace Mkamo.Memopad.Internal.Controls {
    partial class PageContent {
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
            CleanUp();
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this._titleTextBox = new System.Windows.Forms.TextBox();
            this._titleLabel = new System.Windows.Forms.Label();
            this._tagLabel = new System.Windows.Forms.Label();
            this._selectTagDropButton = new ComponentFactory.Krypton.Toolkit.KryptonDropButton();
            this._selectTagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._modifiedDateLabel = new System.Windows.Forms.Label();
            this._editorCanvas = new Mkamo.Editor.Core.EditorCanvas();
            this._sourceLabel = new System.Windows.Forms.Label();
            this._sourceTextBox = new System.Windows.Forms.TextBox();
            this._createdDateLabel = new System.Windows.Forms.Label();
            this._showSubInfoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this._showSourceButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this._tagTextBox = new System.Windows.Forms.TextBox();
            this._inMemoSearcher = new Mkamo.Memopad.Internal.Controls.InMemoSearcher();
            this.SuspendLayout();
            // 
            // _titleTextBox
            // 
            this._titleTextBox.BackColor = System.Drawing.SystemColors.Window;
            this._titleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._titleTextBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._titleTextBox.Location = new System.Drawing.Point(61, 7);
            this._titleTextBox.Name = "_titleTextBox";
            this._titleTextBox.Size = new System.Drawing.Size(128, 16);
            this._titleTextBox.TabIndex = 3;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.BackColor = System.Drawing.Color.Transparent;
            this._titleLabel.ForeColor = System.Drawing.Color.Navy;
            this._titleLabel.Location = new System.Drawing.Point(8, 7);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(58, 12);
            this._titleLabel.TabIndex = 2;
            this._titleLabel.Text = "タイトル(&N):";
            // 
            // _tagLabel
            // 
            this._tagLabel.AutoSize = true;
            this._tagLabel.BackColor = System.Drawing.Color.Transparent;
            this._tagLabel.ForeColor = System.Drawing.Color.Navy;
            this._tagLabel.Location = new System.Drawing.Point(8, 29);
            this._tagLabel.Name = "_tagLabel";
            this._tagLabel.Size = new System.Drawing.Size(38, 12);
            this._tagLabel.TabIndex = 5;
            this._tagLabel.Text = "タグ(&L):";
            // 
            // _selectTagDropButton
            // 
            this._selectTagDropButton.ContextMenuStrip = this._selectTagContextMenuStrip;
            this._selectTagDropButton.Location = new System.Drawing.Point(167, 29);
            this._selectTagDropButton.MaximumSize = new System.Drawing.Size(90, 30);
            this._selectTagDropButton.Name = "_selectTagDropButton";
            this._selectTagDropButton.Size = new System.Drawing.Size(83, 22);
            this._selectTagDropButton.Splitter = false;
            this._selectTagDropButton.StateCommon.Content.Padding = new System.Windows.Forms.Padding(0);
            this._selectTagDropButton.TabIndex = 6;
            this._selectTagDropButton.Values.Text = "タグ選択(&T)";
            // 
            // _selectTagContextMenuStrip
            // 
            this._selectTagContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._selectTagContextMenuStrip.Name = "_selectTagContextMenuStrip";
            this._selectTagContextMenuStrip.ShowImageMargin = false;
            this._selectTagContextMenuStrip.Size = new System.Drawing.Size(36, 4);
            // 
            // _modifiedDateLabel
            // 
            this._modifiedDateLabel.AutoSize = true;
            this._modifiedDateLabel.BackColor = System.Drawing.Color.Transparent;
            this._modifiedDateLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this._modifiedDateLabel.Location = new System.Drawing.Point(90, 86);
            this._modifiedDateLabel.Name = "_modifiedDateLabel";
            this._modifiedDateLabel.Size = new System.Drawing.Size(48, 12);
            this._modifiedDateLabel.TabIndex = 12;
            this._modifiedDateLabel.Text = "modified";
            this._modifiedDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._modifiedDateLabel.UseMnemonic = false;
            // 
            // _editorCanvas
            // 
            this._editorCanvas.AllowDrop = true;
            this._editorCanvas.AutoScroll = true;
            this._editorCanvas.BackColor = System.Drawing.Color.White;
            this._editorCanvas.ForeColor = System.Drawing.Color.Black;
            this._editorCanvas.Location = new System.Drawing.Point(20, 113);
            this._editorCanvas.MultiSelect = true;
            this._editorCanvas.Name = "_editorCanvas";
            this._editorCanvas.Size = new System.Drawing.Size(55, 38);
            this._editorCanvas.TabIndex = 0;
            // 
            // _sourceLabel
            // 
            this._sourceLabel.AutoSize = true;
            this._sourceLabel.BackColor = System.Drawing.Color.Transparent;
            this._sourceLabel.ForeColor = System.Drawing.Color.Navy;
            this._sourceLabel.Location = new System.Drawing.Point(8, 67);
            this._sourceLabel.Name = "_sourceLabel";
            this._sourceLabel.Size = new System.Drawing.Size(58, 12);
            this._sourceLabel.TabIndex = 7;
            this._sourceLabel.Text = "情報元(&S):";
            // 
            // _sourceTextBox
            // 
            this._sourceTextBox.BackColor = System.Drawing.SystemColors.Window;
            this._sourceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._sourceTextBox.Location = new System.Drawing.Point(72, 67);
            this._sourceTextBox.Name = "_sourceTextBox";
            this._sourceTextBox.Size = new System.Drawing.Size(100, 12);
            this._sourceTextBox.TabIndex = 8;
            // 
            // _createdDateLabel
            // 
            this._createdDateLabel.AutoSize = true;
            this._createdDateLabel.BackColor = System.Drawing.Color.Transparent;
            this._createdDateLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this._createdDateLabel.Location = new System.Drawing.Point(8, 86);
            this._createdDateLabel.Name = "_createdDateLabel";
            this._createdDateLabel.Size = new System.Drawing.Size(43, 12);
            this._createdDateLabel.TabIndex = 11;
            this._createdDateLabel.Text = "created";
            this._createdDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._createdDateLabel.UseMnemonic = false;
            // 
            // _showSubInfoButton
            // 
            this._showSubInfoButton.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.ButtonSpec;
            this._showSubInfoButton.Location = new System.Drawing.Point(235, 59);
            this._showSubInfoButton.Name = "_showSubInfoButton";
            this._showSubInfoButton.OverrideFocus.Content.DrawFocus = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this._showSubInfoButton.Size = new System.Drawing.Size(16, 16);
            this._showSubInfoButton.TabIndex = 9;
            this._showSubInfoButton.TabStop = false;
            this._showSubInfoButton.Values.Image = global::Mkamo.Memopad.Properties.Resources._2arrow_up;
            this._showSubInfoButton.Values.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._showSubInfoButton.Values.Text = "";
            this._showSubInfoButton.Click += new System.EventHandler(this._showSubInfoButton_Click);
            // 
            // _showSourceButton
            // 
            this._showSourceButton.AutoSize = true;
            this._showSourceButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._showSourceButton.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.ButtonSpec;
            this._showSourceButton.Location = new System.Drawing.Point(235, 84);
            this._showSourceButton.Name = "_showSourceButton";
            this._showSourceButton.OverrideFocus.Content.DrawFocus = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this._showSourceButton.Size = new System.Drawing.Size(22, 20);
            this._showSourceButton.TabIndex = 10;
            this._showSourceButton.TabStop = false;
            this._showSourceButton.UseMnemonic = false;
            this._showSourceButton.Values.Image = global::Mkamo.Memopad.Properties.Resources.arrow_045_medium;
            this._showSourceButton.Values.ImageStates.ImageCheckedNormal = null;
            this._showSourceButton.Values.ImageStates.ImageCheckedPressed = null;
            this._showSourceButton.Values.ImageStates.ImageCheckedTracking = null;
            this._showSourceButton.Values.ImageStates.ImageDisabled = global::Mkamo.Memopad.Properties.Resources.arrow_045_medium_disabled;
            this._showSourceButton.Values.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._showSourceButton.Values.Text = "";
            this._showSourceButton.Click += new System.EventHandler(this._showSourceButton_Click);
            // 
            // _tagTextBox
            // 
            this._tagTextBox.BackColor = System.Drawing.SystemColors.Window;
            this._tagTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tagTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tagTextBox.Location = new System.Drawing.Point(61, 29);
            this._tagTextBox.Name = "_tagTextBox";
            this._tagTextBox.ReadOnly = true;
            this._tagTextBox.Size = new System.Drawing.Size(100, 12);
            this._tagTextBox.TabIndex = 5;
            this._tagTextBox.TabStop = false;
            this._tagTextBox.Click += new System.EventHandler(this._tagTextBox_Click);
            // 
            // _inMemoSearcher
            // 
            this._inMemoSearcher.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._inMemoSearcher.Location = new System.Drawing.Point(3, 166);
            this._inMemoSearcher.Name = "_inMemoSearcher";
            this._inMemoSearcher.Size = new System.Drawing.Size(464, 28);
            this._inMemoSearcher.TabIndex = 1;
            this._inMemoSearcher.Visible = false;
            // 
            // PageContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this._tagTextBox);
            this.Controls.Add(this._showSubInfoButton);
            this.Controls.Add(this._showSourceButton);
            this.Controls.Add(this._inMemoSearcher);
            this.Controls.Add(this._tagLabel);
            this.Controls.Add(this._createdDateLabel);
            this.Controls.Add(this._modifiedDateLabel);
            this.Controls.Add(this._titleLabel);
            this.Controls.Add(this._selectTagDropButton);
            this.Controls.Add(this._sourceLabel);
            this.Controls.Add(this._sourceTextBox);
            this.Controls.Add(this._titleTextBox);
            this.Controls.Add(this._editorCanvas);
            this.Name = "PageContent";
            this.Size = new System.Drawing.Size(266, 197);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EditorCanvas _editorCanvas;
        private System.Windows.Forms.TextBox _titleTextBox;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.Label _tagLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonDropButton _selectTagDropButton;
        private System.Windows.Forms.Label _modifiedDateLabel;
        private System.Windows.Forms.ContextMenuStrip _selectTagContextMenuStrip;
        private InMemoSearcher _inMemoSearcher;
        private System.Windows.Forms.Label _sourceLabel;
        private System.Windows.Forms.TextBox _sourceTextBox;
        private System.Windows.Forms.Label _createdDateLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton _showSubInfoButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton _showSourceButton;
        private System.Windows.Forms.TextBox _tagTextBox;
    }
}
