namespace EditorCore
{
    partial class EditorForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
            this.button5 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lnk_ShowHiddenObjs = new System.Windows.Forms.LinkLabel();
            this.lnk_hideSelectedObjs = new System.Windows.Forms.LinkLabel();
            this.ListEditingPanel = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.Btn_addType = new System.Windows.Forms.Button();
            this.ObjectsListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.HideGroup_CB = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.ClipBoardMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ClipBoardMenu_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ClipBoardMenu_CopyTransform = new System.Windows.Forms.ToolStripMenuItem();
            this.ClipBoardMenu_CopyPos = new System.Windows.Forms.ToolStripMenuItem();
            this.ClipBoardMenu_CopyRot = new System.Windows.Forms.ToolStripMenuItem();
            this.ClipBoardMenu_CopyScale = new System.Windows.Forms.ToolStripMenuItem();
            this.ClipBoardMenu_CopyFull = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusLbl = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newEditorInstanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenderingCanvas = new System.Windows.Forms.Integration.ElementHost();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.gamePathToolStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UndoMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.LevelFilesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.FindMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.objectByIdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectByCameraIdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectBySwitchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchAppearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchKillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchDeadOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectByViewIdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectByNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectByModelNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ObjectListPanel = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ddmAdd = new System.Windows.Forms.ToolStripDropDownButton();
            this.notWorkingYetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnPaste = new System.Windows.Forms.ToolStripButton();
            this.btnDuplicate = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.ListEditingPanel.SuspendLayout();
            this.ClipBoardMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.ObjectListPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Location = new System.Drawing.Point(105, 234);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(152, 23);
            this.button5.TabIndex = 2;
            this.button5.Text = "Remove selected property";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            this.splitContainer1.Panel1.Controls.Add(this.ObjectListPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.button5);
            this.splitContainer1.Panel2.Controls.Add(this.button4);
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(263, 548);
            this.splitContainer1.SplitterDistance = 284;
            this.splitContainer1.TabIndex = 14;
            // 
            // lnk_ShowHiddenObjs
            // 
            this.lnk_ShowHiddenObjs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnk_ShowHiddenObjs.AutoSize = true;
            this.lnk_ShowHiddenObjs.Location = new System.Drawing.Point(191, 234);
            this.lnk_ShowHiddenObjs.Name = "lnk_ShowHiddenObjs";
            this.lnk_ShowHiddenObjs.Size = new System.Drawing.Size(69, 13);
            this.lnk_ShowHiddenObjs.TabIndex = 19;
            this.lnk_ShowHiddenObjs.TabStop = true;
            this.lnk_ShowHiddenObjs.Text = "Show hidden";
            this.lnk_ShowHiddenObjs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_ShowHiddenObjs_LinkClicked);
            // 
            // lnk_hideSelectedObjs
            // 
            this.lnk_hideSelectedObjs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnk_hideSelectedObjs.AutoSize = true;
            this.lnk_hideSelectedObjs.Location = new System.Drawing.Point(119, 234);
            this.lnk_hideSelectedObjs.Name = "lnk_hideSelectedObjs";
            this.lnk_hideSelectedObjs.Size = new System.Drawing.Size(72, 13);
            this.lnk_hideSelectedObjs.TabIndex = 18;
            this.lnk_hideSelectedObjs.TabStop = true;
            this.lnk_hideSelectedObjs.Text = "Hide selected";
            this.lnk_hideSelectedObjs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_hideSelectedObjs_LinkClicked);
            // 
            // ListEditingPanel
            // 
            this.ListEditingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListEditingPanel.Controls.Add(this.linkLabel1);
            this.ListEditingPanel.Location = new System.Drawing.Point(3, 2);
            this.ListEditingPanel.Name = "ListEditingPanel";
            this.ListEditingPanel.Size = new System.Drawing.Size(257, 24);
            this.ListEditingPanel.TabIndex = 13;
            this.ListEditingPanel.Visible = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(41, 4);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(183, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Editing an object list, click to go back";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ListEditGoBack);
            // 
            // Btn_addType
            // 
            this.Btn_addType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_addType.Location = new System.Drawing.Point(237, 2);
            this.Btn_addType.Name = "Btn_addType";
            this.Btn_addType.Size = new System.Drawing.Size(23, 21);
            this.Btn_addType.TabIndex = 10;
            this.Btn_addType.Text = "+";
            this.Btn_addType.UseVisualStyleBackColor = true;
            this.Btn_addType.Click += new System.EventHandler(this.Btn_addType_Click);
            // 
            // ObjectsListBox
            // 
            this.ObjectsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectsListBox.FormattingEnabled = true;
            this.ObjectsListBox.Location = new System.Drawing.Point(3, 32);
            this.ObjectsListBox.Name = "ObjectsListBox";
            this.ObjectsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ObjectsListBox.Size = new System.Drawing.Size(261, 186);
            this.ObjectsListBox.TabIndex = 8;
            this.ObjectsListBox.SelectedIndexChanged += new System.EventHandler(this.SelectedObjectChanged);
            this.ObjectsListBox.DoubleClick += new System.EventHandler(this.ObjectsList_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Object list:";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(65, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(169, 21);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.SelectedListChanged);
            // 
            // HideGroup_CB
            // 
            this.HideGroup_CB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.HideGroup_CB.AutoSize = true;
            this.HideGroup_CB.Location = new System.Drawing.Point(3, 233);
            this.HideGroup_CB.Name = "HideGroup_CB";
            this.HideGroup_CB.Size = new System.Drawing.Size(82, 17);
            this.HideGroup_CB.TabIndex = 9;
            this.HideGroup_CB.Text = "Hide this list";
            this.HideGroup_CB.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(3, 234);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 1;
            this.button4.Text = "Add property";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.ContextMenuStrip = this.ClipBoardMenu;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(4, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(253, 228);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridChange);
            // 
            // ClipBoardMenu
            // 
            this.ClipBoardMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClipBoardMenu_Paste,
            this.toolStripSeparator1,
            this.ClipBoardMenu_CopyTransform,
            this.ClipBoardMenu_CopyPos,
            this.ClipBoardMenu_CopyRot,
            this.ClipBoardMenu_CopyScale,
            this.ClipBoardMenu_CopyFull});
            this.ClipBoardMenu.Name = "contextMenuStrip1";
            this.ClipBoardMenu.Size = new System.Drawing.Size(159, 142);
            this.ClipBoardMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ClipBoardMenu_Opening);
            // 
            // ClipBoardMenu_Paste
            // 
            this.ClipBoardMenu_Paste.DoubleClickEnabled = true;
            this.ClipBoardMenu_Paste.Name = "ClipBoardMenu_Paste";
            this.ClipBoardMenu_Paste.Size = new System.Drawing.Size(158, 22);
            this.ClipBoardMenu_Paste.Text = "Paste value";
            this.ClipBoardMenu_Paste.Click += new System.EventHandler(this.ClipBoardMenu_Paste_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
            // 
            // ClipBoardMenu_CopyTransform
            // 
            this.ClipBoardMenu_CopyTransform.Name = "ClipBoardMenu_CopyTransform";
            this.ClipBoardMenu_CopyTransform.Size = new System.Drawing.Size(158, 22);
            this.ClipBoardMenu_CopyTransform.Text = "Copy transform";
            this.ClipBoardMenu_CopyTransform.Click += new System.EventHandler(this.ClipBoardMenu_CopyTransform_Click);
            // 
            // ClipBoardMenu_CopyPos
            // 
            this.ClipBoardMenu_CopyPos.Name = "ClipBoardMenu_CopyPos";
            this.ClipBoardMenu_CopyPos.Size = new System.Drawing.Size(158, 22);
            this.ClipBoardMenu_CopyPos.Text = "Copy position";
            this.ClipBoardMenu_CopyPos.Click += new System.EventHandler(this.ClipBoardMenu_CopyPos_Click);
            // 
            // ClipBoardMenu_CopyRot
            // 
            this.ClipBoardMenu_CopyRot.Name = "ClipBoardMenu_CopyRot";
            this.ClipBoardMenu_CopyRot.Size = new System.Drawing.Size(158, 22);
            this.ClipBoardMenu_CopyRot.Text = "Copy rotation";
            this.ClipBoardMenu_CopyRot.Click += new System.EventHandler(this.ClipBoardMenu_CopyRot_Click);
            // 
            // ClipBoardMenu_CopyScale
            // 
            this.ClipBoardMenu_CopyScale.Name = "ClipBoardMenu_CopyScale";
            this.ClipBoardMenu_CopyScale.Size = new System.Drawing.Size(158, 22);
            this.ClipBoardMenu_CopyScale.Text = "Copy scale";
            this.ClipBoardMenu_CopyScale.Click += new System.EventHandler(this.ClipBoardMenu_CopyScale_Click);
            // 
            // ClipBoardMenu_CopyFull
            // 
            this.ClipBoardMenu_CopyFull.Name = "ClipBoardMenu_CopyFull";
            this.ClipBoardMenu_CopyFull.Size = new System.Drawing.Size(158, 22);
            this.ClipBoardMenu_CopyFull.Text = "Copy full object";
            this.ClipBoardMenu_CopyFull.Click += new System.EventHandler(this.ClipBoardMenu_CopyFull_Click);
            // 
            // StatusLbl
            // 
            this.StatusLbl.ForeColor = System.Drawing.Color.Red;
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(12, 20);
            this.StatusLbl.Visible = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(242, 562);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label2.Location = new System.Drawing.Point(749, 562);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Credits";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newEditorInstanceToolStripMenuItem,
            this.toolStripSeparator2,
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator8,
            this.preferencesToolStripMenuItem});
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(37, 20);
            this.FileMenu.Text = "File";
            // 
            // newEditorInstanceToolStripMenuItem
            // 
            this.newEditorInstanceToolStripMenuItem.Name = "newEditorInstanceToolStripMenuItem";
            this.newEditorInstanceToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.newEditorInstanceToolStripMenuItem.Text = "New editor instance";
            this.newEditorInstanceToolStripMenuItem.Click += new System.EventHandler(this.newEditorInstanceToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(176, 6);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(176, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.preferencesToolStripMenuItem.Text = "Settings";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // RenderingCanvas
            // 
            this.RenderingCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderingCanvas.Location = new System.Drawing.Point(0, 0);
            this.RenderingCanvas.Name = "RenderingCanvas";
            this.RenderingCanvas.Size = new System.Drawing.Size(521, 548);
            this.RenderingCanvas.TabIndex = 12;
            this.RenderingCanvas.Text = "r";
            this.RenderingCanvas.Child = null;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.ToolsMenu,
            this.UndoMenu,
            this.LevelFilesMenu,
            this.FindMenu,
            this.StatusLbl});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(788, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ToolsMenu
            // 
            this.ToolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator9,
            this.gamePathToolStripItem});
            this.ToolsMenu.Name = "ToolsMenu";
            this.ToolsMenu.Size = new System.Drawing.Size(47, 20);
            this.ToolsMenu.Text = "Tools";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(132, 6);
            // 
            // gamePathToolStripItem
            // 
            this.gamePathToolStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeToolStripMenuItem});
            this.gamePathToolStripItem.Name = "gamePathToolStripItem";
            this.gamePathToolStripItem.Size = new System.Drawing.Size(135, 22);
            this.gamePathToolStripItem.Text = "Game path:";
            // 
            // changeToolStripMenuItem
            // 
            this.changeToolStripMenuItem.Name = "changeToolStripMenuItem";
            this.changeToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.changeToolStripMenuItem.Text = "Change";
            this.changeToolStripMenuItem.Click += new System.EventHandler(this.changeToolStripMenuItem_Click);
            // 
            // UndoMenu
            // 
            this.UndoMenu.Name = "UndoMenu";
            this.UndoMenu.Size = new System.Drawing.Size(48, 20);
            this.UndoMenu.Text = "Undo";
            this.UndoMenu.DropDownOpening += new System.EventHandler(this.UndoMenu_Open);
            // 
            // LevelFilesMenu
            // 
            this.LevelFilesMenu.Name = "LevelFilesMenu";
            this.LevelFilesMenu.Size = new System.Drawing.Size(70, 20);
            this.LevelFilesMenu.Text = "Level files";
            // 
            // FindMenu
            // 
            this.FindMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.objectByIdToolStripMenuItem,
            this.objectByCameraIdToolStripMenuItem,
            this.objectBySwitchToolStripMenuItem,
            this.objectByViewIdToolStripMenuItem,
            this.objectByNameToolStripMenuItem,
            this.objectByModelNameToolStripMenuItem});
            this.FindMenu.Name = "FindMenu";
            this.FindMenu.Size = new System.Drawing.Size(42, 20);
            this.FindMenu.Text = "Find";
            this.FindMenu.Visible = false;
            // 
            // objectByIdToolStripMenuItem
            // 
            this.objectByIdToolStripMenuItem.Name = "objectByIdToolStripMenuItem";
            this.objectByIdToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.objectByIdToolStripMenuItem.Text = "Object by Id";
            this.objectByIdToolStripMenuItem.Click += new System.EventHandler(this.objectByIdToolStripMenuItem_Click);
            // 
            // objectByCameraIdToolStripMenuItem
            // 
            this.objectByCameraIdToolStripMenuItem.Name = "objectByCameraIdToolStripMenuItem";
            this.objectByCameraIdToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.objectByCameraIdToolStripMenuItem.Text = "Object by CameraId";
            this.objectByCameraIdToolStripMenuItem.Visible = false;
            // 
            // objectBySwitchToolStripMenuItem
            // 
            this.objectBySwitchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.switchAToolStripMenuItem,
            this.switchBToolStripMenuItem,
            this.switchAppearToolStripMenuItem,
            this.switchKillToolStripMenuItem,
            this.switchDeadOnToolStripMenuItem});
            this.objectBySwitchToolStripMenuItem.Name = "objectBySwitchToolStripMenuItem";
            this.objectBySwitchToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.objectBySwitchToolStripMenuItem.Text = "Object by switch";
            this.objectBySwitchToolStripMenuItem.Visible = false;
            // 
            // switchAToolStripMenuItem
            // 
            this.switchAToolStripMenuItem.Name = "switchAToolStripMenuItem";
            this.switchAToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.switchAToolStripMenuItem.Text = "SwitchA";
            // 
            // switchBToolStripMenuItem
            // 
            this.switchBToolStripMenuItem.Name = "switchBToolStripMenuItem";
            this.switchBToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.switchBToolStripMenuItem.Text = "SwitchB";
            // 
            // switchAppearToolStripMenuItem
            // 
            this.switchAppearToolStripMenuItem.Name = "switchAppearToolStripMenuItem";
            this.switchAppearToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.switchAppearToolStripMenuItem.Text = "SwitchAppear";
            // 
            // switchKillToolStripMenuItem
            // 
            this.switchKillToolStripMenuItem.Name = "switchKillToolStripMenuItem";
            this.switchKillToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.switchKillToolStripMenuItem.Text = "SwitchKill";
            // 
            // switchDeadOnToolStripMenuItem
            // 
            this.switchDeadOnToolStripMenuItem.Name = "switchDeadOnToolStripMenuItem";
            this.switchDeadOnToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.switchDeadOnToolStripMenuItem.Text = "SwitchDeadOn";
            // 
            // objectByViewIdToolStripMenuItem
            // 
            this.objectByViewIdToolStripMenuItem.Name = "objectByViewIdToolStripMenuItem";
            this.objectByViewIdToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.objectByViewIdToolStripMenuItem.Text = "Object by ViewId";
            this.objectByViewIdToolStripMenuItem.Visible = false;
            // 
            // objectByNameToolStripMenuItem
            // 
            this.objectByNameToolStripMenuItem.Name = "objectByNameToolStripMenuItem";
            this.objectByNameToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.objectByNameToolStripMenuItem.Text = "Object by Name";
            this.objectByNameToolStripMenuItem.Click += new System.EventHandler(this.objectByNameToolStripMenuItem_Click);
            // 
            // objectByModelNameToolStripMenuItem
            // 
            this.objectByModelNameToolStripMenuItem.Name = "objectByModelNameToolStripMenuItem";
            this.objectByModelNameToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.objectByModelNameToolStripMenuItem.Text = "Object by ModelName";
            this.objectByModelNameToolStripMenuItem.Click += new System.EventHandler(this.objectByModelNameToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.splitContainer2.Location = new System.Drawing.Point(0, 27);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel2.Controls.Add(this.RenderingCanvas);
            this.splitContainer2.Size = new System.Drawing.Size(788, 548);
            this.splitContainer2.SplitterDistance = 263;
            this.splitContainer2.TabIndex = 17;
            // 
            // ObjectListPanel
            // 
            this.ObjectListPanel.Controls.Add(this.ListEditingPanel);
            this.ObjectListPanel.Controls.Add(this.lnk_ShowHiddenObjs);
            this.ObjectListPanel.Controls.Add(this.comboBox1);
            this.ObjectListPanel.Controls.Add(this.lnk_hideSelectedObjs);
            this.ObjectListPanel.Controls.Add(this.Btn_addType);
            this.ObjectListPanel.Controls.Add(this.label1);
            this.ObjectListPanel.Controls.Add(this.ObjectsListBox);
            this.ObjectListPanel.Controls.Add(this.HideGroup_CB);
            this.ObjectListPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ObjectListPanel.Location = new System.Drawing.Point(0, 0);
            this.ObjectListPanel.Name = "ObjectListPanel";
            this.ObjectListPanel.Size = new System.Drawing.Size(263, 253);
            this.ObjectListPanel.TabIndex = 20;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddmAdd,
            this.btnCopy,
            this.btnPaste,
            this.btnDuplicate,
            this.btnDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 259);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(263, 25);
            this.toolStrip1.TabIndex = 21;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ddmAdd
            // 
            this.ddmAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ddmAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.notWorkingYetToolStripMenuItem});
            this.ddmAdd.Image = ((System.Drawing.Image)(resources.GetObject("ddmAdd.Image")));
            this.ddmAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddmAdd.Name = "ddmAdd";
            this.ddmAdd.Size = new System.Drawing.Size(42, 22);
            this.ddmAdd.Text = "Add";
            // 
            // notWorkingYetToolStripMenuItem
            // 
            this.notWorkingYetToolStripMenuItem.Name = "notWorkingYetToolStripMenuItem";
            this.notWorkingYetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.notWorkingYetToolStripMenuItem.Text = "Not working yet";
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCopy.Enabled = false;
            this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(39, 22);
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.Btn_CopyObjs_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPaste.Image = ((System.Drawing.Image)(resources.GetObject("btnPaste.Image")));
            this.btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(39, 22);
            this.btnPaste.Text = "Paste";
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnDuplicate
            // 
            this.btnDuplicate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDuplicate.Image = ((System.Drawing.Image)(resources.GetObject("btnDuplicate.Image")));
            this.btnDuplicate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.Size = new System.Drawing.Size(61, 22);
            this.btnDuplicate.Text = "Duplicate";
            this.btnDuplicate.Visible = false;
            this.btnDuplicate.Click += new System.EventHandler(this.DuplicateSelectedObj_btn);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(44, 22);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btn_delObj_Click);
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 575);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.splitContainer2);
            this.Name = "EditorForm";
            this.Text = "EditorCore";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditorForm_Closed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ListEditingPanel.ResumeLayout(false);
            this.ListEditingPanel.PerformLayout();
            this.ClipBoardMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ObjectListPanel.ResumeLayout(false);
            this.ObjectListPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button Btn_addType;
        private System.Windows.Forms.CheckBox HideGroup_CB;
        public System.Windows.Forms.ListBox ObjectsListBox;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button4;
        public System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ContextMenuStrip ClipBoardMenu;
        private System.Windows.Forms.ToolStripMenuItem ClipBoardMenu_Paste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ClipBoardMenu_CopyPos;
        private System.Windows.Forms.ToolStripMenuItem ClipBoardMenu_CopyRot;
        private System.Windows.Forms.ToolStripMenuItem ClipBoardMenu_CopyScale;
        private System.Windows.Forms.ToolStripMenuItem ClipBoardMenu_CopyFull;
        private System.Windows.Forms.ToolStripMenuItem StatusLbl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.Integration.ElementHost RenderingCanvas;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem gamePathToolStripItem;
        private System.Windows.Forms.ToolStripMenuItem changeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UndoMenu;
        private System.Windows.Forms.ToolStripMenuItem LevelFilesMenu;
        private System.Windows.Forms.ToolStripMenuItem FindMenu;
        private System.Windows.Forms.ToolStripMenuItem objectByIdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectByCameraIdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectBySwitchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchAppearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchKillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchDeadOnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectByViewIdToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStripMenuItem ClipBoardMenu_CopyTransform;
        private System.Windows.Forms.LinkLabel lnk_ShowHiddenObjs;
        private System.Windows.Forms.LinkLabel lnk_hideSelectedObjs;
        private System.Windows.Forms.ToolStripMenuItem objectByNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectByModelNameToolStripMenuItem;
		private System.Windows.Forms.Panel ListEditingPanel;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.ToolStripMenuItem newEditorInstanceToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Panel ObjectListPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton ddmAdd;
        private System.Windows.Forms.ToolStripMenuItem notWorkingYetToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripButton btnPaste;
        private System.Windows.Forms.ToolStripButton btnDuplicate;
        private System.Windows.Forms.ToolStripButton btnDelete;
    }
}

