namespace StorageTools
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.создатьХранилищеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addBooksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.добавитьДаташитыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.добавитДьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.добавитьДокументыTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.проверитьХранилищеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.дополнительноToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.импортироватьМодульИнвентарьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.очиститьОкноЛогаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
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
            this.toolStripContainer1.ContentPanel.Controls.Add(this.textBox_Log);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(437, 143);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(437, 189);
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
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(437, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // textBox_Log
            // 
            this.textBox_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Log.Location = new System.Drawing.Point(0, 0);
            this.textBox_Log.MaxLength = 1024000;
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Log.Size = new System.Drawing.Size(437, 143);
            this.textBox_Log.TabIndex = 0;
            this.textBox_Log.WordWrap = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(437, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.fileToolStripMenuItem.Text = "Файл";
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.создатьХранилищеToolStripMenuItem,
            this.toolStripSeparator1,
            this.addBooksToolStripMenuItem,
            this.добавитьДаташитыToolStripMenuItem,
            this.добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem,
            this.добавитДьToolStripMenuItem,
            this.добавитьДокументыTXTToolStripMenuItem,
            this.toolStripSeparator2,
            this.проверитьХранилищеToolStripMenuItem,
            this.toolStripSeparator3,
            this.дополнительноToolStripMenuItem,
            this.очиститьОкноЛогаToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.actionToolStripMenuItem.Text = "Действия";
            // 
            // создатьХранилищеToolStripMenuItem
            // 
            this.создатьХранилищеToolStripMenuItem.Name = "создатьХранилищеToolStripMenuItem";
            this.создатьХранилищеToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.создатьХранилищеToolStripMenuItem.Text = "Создать Хранилище...";
            this.создатьХранилищеToolStripMenuItem.Click += new System.EventHandler(this.создатьХранилищеToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(361, 6);
            // 
            // addBooksToolStripMenuItem
            // 
            this.addBooksToolStripMenuItem.Name = "addBooksToolStripMenuItem";
            this.addBooksToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.addBooksToolStripMenuItem.Text = "ДобавитьКниги...";
            this.addBooksToolStripMenuItem.Click += new System.EventHandler(this.addBooksToolStripMenuItem_Click);
            // 
            // добавитьДаташитыToolStripMenuItem
            // 
            this.добавитьДаташитыToolStripMenuItem.Name = "добавитьДаташитыToolStripMenuItem";
            this.добавитьДаташитыToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.добавитьДаташитыToolStripMenuItem.Text = "Добавить Даташиты...";
            this.добавитьДаташитыToolStripMenuItem.Click += new System.EventHandler(this.добавитьДаташитыToolStripMenuItem_Click);
            // 
            // добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem
            // 
            this.добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem.Name = "добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem";
            this.добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem.Text = "Добавить Даташиты CSV Файл-Описание-Название...";
            this.добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem.ToolTipText = "Добавить даташиты описанные в КСВ-файле как Файл-Описание-Название Название Назва" +
                "ние\r\nНесколько даташитов в одной строке названия разделяются пробелами и-или зап" +
                "ятыми.";
            this.добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem.Click += new System.EventHandler(this.добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem_Click);
            // 
            // добавитДьToolStripMenuItem
            // 
            this.добавитДьToolStripMenuItem.Name = "добавитДьToolStripMenuItem";
            this.добавитДьToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.добавитДьToolStripMenuItem.Text = "Добавить Документы CSV Файл-Описание-Название...";
            this.добавитДьToolStripMenuItem.ToolTipText = "Добавить документы описанные в КСВ-файле как Файл-Описание-НазваниеДокумента";
            this.добавитДьToolStripMenuItem.Click += new System.EventHandler(this.добавитДьToolStripMenuItem_Click);
            // 
            // добавитьДокументыTXTToolStripMenuItem
            // 
            this.добавитьДокументыTXTToolStripMenuItem.Name = "добавитьДокументыTXTToolStripMenuItem";
            this.добавитьДокументыTXTToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.добавитьДокументыTXTToolStripMenuItem.Text = "Добавить  документы TXT";
            this.добавитьДокументыTXTToolStripMenuItem.ToolTipText = "Добавить в Хранилище  файлы .txt, только  из текущего каталога,  как документы, б" +
                "ез описаний и изображений";
            this.добавитьДокументыTXTToolStripMenuItem.Click += new System.EventHandler(this.добавитьДокументыTXTToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(361, 6);
            // 
            // проверитьХранилищеToolStripMenuItem
            // 
            this.проверитьХранилищеToolStripMenuItem.Name = "проверитьХранилищеToolStripMenuItem";
            this.проверитьХранилищеToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.проверитьХранилищеToolStripMenuItem.Text = "Проверить Хранилище...";
            this.проверитьХранилищеToolStripMenuItem.Click += new System.EventHandler(this.проверитьХранилищеToolStripMenuItem_Click);
            // 
            // дополнительноToolStripMenuItem
            // 
            this.дополнительноToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.импортироватьМодульИнвентарьToolStripMenuItem});
            this.дополнительноToolStripMenuItem.Name = "дополнительноToolStripMenuItem";
            this.дополнительноToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.дополнительноToolStripMenuItem.Text = "Дополнительно";
            // 
            // импортироватьМодульИнвентарьToolStripMenuItem
            // 
            this.импортироватьМодульИнвентарьToolStripMenuItem.Name = "импортироватьМодульИнвентарьToolStripMenuItem";
            this.импортироватьМодульИнвентарьToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.импортироватьМодульИнвентарьToolStripMenuItem.Text = "Импортировать модуль Инвентарь...";
            this.импортироватьМодульИнвентарьToolStripMenuItem.Click += new System.EventHandler(this.импортироватьМодульИнвентарьToolStripMenuItem_Click);
            // 
            // очиститьОкноЛогаToolStripMenuItem
            // 
            this.очиститьОкноЛогаToolStripMenuItem.Name = "очиститьОкноЛогаToolStripMenuItem";
            this.очиститьОкноЛогаToolStripMenuItem.Size = new System.Drawing.Size(364, 22);
            this.очиститьОкноЛогаToolStripMenuItem.Text = "Очистить окно лога";
            this.очиститьОкноЛогаToolStripMenuItem.Click += new System.EventHandler(this.очиститьОкноЛогаToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.helpToolStripMenuItem.Text = "Справка";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(361, 6);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 189);
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addBooksToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.ToolStripMenuItem создатьХранилищеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem проверитьХранилищеToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem добавитьДаташитыToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem дополнительноToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem импортироватьМодульИнвентарьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem добавитДьToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem добавитьДокументыTXTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem очиститьОкноЛогаToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}

