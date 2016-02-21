using Mkamo.Memopad.Internal.Controls;
using Mkamo.Control.TabControlEx;
namespace Mkamo.Memopad.Internal.Forms {
    partial class MemopadForm {
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
            CleanUp();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemopadForm));
            this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._memoCountStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._smartFilterToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._messageToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._canvasSizeToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._spaceRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._spaceDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._finderSplitContainer = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this._finderHeaderGroup = new ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup();
            this._configForTagButtonSpecHeaderGroup = new ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup();
            this._workspaceTagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._showDescendantTagsMemoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._workspaceFoldButtonSpecHeaderGroup = new ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup();
            this._workspaceViewPanel = new System.Windows.Forms.Panel();
            this._workspaceView = new Mkamo.Memopad.Internal.Controls.WorkspaceView();
            this._adPanel = new System.Windows.Forms.Panel();
            this._adLinkLabel = new System.Windows.Forms.LinkLabel();
            this._conditionPanel = new System.Windows.Forms.Panel();
            this._conditionTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._searchButtonSpec = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            this._cancelSearchButtonSpec = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            this._memoListSplitContainer = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this._memoListHeaderGroup = new ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup();
            this._memoListViewSmartFilterButtonSpecHeaderGroup = new ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup();
            this._memoListViewSmartFilterContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._memoListViewManageSmartFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._memoListViewSmartFilterSplitterToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this._clearSmartFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._memoListViewDisplayItemButtonSpecHeaderGroup = new ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup();
            this._memoListViewDisplayItemContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._createdDateDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._modifiedDateDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._accessedDateDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tagDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._summaryTextDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._memoListViewSortButtonSpecHeaderGroup = new ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup();
            this._memoListViewSortContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._sortByTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._sortByCreatedDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._sortByModifiedDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._sortByAccessedDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this._sortByAscendingOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._sortByDescendingOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripSeparator();
            this._sortByImortanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._memoListFoldButtonSpecHeaderGroup = new ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup();
            this._memoListView = new Mkamo.Memopad.Internal.Controls.MemoListView();
            this._tabControl = new Mkamo.Control.TabControlEx.TabControlEx();
            this._mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this._fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._createMemoFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._createFusenMemoFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._createMemoFromClipboardFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this._saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._saveAsHtmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._saveAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._saveAsEmfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._saveAsPngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._saveAsJpegToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._sendMailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripSeparator();
            this._exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exportHtmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exportTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exportNextToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this._printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._nextPrintSeparatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this._noteFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exportToFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._importFromFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this._exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this._cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this._selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this._findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._focusConditionTextBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._showWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._showMemoListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this._showStartPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this._showFusenMemosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._nextShowFusenMemosSeparatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this._compactModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._displayTopMostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._dummyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._manageTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._manageSmartFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this._optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._showHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this._checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._aboutBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mainToolStrip = new System.Windows.Forms.ToolStrip();
            this._createMemoToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this._createMemoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._createFusenMemoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._createMemoFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this._activeFolderToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this._showAllFusenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._showAsFusenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._nextShowAsFusenToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this._cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this._undoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._redoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this._searchInMemoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this._selectToolToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._handToolToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._adjustSpaceToolToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._addFreehandToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._addFigureToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._addImageToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._addImageFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._addImageFromScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._addFileToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._addEmbededFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._addShortcutFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._addFolderShortcutFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._addTableToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._tableNextToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this._setNodeStyleToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._setLineStyleToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._shapeColorButtonToolStripItem = new Mkamo.Control.ToolStrip.KryptonColorButtonToolStripItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this._memoMarkToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this._importantToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._unimportantToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._editToolStrip = new System.Windows.Forms.ToolStrip();
            this._paragraphKindToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this._fontNameToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this._fontSizeToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this._fontBoldToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._fontItalicToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._fontUnderlineToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._fontStrikeoutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._textColorButtonToolStripItem = new Mkamo.Control.ToolStrip.KryptonColorButtonToolStripItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this._leftHorizontalAlignmentToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._centerHorizontalAlignmentToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._rightHorizontalAlignmentToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._verticalAlignToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._topVAlignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._centerVAlignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._bottomVAlignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._unorderedListToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._orderedListToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._specialListToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._selectSpecialListToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._checkBoxListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._triStateCheckBoxListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._starListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._leftArrowListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._rightArrowListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this._outdentToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._indentToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this._addCommentToolStripButton = new System.Windows.Forms.ToolStripButton();
            this._tabControlContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._closeMemoTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._closeOtherMemoTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._closeAllMemoTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this._showFusenTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._nextShowFusenSeparatorTabToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this._removeMemoTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this._maximizeEditorSizeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._restoreEditorSizeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this._toolStripContainer.ContentPanel.SuspendLayout();
            this._toolStripContainer.TopToolStripPanel.SuspendLayout();
            this._toolStripContainer.SuspendLayout();
            this._statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._finderSplitContainer)).BeginInit();
            this._finderSplitContainer.Panel1.SuspendLayout();
            this._finderSplitContainer.Panel2.SuspendLayout();
            this._finderSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._finderHeaderGroup)).BeginInit();
            this._finderHeaderGroup.Panel.SuspendLayout();
            this._finderHeaderGroup.SuspendLayout();
            this._workspaceTagContextMenuStrip.SuspendLayout();
            this._workspaceViewPanel.SuspendLayout();
            this._adPanel.SuspendLayout();
            this._conditionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._memoListSplitContainer)).BeginInit();
            this._memoListSplitContainer.Panel1.SuspendLayout();
            this._memoListSplitContainer.Panel2.SuspendLayout();
            this._memoListSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._memoListHeaderGroup)).BeginInit();
            this._memoListHeaderGroup.Panel.SuspendLayout();
            this._memoListHeaderGroup.SuspendLayout();
            this._memoListViewSmartFilterContextMenuStrip.SuspendLayout();
            this._memoListViewDisplayItemContextMenuStrip.SuspendLayout();
            this._memoListViewSortContextMenuStrip.SuspendLayout();
            this._mainMenuStrip.SuspendLayout();
            this._mainToolStrip.SuspendLayout();
            this._editToolStrip.SuspendLayout();
            this._tabControlContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _toolStripContainer
            // 
            // 
            // _toolStripContainer.BottomToolStripPanel
            // 
            this._toolStripContainer.BottomToolStripPanel.Controls.Add(this._statusStrip);
            // 
            // _toolStripContainer.ContentPanel
            // 
            this._toolStripContainer.ContentPanel.BackColor = System.Drawing.SystemColors.Control;
            this._toolStripContainer.ContentPanel.Controls.Add(this._finderSplitContainer);
            this._toolStripContainer.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
            this._toolStripContainer.ContentPanel.Size = new System.Drawing.Size(892, 574);
            this._toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._toolStripContainer.LeftToolStripPanelVisible = false;
            this._toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this._toolStripContainer.Name = "_toolStripContainer";
            this._toolStripContainer.RightToolStripPanelVisible = false;
            this._toolStripContainer.Size = new System.Drawing.Size(892, 673);
            this._toolStripContainer.TabIndex = 0;
            this._toolStripContainer.Text = "toolStripContainer1";
            // 
            // _toolStripContainer.TopToolStripPanel
            // 
            this._toolStripContainer.TopToolStripPanel.Controls.Add(this._mainMenuStrip);
            this._toolStripContainer.TopToolStripPanel.Controls.Add(this._mainToolStrip);
            this._toolStripContainer.TopToolStripPanel.Controls.Add(this._editToolStrip);
            // 
            // _statusStrip
            // 
            this._statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._statusStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._memoCountStatusLabel,
            this._smartFilterToolStripStatusLabel,
            this._messageToolStripStatusLabel,
            this._canvasSizeToolStripDropDownButton});
            this._statusStrip.Location = new System.Drawing.Point(0, 0);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this._statusStrip.Size = new System.Drawing.Size(892, 25);
            this._statusStrip.TabIndex = 0;
            // 
            // _memoCountStatusLabel
            // 
            this._memoCountStatusLabel.Image = global::Mkamo.Memopad.Properties.Resources.sticky_notes;
            this._memoCountStatusLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._memoCountStatusLabel.Margin = new System.Windows.Forms.Padding(0, 3, 10, 2);
            this._memoCountStatusLabel.Name = "_memoCountStatusLabel";
            this._memoCountStatusLabel.Size = new System.Drawing.Size(61, 20);
            this._memoCountStatusLabel.Text = "ノート数";
            // 
            // _smartFilterToolStripStatusLabel
            // 
            this._smartFilterToolStripStatusLabel.Image = ((System.Drawing.Image)(resources.GetObject("_smartFilterToolStripStatusLabel.Image")));
            this._smartFilterToolStripStatusLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._smartFilterToolStripStatusLabel.Margin = new System.Windows.Forms.Padding(0, 3, 10, 2);
            this._smartFilterToolStripStatusLabel.Name = "_smartFilterToolStripStatusLabel";
            this._smartFilterToolStripStatusLabel.Size = new System.Drawing.Size(92, 20);
            this._smartFilterToolStripStatusLabel.Text = "スマートフィルタ";
            // 
            // _messageToolStripStatusLabel
            // 
            this._messageToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this._messageToolStripStatusLabel.Name = "_messageToolStripStatusLabel";
            this._messageToolStripStatusLabel.Size = new System.Drawing.Size(594, 20);
            this._messageToolStripStatusLabel.Spring = true;
            this._messageToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _canvasSizeToolStripDropDownButton
            // 
            this._canvasSizeToolStripDropDownButton.AutoSize = false;
            this._canvasSizeToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._spaceRightToolStripMenuItem,
            this._spaceDownToolStripMenuItem});
            this._canvasSizeToolStripDropDownButton.Image = global::Mkamo.Memopad.Properties.Resources.canvas_size;
            this._canvasSizeToolStripDropDownButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._canvasSizeToolStripDropDownButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._canvasSizeToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._canvasSizeToolStripDropDownButton.Name = "_canvasSizeToolStripDropDownButton";
            this._canvasSizeToolStripDropDownButton.Size = new System.Drawing.Size(110, 23);
            this._canvasSizeToolStripDropDownButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _spaceRightToolStripMenuItem
            // 
            this._spaceRightToolStripMenuItem.Name = "_spaceRightToolStripMenuItem";
            this._spaceRightToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._spaceRightToolStripMenuItem.Text = "右に余白(&R)";
            this._spaceRightToolStripMenuItem.Click += new System.EventHandler(this._spaceRightToolStripMenuItem_Click);
            // 
            // _spaceDownToolStripMenuItem
            // 
            this._spaceDownToolStripMenuItem.Name = "_spaceDownToolStripMenuItem";
            this._spaceDownToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._spaceDownToolStripMenuItem.Text = "下に余白(&B)";
            this._spaceDownToolStripMenuItem.Click += new System.EventHandler(this._spaceDownToolStripMenuItem_Click);
            // 
            // _finderSplitContainer
            // 
            this._finderSplitContainer.Cursor = System.Windows.Forms.Cursors.Default;
            this._finderSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._finderSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._finderSplitContainer.Location = new System.Drawing.Point(0, 0);
            this._finderSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this._finderSplitContainer.Name = "_finderSplitContainer";
            // 
            // _finderSplitContainer.Panel1
            // 
            this._finderSplitContainer.Panel1.Controls.Add(this._finderHeaderGroup);
            this._finderSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this._finderSplitContainer.Panel1MinSize = 100;
            // 
            // _finderSplitContainer.Panel2
            // 
            this._finderSplitContainer.Panel2.Controls.Add(this._memoListSplitContainer);
            this._finderSplitContainer.Panel2MinSize = 100;
            this._finderSplitContainer.Size = new System.Drawing.Size(892, 574);
            this._finderSplitContainer.SplitterDistance = 200;
            this._finderSplitContainer.TabIndex = 0;
            // 
            // _finderHeaderGroup
            // 
            this._finderHeaderGroup.AllowButtonSpecToolTips = true;
            this._finderHeaderGroup.AutoCollapseArrow = false;
            this._finderHeaderGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this._finderHeaderGroup.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup[] {
            this._configForTagButtonSpecHeaderGroup,
            this._workspaceFoldButtonSpecHeaderGroup});
            this._finderHeaderGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._finderHeaderGroup.HeaderStylePrimary = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
            this._finderHeaderGroup.HeaderVisibleSecondary = false;
            this._finderHeaderGroup.Location = new System.Drawing.Point(0, 2);
            this._finderHeaderGroup.Name = "_finderHeaderGroup";
            // 
            // _finderHeaderGroup.Panel
            // 
            this._finderHeaderGroup.Panel.Controls.Add(this._workspaceViewPanel);
            this._finderHeaderGroup.Panel.Controls.Add(this._adPanel);
            this._finderHeaderGroup.Panel.Controls.Add(this._conditionPanel);
            this._finderHeaderGroup.Size = new System.Drawing.Size(200, 570);
            this._finderHeaderGroup.TabIndex = 0;
            this._finderHeaderGroup.ValuesPrimary.Heading = "ワークスペース";
            this._finderHeaderGroup.ValuesPrimary.Image = null;
            // 
            // _configForTagButtonSpecHeaderGroup
            // 
            this._configForTagButtonSpecHeaderGroup.ContextMenuStrip = this._workspaceTagContextMenuStrip;
            this._configForTagButtonSpecHeaderGroup.Image = global::Mkamo.Memopad.Properties.Resources.tags_label;
            this._configForTagButtonSpecHeaderGroup.ToolTipTitle = "タグ";
            this._configForTagButtonSpecHeaderGroup.UniqueName = "798E6FF0B61341CCFE946EFA46CA8768";
            // 
            // _workspaceTagContextMenuStrip
            // 
            this._workspaceTagContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._workspaceTagContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._showDescendantTagsMemoToolStripMenuItem});
            this._workspaceTagContextMenuStrip.Name = "_workspaceTagContextMenuStrip";
            this._workspaceTagContextMenuStrip.Size = new System.Drawing.Size(201, 26);
            this._workspaceTagContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._workspaceTagContextMenuStrip_Opening);
            // 
            // _showDescendantTagsMemoToolStripMenuItem
            // 
            this._showDescendantTagsMemoToolStripMenuItem.Name = "_showDescendantTagsMemoToolStripMenuItem";
            this._showDescendantTagsMemoToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this._showDescendantTagsMemoToolStripMenuItem.Text = "子孫タグのノートも表示(&D)";
            this._showDescendantTagsMemoToolStripMenuItem.Click += new System.EventHandler(this._showDescendantTagsMemoToolStripMenuItem_Click);
            // 
            // _workspaceFoldButtonSpecHeaderGroup
            // 
            this._workspaceFoldButtonSpecHeaderGroup.Type = ComponentFactory.Krypton.Toolkit.PaletteButtonSpecStyle.ArrowLeft;
            this._workspaceFoldButtonSpecHeaderGroup.UniqueName = "24CF8058096740867982C9B516FD4424";
            this._workspaceFoldButtonSpecHeaderGroup.Click += new System.EventHandler(this._finderFoldButtonSpecHeaderGroup_Click);
            // 
            // _workspaceViewPanel
            // 
            this._workspaceViewPanel.Controls.Add(this._workspaceView);
            this._workspaceViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workspaceViewPanel.Location = new System.Drawing.Point(0, 35);
            this._workspaceViewPanel.Name = "_workspaceViewPanel";
            this._workspaceViewPanel.Size = new System.Drawing.Size(198, 473);
            this._workspaceViewPanel.TabIndex = 1;
            // 
            // _workspaceView
            // 
            this._workspaceView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workspaceView.Location = new System.Drawing.Point(0, 0);
            this._workspaceView.Name = "_workspaceView";
            this._workspaceView.Size = new System.Drawing.Size(198, 473);
            this._workspaceView.TabIndex = 2;
            this._workspaceView.Visible = false;
            // 
            // _adPanel
            // 
            this._adPanel.BackColor = System.Drawing.Color.White;
            this._adPanel.Controls.Add(this._adLinkLabel);
            this._adPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._adPanel.Location = new System.Drawing.Point(0, 508);
            this._adPanel.Name = "_adPanel";
            this._adPanel.Size = new System.Drawing.Size(198, 35);
            this._adPanel.TabIndex = 2;
            // 
            // _adLinkLabel
            // 
            this._adLinkLabel.AutoSize = true;
            this._adLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(70)))), ((int)(((byte)(210)))));
            this._adLinkLabel.Location = new System.Drawing.Point(8, 12);
            this._adLinkLabel.Name = "_adLinkLabel";
            this._adLinkLabel.Size = new System.Drawing.Size(131, 12);
            this._adLinkLabel.TabIndex = 0;
            this._adLinkLabel.TabStop = true;
            this._adLinkLabel.Text = "プレミアムライセンスについて";
            this._adLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._adLinkLabel_LinkClicked);
            // 
            // _conditionPanel
            // 
            this._conditionPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this._conditionPanel.Controls.Add(this._conditionTextBox);
            this._conditionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._conditionPanel.Location = new System.Drawing.Point(0, 0);
            this._conditionPanel.Name = "_conditionPanel";
            this._conditionPanel.Padding = new System.Windows.Forms.Padding(5);
            this._conditionPanel.Size = new System.Drawing.Size(198, 35);
            this._conditionPanel.TabIndex = 0;
            // 
            // _conditionTextBox
            // 
            this._conditionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._conditionTextBox.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this._searchButtonSpec,
            this._cancelSearchButtonSpec});
            this._conditionTextBox.Location = new System.Drawing.Point(7, 8);
            this._conditionTextBox.Name = "_conditionTextBox";
            this._conditionTextBox.Size = new System.Drawing.Size(182, 24);
            this._conditionTextBox.TabIndex = 4;
            this._conditionTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._searchTextBox_KeyDown);
            // 
            // _searchButtonSpec
            // 
            this._searchButtonSpec.Image = global::Mkamo.Memopad.Properties.Resources.magnifier;
            this._searchButtonSpec.UniqueName = "0A4795DC045B457A528EEC22D4C6688C";
            this._searchButtonSpec.Click += new System.EventHandler(this._searchButtonSpec_Click);
            // 
            // _cancelSearchButtonSpec
            // 
            this._cancelSearchButtonSpec.Type = ComponentFactory.Krypton.Toolkit.PaletteButtonSpecStyle.Close;
            this._cancelSearchButtonSpec.UniqueName = "BDB6D2D8700747857883F7FF1F01FDE0";
            this._cancelSearchButtonSpec.Click += new System.EventHandler(this._cancelSearchButtonSpec_Click);
            // 
            // _memoListSplitContainer
            // 
            this._memoListSplitContainer.Cursor = System.Windows.Forms.Cursors.Default;
            this._memoListSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._memoListSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._memoListSplitContainer.Location = new System.Drawing.Point(0, 0);
            this._memoListSplitContainer.Name = "_memoListSplitContainer";
            // 
            // _memoListSplitContainer.Panel1
            // 
            this._memoListSplitContainer.Panel1.Controls.Add(this._memoListHeaderGroup);
            this._memoListSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this._memoListSplitContainer.Panel1MinSize = 100;
            // 
            // _memoListSplitContainer.Panel2
            // 
            this._memoListSplitContainer.Panel2.Controls.Add(this._tabControl);
            this._memoListSplitContainer.Panel2MinSize = 100;
            this._memoListSplitContainer.Size = new System.Drawing.Size(687, 574);
            this._memoListSplitContainer.SplitterDistance = 200;
            this._memoListSplitContainer.SplitterWidth = 3;
            this._memoListSplitContainer.TabIndex = 2;
            // 
            // _memoListHeaderGroup
            // 
            this._memoListHeaderGroup.AllowButtonSpecToolTips = true;
            this._memoListHeaderGroup.AutoCollapseArrow = false;
            this._memoListHeaderGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this._memoListHeaderGroup.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup[] {
            this._memoListViewSmartFilterButtonSpecHeaderGroup,
            this._memoListViewDisplayItemButtonSpecHeaderGroup,
            this._memoListViewSortButtonSpecHeaderGroup,
            this._memoListFoldButtonSpecHeaderGroup});
            this._memoListHeaderGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._memoListHeaderGroup.HeaderStylePrimary = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
            this._memoListHeaderGroup.HeaderVisibleSecondary = false;
            this._memoListHeaderGroup.Location = new System.Drawing.Point(0, 2);
            this._memoListHeaderGroup.Name = "_memoListHeaderGroup";
            // 
            // _memoListHeaderGroup.Panel
            // 
            this._memoListHeaderGroup.Panel.Controls.Add(this._memoListView);
            this._memoListHeaderGroup.Size = new System.Drawing.Size(200, 570);
            this._memoListHeaderGroup.TabIndex = 0;
            this._memoListHeaderGroup.ValuesPrimary.Heading = "ノートリスト";
            this._memoListHeaderGroup.ValuesPrimary.Image = null;
            // 
            // _memoListViewSmartFilterButtonSpecHeaderGroup
            // 
            this._memoListViewSmartFilterButtonSpecHeaderGroup.ContextMenuStrip = this._memoListViewSmartFilterContextMenuStrip;
            this._memoListViewSmartFilterButtonSpecHeaderGroup.Image = global::Mkamo.Memopad.Properties.Resources.filter;
            this._memoListViewSmartFilterButtonSpecHeaderGroup.ToolTipTitle = "スマートフィルタ";
            this._memoListViewSmartFilterButtonSpecHeaderGroup.UniqueName = "6628DED6D2DF4BE3D59DA36F61CF6292";
            // 
            // _memoListViewSmartFilterContextMenuStrip
            // 
            this._memoListViewSmartFilterContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._memoListViewSmartFilterContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._memoListViewManageSmartFilterToolStripMenuItem,
            this._memoListViewSmartFilterSplitterToolStripMenuItem,
            this._clearSmartFilterToolStripMenuItem});
            this._memoListViewSmartFilterContextMenuStrip.Name = "_memoListViewFilterContextMenuStrip";
            this._memoListViewSmartFilterContextMenuStrip.Size = new System.Drawing.Size(206, 54);
            this._memoListViewSmartFilterContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._memoListViewSmartFilterContextMenuStrip_Opening);
            // 
            // _memoListViewManageSmartFilterToolStripMenuItem
            // 
            this._memoListViewManageSmartFilterToolStripMenuItem.Name = "_memoListViewManageSmartFilterToolStripMenuItem";
            this._memoListViewManageSmartFilterToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this._memoListViewManageSmartFilterToolStripMenuItem.Text = "スマートフィルタの管理(&M)...";
            this._memoListViewManageSmartFilterToolStripMenuItem.Click += new System.EventHandler(this._memoListViewManageSmartFilterToolStripMenuItem_Click);
            // 
            // _memoListViewSmartFilterSplitterToolStripMenuItem
            // 
            this._memoListViewSmartFilterSplitterToolStripMenuItem.Name = "_memoListViewSmartFilterSplitterToolStripMenuItem";
            this._memoListViewSmartFilterSplitterToolStripMenuItem.Size = new System.Drawing.Size(202, 6);
            // 
            // _clearSmartFilterToolStripMenuItem
            // 
            this._clearSmartFilterToolStripMenuItem.Name = "_clearSmartFilterToolStripMenuItem";
            this._clearSmartFilterToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this._clearSmartFilterToolStripMenuItem.Text = "スマートフィルタなし";
            this._clearSmartFilterToolStripMenuItem.Click += new System.EventHandler(this._clearSmartFilterToolStripMenuItem_Click);
            // 
            // _memoListViewDisplayItemButtonSpecHeaderGroup
            // 
            this._memoListViewDisplayItemButtonSpecHeaderGroup.ContextMenuStrip = this._memoListViewDisplayItemContextMenuStrip;
            this._memoListViewDisplayItemButtonSpecHeaderGroup.Image = global::Mkamo.Memopad.Properties.Resources.ui_list_box_blue;
            this._memoListViewDisplayItemButtonSpecHeaderGroup.ToolTipTitle = "表示項目";
            this._memoListViewDisplayItemButtonSpecHeaderGroup.UniqueName = "44BB3EC729484BC66081D0C5DB178CD9";
            // 
            // _memoListViewDisplayItemContextMenuStrip
            // 
            this._memoListViewDisplayItemContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._memoListViewDisplayItemContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._createdDateDisplayToolStripMenuItem,
            this._modifiedDateDisplayToolStripMenuItem,
            this._accessedDateDisplayToolStripMenuItem,
            this._tagDisplayToolStripMenuItem,
            this._summaryTextDisplayToolStripMenuItem});
            this._memoListViewDisplayItemContextMenuStrip.Name = "_memoListViewVisibleItemContextMenuStrip";
            this._memoListViewDisplayItemContextMenuStrip.Size = new System.Drawing.Size(135, 114);
            this._memoListViewDisplayItemContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._memoListViewDisplayItemContextMenuStrip_Opening);
            // 
            // _createdDateDisplayToolStripMenuItem
            // 
            this._createdDateDisplayToolStripMenuItem.Name = "_createdDateDisplayToolStripMenuItem";
            this._createdDateDisplayToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._createdDateDisplayToolStripMenuItem.Text = "作成日時";
            this._createdDateDisplayToolStripMenuItem.Click += new System.EventHandler(this._createdDateDisplayToolStripMenuItem_Click);
            // 
            // _modifiedDateDisplayToolStripMenuItem
            // 
            this._modifiedDateDisplayToolStripMenuItem.Name = "_modifiedDateDisplayToolStripMenuItem";
            this._modifiedDateDisplayToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._modifiedDateDisplayToolStripMenuItem.Text = "更新日時";
            this._modifiedDateDisplayToolStripMenuItem.Click += new System.EventHandler(this._modifiedDateDisplayToolStripMenuItem_Click);
            // 
            // _accessedDateDisplayToolStripMenuItem
            // 
            this._accessedDateDisplayToolStripMenuItem.Name = "_accessedDateDisplayToolStripMenuItem";
            this._accessedDateDisplayToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._accessedDateDisplayToolStripMenuItem.Text = "アクセス日時";
            this._accessedDateDisplayToolStripMenuItem.Click += new System.EventHandler(this._accessedDateDisplayToolStripMenuItem_Click);
            // 
            // _tagDisplayToolStripMenuItem
            // 
            this._tagDisplayToolStripMenuItem.Name = "_tagDisplayToolStripMenuItem";
            this._tagDisplayToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._tagDisplayToolStripMenuItem.Text = "タグ";
            this._tagDisplayToolStripMenuItem.Click += new System.EventHandler(this._tagDisplayToolStripMenuItem_Click);
            // 
            // _summaryTextDisplayToolStripMenuItem
            // 
            this._summaryTextDisplayToolStripMenuItem.Name = "_summaryTextDisplayToolStripMenuItem";
            this._summaryTextDisplayToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._summaryTextDisplayToolStripMenuItem.Text = "要約文";
            this._summaryTextDisplayToolStripMenuItem.Click += new System.EventHandler(this._summaryTextDisplayToolStripMenuItem_Click);
            // 
            // _memoListViewSortButtonSpecHeaderGroup
            // 
            this._memoListViewSortButtonSpecHeaderGroup.ContextMenuStrip = this._memoListViewSortContextMenuStrip;
            this._memoListViewSortButtonSpecHeaderGroup.Image = global::Mkamo.Memopad.Properties.Resources.SortHS;
            this._memoListViewSortButtonSpecHeaderGroup.ToolTipTitle = "表示順序";
            this._memoListViewSortButtonSpecHeaderGroup.UniqueName = "BB6E596890E148381EBF612D72D660EB";
            // 
            // _memoListViewSortContextMenuStrip
            // 
            this._memoListViewSortContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._memoListViewSortContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._sortByTitleToolStripMenuItem,
            this._sortByCreatedDateToolStripMenuItem,
            this._sortByModifiedDateToolStripMenuItem,
            this._sortByAccessedDateToolStripMenuItem,
            this.toolStripMenuItem4,
            this._sortByAscendingOrderToolStripMenuItem,
            this._sortByDescendingOrderToolStripMenuItem,
            this.toolStripMenuItem15,
            this._sortByImortanceToolStripMenuItem});
            this._memoListViewSortContextMenuStrip.Name = "_memoListViewSortContextMenuStrip";
            this._memoListViewSortContextMenuStrip.Size = new System.Drawing.Size(135, 170);
            this._memoListViewSortContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._memoListViewSortContextMenuStrip_Opening);
            // 
            // _sortByTitleToolStripMenuItem
            // 
            this._sortByTitleToolStripMenuItem.Name = "_sortByTitleToolStripMenuItem";
            this._sortByTitleToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._sortByTitleToolStripMenuItem.Text = "タイトル";
            this._sortByTitleToolStripMenuItem.Click += new System.EventHandler(this._sortByTitleToolStripMenuItem_Click);
            // 
            // _sortByCreatedDateToolStripMenuItem
            // 
            this._sortByCreatedDateToolStripMenuItem.Name = "_sortByCreatedDateToolStripMenuItem";
            this._sortByCreatedDateToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._sortByCreatedDateToolStripMenuItem.Text = "作成日時";
            this._sortByCreatedDateToolStripMenuItem.Click += new System.EventHandler(this._sortByCreatedDateToolStripMenuItem_Click);
            // 
            // _sortByModifiedDateToolStripMenuItem
            // 
            this._sortByModifiedDateToolStripMenuItem.Name = "_sortByModifiedDateToolStripMenuItem";
            this._sortByModifiedDateToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._sortByModifiedDateToolStripMenuItem.Text = "更新日時";
            this._sortByModifiedDateToolStripMenuItem.Click += new System.EventHandler(this._sortByModifiedDateToolStripMenuItem_Click);
            // 
            // _sortByAccessedDateToolStripMenuItem
            // 
            this._sortByAccessedDateToolStripMenuItem.Name = "_sortByAccessedDateToolStripMenuItem";
            this._sortByAccessedDateToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._sortByAccessedDateToolStripMenuItem.Text = "アクセス日時";
            this._sortByAccessedDateToolStripMenuItem.Click += new System.EventHandler(this._sortByAccessedDateToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(131, 6);
            // 
            // _sortByAscendingOrderToolStripMenuItem
            // 
            this._sortByAscendingOrderToolStripMenuItem.Name = "_sortByAscendingOrderToolStripMenuItem";
            this._sortByAscendingOrderToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._sortByAscendingOrderToolStripMenuItem.Text = "昇順";
            this._sortByAscendingOrderToolStripMenuItem.Click += new System.EventHandler(this._sortByAscendingOrderToolStripMenuItem_Click);
            // 
            // _sortByDescendingOrderToolStripMenuItem
            // 
            this._sortByDescendingOrderToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._sortByDescendingOrderToolStripMenuItem.Name = "_sortByDescendingOrderToolStripMenuItem";
            this._sortByDescendingOrderToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._sortByDescendingOrderToolStripMenuItem.Text = "降順";
            this._sortByDescendingOrderToolStripMenuItem.Click += new System.EventHandler(this._sortByDescendingOrderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(131, 6);
            // 
            // _sortByImortanceToolStripMenuItem
            // 
            this._sortByImortanceToolStripMenuItem.Name = "_sortByImortanceToolStripMenuItem";
            this._sortByImortanceToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._sortByImortanceToolStripMenuItem.Text = "重要度";
            this._sortByImortanceToolStripMenuItem.Click += new System.EventHandler(this._sortByImortanceToolStripMenuItem_Click);
            // 
            // _memoListFoldButtonSpecHeaderGroup
            // 
            this._memoListFoldButtonSpecHeaderGroup.Type = ComponentFactory.Krypton.Toolkit.PaletteButtonSpecStyle.ArrowLeft;
            this._memoListFoldButtonSpecHeaderGroup.UniqueName = "D9DF40E91727483198ABC525ACA624D2";
            this._memoListFoldButtonSpecHeaderGroup.Click += new System.EventHandler(this._memoListButtonSpecHeaderGroup_Click);
            // 
            // _memoListView
            // 
            this._memoListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._memoListView.Location = new System.Drawing.Point(0, 0);
            this._memoListView.Margin = new System.Windows.Forms.Padding(0);
            this._memoListView.Name = "_memoListView";
            this._memoListView.Size = new System.Drawing.Size(198, 543);
            this._memoListView.TabIndex = 0;
            // 
            // _tabControl
            // 
            this._tabControl.AllowDrop = true;
            this._tabControl.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.ItemSize = new System.Drawing.Size(100, 24);
            this._tabControl.Location = new System.Drawing.Point(0, 0);
            this._tabControl.Margin = new System.Windows.Forms.Padding(0);
            this._tabControl.Multiline = true;
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(484, 574);
            this._tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this._tabControl.TabIndex = 3;
            this._tabControl.TabStop = false;
            // 
            // _mainMenuStrip
            // 
            this._mainMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._mainMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._fileToolStripMenuItem,
            this._editToolStripMenuItem,
            this._viewToolStripMenuItem,
            this._recentToolStripMenuItem,
            this._toolToolStripMenuItem,
            this._helpToolStripMenuItem});
            this._mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this._mainMenuStrip.Name = "_mainMenuStrip";
            this._mainMenuStrip.Size = new System.Drawing.Size(892, 24);
            this._mainMenuStrip.TabIndex = 0;
            this._mainMenuStrip.Text = "menuStrip1";
            // 
            // _fileToolStripMenuItem
            // 
            this._fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._createMemoFileToolStripMenuItem,
            this._createFusenMemoFileToolStripMenuItem,
            this._createMemoFromClipboardFileToolStripMenuItem,
            this.toolStripMenuItem2,
            this._saveAsToolStripMenuItem,
            this._sendToolStripMenuItem,
            this.toolStripMenuItem16,
            this._exportToolStripMenuItem,
            this._exportNextToolStripMenuItem,
            this._printToolStripMenuItem,
            this._nextPrintSeparatorToolStripMenuItem,
            this._noteFolderToolStripMenuItem,
            this.toolStripMenuItem9,
            this._exitToolStripMenuItem});
            this._fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
            this._fileToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this._fileToolStripMenuItem.Text = "ファイル(&F)";
            this._fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this._fileToolStripMenuItem_DropDownOpening);
            // 
            // _createMemoFileToolStripMenuItem
            // 
            this._createMemoFileToolStripMenuItem.Name = "_createMemoFileToolStripMenuItem";
            this._createMemoFileToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._createMemoFileToolStripMenuItem.Text = "ノートを作成(&N)";
            this._createMemoFileToolStripMenuItem.Click += new System.EventHandler(this._newMemoToolStripMenuItem_Click);
            // 
            // _createFusenMemoFileToolStripMenuItem
            // 
            this._createFusenMemoFileToolStripMenuItem.Name = "_createFusenMemoFileToolStripMenuItem";
            this._createFusenMemoFileToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._createFusenMemoFileToolStripMenuItem.Text = "付箋ノートを作成(&F)";
            this._createFusenMemoFileToolStripMenuItem.Click += new System.EventHandler(this._newFusenMemoToolStripMenuItem_Click);
            // 
            // _createMemoFromClipboardFileToolStripMenuItem
            // 
            this._createMemoFromClipboardFileToolStripMenuItem.Name = "_createMemoFromClipboardFileToolStripMenuItem";
            this._createMemoFromClipboardFileToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._createMemoFromClipboardFileToolStripMenuItem.Text = "クリップボードからノートを作成(&C)";
            this._createMemoFromClipboardFileToolStripMenuItem.Click += new System.EventHandler(this._createMemoFromClipboardMenuToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(224, 6);
            // 
            // _saveAsToolStripMenuItem
            // 
            this._saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._saveAsHtmlToolStripMenuItem,
            this._saveAsTextToolStripMenuItem,
            this._saveAsEmfToolStripMenuItem,
            this._saveAsPngToolStripMenuItem,
            this._saveAsJpegToolStripMenuItem});
            this._saveAsToolStripMenuItem.Name = "_saveAsToolStripMenuItem";
            this._saveAsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._saveAsToolStripMenuItem.Text = "他の形式で保存(&A)";
            // 
            // _saveAsHtmlToolStripMenuItem
            // 
            this._saveAsHtmlToolStripMenuItem.Name = "_saveAsHtmlToolStripMenuItem";
            this._saveAsHtmlToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._saveAsHtmlToolStripMenuItem.Text = "HTML(&H)...";
            this._saveAsHtmlToolStripMenuItem.Click += new System.EventHandler(this._saveAsHtmlToolStripMenuItem_Click);
            // 
            // _saveAsTextToolStripMenuItem
            // 
            this._saveAsTextToolStripMenuItem.Name = "_saveAsTextToolStripMenuItem";
            this._saveAsTextToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._saveAsTextToolStripMenuItem.Text = "テキスト(&T)...";
            this._saveAsTextToolStripMenuItem.Click += new System.EventHandler(this._saveAsTextToolStripMenuItem_Click);
            // 
            // _saveAsEmfToolStripMenuItem
            // 
            this._saveAsEmfToolStripMenuItem.Name = "_saveAsEmfToolStripMenuItem";
            this._saveAsEmfToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._saveAsEmfToolStripMenuItem.Text = "EMF(&E)...";
            this._saveAsEmfToolStripMenuItem.Click += new System.EventHandler(this._saveAsEmfToolStripMenuItem_Click);
            // 
            // _saveAsPngToolStripMenuItem
            // 
            this._saveAsPngToolStripMenuItem.Name = "_saveAsPngToolStripMenuItem";
            this._saveAsPngToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._saveAsPngToolStripMenuItem.Text = "PNG(&P)...";
            this._saveAsPngToolStripMenuItem.Click += new System.EventHandler(this._saveAsPngToolStripMenuItem_Click);
            // 
            // _saveAsJpegToolStripMenuItem
            // 
            this._saveAsJpegToolStripMenuItem.Name = "_saveAsJpegToolStripMenuItem";
            this._saveAsJpegToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this._saveAsJpegToolStripMenuItem.Text = "JPEG(&J)...";
            this._saveAsJpegToolStripMenuItem.Click += new System.EventHandler(this._saveAsJpegToolStripMenuItem_Click);
            // 
            // _sendToolStripMenuItem
            // 
            this._sendToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._sendMailToolStripMenuItem});
            this._sendToolStripMenuItem.Name = "_sendToolStripMenuItem";
            this._sendToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._sendToolStripMenuItem.Text = "送る(&S)";
            // 
            // _sendMailToolStripMenuItem
            // 
            this._sendMailToolStripMenuItem.Name = "_sendMailToolStripMenuItem";
            this._sendMailToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this._sendMailToolStripMenuItem.Text = "メール(&M)...";
            this._sendMailToolStripMenuItem.Click += new System.EventHandler(this._sendMailToolStripMenuItem_Click);
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(224, 6);
            // 
            // _exportToolStripMenuItem
            // 
            this._exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._exportHtmlToolStripMenuItem,
            this._exportTextToolStripMenuItem});
            this._exportToolStripMenuItem.Name = "_exportToolStripMenuItem";
            this._exportToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._exportToolStripMenuItem.Text = "エクスポート(&E)";
            // 
            // _exportHtmlToolStripMenuItem
            // 
            this._exportHtmlToolStripMenuItem.Name = "_exportHtmlToolStripMenuItem";
            this._exportHtmlToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this._exportHtmlToolStripMenuItem.Text = "HTML(&H)...";
            this._exportHtmlToolStripMenuItem.Click += new System.EventHandler(this._exportHtmlToolStripMenuItem_Click);
            // 
            // _exportTextToolStripMenuItem
            // 
            this._exportTextToolStripMenuItem.Name = "_exportTextToolStripMenuItem";
            this._exportTextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this._exportTextToolStripMenuItem.Text = "テキスト(&T)...";
            this._exportTextToolStripMenuItem.Click += new System.EventHandler(this._exportTextToolStripMenuItem_Click);
            // 
            // _exportNextToolStripMenuItem
            // 
            this._exportNextToolStripMenuItem.Name = "_exportNextToolStripMenuItem";
            this._exportNextToolStripMenuItem.Size = new System.Drawing.Size(224, 6);
            // 
            // _printToolStripMenuItem
            // 
            this._printToolStripMenuItem.Name = "_printToolStripMenuItem";
            this._printToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._printToolStripMenuItem.Text = "印刷(&P)...";
            this._printToolStripMenuItem.Click += new System.EventHandler(this._printToolStripMenuItem_Click);
            // 
            // _nextPrintSeparatorToolStripMenuItem
            // 
            this._nextPrintSeparatorToolStripMenuItem.Name = "_nextPrintSeparatorToolStripMenuItem";
            this._nextPrintSeparatorToolStripMenuItem.Size = new System.Drawing.Size(224, 6);
            // 
            // _noteFolderToolStripMenuItem
            // 
            this._noteFolderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._exportToFolderToolStripMenuItem,
            this._importFromFolderToolStripMenuItem});
            this._noteFolderToolStripMenuItem.Name = "_noteFolderToolStripMenuItem";
            this._noteFolderToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._noteFolderToolStripMenuItem.Text = "ノート格納フォルダ(&D)";
            // 
            // _exportToFolderToolStripMenuItem
            // 
            this._exportToFolderToolStripMenuItem.Name = "_exportToFolderToolStripMenuItem";
            this._exportToFolderToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this._exportToFolderToolStripMenuItem.Text = "エクスポート(&E)...";
            this._exportToFolderToolStripMenuItem.Click += new System.EventHandler(this._exportToFolderToolStripMenuItem_Click);
            // 
            // _importFromFolderToolStripMenuItem
            // 
            this._importFromFolderToolStripMenuItem.Name = "_importFromFolderToolStripMenuItem";
            this._importFromFolderToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this._importFromFolderToolStripMenuItem.Text = "インポート(&I)...";
            this._importFromFolderToolStripMenuItem.Click += new System.EventHandler(this._importFromFolderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(224, 6);
            // 
            // _exitToolStripMenuItem
            // 
            this._exitToolStripMenuItem.Name = "_exitToolStripMenuItem";
            this._exitToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._exitToolStripMenuItem.Text = "終了(&X)";
            this._exitToolStripMenuItem.Click += new System.EventHandler(this._exitToolStripMenuItem_Click);
            // 
            // _editToolStripMenuItem
            // 
            this._editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._undoToolStripMenuItem,
            this._redoToolStripMenuItem,
            this.toolStripSeparator5,
            this._cutToolStripMenuItem,
            this._copyToolStripMenuItem,
            this._pasteToolStripMenuItem,
            this._deleteToolStripMenuItem,
            this.toolStripSeparator6,
            this._selectAllToolStripMenuItem,
            this.toolStripMenuItem3,
            this._findToolStripMenuItem});
            this._editToolStripMenuItem.Name = "_editToolStripMenuItem";
            this._editToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this._editToolStripMenuItem.Text = "編集(&E)";
            this._editToolStripMenuItem.DropDownOpening += new System.EventHandler(this._editToolStripMenuItem_DropDownOpening);
            // 
            // _undoToolStripMenuItem
            // 
            this._undoToolStripMenuItem.Name = "_undoToolStripMenuItem";
            this._undoToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this._undoToolStripMenuItem.Text = "元に戻す(&U)";
            this._undoToolStripMenuItem.Click += new System.EventHandler(this._undoToolStripMenuItem_Click);
            // 
            // _redoToolStripMenuItem
            // 
            this._redoToolStripMenuItem.Name = "_redoToolStripMenuItem";
            this._redoToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this._redoToolStripMenuItem.Text = "やり直し(&R)";
            this._redoToolStripMenuItem.Click += new System.EventHandler(this._redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(147, 6);
            // 
            // _cutToolStripMenuItem
            // 
            this._cutToolStripMenuItem.Name = "_cutToolStripMenuItem";
            this._cutToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this._cutToolStripMenuItem.Text = "切り取り(&T)";
            this._cutToolStripMenuItem.Click += new System.EventHandler(this._cutToolStripMenuItem_Click);
            // 
            // _copyToolStripMenuItem
            // 
            this._copyToolStripMenuItem.Name = "_copyToolStripMenuItem";
            this._copyToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this._copyToolStripMenuItem.Text = "コピー(&C)";
            this._copyToolStripMenuItem.Click += new System.EventHandler(this._copyToolStripMenuItem_Click);
            // 
            // _pasteToolStripMenuItem
            // 
            this._pasteToolStripMenuItem.Name = "_pasteToolStripMenuItem";
            this._pasteToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this._pasteToolStripMenuItem.Text = "貼り付け(&P)";
            this._pasteToolStripMenuItem.Click += new System.EventHandler(this._pasteToolStripMenuItem_Click);
            // 
            // _deleteToolStripMenuItem
            // 
            this._deleteToolStripMenuItem.Name = "_deleteToolStripMenuItem";
            this._deleteToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this._deleteToolStripMenuItem.Text = "削除(&D)";
            this._deleteToolStripMenuItem.Click += new System.EventHandler(this._deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(147, 6);
            // 
            // _selectAllToolStripMenuItem
            // 
            this._selectAllToolStripMenuItem.Name = "_selectAllToolStripMenuItem";
            this._selectAllToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this._selectAllToolStripMenuItem.Text = "すべて選択(&A)";
            this._selectAllToolStripMenuItem.Click += new System.EventHandler(this._selectAllToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(147, 6);
            // 
            // _findToolStripMenuItem
            // 
            this._findToolStripMenuItem.Name = "_findToolStripMenuItem";
            this._findToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this._findToolStripMenuItem.Text = "ノート内検索(&F)";
            this._findToolStripMenuItem.Click += new System.EventHandler(this._findToolStripMenuItem_Click);
            // 
            // _viewToolStripMenuItem
            // 
            this._viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._focusConditionTextBoxToolStripMenuItem,
            this._showWorkspaceToolStripMenuItem,
            this._showMemoListToolStripMenuItem,
            this.toolStripMenuItem5,
            this._showStartPageToolStripMenuItem,
            this.toolStripMenuItem12,
            this._showFusenMemosToolStripMenuItem,
            this._nextShowFusenMemosSeparatorToolStripMenuItem,
            this._compactModeToolStripMenuItem,
            this._displayTopMostToolStripMenuItem});
            this._viewToolStripMenuItem.Name = "_viewToolStripMenuItem";
            this._viewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this._viewToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this._viewToolStripMenuItem.Text = "表示(&V)";
            this._viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this._viewToolStripMenuItem_DropDownOpening);
            // 
            // _focusConditionTextBoxToolStripMenuItem
            // 
            this._focusConditionTextBoxToolStripMenuItem.Name = "_focusConditionTextBoxToolStripMenuItem";
            this._focusConditionTextBoxToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
            this._focusConditionTextBoxToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this._focusConditionTextBoxToolStripMenuItem.Text = "検索(&S)";
            this._focusConditionTextBoxToolStripMenuItem.Click += new System.EventHandler(this._focusConditionTextBoxToolStripMenuItem_Click);
            // 
            // _showWorkspaceToolStripMenuItem
            // 
            this._showWorkspaceToolStripMenuItem.Name = "_showWorkspaceToolStripMenuItem";
            this._showWorkspaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.W)));
            this._showWorkspaceToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this._showWorkspaceToolStripMenuItem.Text = "ワークスペース(&W)";
            this._showWorkspaceToolStripMenuItem.Click += new System.EventHandler(this._showWorkspaceToolStripMenuItem_Click);
            // 
            // _showMemoListToolStripMenuItem
            // 
            this._showMemoListToolStripMenuItem.Name = "_showMemoListToolStripMenuItem";
            this._showMemoListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.L)));
            this._showMemoListToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this._showMemoListToolStripMenuItem.Text = "ノートリスト(&L)";
            this._showMemoListToolStripMenuItem.Click += new System.EventHandler(this._showMemoListToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(223, 6);
            // 
            // _showStartPageToolStripMenuItem
            // 
            this._showStartPageToolStripMenuItem.Name = "_showStartPageToolStripMenuItem";
            this._showStartPageToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this._showStartPageToolStripMenuItem.Text = "スタートページ(&S)";
            this._showStartPageToolStripMenuItem.Click += new System.EventHandler(this._showStartPageToolStripMenuItem_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(223, 6);
            // 
            // _showFusenMemosToolStripMenuItem
            // 
            this._showFusenMemosToolStripMenuItem.Name = "_showFusenMemosToolStripMenuItem";
            this._showFusenMemosToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this._showFusenMemosToolStripMenuItem.Text = "付箋ノート(&F)";
            this._showFusenMemosToolStripMenuItem.Click += new System.EventHandler(this._showFusenMemosToolStripMenuItem_Click);
            // 
            // _nextShowFusenMemosSeparatorToolStripMenuItem
            // 
            this._nextShowFusenMemosSeparatorToolStripMenuItem.Name = "_nextShowFusenMemosSeparatorToolStripMenuItem";
            this._nextShowFusenMemosSeparatorToolStripMenuItem.Size = new System.Drawing.Size(223, 6);
            // 
            // _compactModeToolStripMenuItem
            // 
            this._compactModeToolStripMenuItem.Name = "_compactModeToolStripMenuItem";
            this._compactModeToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this._compactModeToolStripMenuItem.Text = "コンパクト表示(&C)";
            this._compactModeToolStripMenuItem.Click += new System.EventHandler(this._compactModeToolStripMenuItem_Click);
            // 
            // _displayTopMostToolStripMenuItem
            // 
            this._displayTopMostToolStripMenuItem.Name = "_displayTopMostToolStripMenuItem";
            this._displayTopMostToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this._displayTopMostToolStripMenuItem.Text = "常に手前に表示(&T)";
            this._displayTopMostToolStripMenuItem.Click += new System.EventHandler(this._displayTopMostToolStripMenuItem_Click);
            // 
            // _recentToolStripMenuItem
            // 
            this._recentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._dummyToolStripMenuItem});
            this._recentToolStripMenuItem.Name = "_recentToolStripMenuItem";
            this._recentToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this._recentToolStripMenuItem.Text = "履歴(&R)";
            this._recentToolStripMenuItem.DropDownOpening += new System.EventHandler(this._recentToolStripMenuItem_DropDownOpening);
            // 
            // _dummyToolStripMenuItem
            // 
            this._dummyToolStripMenuItem.Name = "_dummyToolStripMenuItem";
            this._dummyToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this._dummyToolStripMenuItem.Text = "ダミー";
            // 
            // _toolToolStripMenuItem
            // 
            this._toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._manageTagToolStripMenuItem,
            this._manageSmartFilterToolStripMenuItem,
            this.toolStripMenuItem6,
            this._optionToolStripMenuItem});
            this._toolToolStripMenuItem.Name = "_toolToolStripMenuItem";
            this._toolToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this._toolToolStripMenuItem.Text = "ツール(&T)";
            // 
            // _manageTagToolStripMenuItem
            // 
            this._manageTagToolStripMenuItem.Name = "_manageTagToolStripMenuItem";
            this._manageTagToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this._manageTagToolStripMenuItem.Text = "タグの管理(&T)...";
            this._manageTagToolStripMenuItem.Click += new System.EventHandler(this._manageTagToolStripMenuItem_Click);
            // 
            // _manageSmartFilterToolStripMenuItem
            // 
            this._manageSmartFilterToolStripMenuItem.Name = "_manageSmartFilterToolStripMenuItem";
            this._manageSmartFilterToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this._manageSmartFilterToolStripMenuItem.Text = "スマートフィルタの管理(&F)...";
            this._manageSmartFilterToolStripMenuItem.Click += new System.EventHandler(this._manageSmartFilterToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(197, 6);
            // 
            // _optionToolStripMenuItem
            // 
            this._optionToolStripMenuItem.Name = "_optionToolStripMenuItem";
            this._optionToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this._optionToolStripMenuItem.Text = "オプション(&O)...";
            this._optionToolStripMenuItem.Click += new System.EventHandler(this._optionToolStripMenuItem_Click);
            // 
            // _helpToolStripMenuItem
            // 
            this._helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._showHelpToolStripMenuItem,
            this.toolStripMenuItem7,
            this._checkForUpdatesToolStripMenuItem,
            this._aboutBoxToolStripMenuItem});
            this._helpToolStripMenuItem.Name = "_helpToolStripMenuItem";
            this._helpToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this._helpToolStripMenuItem.Text = "ヘルプ(&H)";
            // 
            // _showHelpToolStripMenuItem
            // 
            this._showHelpToolStripMenuItem.Name = "_showHelpToolStripMenuItem";
            this._showHelpToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this._showHelpToolStripMenuItem.Text = "ヘルプ(&H)";
            this._showHelpToolStripMenuItem.Click += new System.EventHandler(this._showHelpToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(190, 6);
            // 
            // _checkForUpdatesToolStripMenuItem
            // 
            this._checkForUpdatesToolStripMenuItem.Name = "_checkForUpdatesToolStripMenuItem";
            this._checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this._checkForUpdatesToolStripMenuItem.Text = "最新版を確認(&L)";
            this._checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this._checkForUpdatesToolStripMenuItem_Click);
            // 
            // _aboutBoxToolStripMenuItem
            // 
            this._aboutBoxToolStripMenuItem.Name = "_aboutBoxToolStripMenuItem";
            this._aboutBoxToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this._aboutBoxToolStripMenuItem.Text = "バージョン情報(&A)...";
            this._aboutBoxToolStripMenuItem.Click += new System.EventHandler(this._aboutBoxToolStripMenuItem_Click);
            // 
            // _mainToolStrip
            // 
            this._mainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._mainToolStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._mainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._createMemoToolStripSplitButton,
            this.toolStripSeparator14,
            this._activeFolderToolStripDropDownButton,
            this.toolStripSeparator15,
            this._showAllFusenToolStripButton,
            this._showAsFusenToolStripButton,
            this._nextShowAsFusenToolStripSeparator,
            this._cutToolStripButton,
            this._copyToolStripButton,
            this._pasteToolStripButton,
            this.toolStripSeparator4,
            this._undoToolStripButton,
            this._redoToolStripButton,
            this.toolStripSeparator17,
            this._searchInMemoToolStripButton,
            this.toolStripSeparator9,
            this._selectToolToolStripButton,
            this._handToolToolStripButton,
            this._adjustSpaceToolToolStripButton,
            this._addFreehandToolStripDropDownButton,
            this._addFigureToolStripDropDownButton,
            this._addImageToolStripButton,
            this._addFileToolStripDropDownButton,
            this._addTableToolStripButton,
            this._tableNextToolStripSeparator,
            this._setNodeStyleToolStripDropDownButton,
            this._setLineStyleToolStripDropDownButton,
            this._shapeColorButtonToolStripItem,
            this.toolStripSeparator11,
            this._memoMarkToolStripSplitButton,
            this._importantToolStripButton,
            this._unimportantToolStripButton});
            this._mainToolStrip.Location = new System.Drawing.Point(3, 24);
            this._mainToolStrip.Name = "_mainToolStrip";
            this._mainToolStrip.Size = new System.Drawing.Size(705, 25);
            this._mainToolStrip.TabIndex = 0;
            // 
            // _createMemoToolStripSplitButton
            // 
            this._createMemoToolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._createMemoToolStripMenuItem,
            this._createFusenMemoToolStripMenuItem,
            this._createMemoFromClipboardToolStripMenuItem});
            this._createMemoToolStripSplitButton.Image = global::Mkamo.Memopad.Properties.Resources.sticky_note;
            this._createMemoToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._createMemoToolStripSplitButton.Name = "_createMemoToolStripSplitButton";
            this._createMemoToolStripSplitButton.Size = new System.Drawing.Size(98, 22);
            this._createMemoToolStripSplitButton.Text = "ノートを作成";
            this._createMemoToolStripSplitButton.ButtonClick += new System.EventHandler(this._createMemoToolStripSplitButton_ButtonClick);
            // 
            // _createMemoToolStripMenuItem
            // 
            this._createMemoToolStripMenuItem.Name = "_createMemoToolStripMenuItem";
            this._createMemoToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._createMemoToolStripMenuItem.Text = "ノートを作成(&N)";
            this._createMemoToolStripMenuItem.Click += new System.EventHandler(this._createMemoToolStripMenuItem_Click);
            // 
            // _createFusenMemoToolStripMenuItem
            // 
            this._createFusenMemoToolStripMenuItem.Name = "_createFusenMemoToolStripMenuItem";
            this._createFusenMemoToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._createFusenMemoToolStripMenuItem.Text = "付箋ノートを作成(&F)";
            this._createFusenMemoToolStripMenuItem.Click += new System.EventHandler(this._createFusenMemoToolStripMenuItem_Click);
            // 
            // _createMemoFromClipboardToolStripMenuItem
            // 
            this._createMemoFromClipboardToolStripMenuItem.Name = "_createMemoFromClipboardToolStripMenuItem";
            this._createMemoFromClipboardToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this._createMemoFromClipboardToolStripMenuItem.Text = "クリップボードからノートを作成(&C)";
            this._createMemoFromClipboardToolStripMenuItem.Click += new System.EventHandler(this._createMemoFromClipboardToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
            // 
            // _activeFolderToolStripDropDownButton
            // 
            this._activeFolderToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._activeFolderToolStripDropDownButton.Image = global::Mkamo.Memopad.Properties.Resources.clear_folder_horizontal;
            this._activeFolderToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._activeFolderToolStripDropDownButton.Name = "_activeFolderToolStripDropDownButton";
            this._activeFolderToolStripDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._activeFolderToolStripDropDownButton.Text = "アクティブクリアファイルの選択";
            this._activeFolderToolStripDropDownButton.Visible = false;
            this._activeFolderToolStripDropDownButton.DropDownOpening += new System.EventHandler(this._activeFolderToolStripDropDownButton_DropDownOpening);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 25);
            this.toolStripSeparator15.Visible = false;
            // 
            // _showAllFusenToolStripButton
            // 
            this._showAllFusenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._showAllFusenToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.show_sticky_notes;
            this._showAllFusenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._showAllFusenToolStripButton.Name = "_showAllFusenToolStripButton";
            this._showAllFusenToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._showAllFusenToolStripButton.Text = "付箋を表示";
            this._showAllFusenToolStripButton.Click += new System.EventHandler(this._showAllFusenToolStripButton_Click);
            // 
            // _showAsFusenToolStripButton
            // 
            this._showAsFusenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._showAsFusenToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.sticky_note_right_arrow;
            this._showAsFusenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._showAsFusenToolStripButton.Name = "_showAsFusenToolStripButton";
            this._showAsFusenToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._showAsFusenToolStripButton.Text = "付箋として表示";
            this._showAsFusenToolStripButton.Click += new System.EventHandler(this._showAsFusenToolStripButton_Click);
            // 
            // _nextShowAsFusenToolStripSeparator
            // 
            this._nextShowAsFusenToolStripSeparator.Name = "_nextShowAsFusenToolStripSeparator";
            this._nextShowAsFusenToolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // _cutToolStripButton
            // 
            this._cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._cutToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.scissors_blue;
            this._cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._cutToolStripButton.Name = "_cutToolStripButton";
            this._cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._cutToolStripButton.Text = "切り取り";
            // 
            // _copyToolStripButton
            // 
            this._copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._copyToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.document_copy;
            this._copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._copyToolStripButton.Name = "_copyToolStripButton";
            this._copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._copyToolStripButton.Text = "コピー";
            // 
            // _pasteToolStripButton
            // 
            this._pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._pasteToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.clipboard_paste;
            this._pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._pasteToolStripButton.Name = "_pasteToolStripButton";
            this._pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._pasteToolStripButton.Text = "貼り付け";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // _undoToolStripButton
            // 
            this._undoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._undoToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.arrow_curve_180_left;
            this._undoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._undoToolStripButton.Name = "_undoToolStripButton";
            this._undoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._undoToolStripButton.Text = "元に戻す";
            // 
            // _redoToolStripButton
            // 
            this._redoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._redoToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.arrow_curve;
            this._redoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._redoToolStripButton.Name = "_redoToolStripButton";
            this._redoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._redoToolStripButton.Text = "やり直し";
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(6, 25);
            // 
            // _searchInMemoToolStripButton
            // 
            this._searchInMemoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._searchInMemoToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.page_find;
            this._searchInMemoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._searchInMemoToolStripButton.Name = "_searchInMemoToolStripButton";
            this._searchInMemoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._searchInMemoToolStripButton.Text = "ノート内検索";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // _selectToolToolStripButton
            // 
            this._selectToolToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._selectToolToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.cursor;
            this._selectToolToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._selectToolToolStripButton.Name = "_selectToolToolStripButton";
            this._selectToolToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._selectToolToolStripButton.Text = "選択と入力";
            // 
            // _handToolToolStripButton
            // 
            this._handToolToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._handToolToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.hand;
            this._handToolToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._handToolToolStripButton.Name = "_handToolToolStripButton";
            this._handToolToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._handToolToolStripButton.Text = "手のひら";
            // 
            // _adjustSpaceToolToolStripButton
            // 
            this._adjustSpaceToolToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._adjustSpaceToolToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.insert_space;
            this._adjustSpaceToolToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._adjustSpaceToolToolStripButton.Name = "_adjustSpaceToolToolStripButton";
            this._adjustSpaceToolToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._adjustSpaceToolToolStripButton.Text = "スペースの調節";
            // 
            // _addFreehandToolStripDropDownButton
            // 
            this._addFreehandToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addFreehandToolStripDropDownButton.Image = global::Mkamo.Memopad.Properties.Resources.pencil;
            this._addFreehandToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._addFreehandToolStripDropDownButton.Name = "_addFreehandToolStripDropDownButton";
            this._addFreehandToolStripDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._addFreehandToolStripDropDownButton.Text = "手書き";
            // 
            // _addFigureToolStripDropDownButton
            // 
            this._addFigureToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addFigureToolStripDropDownButton.Image = global::Mkamo.Memopad.Properties.Resources.shape_square;
            this._addFigureToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._addFigureToolStripDropDownButton.Name = "_addFigureToolStripDropDownButton";
            this._addFigureToolStripDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._addFigureToolStripDropDownButton.Text = "図形を追加";
            // 
            // _addImageToolStripButton
            // 
            this._addImageToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addImageToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._addImageFromFileToolStripMenuItem,
            this._addImageFromScreenToolStripMenuItem});
            this._addImageToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.picture;
            this._addImageToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._addImageToolStripButton.Name = "_addImageToolStripButton";
            this._addImageToolStripButton.Size = new System.Drawing.Size(29, 22);
            this._addImageToolStripButton.Text = "画像を追加";
            // 
            // _addImageFromFileToolStripMenuItem
            // 
            this._addImageFromFileToolStripMenuItem.Name = "_addImageFromFileToolStripMenuItem";
            this._addImageFromFileToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this._addImageFromFileToolStripMenuItem.Text = "画像ファイル(&F)...";
            // 
            // _addImageFromScreenToolStripMenuItem
            // 
            this._addImageFromScreenToolStripMenuItem.Name = "_addImageFromScreenToolStripMenuItem";
            this._addImageFromScreenToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this._addImageFromScreenToolStripMenuItem.Text = "画面の領域(&S)";
            // 
            // _addFileToolStripDropDownButton
            // 
            this._addFileToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addFileToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._addEmbededFileToolStripMenuItem,
            this._addShortcutFileToolStripMenuItem,
            this._addFolderShortcutFileToolStripMenuItem});
            this._addFileToolStripDropDownButton.Image = global::Mkamo.Memopad.Properties.Resources.page;
            this._addFileToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._addFileToolStripDropDownButton.Name = "_addFileToolStripDropDownButton";
            this._addFileToolStripDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._addFileToolStripDropDownButton.Text = "ファイルを追加";
            // 
            // _addEmbededFileToolStripMenuItem
            // 
            this._addEmbededFileToolStripMenuItem.Name = "_addEmbededFileToolStripMenuItem";
            this._addEmbededFileToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this._addEmbededFileToolStripMenuItem.Text = "ファイル(&F)...";
            // 
            // _addShortcutFileToolStripMenuItem
            // 
            this._addShortcutFileToolStripMenuItem.Name = "_addShortcutFileToolStripMenuItem";
            this._addShortcutFileToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this._addShortcutFileToolStripMenuItem.Text = "ファイルのショートカット(&S)...";
            // 
            // _addFolderShortcutFileToolStripMenuItem
            // 
            this._addFolderShortcutFileToolStripMenuItem.Name = "_addFolderShortcutFileToolStripMenuItem";
            this._addFolderShortcutFileToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this._addFolderShortcutFileToolStripMenuItem.Text = "フォルダのショートカット(&D)...";
            // 
            // _addTableToolStripButton
            // 
            this._addTableToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addTableToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.table;
            this._addTableToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._addTableToolStripButton.Name = "_addTableToolStripButton";
            this._addTableToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._addTableToolStripButton.Text = "表を追加";
            // 
            // _tableNextToolStripSeparator
            // 
            this._tableNextToolStripSeparator.Name = "_tableNextToolStripSeparator";
            this._tableNextToolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // _setNodeStyleToolStripDropDownButton
            // 
            this._setNodeStyleToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._setNodeStyleToolStripDropDownButton.Image = global::Mkamo.Memopad.Properties.Resources.shape_square_edit;
            this._setNodeStyleToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._setNodeStyleToolStripDropDownButton.Name = "_setNodeStyleToolStripDropDownButton";
            this._setNodeStyleToolStripDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._setNodeStyleToolStripDropDownButton.Text = "図形のスタイルを選択";
            // 
            // _setLineStyleToolStripDropDownButton
            // 
            this._setLineStyleToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._setLineStyleToolStripDropDownButton.Image = global::Mkamo.Memopad.Properties.Resources.line_style;
            this._setLineStyleToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._setLineStyleToolStripDropDownButton.Name = "_setLineStyleToolStripDropDownButton";
            this._setLineStyleToolStripDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._setLineStyleToolStripDropDownButton.Text = "線のスタイルを選択";
            // 
            // _shapeColorButtonToolStripItem
            // 
            // 
            // _shapeColorButtonToolStripItem
            // 
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.AccessibleName = "_shapeColorButtonToolStripItem";
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.LowProfile;
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Location = new System.Drawing.Point(584, 1);
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Name = "kryptonColorButtonToolStripItem1";
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Size = new System.Drawing.Size(34, 22);
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Strings.MoreColors = "その他の色(&M)...";
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Strings.NoColor = "塗りつぶさない(&N)";
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Strings.StandardColors = "標準の色";
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Strings.ThemeColors = "テーマの色";
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.TabIndex = 0;
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Values.Image = global::Mkamo.Memopad.Properties.Resources.paint_can_color;
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.Values.Text = "";
            this._shapeColorButtonToolStripItem.KryptonColorButtonControl.VisibleRecent = false;
            this._shapeColorButtonToolStripItem.Name = "_shapeColorButtonToolStripItem";
            this._shapeColorButtonToolStripItem.Size = new System.Drawing.Size(34, 22);
            this._shapeColorButtonToolStripItem.ToolTipText = "背景色";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // _memoMarkToolStripSplitButton
            // 
            this._memoMarkToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._memoMarkToolStripSplitButton.Image = global::Mkamo.Memopad.Properties.Resources.star;
            this._memoMarkToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._memoMarkToolStripSplitButton.Name = "_memoMarkToolStripSplitButton";
            this._memoMarkToolStripSplitButton.Size = new System.Drawing.Size(32, 22);
            this._memoMarkToolStripSplitButton.Text = "マークの設定";
            // 
            // _importantToolStripButton
            // 
            this._importantToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._importantToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.sticky_note_important;
            this._importantToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._importantToolStripButton.Name = "_importantToolStripButton";
            this._importantToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._importantToolStripButton.Text = "重要度高";
            // 
            // _unimportantToolStripButton
            // 
            this._unimportantToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._unimportantToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.sticky_note_unimportant;
            this._unimportantToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._unimportantToolStripButton.Name = "_unimportantToolStripButton";
            this._unimportantToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._unimportantToolStripButton.Text = "重要度低";
            // 
            // _editToolStrip
            // 
            this._editToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._editToolStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._editToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._editToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._paragraphKindToolStripComboBox,
            this.toolStripSeparator18,
            this._fontNameToolStripComboBox,
            this._fontSizeToolStripComboBox,
            this._fontBoldToolStripButton,
            this._fontItalicToolStripButton,
            this._fontUnderlineToolStripButton,
            this._fontStrikeoutToolStripButton,
            this.toolStripSeparator1,
            this._textColorButtonToolStripItem,
            this.toolStripSeparator10,
            this._leftHorizontalAlignmentToolStripButton,
            this._centerHorizontalAlignmentToolStripButton,
            this._rightHorizontalAlignmentToolStripButton,
            this.toolStripSeparator2,
            this._verticalAlignToolStripDropDownButton,
            this.toolStripSeparator3,
            this._unorderedListToolStripButton,
            this._orderedListToolStripButton,
            this._specialListToolStripButton,
            this._selectSpecialListToolStripDropDownButton,
            this.toolStripSeparator7,
            this._outdentToolStripButton,
            this._indentToolStripButton,
            this.toolStripSeparator20,
            this._addCommentToolStripButton});
            this._editToolStrip.Location = new System.Drawing.Point(3, 49);
            this._editToolStrip.Name = "_editToolStrip";
            this._editToolStrip.Size = new System.Drawing.Size(646, 25);
            this._editToolStrip.TabIndex = 0;
            // 
            // _paragraphKindToolStripComboBox
            // 
            this._paragraphKindToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._paragraphKindToolStripComboBox.Name = "_paragraphKindToolStripComboBox";
            this._paragraphKindToolStripComboBox.Size = new System.Drawing.Size(80, 25);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(6, 25);
            // 
            // _fontNameToolStripComboBox
            // 
            this._fontNameToolStripComboBox.AutoSize = false;
            this._fontNameToolStripComboBox.DropDownHeight = 300;
            this._fontNameToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._fontNameToolStripComboBox.DropDownWidth = 180;
            this._fontNameToolStripComboBox.IntegralHeight = false;
            this._fontNameToolStripComboBox.Name = "_fontNameToolStripComboBox";
            this._fontNameToolStripComboBox.Size = new System.Drawing.Size(100, 22);
            // 
            // _fontSizeToolStripComboBox
            // 
            this._fontSizeToolStripComboBox.AutoSize = false;
            this._fontSizeToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._fontSizeToolStripComboBox.Name = "_fontSizeToolStripComboBox";
            this._fontSizeToolStripComboBox.Size = new System.Drawing.Size(40, 22);
            // 
            // _fontBoldToolStripButton
            // 
            this._fontBoldToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._fontBoldToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.text_bold;
            this._fontBoldToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._fontBoldToolStripButton.Name = "_fontBoldToolStripButton";
            this._fontBoldToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._fontBoldToolStripButton.Text = "太字";
            // 
            // _fontItalicToolStripButton
            // 
            this._fontItalicToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._fontItalicToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.text_italic;
            this._fontItalicToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._fontItalicToolStripButton.Name = "_fontItalicToolStripButton";
            this._fontItalicToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._fontItalicToolStripButton.Text = "斜体";
            // 
            // _fontUnderlineToolStripButton
            // 
            this._fontUnderlineToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._fontUnderlineToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.text_underline;
            this._fontUnderlineToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._fontUnderlineToolStripButton.Name = "_fontUnderlineToolStripButton";
            this._fontUnderlineToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._fontUnderlineToolStripButton.Text = "下線";
            // 
            // _fontStrikeoutToolStripButton
            // 
            this._fontStrikeoutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._fontStrikeoutToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.text_strikethrough;
            this._fontStrikeoutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._fontStrikeoutToolStripButton.Name = "_fontStrikeoutToolStripButton";
            this._fontStrikeoutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._fontStrikeoutToolStripButton.Text = "打ち消し線";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _textColorButtonToolStripItem
            // 
            // 
            // _textColorButtonToolStripItem
            // 
            this._textColorButtonToolStripItem.KryptonColorButtonControl.AccessibleName = "_textColorButtonToolStripItem";
            this._textColorButtonToolStripItem.KryptonColorButtonControl.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.LowProfile;
            this._textColorButtonToolStripItem.KryptonColorButtonControl.Location = new System.Drawing.Point(330, 1);
            this._textColorButtonToolStripItem.KryptonColorButtonControl.Name = "_colorButtonToolStripItem";
            this._textColorButtonToolStripItem.KryptonColorButtonControl.Size = new System.Drawing.Size(34, 22);
            this._textColorButtonToolStripItem.KryptonColorButtonControl.Strings.MoreColors = "その他の色(&M)...";
            this._textColorButtonToolStripItem.KryptonColorButtonControl.Strings.StandardColors = "標準の色";
            this._textColorButtonToolStripItem.KryptonColorButtonControl.Strings.ThemeColors = "テーマの色";
            this._textColorButtonToolStripItem.KryptonColorButtonControl.TabIndex = 0;
            this._textColorButtonToolStripItem.KryptonColorButtonControl.Values.Image = global::Mkamo.Memopad.Properties.Resources.edit_color;
            this._textColorButtonToolStripItem.KryptonColorButtonControl.Values.Text = "";
            this._textColorButtonToolStripItem.KryptonColorButtonControl.VisibleNoColor = false;
            this._textColorButtonToolStripItem.Name = "_textColorButtonToolStripItem";
            this._textColorButtonToolStripItem.Size = new System.Drawing.Size(34, 22);
            this._textColorButtonToolStripItem.ToolTipText = "文字の色";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // _leftHorizontalAlignmentToolStripButton
            // 
            this._leftHorizontalAlignmentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._leftHorizontalAlignmentToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.edit_alignment;
            this._leftHorizontalAlignmentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._leftHorizontalAlignmentToolStripButton.Name = "_leftHorizontalAlignmentToolStripButton";
            this._leftHorizontalAlignmentToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._leftHorizontalAlignmentToolStripButton.Text = "左寄せ";
            // 
            // _centerHorizontalAlignmentToolStripButton
            // 
            this._centerHorizontalAlignmentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._centerHorizontalAlignmentToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.edit_alignment_center;
            this._centerHorizontalAlignmentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._centerHorizontalAlignmentToolStripButton.Name = "_centerHorizontalAlignmentToolStripButton";
            this._centerHorizontalAlignmentToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._centerHorizontalAlignmentToolStripButton.Text = "中央";
            // 
            // _rightHorizontalAlignmentToolStripButton
            // 
            this._rightHorizontalAlignmentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._rightHorizontalAlignmentToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.edit_alignment_right;
            this._rightHorizontalAlignmentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._rightHorizontalAlignmentToolStripButton.Name = "_rightHorizontalAlignmentToolStripButton";
            this._rightHorizontalAlignmentToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._rightHorizontalAlignmentToolStripButton.Text = "右寄せ";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // _verticalAlignToolStripDropDownButton
            // 
            this._verticalAlignToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._verticalAlignToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._topVAlignToolStripMenuItem,
            this._centerVAlignToolStripMenuItem,
            this._bottomVAlignToolStripMenuItem});
            this._verticalAlignToolStripDropDownButton.Image = global::Mkamo.Memopad.Properties.Resources.edit_vertical_alignment_middle;
            this._verticalAlignToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._verticalAlignToolStripDropDownButton.Name = "_verticalAlignToolStripDropDownButton";
            this._verticalAlignToolStripDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._verticalAlignToolStripDropDownButton.Text = "縦配置を設定";
            // 
            // _topVAlignToolStripMenuItem
            // 
            this._topVAlignToolStripMenuItem.Name = "_topVAlignToolStripMenuItem";
            this._topVAlignToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this._topVAlignToolStripMenuItem.Text = "上寄せ(&T)";
            // 
            // _centerVAlignToolStripMenuItem
            // 
            this._centerVAlignToolStripMenuItem.Name = "_centerVAlignToolStripMenuItem";
            this._centerVAlignToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this._centerVAlignToolStripMenuItem.Text = "中央(&C)";
            // 
            // _bottomVAlignToolStripMenuItem
            // 
            this._bottomVAlignToolStripMenuItem.Name = "_bottomVAlignToolStripMenuItem";
            this._bottomVAlignToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this._bottomVAlignToolStripMenuItem.Text = "下寄せ(&B)";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // _unorderedListToolStripButton
            // 
            this._unorderedListToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._unorderedListToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.List_Bullets;
            this._unorderedListToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._unorderedListToolStripButton.Name = "_unorderedListToolStripButton";
            this._unorderedListToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._unorderedListToolStripButton.Text = "箇条書き";
            // 
            // _orderedListToolStripButton
            // 
            this._orderedListToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._orderedListToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.List_Numbered;
            this._orderedListToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._orderedListToolStripButton.Name = "_orderedListToolStripButton";
            this._orderedListToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._orderedListToolStripButton.Text = "番号付き箇条書き";
            // 
            // _specialListToolStripButton
            // 
            this._specialListToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._specialListToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.list_special;
            this._specialListToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._specialListToolStripButton.Name = "_specialListToolStripButton";
            this._specialListToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._specialListToolStripButton.Text = "特殊箇条書き";
            // 
            // _selectSpecialListToolStripDropDownButton
            // 
            this._selectSpecialListToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this._selectSpecialListToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._checkBoxListToolStripMenuItem,
            this._triStateCheckBoxListToolStripMenuItem,
            this._starListToolStripMenuItem,
            this._leftArrowListToolStripMenuItem,
            this._rightArrowListToolStripMenuItem});
            this._selectSpecialListToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("_selectSpecialListToolStripDropDownButton.Image")));
            this._selectSpecialListToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._selectSpecialListToolStripDropDownButton.Name = "_selectSpecialListToolStripDropDownButton";
            this._selectSpecialListToolStripDropDownButton.Size = new System.Drawing.Size(13, 22);
            this._selectSpecialListToolStripDropDownButton.Text = "特殊箇条書き";
            // 
            // _checkBoxListToolStripMenuItem
            // 
            this._checkBoxListToolStripMenuItem.Image = global::Mkamo.Memopad.Properties.Resources.checkbox_checked;
            this._checkBoxListToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._checkBoxListToolStripMenuItem.Name = "_checkBoxListToolStripMenuItem";
            this._checkBoxListToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this._checkBoxListToolStripMenuItem.Text = "チェックボックス(&C)";
            // 
            // _triStateCheckBoxListToolStripMenuItem
            // 
            this._triStateCheckBoxListToolStripMenuItem.Image = global::Mkamo.Memopad.Properties.Resources.checkbox_indeterminate;
            this._triStateCheckBoxListToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._triStateCheckBoxListToolStripMenuItem.Name = "_triStateCheckBoxListToolStripMenuItem";
            this._triStateCheckBoxListToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this._triStateCheckBoxListToolStripMenuItem.Text = "3値チェックボックス(&T)";
            // 
            // _starListToolStripMenuItem
            // 
            this._starListToolStripMenuItem.Image = global::Mkamo.Memopad.Properties.Resources.star_small;
            this._starListToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._starListToolStripMenuItem.Name = "_starListToolStripMenuItem";
            this._starListToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this._starListToolStripMenuItem.Text = "スター(&S)";
            // 
            // _leftArrowListToolStripMenuItem
            // 
            this._leftArrowListToolStripMenuItem.Image = global::Mkamo.Memopad.Properties.Resources.left_arrow;
            this._leftArrowListToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._leftArrowListToolStripMenuItem.Name = "_leftArrowListToolStripMenuItem";
            this._leftArrowListToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this._leftArrowListToolStripMenuItem.Text = "左矢印(&L)";
            // 
            // _rightArrowListToolStripMenuItem
            // 
            this._rightArrowListToolStripMenuItem.Image = global::Mkamo.Memopad.Properties.Resources.right_arrow;
            this._rightArrowListToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._rightArrowListToolStripMenuItem.Name = "_rightArrowListToolStripMenuItem";
            this._rightArrowListToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this._rightArrowListToolStripMenuItem.Text = "右矢印(&R)";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // _outdentToolStripButton
            // 
            this._outdentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._outdentToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.IndentRTL;
            this._outdentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._outdentToolStripButton.Name = "_outdentToolStripButton";
            this._outdentToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._outdentToolStripButton.Text = "アウトデント";
            // 
            // _indentToolStripButton
            // 
            this._indentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._indentToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.Indent;
            this._indentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._indentToolStripButton.Name = "_indentToolStripButton";
            this._indentToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._indentToolStripButton.Text = "インデント";
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(6, 25);
            // 
            // _addCommentToolStripButton
            // 
            this._addCommentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addCommentToolStripButton.Image = global::Mkamo.Memopad.Properties.Resources.add_comment;
            this._addCommentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._addCommentToolStripButton.Name = "_addCommentToolStripButton";
            this._addCommentToolStripButton.Size = new System.Drawing.Size(23, 22);
            this._addCommentToolStripButton.Text = "コメントを追加";
            // 
            // _tabControlContextMenuStrip
            // 
            this._tabControlContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._tabControlContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._closeMemoTabToolStripMenuItem,
            this._closeOtherMemoTabsToolStripMenuItem,
            this._closeAllMemoTabsToolStripMenuItem,
            this.toolStripMenuItem11,
            this._showFusenTabToolStripMenuItem,
            this._nextShowFusenSeparatorTabToolStripMenuItem,
            this._removeMemoTabToolStripMenuItem,
            this.toolStripMenuItem1,
            this._maximizeEditorSizeTabToolStripMenuItem,
            this._restoreEditorSizeTabToolStripMenuItem});
            this._tabControlContextMenuStrip.Name = "tabContextMenuStrip";
            this._tabControlContextMenuStrip.Size = new System.Drawing.Size(186, 176);
            this._tabControlContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._tabControlContextMenuStrip_Opening);
            // 
            // _closeMemoTabToolStripMenuItem
            // 
            this._closeMemoTabToolStripMenuItem.Name = "_closeMemoTabToolStripMenuItem";
            this._closeMemoTabToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this._closeMemoTabToolStripMenuItem.Text = "閉じる(&C)";
            this._closeMemoTabToolStripMenuItem.Click += new System.EventHandler(this._closeMemoTabToolStripMenuItem_Click);
            // 
            // _closeOtherMemoTabsToolStripMenuItem
            // 
            this._closeOtherMemoTabsToolStripMenuItem.Name = "_closeOtherMemoTabsToolStripMenuItem";
            this._closeOtherMemoTabsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this._closeOtherMemoTabsToolStripMenuItem.Text = "他のタブを閉じる(&O)";
            this._closeOtherMemoTabsToolStripMenuItem.Click += new System.EventHandler(this._closeOtherMemoTabsToolStripMenuItem_Click);
            // 
            // _closeAllMemoTabsToolStripMenuItem
            // 
            this._closeAllMemoTabsToolStripMenuItem.Name = "_closeAllMemoTabsToolStripMenuItem";
            this._closeAllMemoTabsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this._closeAllMemoTabsToolStripMenuItem.Text = "すべてのタブを閉じる(&A)";
            this._closeAllMemoTabsToolStripMenuItem.Click += new System.EventHandler(this._closeAllMemoTabsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(182, 6);
            // 
            // _showFusenTabToolStripMenuItem
            // 
            this._showFusenTabToolStripMenuItem.Name = "_showFusenTabToolStripMenuItem";
            this._showFusenTabToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this._showFusenTabToolStripMenuItem.Text = "付箋として表示(&F)";
            this._showFusenTabToolStripMenuItem.Click += new System.EventHandler(this._showFusenTabToolStripMenuItem_Click);
            // 
            // _nextShowFusenSeparatorTabToolStripMenuItem
            // 
            this._nextShowFusenSeparatorTabToolStripMenuItem.Name = "_nextShowFusenSeparatorTabToolStripMenuItem";
            this._nextShowFusenSeparatorTabToolStripMenuItem.Size = new System.Drawing.Size(182, 6);
            // 
            // _removeMemoTabToolStripMenuItem
            // 
            this._removeMemoTabToolStripMenuItem.Name = "_removeMemoTabToolStripMenuItem";
            this._removeMemoTabToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this._removeMemoTabToolStripMenuItem.Text = "ノートを削除(&D)";
            this._removeMemoTabToolStripMenuItem.Click += new System.EventHandler(this._removeMemoTabToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(182, 6);
            // 
            // _maximizeEditorSizeTabToolStripMenuItem
            // 
            this._maximizeEditorSizeTabToolStripMenuItem.Name = "_maximizeEditorSizeTabToolStripMenuItem";
            this._maximizeEditorSizeTabToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this._maximizeEditorSizeTabToolStripMenuItem.Text = "最大化(&X)";
            this._maximizeEditorSizeTabToolStripMenuItem.Click += new System.EventHandler(this._maximizeEditorSizeTabToolStripMenuItem_Click);
            // 
            // _restoreEditorSizeTabToolStripMenuItem
            // 
            this._restoreEditorSizeTabToolStripMenuItem.Name = "_restoreEditorSizeTabToolStripMenuItem";
            this._restoreEditorSizeTabToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this._restoreEditorSizeTabToolStripMenuItem.Text = "元のサイズに戻す(&R)";
            this._restoreEditorSizeTabToolStripMenuItem.Click += new System.EventHandler(this._restoreEditorSizeTabToolStripMenuItem_Click);
            // 
            // MemopadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(892, 673);
            this.Controls.Add(this._toolStripContainer);
            this.MainMenuStrip = this._mainMenuStrip;
            this.Name = "MemopadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MochaNote";
            this._toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this._toolStripContainer.BottomToolStripPanel.PerformLayout();
            this._toolStripContainer.ContentPanel.ResumeLayout(false);
            this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this._toolStripContainer.TopToolStripPanel.PerformLayout();
            this._toolStripContainer.ResumeLayout(false);
            this._toolStripContainer.PerformLayout();
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this._finderSplitContainer.Panel1.ResumeLayout(false);
            this._finderSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._finderSplitContainer)).EndInit();
            this._finderSplitContainer.ResumeLayout(false);
            this._finderHeaderGroup.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._finderHeaderGroup)).EndInit();
            this._finderHeaderGroup.ResumeLayout(false);
            this._workspaceTagContextMenuStrip.ResumeLayout(false);
            this._workspaceViewPanel.ResumeLayout(false);
            this._adPanel.ResumeLayout(false);
            this._adPanel.PerformLayout();
            this._conditionPanel.ResumeLayout(false);
            this._conditionPanel.PerformLayout();
            this._memoListSplitContainer.Panel1.ResumeLayout(false);
            this._memoListSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._memoListSplitContainer)).EndInit();
            this._memoListSplitContainer.ResumeLayout(false);
            this._memoListHeaderGroup.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._memoListHeaderGroup)).EndInit();
            this._memoListHeaderGroup.ResumeLayout(false);
            this._memoListViewSmartFilterContextMenuStrip.ResumeLayout(false);
            this._memoListViewDisplayItemContextMenuStrip.ResumeLayout(false);
            this._memoListViewSortContextMenuStrip.ResumeLayout(false);
            this._mainMenuStrip.ResumeLayout(false);
            this._mainMenuStrip.PerformLayout();
            this._mainToolStrip.ResumeLayout(false);
            this._mainToolStrip.PerformLayout();
            this._editToolStrip.ResumeLayout(false);
            this._editToolStrip.PerformLayout();
            this._tabControlContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer _toolStripContainer;
        private System.Windows.Forms.ToolStrip _editToolStrip;
        private System.Windows.Forms.ToolStripComboBox _fontNameToolStripComboBox;
        private System.Windows.Forms.ToolStripButton _fontBoldToolStripButton;
        private System.Windows.Forms.ToolStripButton _fontItalicToolStripButton;
        private System.Windows.Forms.ToolStripButton _fontUnderlineToolStripButton;
        private System.Windows.Forms.ToolStripButton _fontStrikeoutToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _leftHorizontalAlignmentToolStripButton;
        private System.Windows.Forms.ToolStripButton _centerHorizontalAlignmentToolStripButton;
        private System.Windows.Forms.ToolStripButton _rightHorizontalAlignmentToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.MenuStrip _mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _createMemoFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator _exportNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _exitToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip _tabControlContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _closeMemoTabToolStripMenuItem;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer _finderSplitContainer;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer _memoListSplitContainer;
        private System.Windows.Forms.ToolStripMenuItem _editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem _cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem _toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup _finderHeaderGroup;
        private ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup _memoListHeaderGroup;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup _workspaceFoldButtonSpecHeaderGroup;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup _memoListFoldButtonSpecHeaderGroup;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup _memoListViewSortButtonSpecHeaderGroup;
        private System.Windows.Forms.ToolStripMenuItem _aboutBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox _fontSizeToolStripComboBox;
        private System.Windows.Forms.ToolStripMenuItem _manageTagToolStripMenuItem;
        private MemoListView _memoListView;
        private System.Windows.Forms.ToolStripButton _unorderedListToolStripButton;
        private System.Windows.Forms.ToolStripButton _orderedListToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton _indentToolStripButton;
        private System.Windows.Forms.ToolStripButton _outdentToolStripButton;
        private System.Windows.Forms.ContextMenuStrip _memoListViewSortContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _sortByTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _sortByCreatedDateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _sortByModifiedDateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _closeAllMemoTabsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _closeOtherMemoTabsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem _findToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton _verticalAlignToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem _topVAlignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _centerVAlignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _bottomVAlignToolStripMenuItem;
        private Mkamo.Control.ToolStrip.KryptonColorButtonToolStripItem _textColorButtonToolStripItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem _sortByAccessedDateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem _sortByAscendingOrderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _sortByDescendingOrderToolStripMenuItem;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup _memoListViewDisplayItemButtonSpecHeaderGroup;
        private System.Windows.Forms.ContextMenuStrip _memoListViewDisplayItemContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _createdDateDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _modifiedDateDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _accessedDateDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _tagDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem _displayTopMostToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem _optionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _showHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem _printToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
        private System.Windows.Forms.ToolStripSeparator _nextPrintSeparatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _showWorkspaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _showMemoListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _compactModeToolStripMenuItem;
        private TabControlEx _tabControl;
        private System.Windows.Forms.Panel _workspaceViewPanel;
        private WorkspaceView _workspaceView;
        private System.Windows.Forms.Panel _conditionPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _conditionTextBox;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny _searchButtonSpec;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny _cancelSearchButtonSpec;
        private System.Windows.Forms.ToolStripMenuItem _focusConditionTextBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem _removeMemoTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _showStartPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem12;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup _configForTagButtonSpecHeaderGroup;
        private System.Windows.Forms.ContextMenuStrip _workspaceTagContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _showDescendantTagsMemoToolStripMenuItem;
        private System.Windows.Forms.ToolStrip _mainToolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripDropDownButton _activeFolderToolStripDropDownButton;
        private System.Windows.Forms.ToolStripButton _cutToolStripButton;
        private System.Windows.Forms.ToolStripButton _copyToolStripButton;
        private System.Windows.Forms.ToolStripButton _pasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton _undoToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripDropDownButton _addFigureToolStripDropDownButton;
        private System.Windows.Forms.ToolStripDropDownButton _addImageToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem _addImageFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _addImageFromScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton _addFileToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem _addEmbededFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _addShortcutFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton _addTableToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton _setNodeStyleToolStripDropDownButton;
        private System.Windows.Forms.ToolStripSplitButton _memoMarkToolStripSplitButton;
        private Mkamo.Control.ToolStrip.KryptonColorButtonToolStripItem _shapeColorButtonToolStripItem;
        private System.Windows.Forms.ToolStripSeparator _nextShowAsFusenToolStripSeparator;
        private System.Windows.Forms.ToolStripButton _searchInMemoToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton _redoToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem _summaryTextDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton _createMemoToolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem _createMemoFromClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _createMemoFromClipboardFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _createMemoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator _tableNextToolStripSeparator;
        private System.Windows.Forms.ToolStripDropDownButton _addFreehandToolStripDropDownButton;
        private System.Windows.Forms.ToolStripButton _selectToolToolStripButton;
        private System.Windows.Forms.ToolStripButton _handToolToolStripButton;
        private System.Windows.Forms.ToolStripComboBox _paragraphKindToolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem _showFusenTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator _nextShowFusenSeparatorTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton _showAsFusenToolStripButton;
        private System.Windows.Forms.ToolStripButton _importantToolStripButton;
        private System.Windows.Forms.ToolStripButton _unimportantToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem _sortByImortanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _dummyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _saveAsHtmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _saveAsEmfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _saveAsPngToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _createFusenMemoFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _createFusenMemoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _showFusenMemosToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator _nextShowFusenMemosSeparatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _addFolderShortcutFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton _showAllFusenToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem _noteFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _exportToFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _importFromFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _sendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _sendMailToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripButton _addCommentToolStripButton;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _memoCountStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem _saveAsTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _exportTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem16;
        private System.Windows.Forms.ToolStripMenuItem _exportHtmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton _specialListToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton _selectSpecialListToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem _starListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _leftArrowListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _rightArrowListToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel _messageToolStripStatusLabel;
        private System.Windows.Forms.ToolStripDropDownButton _canvasSizeToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem _spaceRightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _spaceDownToolStripMenuItem;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecHeaderGroup _memoListViewSmartFilterButtonSpecHeaderGroup;
        private System.Windows.Forms.ContextMenuStrip _memoListViewSmartFilterContextMenuStrip;
        private System.Windows.Forms.ToolStripSeparator _memoListViewSmartFilterSplitterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _manageSmartFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel _smartFilterToolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem _clearSmartFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _memoListViewManageSmartFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _maximizeEditorSizeTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _restoreEditorSizeTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem _saveAsJpegToolStripMenuItem;
        private System.Windows.Forms.Panel _adPanel;
        private System.Windows.Forms.LinkLabel _adLinkLabel;
        private System.Windows.Forms.ToolStripButton _adjustSpaceToolToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem _checkBoxListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _triStateCheckBoxListToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton _setLineStyleToolStripDropDownButton;

    }
}
