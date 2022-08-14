using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using InventoryModuleManager2;

namespace StorageSearch
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Объект поисковика
        /// </summary>
        private StorageSearcher m_searcher;
        /// <summary>
        /// Предел занимаемой приложением памяти - для ограничения поиска
        /// </summary>
        private long m_RamLimitForApplication;

  
        public Form1()
        {
            InitializeComponent();

            m_searcher = new StorageSearcher();
            
 

        }

        private void button_Search_Click(object sender, EventArgs e)
        {
            //если условия поиска не заданы, ничего не делаем
            String query = this.comboBox_query.Text;
            if (String.IsNullOrEmpty(query)) return;

            //показываем окно прогресса поиска
            ProgressForm progform = new ProgressForm();
            progform.Title = "Подготовка поиска..";
            progform.Progress.Value = 0;
            progform.Show(this);
            Application.DoEvents();

            //получаем из деревьев выбранные для поиска классы и хранилища
            List<String> selClasses = m_searcher.MakeSelectedClassesList(this.treeView_Classes);
            List<String> selStorages = m_searcher.MakeSelectedStoragesList(this.treeView_Places);
            SearchCheckboxStates flags = this.GetSearchCheckboxes();

            //ищем порциями - по хранилищам раздельно
            List<ResultItem> results = null;
            //если установлен флаг искать в результатах, то ищем по результатам только
            //если же результаты пустые, ищем в Хранилище.
            if(((flags & SearchCheckboxStates.Previous) != SearchCheckboxStates.None) && (listView_Results.Items.Count > 0))
            {
                this.m_searcher.SearchInResults(listView_Results, query, flags, selClasses.ToArray());
            }
            else
            {
                progform.Progress.Maximum = selStorages.Count;
                
                
                //очистить список результатов
                listView_Results.Items.Clear();
                //иначе ищем в хранилище
                foreach (String storage in selStorages)
                {
                    //тут проверяем объем занятой приложением памяти - если слишком много - прерываем цикл.
                    GC.Collect();
                    //long totalMem = GC.GetTotalMemory(true);
                    long workingSet = Process.GetCurrentProcess().WorkingSet64;
                    if (workingSet > m_RamLimitForApplication) break;//TODO: тут надо вывести сообщение где-то, что поиск прерван из-за превышения предела занимаемой памяти. 

                    progform.Title = storage;
                    Application.DoEvents();

                    //тут собственно ищем в хранилище
                    results = m_searcher.Search(query, flags, storage, selClasses.ToArray());
                    if (results == null)
                    {
                        MessageBox.Show(String.Format("Error! \nStorage={0}\nQuery={1}", storage, query), "Search error!"); 

                    }
                    else
                    {
                        //TODO: тут сразу показываем найденные результаты в списке.
                        m_searcher.AddResults(this.listView_Results, results);
                    }
                    results.Clear();//очистить список от ненужных теперь элементов.

                    if (progform.DialogResult == DialogResult.OK)
                    {
                        Application.DoEvents();
                        break;
                    }
                    progform.Progress.Value += 1;
                    //дать окну отработать обновления
                    Application.DoEvents();
                }
            }

            progform.Close();


            return;
        }

        #region *** Form functions ***
        /// <summary>
        /// NT-Установить чекбоксы согласно строке из настроек приложения
        /// </summary>
        /// <param name="p">Состояние чекбоксов приложения</param>
        private void SetSearchCheckboxes(SearchCheckboxStates p)
        {
            CheckState c;
            if ((p & SearchCheckboxStates.Title) == SearchCheckboxStates.None)
                c = CheckState.Unchecked;
            else c = CheckState.Checked;
            this.checkBox_Title.CheckState = c;
            if ((p & SearchCheckboxStates.Descr) == SearchCheckboxStates.None)
                c = CheckState.Unchecked;
            else c = CheckState.Checked;
            this.checkBox_Descr.CheckState = c;
            if ((p & SearchCheckboxStates.DocDescr) == SearchCheckboxStates.None)
                c = CheckState.Unchecked;
            else c = CheckState.Checked;
            this.checkBox_DocDescr.CheckState = c;
            if ((p & SearchCheckboxStates.PicDescr) == SearchCheckboxStates.None)
                c = CheckState.Unchecked;
            else c = CheckState.Checked;
            this.checkBox_PicDescr.CheckState = c;
            if ((p & SearchCheckboxStates.Previous) == SearchCheckboxStates.None)
                c = CheckState.Unchecked;
            else c = CheckState.Checked;
            this.checkBox_Existing.CheckState = c;

            return;
        }
        /// <summary>
        /// NT-Собрать из чекбоксов строку для настроек приложения
        /// </summary>
        /// <returns>Состояние чекбоксов приложения</returns>
        private SearchCheckboxStates GetSearchCheckboxes()
        {
            SearchCheckboxStates c = SearchCheckboxStates.None;
            if (this.checkBox_Title.CheckState == CheckState.Checked)
                c = c | SearchCheckboxStates.Title;
            if (this.checkBox_Descr.CheckState == CheckState.Checked)
                c = c | SearchCheckboxStates.Descr;
            if (this.checkBox_DocDescr.CheckState == CheckState.Checked)
                c = c | SearchCheckboxStates.DocDescr;
            if (this.checkBox_PicDescr.CheckState == CheckState.Checked)
                c = c | SearchCheckboxStates.PicDescr;
            if (this.checkBox_Existing.CheckState == CheckState.Checked)
                c = c | SearchCheckboxStates.Previous;

            return c;
        }


        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            //восстановить размеры и положение главного окна из настроек приложения

            //загрузить лимит используемой оперативной памяти для поиска
            m_RamLimitForApplication = (long)Properties.Settings.Default.MemoryLimit;
            if (m_RamLimitForApplication < 64 * 1024 * 1024)//нижний предел 64мб, иначе приложение вообще может не искать ничего при большом числе хранилищ.
                m_RamLimitForApplication = 64 * 1024 * 1024;
            //установить чекбоксы на панели поиска из настроек приложения
            SetSearchCheckboxes((SearchCheckboxStates)Properties.Settings.Default.CheckBoxState);
            //Заполнить комбобокс паттернов
            m_searcher.LoadLastUsedQueryies(Properties.Settings.Default.SearchPatternsXml);
            this.comboBox_query.Items.AddRange(m_searcher.LastQueries);
            //Заполнить дерево мест и потом дерево классов
            ProgressForm pf = new ProgressForm();
            pf.Title = "Загружается дерево мест..";
            pf.Progress.Style = ProgressBarStyle.Marquee;
            pf.Progress.Maximum = 100;
            pf.Progress.Value = 100;
            pf.Show(this);
            Application.DoEvents();
            
            m_searcher.LoadPlacesTree(Properties.Settings.Default.StoragePathesXml, this.treeView_Places);
            
            pf.Title = "Загружается дерево классов..";
            Application.DoEvents();
            
            m_searcher.LoadClassesTree( this.treeView_Classes);
            
            pf.Close();
            pf.Dispose();
            return;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Сохранить все, что берется из настроек приложения:
            
            //Сохранить чекбоксы в настройках приложения
            Properties.Settings.Default.CheckBoxState = (int)GetSearchCheckboxes();
            //Сохранить список запросов
            Properties.Settings.Default.SearchPatternsXml = m_searcher.GetSearchPatternsCollection();
            //сохранить список путей поиска хранилищ
            Properties.Settings.Default.StoragePathesXml = m_searcher.GetStoragePathesCollection();
            //сохранить список найденных классов
            //Properties.Settings.Default.StorageClassesXml = m_searcher.GetStorageClassesCollection();

            //Сохранить настройки в файле настроек
            Properties.Settings.Default.Save();

            

            return;
        }

        private void addSearchPlaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select folder with storages:";
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog(this) != DialogResult.OK)
                return;
            String newPlacePath = fbd.SelectedPath;
            //add this path to list and show to places tree
            m_searcher.addNewPlace(newPlacePath, treeView_Places);
            //reload class tree
            m_searcher.LoadClassesTree(treeView_Classes);
            return;
        }

        private void searchPlacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchPlacesForm spf = new SearchPlacesForm();
            spf.Places = this.m_searcher.m_searchPlacesUserDefined.ToArray();
            if (spf.ShowDialog(this) == DialogResult.OK)
            {
                //тут перезагрузить дерево мест подобно тому как из настроек приложения
                //и если приложение вылетит, то эти изменения не запишутся в настройки
                //это повысит надежность приложения
                m_searcher.LoadPlacesTree(spf.Places, this.treeView_Places);
                m_searcher.LoadClassesTree(this.treeView_Classes);
            }
            return;
        }

        /// <summary>
        /// NT-Рекурсивно изменить чекбоксы для всех нижележащих нод.
        /// </summary>
        /// <param name="node"></param>
        private void RecursiveCheckNodeDown(TreeNode node)
        {
            bool state = node.Checked;
            foreach (TreeNode t in node.Nodes)
            {
                t.Checked = state;
                RecursiveCheckNodeDown(t);
            }
            return;
        }

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            RecursiveCheckNodeDown(e.Node);
            return;
        }

        private void listView_Results_ItemActivate(object sender, EventArgs e)
        {
            //selected item
            if (this.listView_Results.SelectedItems.Count > 0)
            {
                ListViewItem selected = this.listView_Results.SelectedItems[0];
                ResultItem ri = (ResultItem)selected.Tag;
                //show details form for this entity
                //get entity from storage and show in form
                this.m_searcher.ShowEntityForm(this, ri);
            }
            return;
        }

        private void exportClassTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sfd.Title = "Создать файл дерева классов";
            sfd.AddExtension = true;
            sfd.DefaultExt = ".txt";
            sfd.FileName = "classtree.txt";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            //else
            this.m_searcher.exportClassTree(sfd.FileName, this.treeView_Classes);
        }



    }
}
