namespace SARCExt
{
	partial class SarcEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.replaceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.saveSzsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.extractAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.contextMenuStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(0, 27);
			this.listBox1.Name = "listBox1";
			this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox1.Size = new System.Drawing.Size(386, 225);
			this.listBox1.TabIndex = 0;
			this.listBox1.DoubleClick += new System.EventHandler(this.ListBoxDoubleClick);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.extractToolStripMenuItem,
            this.replaceToolStripMenuItem1});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(116, 70);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// extractToolStripMenuItem
			// 
			this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
			this.extractToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
			this.extractToolStripMenuItem.Text = "Extract";
			this.extractToolStripMenuItem.Click += new System.EventHandler(this.extractToolStripMenuItem_Click);
			// 
			// replaceToolStripMenuItem1
			// 
			this.replaceToolStripMenuItem1.Name = "replaceToolStripMenuItem1";
			this.replaceToolStripMenuItem1.Size = new System.Drawing.Size(115, 22);
			this.replaceToolStripMenuItem1.Text = "Replace";
			this.replaceToolStripMenuItem1.Click += new System.EventHandler(this.replaceToolStripMenuItem1_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveSzsToolStripMenuItem,
            this.addFilesToolStripMenuItem,
            this.extractAllFilesToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(386, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// saveSzsToolStripMenuItem
			// 
			this.saveSzsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem,
            this.saveAsToolStripMenuItem});
			this.saveSzsToolStripMenuItem.Name = "saveSzsToolStripMenuItem";
			this.saveSzsToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.saveSzsToolStripMenuItem.Text = "Save";
			// 
			// replaceToolStripMenuItem
			// 
			this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
			this.replaceToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.replaceToolStripMenuItem.Text = "Replace";
			this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.saveAsToolStripMenuItem.Text = "Save as..";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// addFilesToolStripMenuItem
			// 
			this.addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
			this.addFilesToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
			this.addFilesToolStripMenuItem.Text = "Add files";
			this.addFilesToolStripMenuItem.Click += new System.EventHandler(this.addFilesToolStripMenuItem_Click);
			// 
			// extractAllFilesToolStripMenuItem
			// 
			this.extractAllFilesToolStripMenuItem.Name = "extractAllFilesToolStripMenuItem";
			this.extractAllFilesToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
			this.extractAllFilesToolStripMenuItem.Text = "Extract all files";
			this.extractAllFilesToolStripMenuItem.Click += new System.EventHandler(this.extractAllFilesToolStripMenuItem_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 259);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Compression level [0-9] :";
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericUpDown1.Location = new System.Drawing.Point(128, 256);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(55, 20);
			this.numericUpDown1.TabIndex = 3;
			this.numericUpDown1.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// SarcEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(386, 279);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.Name = "SarcEditor";
			this.Text = "SarcEditor";
			this.Load += new System.EventHandler(this.SarcEditor_Load);
			this.contextMenuStrip1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem saveSzsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem extractAllFilesToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
	}
}