namespace StorageSearch
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView_Places = new System.Windows.Forms.TreeView();
            this.imageList_TreeIcons = new System.Windows.Forms.ImageList(this.components);
            this.treeView_Classes = new System.Windows.Forms.TreeView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBox_query = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_Existing = new System.Windows.Forms.CheckBox();
            this.button_Search = new System.Windows.Forms.Button();
            this.checkBox_Title = new System.Windows.Forms.CheckBox();
            this.checkBox_PicDescr = new System.Windows.Forms.CheckBox();
            this.checkBox_DocDescr = new System.Windows.Forms.CheckBox();
            this.checkBox_Descr = new System.Windows.Forms.CheckBox();
            this.listView_Results = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.imageList_ListViewIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchPlacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportClassTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(572, 360);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(572, 406);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(572, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(572, 360);
            this.splitContainer1.SplitterDistance = 190;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView_Places);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.treeView_Classes);
            this.splitContainer2.Size = new System.Drawing.Size(190, 360);
            this.splitContainer2.SplitterDistance = 177;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView_Places
            // 
            this.treeView_Places.CheckBoxes = true;
            this.treeView_Places.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Places.ImageIndex = 0;
            this.treeView_Places.ImageList = this.imageList_TreeIcons;
            this.treeView_Places.Location = new System.Drawing.Point(0, 0);
            this.treeView_Places.Name = "treeView_Places";
            this.treeView_Places.SelectedImageIndex = 0;
            this.treeView_Places.Size = new System.Drawing.Size(190, 177);
            this.treeView_Places.TabIndex = 0;
            this.treeView_Places.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            // 
            // imageList_TreeIcons
            // 
            this.imageList_TreeIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_TreeIcons.ImageStream")));
            this.imageList_TreeIcons.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList_TreeIcons.Images.SetKeyName(0, "error");
            this.imageList_TreeIcons.Images.SetKeyName(1, "folder");
            this.imageList_TreeIcons.Images.SetKeyName(2, "folderopen");
            this.imageList_TreeIcons.Images.SetKeyName(3, "home");
            this.imageList_TreeIcons.Images.SetKeyName(4, "db");
            this.imageList_TreeIcons.Images.SetKeyName(5, "clas");
            // 
            // treeView_Classes
            // 
            this.treeView_Classes.CheckBoxes = true;
            this.treeView_Classes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Classes.ImageIndex = 0;
            this.treeView_Classes.ImageList = this.imageList_TreeIcons;
            this.treeView_Classes.Location = new System.Drawing.Point(0, 0);
            this.treeView_Classes.Name = "treeView_Classes";
            this.treeView_Classes.PathSeparator = "::";
            this.treeView_Classes.SelectedImageIndex = 0;
            this.treeView_Classes.Size = new System.Drawing.Size(190, 179);
            this.treeView_Classes.TabIndex = 0;
            this.treeView_Classes.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.panel1);
            this.splitContainer3.Panel1MinSize = 105;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.listView_Results);
            this.splitContainer3.Panel2MinSize = 125;
            this.splitContainer3.Size = new System.Drawing.Size(378, 360);
            this.splitContainer3.SplitterDistance = 114;
            this.splitContainer3.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBox_query);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(3, 5);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.MaximumSize = new System.Drawing.Size(800, 105);
            this.panel1.MinimumSize = new System.Drawing.Size(370, 105);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(370, 105);
            this.panel1.TabIndex = 6;
            // 
            // comboBox_query
            // 
            this.comboBox_query.FormattingEnabled = true;
            this.comboBox_query.Location = new System.Drawing.Point(4, 2);
            this.comboBox_query.Name = "comboBox_query";
            this.comboBox_query.Size = new System.Drawing.Size(363, 21);
            this.comboBox_query.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_Existing);
            this.groupBox1.Controls.Add(this.button_Search);
            this.groupBox1.Controls.Add(this.checkBox_Title);
            this.groupBox1.Controls.Add(this.checkBox_PicDescr);
            this.groupBox1.Controls.Add(this.checkBox_DocDescr);
            this.groupBox1.Controls.Add(this.checkBox_Descr);
            this.groupBox1.Location = new System.Drawing.Point(3, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 69);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Искать в:";
            // 
            // checkBox_Existing
            // 
            this.checkBox_Existing.AutoSize = true;
            this.checkBox_Existing.Enabled = false;
            this.checkBox_Existing.Location = new System.Drawing.Point(270, 17);
            this.checkBox_Existing.Name = "checkBox_Existing";
            this.checkBox_Existing.Size = new System.Drawing.Size(84, 17);
            this.checkBox_Existing.TabIndex = 5;
            this.checkBox_Existing.Text = "Найденном";
            this.checkBox_Existing.UseVisualStyleBackColor = true;
            // 
            // button_Search
            // 
            this.button_Search.Location = new System.Drawing.Point(279, 40);
            this.button_Search.Name = "button_Search";
            this.button_Search.Size = new System.Drawing.Size(75, 23);
            this.button_Search.TabIndex = 0;
            this.button_Search.Text = "Поиск";
            this.button_Search.UseVisualStyleBackColor = true;
            this.button_Search.Click += new System.EventHandler(this.button_Search_Click);
            // 
            // checkBox_Title
            // 
            this.checkBox_Title.AutoSize = true;
            this.checkBox_Title.Location = new System.Drawing.Point(6, 19);
            this.checkBox_Title.Name = "checkBox_Title";
            this.checkBox_Title.Size = new System.Drawing.Size(76, 17);
            this.checkBox_Title.TabIndex = 1;
            this.checkBox_Title.Text = "Названии";
            this.checkBox_Title.UseVisualStyleBackColor = true;
            // 
            // checkBox_PicDescr
            // 
            this.checkBox_PicDescr.AutoSize = true;
            this.checkBox_PicDescr.Enabled = false;
            this.checkBox_PicDescr.Location = new System.Drawing.Point(100, 42);
            this.checkBox_PicDescr.Name = "checkBox_PicDescr";
            this.checkBox_PicDescr.Size = new System.Drawing.Size(147, 17);
            this.checkBox_PicDescr.TabIndex = 4;
            this.checkBox_PicDescr.Text = "Описании изображения";
            this.checkBox_PicDescr.UseMnemonic = false;
            this.checkBox_PicDescr.UseVisualStyleBackColor = true;
            // 
            // checkBox_DocDescr
            // 
            this.checkBox_DocDescr.AutoSize = true;
            this.checkBox_DocDescr.Enabled = false;
            this.checkBox_DocDescr.Location = new System.Drawing.Point(100, 19);
            this.checkBox_DocDescr.Name = "checkBox_DocDescr";
            this.checkBox_DocDescr.Size = new System.Drawing.Size(133, 17);
            this.checkBox_DocDescr.TabIndex = 3;
            this.checkBox_DocDescr.Text = "Описании документа";
            this.checkBox_DocDescr.UseVisualStyleBackColor = true;
            // 
            // checkBox_Descr
            // 
            this.checkBox_Descr.AutoSize = true;
            this.checkBox_Descr.Location = new System.Drawing.Point(6, 42);
            this.checkBox_Descr.Name = "checkBox_Descr";
            this.checkBox_Descr.Size = new System.Drawing.Size(76, 17);
            this.checkBox_Descr.TabIndex = 2;
            this.checkBox_Descr.Text = "Описании";
            this.checkBox_Descr.UseVisualStyleBackColor = true;
            // 
            // listView_Results
            // 
            this.listView_Results.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView_Results.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView_Results.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listView_Results.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Results.FullRowSelect = true;
            this.listView_Results.GridLines = true;
            this.listView_Results.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_Results.HideSelection = false;
            this.listView_Results.Location = new System.Drawing.Point(0, 0);
            this.listView_Results.Margin = new System.Windows.Forms.Padding(0);
            this.listView_Results.MultiSelect = false;
            this.listView_Results.Name = "listView_Results";
            this.listView_Results.ShowGroups = false;
            this.listView_Results.Size = new System.Drawing.Size(378, 242);
            this.listView_Results.SmallImageList = this.imageList_ListViewIcons;
            this.listView_Results.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_Results.TabIndex = 0;
            this.listView_Results.UseCompatibleStateImageBehavior = false;
            this.listView_Results.View = System.Windows.Forms.View.Details;
            this.listView_Results.ItemActivate += new System.EventHandler(this.listView_Results_ItemActivate);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Название";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Категория";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Описание";
            this.columnHeader3.Width = 170;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Документ";
            this.columnHeader4.Width = 40;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Изображение";
            this.columnHeader5.Width = 40;
            // 
            // imageList_ListViewIcons
            // 
            this.imageList_ListViewIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_ListViewIcons.ImageStream")));
            this.imageList_ListViewIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_ListViewIcons.Images.SetKeyName(0, "CONTACTS.ICO");
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(572, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchPlacesToolStripMenuItem,
            this.exportClassTreeToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // searchPlacesToolStripMenuItem
            // 
            this.searchPlacesToolStripMenuItem.Name = "searchPlacesToolStripMenuItem";
            this.searchPlacesToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.searchPlacesToolStripMenuItem.Text = "Search places...";
            this.searchPlacesToolStripMenuItem.Click += new System.EventHandler(this.searchPlacesToolStripMenuItem_Click);
            // 
            // exportClassTreeToolStripMenuItem
            // 
            this.exportClassTreeToolStripMenuItem.Name = "exportClassTreeToolStripMenuItem";
            this.exportClassTreeToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.exportClassTreeToolStripMenuItem.Text = "ExportClassTree...";
            this.exportClassTreeToolStripMenuItem.Click += new System.EventHandler(this.exportClassTreeToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.button_Search;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 406);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(580, 440);
            this.Name = "Form1";
            this.Text = "Storage Search";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView_Classes;
        private System.Windows.Forms.TreeView treeView_Places;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Search;
        private System.Windows.Forms.CheckBox checkBox_Title;
        private System.Windows.Forms.CheckBox checkBox_PicDescr;
        private System.Windows.Forms.CheckBox checkBox_DocDescr;
        private System.Windows.Forms.CheckBox checkBox_Descr;
        private System.Windows.Forms.ListView listView_Results;
        private System.Windows.Forms.ComboBox comboBox_query;
        private System.Windows.Forms.ImageList imageList_TreeIcons;
        private System.Windows.Forms.CheckBox checkBox_Existing;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchPlacesToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ImageList imageList_ListViewIcons;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStripMenuItem exportClassTreeToolStripMenuItem;
    }
}

