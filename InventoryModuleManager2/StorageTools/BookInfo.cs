using System;
using System.Windows.Forms;
using System.Drawing;

namespace StorageTools
{
    public partial class BookInfo : Form
    {
        /// <summary>
        /// Класс Сущности
        /// </summary>
        public String m_category;
        /// <summary>
        /// Первый вариант названия Сущности
        /// Он же выходное поле - выбранное название Сущности
        /// </summary>
        public String m_title1;
        /// <summary>
        /// Второй вариант названия Сущности
        /// </summary>
        public String m_title2;
        /// <summary>
        /// Описание сущности
        /// </summary>
        public String m_description;
        /// <summary>
        /// Путь картинки сущности
        /// </summary>
        public String m_picturePath;
        /// <summary>
        /// Обрабатывать все последующие книги автоматически
        /// </summary>
        public bool m_AllowAuto;
        
        public BookInfo()
        {
            InitializeComponent();
            //установить позицию формы, чтобы она не ползла по экрану как обычно, и можно было тупо кликать мышкой по выбранной кнопке не глядя.
            this.Location = new Point(100, 100);
            //первый раз форма нужна, чтобы указать категорию документа. А потом можно поставить галочку и все.
            m_AllowAuto = false;
        }

        private void BookInfo_Load(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(m_category))
                this.textBox_classname.Text = m_category;
            if (!String.IsNullOrEmpty(m_title1))
                this.textBox_title1.Text = m_title1;

            if (!String.IsNullOrEmpty(m_title2))
                this.textBox_title2.Text = m_title2;
            else
            {   //спрятать ненужные контролы чтобы они не мешались при работе
                this.textBox_title2.Visible = false;
                this.button2.Visible = false;
                this.label3.Visible = false;
            }
            if (!String.IsNullOrEmpty(m_description))
                this.textBox_Description.Text = m_description;
            if (!String.IsNullOrEmpty(m_picturePath))
            {
                this.pictureBox_cover.Load(m_picturePath);
            }
            //установить или снять галочку чекбокса
            this.checkBox_AllowAll.Checked = m_AllowAuto;

            this.button1.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //проверить что указана категория документа, иначе будет ошибка при добавлении в Хранилище
            if (String.IsNullOrEmpty(this.textBox_classname.Text.Trim()))
            {
                MessageBox.Show(this, "Должна быть указана категория документа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //user select 1 variant
            this.m_category = this.textBox_classname.Text;
            this.m_description = this.textBox_Description.Text;
            m_title1 = textBox_title1.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //проверить что указана категория документа, иначе будет ошибка при добавлении в Хранилище
            if (String.IsNullOrEmpty(this.textBox_classname.Text.Trim()))
            {
                MessageBox.Show(this, "Должна быть указана категория документа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //user select 2 variant
            this.m_category = this.textBox_classname.Text;
            this.m_description = this.textBox_Description.Text;
            m_title1 = textBox_title2.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkBox_AllowAll_CheckedChanged(object sender, EventArgs e)
        {
            this.m_AllowAuto = this.checkBox_AllowAll.Checked;
        }
    }
}
