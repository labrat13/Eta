using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StorageSearch
{
    public partial class SearchPlacesForm : Form
    {
        public SearchPlacesForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Get or Set places 
        /// </summary>
        public string[] Places
        {
            get 
            {
                List<string> li = new List<string>();
                foreach (Object ob in this.listBox_places.Items)
                    li.Add(ob.ToString());
                return li.ToArray();
            }
            set 
            { 
                this.listBox_places.Items.Clear();
                this.listBox_places.Items.AddRange(value);
            }
        }


        private void button_Up_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox_places.SelectedIndex;
            //если это самый верхний элемент, нельзя поднять его выше
            if (selectedIndex <= 0) 
                return;
            //иначе обменять с соседним более верхним
            int newIndex = selectedIndex - 1;
            //get objects
            Object sel = listBox_places.Items[selectedIndex];
            Object other = listBox_places.Items[newIndex];
            //exchange
            listBox_places.Items[selectedIndex] = other;
            listBox_places.Items[newIndex] = sel;
            //selection
            listBox_places.SelectedIndex = newIndex;

            return;
        }

        private void button_Down_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox_places.SelectedIndex;
            //если это самый нижний элемент, нельзя спустить его ниже
            if ((selectedIndex < 0) || (selectedIndex >= listBox_places.Items.Count - 1))
                return;
            //иначе обменять с соседним более верхним
            int newIndex = selectedIndex + 1;
            //get objects
            Object sel = listBox_places.Items[selectedIndex];
            Object other = listBox_places.Items[newIndex];
            //exchange
            listBox_places.Items[selectedIndex] = other;
            listBox_places.Items[newIndex] = sel;
            //selection
            listBox_places.SelectedIndex = newIndex;
            return;
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Выберите каталог, содержащий Хранилища:";
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog(this) != DialogResult.OK)
                return;
            String newPlacePath = fbd.SelectedPath;
            this.listBox_places.Items.Add(newPlacePath);
            return;
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox_places.SelectedIndex;
            //если ничего не выделено, выходим
            if (selectedIndex < 0)
                return;
            //тут надо удалить элемент из списка
            listBox_places.Items.RemoveAt(selectedIndex);

            return;
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
