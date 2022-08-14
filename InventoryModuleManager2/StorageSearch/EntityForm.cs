using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using InventoryModuleManager2;
using System.IO;
using System.Diagnostics;

namespace StorageSearch
{
    public partial class EntityForm : Form
    {
        /// <summary>
        /// Запись сущности из Хранилища для отображения
        /// </summary>
        private EntityRecord m_Entity;
        
        
        public EntityForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запись сущности из Хранилища для отображения
        /// </summary>
        public EntityRecord Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        private void EntityForm_Load(object sender, EventArgs e)
        {
            this.textBox_title.Text = this.Entity.Title;
            this.textBox_descr.Text = this.Entity.Description;
            this.comboBox_category.Text = this.Entity.EntityType.ClassPath;
            //load image - to temp directory
            if (this.Entity.Picture != null)
            {
                MemoryStream ms = new MemoryStream((Int32)this.Entity.Picture.Length);
                this.Entity.Picture.GetFile(ms);
                this.pictureBox_Picture.Image = Image.FromStream(ms);
                ms.Close();
            }
            else
            {
                //выключить контрол изображения
                this.pictureBox_Picture.Enabled = false;
            }
            //выключить кнопку документа если нет документа
            if (this.Entity.Document != null)
            {
                this.button_showDoc.Enabled = true;
            }
            else
                this.button_showDoc.Enabled = false;
        }

        private void EntityForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO: Тут надо записывать данные из формы обратно в объект записи
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public static void ShowEntityForm(IWin32Window owner, EntityRecord er)
        {
            EntityForm f = new EntityForm();
            f.Entity = er;
            f.ShowDialog(owner);//TODO: Add code here...

            return;
        }

        private void button_showDoc_Click(object sender, EventArgs e)
        {
            try
            {
                //показать документ открыв его
                String temppath = "C:\\Temp";
                if (!Directory.Exists(temppath))
                    Directory.CreateDirectory(temppath);
                //make temp file name
                string ext = Path.GetExtension(this.Entity.Document.StoragePath);
                FileStream fs = null;
                String filename = String.Empty;
                for (int i = 0; i < 10; i++)
                {
                    filename = Path.Combine(temppath, Path.ChangeExtension("docfile" + i.ToString(), ext));
                    //try open doc file
                    fs = InventoryModuleManager2.Utility.tryCreateFile(filename);
                    if (fs != null)
                        break;
                }
                if (fs == null)
                    MessageBox.Show(this, "Открыто слишком много документов. Закройте некоторые и повторите попытку.");
                else
                {
                    //write file content
                    this.Entity.Document.GetFile(fs);
                    fs.Close();
                    //open with ShellExecute
                    Process.Start(filename);
                    //TODO: Add code here...
                    //тут можно несколько файлов держать открытыми, 
                    //чтобы несколько пдфок открывать за раз, а не одну только как сейчас.
                    //для этог надо проверить что файл открыт, и если уже открыт, то создать другое имя файла и попробовать его открыть.
                    //а если уже не получится, то как и сейчас, тихо выйти.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "При открытии документа возникла ошибка.");
            }
            return;
        }




    }
}
