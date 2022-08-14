namespace StorageTools
{
    partial class BookInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_classname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox_cover = new System.Windows.Forms.PictureBox();
            this.textBox_Description = new System.Windows.Forms.TextBox();
            this.textBox_title1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_title2 = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_AllowAll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_cover)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(236, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Категория документа";
            // 
            // textBox_classname
            // 
            this.textBox_classname.Location = new System.Drawing.Point(239, 25);
            this.textBox_classname.Name = "textBox_classname";
            this.textBox_classname.Size = new System.Drawing.Size(433, 20);
            this.textBox_classname.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(404, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Выберите подходящий вариант названия документа или укажите его вручную";
            // 
            // pictureBox_cover
            // 
            this.pictureBox_cover.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_cover.Location = new System.Drawing.Point(16, 12);
            this.pictureBox_cover.Name = "pictureBox_cover";
            this.pictureBox_cover.Size = new System.Drawing.Size(214, 191);
            this.pictureBox_cover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_cover.TabIndex = 3;
            this.pictureBox_cover.TabStop = false;
            // 
            // textBox_Description
            // 
            this.textBox_Description.Location = new System.Drawing.Point(16, 215);
            this.textBox_Description.Multiline = true;
            this.textBox_Description.Name = "textBox_Description";
            this.textBox_Description.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Description.Size = new System.Drawing.Size(671, 147);
            this.textBox_Description.TabIndex = 4;
            // 
            // textBox_title1
            // 
            this.textBox_title1.Location = new System.Drawing.Point(239, 64);
            this.textBox_title1.Name = "textBox_title1";
            this.textBox_title1.Size = new System.Drawing.Size(436, 20);
            this.textBox_title1.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(616, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Выбрать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(616, 145);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Выбрать";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Или другой вариант??";
            // 
            // textBox_title2
            // 
            this.textBox_title2.Location = new System.Drawing.Point(239, 119);
            this.textBox_title2.Name = "textBox_title2";
            this.textBox_title2.Size = new System.Drawing.Size(436, 20);
            this.textBox_title2.TabIndex = 9;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(616, 185);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Пропустить";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(236, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(340, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Проверьте описание документа и исправьте при необходимости:";
            // 
            // checkBox_AllowAll
            // 
            this.checkBox_AllowAll.AutoSize = true;
            this.checkBox_AllowAll.Location = new System.Drawing.Point(239, 161);
            this.checkBox_AllowAll.Name = "checkBox_AllowAll";
            this.checkBox_AllowAll.Size = new System.Drawing.Size(245, 17);
            this.checkBox_AllowAll.TabIndex = 12;
            this.checkBox_AllowAll.Text = "Обрабатывать далее книги автоматически";
            this.checkBox_AllowAll.UseVisualStyleBackColor = true;
            this.checkBox_AllowAll.CheckedChanged += new System.EventHandler(this.checkBox_AllowAll_CheckedChanged);
            // 
            // BookInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 376);
            this.Controls.Add(this.checkBox_AllowAll);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBox_title2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_title1);
            this.Controls.Add(this.textBox_Description);
            this.Controls.Add(this.pictureBox_cover);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_classname);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "BookInfo";
            this.Text = "Свойства документа";
            this.Load += new System.EventHandler(this.BookInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_cover)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_classname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox_cover;
        private System.Windows.Forms.TextBox textBox_Description;
        private System.Windows.Forms.TextBox textBox_title1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_title2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_AllowAll;
    }
}