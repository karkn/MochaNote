namespace Mkamo.Memopad.Internal.Controls {
    partial class TagSelector {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagSelector));
            this._tagTreeView = new System.Windows.Forms.TreeView();
            this._tagTreeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this._tagTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._createTagGroupBox = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this._superTagComboBox = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this._createTagTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._createTagButton = new System.Windows.Forms.Button();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.buttonSpecAny1 = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            this.buttonSpecAny2 = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            ((System.ComponentModel.ISupportInitialize) (this._createTagGroupBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this._createTagGroupBox.Panel)).BeginInit();
            this._createTagGroupBox.Panel.SuspendLayout();
            this._createTagGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this._superTagComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tagTreeView
            // 
            this._tagTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tagTreeView.CheckBoxes = true;
            this._tagTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tagTreeView.ForeColor = System.Drawing.SystemColors.WindowText;
            this._tagTreeView.ImageIndex = 0;
            this._tagTreeView.ImageList = this._tagTreeViewImageList;
            this._tagTreeView.ImeMode = System.Windows.Forms.ImeMode.Off;
            this._tagTreeView.Location = new System.Drawing.Point(1, 1);
            this._tagTreeView.Name = "_tagTreeView";
            this._tagTreeView.SelectedImageIndex = 0;
            this._tagTreeView.Size = new System.Drawing.Size(238, 261);
            this._tagTreeView.TabIndex = 0;
            // 
            // _tagTreeViewImageList
            // 
            this._tagTreeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("_tagTreeViewImageList.ImageStream")));
            this._tagTreeViewImageList.TransparentColor = System.Drawing.Color.Magenta;
            this._tagTreeViewImageList.Images.SetKeyName(0, "tag-label.png");
            this._tagTreeViewImageList.Images.SetKeyName(1, "tags-label.png");
            this._tagTreeViewImageList.Images.SetKeyName(2, "box.png");
            // 
            // _tagTextBox
            // 
            this._tagTextBox.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this.buttonSpecAny1});
            this._tagTextBox.Location = new System.Drawing.Point(0, 3);
            this._tagTextBox.Name = "_tagTextBox";
            this._tagTextBox.Size = new System.Drawing.Size(240, 24);
            this._tagTextBox.TabIndex = 0;
            this._tagTextBox.TextChanged += new System.EventHandler(this._tagTextBox_TextChanged);
            // 
            // _createTagGroupBox
            // 
            this._createTagGroupBox.Location = new System.Drawing.Point(0, 301);
            this._createTagGroupBox.Name = "_createTagGroupBox";
            // 
            // _createTagGroupBox.Panel
            // 
            this._createTagGroupBox.Panel.Controls.Add(this._superTagComboBox);
            this._createTagGroupBox.Panel.Controls.Add(this._createTagTextBox);
            this._createTagGroupBox.Panel.Controls.Add(this._createTagButton);
            this._createTagGroupBox.Panel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this._createTagGroupBox.Size = new System.Drawing.Size(240, 86);
            this._createTagGroupBox.StateCommon.Back.Color1 = System.Drawing.Color.Transparent;
            this._createTagGroupBox.TabIndex = 2;
            this._createTagGroupBox.Values.Heading = "タグの新規作成";
            // 
            // _superTagComboBox
            // 
            this._superTagComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._superTagComboBox.DropDownWidth = 200;
            this._superTagComboBox.FormattingEnabled = true;
            this._superTagComboBox.Location = new System.Drawing.Point(8, 32);
            this._superTagComboBox.Name = "_superTagComboBox";
            this._superTagComboBox.Size = new System.Drawing.Size(149, 23);
            this._superTagComboBox.TabIndex = 1;
            this._superTagComboBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this._superTagComboBox_Format);
            // 
            // _createTagTextBox
            // 
            this._createTagTextBox.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this.buttonSpecAny2});
            this._createTagTextBox.Location = new System.Drawing.Point(8, 3);
            this._createTagTextBox.Name = "_createTagTextBox";
            this._createTagTextBox.Size = new System.Drawing.Size(217, 24);
            this._createTagTextBox.TabIndex = 0;
            this._createTagTextBox.TextChanged += new System.EventHandler(this._createTagTextBox_TextChanged);
            // 
            // _createTagButton
            // 
            this._createTagButton.BackColor = System.Drawing.SystemColors.Control;
            this._createTagButton.Location = new System.Drawing.Point(163, 32);
            this._createTagButton.Name = "_createTagButton";
            this._createTagButton.Size = new System.Drawing.Size(62, 23);
            this._createTagButton.TabIndex = 2;
            this._createTagButton.Text = "作成(&C)";
            this._createTagButton.UseVisualStyleBackColor = false;
            this._createTagButton.Click += new System.EventHandler(this._createTagButton_Click);
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this._tagTreeView);
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 32);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.kryptonPanel1.Size = new System.Drawing.Size(240, 263);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // buttonSpecAny1
            // 
            this.buttonSpecAny1.Enabled = ComponentFactory.Krypton.Toolkit.ButtonEnabled.False;
            this.buttonSpecAny1.Image = global::Mkamo.Memopad.Properties.Resources.magnifier;
            this.buttonSpecAny1.UniqueName = "BEA348B4771648650CA5EA6B51B4F484";
            // 
            // buttonSpecAny2
            // 
            this.buttonSpecAny2.Enabled = ComponentFactory.Krypton.Toolkit.ButtonEnabled.False;
            this.buttonSpecAny2.Image = global::Mkamo.Memopad.Properties.Resources.tag_plus;
            this.buttonSpecAny2.UniqueName = "C440A6CCB3224BEAECB339CD35AC6CF0";
            // 
            // TagSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.kryptonPanel1);
            this.Controls.Add(this._createTagGroupBox);
            this.Controls.Add(this._tagTextBox);
            this.Name = "TagSelector";
            this.Size = new System.Drawing.Size(240, 398);
            ((System.ComponentModel.ISupportInitialize) (this._createTagGroupBox.Panel)).EndInit();
            this._createTagGroupBox.Panel.ResumeLayout(false);
            this._createTagGroupBox.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this._createTagGroupBox)).EndInit();
            this._createTagGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this._superTagComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _tagTreeView;
        private System.Windows.Forms.ImageList _tagTreeViewImageList;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _tagTextBox;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox _createTagGroupBox;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox _superTagComboBox;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _createTagTextBox;
        private System.Windows.Forms.Button _createTagButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny buttonSpecAny1;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny buttonSpecAny2;
    }
}
