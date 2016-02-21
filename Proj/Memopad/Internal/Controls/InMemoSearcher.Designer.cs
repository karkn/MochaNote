namespace Mkamo.Memopad.Internal.Controls {
    partial class InMemoSearcher {
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
            this._searchLabel = new System.Windows.Forms.Label();
            this._searchTextBox = new System.Windows.Forms.TextBox();
            this._replaceLabel = new System.Windows.Forms.Label();
            this._replaceTextBox = new System.Windows.Forms.TextBox();
            this._closeButton = new System.Windows.Forms.Button();
            this._replaceBackwardButton = new System.Windows.Forms.Button();
            this._searchBackwardButton = new System.Windows.Forms.Button();
            this._replaceForwardButton = new System.Windows.Forms.Button();
            this._searchForwardButton = new System.Windows.Forms.Button();
            this._highlightCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _searchLabel
            // 
            this._searchLabel.AutoSize = true;
            this._searchLabel.BackColor = System.Drawing.Color.Transparent;
            this._searchLabel.Location = new System.Drawing.Point(10, 8);
            this._searchLabel.Name = "_searchLabel";
            this._searchLabel.Size = new System.Drawing.Size(46, 12);
            this._searchLabel.TabIndex = 0;
            this._searchLabel.Text = "検索(&F):";
            // 
            // _searchTextBox
            // 
            this._searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._searchTextBox.Location = new System.Drawing.Point(64, 8);
            this._searchTextBox.Name = "_searchTextBox";
            this._searchTextBox.Size = new System.Drawing.Size(100, 12);
            this._searchTextBox.TabIndex = 1;
            this._searchTextBox.TextChanged += new System.EventHandler(this._searchTextBox_TextChanged);
            // 
            // _replaceLabel
            // 
            this._replaceLabel.AutoSize = true;
            this._replaceLabel.BackColor = System.Drawing.Color.Transparent;
            this._replaceLabel.Location = new System.Drawing.Point(238, 8);
            this._replaceLabel.Name = "_replaceLabel";
            this._replaceLabel.Size = new System.Drawing.Size(47, 12);
            this._replaceLabel.TabIndex = 4;
            this._replaceLabel.Text = "置換(&R):";
            // 
            // _replaceTextBox
            // 
            this._replaceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._replaceTextBox.Location = new System.Drawing.Point(292, 8);
            this._replaceTextBox.Name = "_replaceTextBox";
            this._replaceTextBox.Size = new System.Drawing.Size(100, 12);
            this._replaceTextBox.TabIndex = 5;
            // 
            // _closeButton
            // 
            this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._closeButton.BackColor = System.Drawing.Color.Transparent;
            this._closeButton.FlatAppearance.BorderSize = 0;
            this._closeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._closeButton.Image = global::Mkamo.Memopad.Properties.Resources.cross_button;
            this._closeButton.Location = new System.Drawing.Point(617, 3);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(23, 22);
            this._closeButton.TabIndex = 9;
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
            // 
            // _replaceBackwardButton
            // 
            this._replaceBackwardButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._replaceBackwardButton.BackColor = System.Drawing.Color.Transparent;
            this._replaceBackwardButton.FlatAppearance.BorderSize = 0;
            this._replaceBackwardButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._replaceBackwardButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._replaceBackwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._replaceBackwardButton.Image = global::Mkamo.Memopad.Properties.Resources.arrow_090;
            this._replaceBackwardButton.Location = new System.Drawing.Point(431, 3);
            this._replaceBackwardButton.Name = "_replaceBackwardButton";
            this._replaceBackwardButton.Size = new System.Drawing.Size(23, 22);
            this._replaceBackwardButton.TabIndex = 7;
            this._replaceBackwardButton.UseVisualStyleBackColor = true;
            this._replaceBackwardButton.Click += new System.EventHandler(this._replaceBackwardButton_Click);
            // 
            // _searchBackwardButton
            // 
            this._searchBackwardButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._searchBackwardButton.BackColor = System.Drawing.Color.Transparent;
            this._searchBackwardButton.FlatAppearance.BorderSize = 0;
            this._searchBackwardButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._searchBackwardButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._searchBackwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._searchBackwardButton.Image = global::Mkamo.Memopad.Properties.Resources.arrow_090;
            this._searchBackwardButton.Location = new System.Drawing.Point(203, 3);
            this._searchBackwardButton.Name = "_searchBackwardButton";
            this._searchBackwardButton.Size = new System.Drawing.Size(23, 22);
            this._searchBackwardButton.TabIndex = 3;
            this._searchBackwardButton.UseVisualStyleBackColor = true;
            this._searchBackwardButton.Click += new System.EventHandler(this._searchBackwordButton_Click);
            // 
            // _replaceForwardButton
            // 
            this._replaceForwardButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._replaceForwardButton.BackColor = System.Drawing.Color.Transparent;
            this._replaceForwardButton.FlatAppearance.BorderSize = 0;
            this._replaceForwardButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._replaceForwardButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._replaceForwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._replaceForwardButton.Image = global::Mkamo.Memopad.Properties.Resources.arrow_270;
            this._replaceForwardButton.Location = new System.Drawing.Point(403, 3);
            this._replaceForwardButton.Name = "_replaceForwardButton";
            this._replaceForwardButton.Size = new System.Drawing.Size(23, 22);
            this._replaceForwardButton.TabIndex = 6;
            this._replaceForwardButton.UseVisualStyleBackColor = true;
            this._replaceForwardButton.Click += new System.EventHandler(this._replaceForwardButton_Click);
            // 
            // _searchForwardButton
            // 
            this._searchForwardButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._searchForwardButton.BackColor = System.Drawing.Color.Transparent;
            this._searchForwardButton.FlatAppearance.BorderSize = 0;
            this._searchForwardButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._searchForwardButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._searchForwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._searchForwardButton.Image = global::Mkamo.Memopad.Properties.Resources.arrow_270;
            this._searchForwardButton.Location = new System.Drawing.Point(175, 3);
            this._searchForwardButton.Name = "_searchForwardButton";
            this._searchForwardButton.Size = new System.Drawing.Size(23, 22);
            this._searchForwardButton.TabIndex = 2;
            this._searchForwardButton.UseVisualStyleBackColor = true;
            this._searchForwardButton.Click += new System.EventHandler(this._searchForwardButton_Click);
            // 
            // _highlightCheckBox
            // 
            this._highlightCheckBox.AutoSize = true;
            this._highlightCheckBox.BackColor = System.Drawing.Color.Transparent;
            this._highlightCheckBox.Location = new System.Drawing.Point(464, 7);
            this._highlightCheckBox.Name = "_highlightCheckBox";
            this._highlightCheckBox.Size = new System.Drawing.Size(117, 16);
            this._highlightCheckBox.TabIndex = 8;
            this._highlightCheckBox.Text = "すべて強調表示(&H)";
            this._highlightCheckBox.UseVisualStyleBackColor = false;
            this._highlightCheckBox.CheckedChanged += new System.EventHandler(this._highlightCheckBox_CheckedChanged);
            // 
            // InMemoSearcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._closeButton);
            this.Controls.Add(this._highlightCheckBox);
            this.Controls.Add(this._replaceBackwardButton);
            this.Controls.Add(this._searchBackwardButton);
            this.Controls.Add(this._replaceForwardButton);
            this.Controls.Add(this._searchForwardButton);
            this.Controls.Add(this._replaceTextBox);
            this.Controls.Add(this._replaceLabel);
            this.Controls.Add(this._searchTextBox);
            this.Controls.Add(this._searchLabel);
            this.Name = "InMemoSearcher";
            this.Size = new System.Drawing.Size(643, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _searchLabel;
        private System.Windows.Forms.TextBox _searchTextBox;
        private System.Windows.Forms.Button _searchForwardButton;
        private System.Windows.Forms.Button _searchBackwardButton;
        private System.Windows.Forms.Label _replaceLabel;
        private System.Windows.Forms.TextBox _replaceTextBox;
        private System.Windows.Forms.Button _replaceForwardButton;
        private System.Windows.Forms.Button _replaceBackwardButton;
        private System.Windows.Forms.Button _closeButton;
        private System.Windows.Forms.CheckBox _highlightCheckBox;
    }
}
