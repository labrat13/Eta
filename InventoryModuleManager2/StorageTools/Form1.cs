using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using InventoryModuleManager2;
using System.IO;
using InventoryModuleManager2.ClassificationEntity;
using System.Data.OleDb;

namespace StorageTools
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Путь каталога Хранилища
        /// </summary>
        private String m_storageFolder;
        /// <summary>
        /// Путь каталога исходных файлов
        /// </summary>
        private String m_sourceFolder;
        /// <summary>
        /// Расширения файлов описаний Сущности
        /// </summary>
        private String[] m_textExtensions;
        /// <summary>
        /// Расширения файлов изображений Сущности
        /// </summary>
        private String[] m_picExtensions;
        /// <summary>
        /// Расширения файлов документов Сущности
        /// </summary>
        private String[] m_docExtensions;
        /// <summary>
        /// Глобальный флаг, показывать ли форму свойств сущности при добавлени сущности в хранилище.
        /// Если не показывать, то проще и быстрее добавлять. 
        /// Но нужно быть уверенным в описаниях сущностей.
        /// Сейчас это работает только для даташитов. Книги итп всегда показывают форму свойств.
        /// </summary>
        private bool m_ShowDescriptionForm;


        public Form1()
        {
            InitializeComponent();

            
        }
        /// <summary>
        /// изменить название формы чтобы оно отражало прогресс процесса
        /// </summary>
        /// <param name="operation">Название процесса или String.Empty или  null если процесс не выполняется</param>
        /// <param name="storageTitle">Название открытого Хранилища или String.Empty или  null если Хранилище не используется.</param>
        /// <param name="srcFolderPath">Путь к папке исходных файлов или String.Empty или  null если процесс не использует исходный каталог.</param>
        /// <param name="percent">значение прогресса в процентах. Укажите значение -1 если прогресс не используется.</param>
        private void SetFormTitle(String operation, String storageTitle, String srcFolderPath, int percent)
        {
            string result = "";
            //сформировать строку вида 38% Добавление TXT - КнигиЛибрусек - С:\temp\librusek1\1
            //если operation  = null, вывести только название приложения.
            if (String.IsNullOrEmpty(operation))
                result = Application.ProductName;
            else
            {
                result = result + operation;

                if (!String.IsNullOrEmpty(storageTitle))
                    result = result + " - " + storageTitle;
                if (!String.IsNullOrEmpty(srcFolderPath))
                    result = result + " - " + srcFolderPath;
                if (percent > -1)
                    result = percent.ToString() + "% " + result;
            }
            //set form title
            this.Text = result;
            Application.DoEvents();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Тут эти расширения загружать из файла настроек, чтобы пользователь мог настроить типы документов и их компаньонов
            //Хранить в настройках в виде строки, разделяя пробелами: .gif .jpg .jpeg .tiff .png 
            this.m_sourceFolder = Properties.Settings.Default.LastSourceFolderPath;
            this.m_storageFolder = Properties.Settings.Default.LastStorageFolderPath;
            m_textExtensions = FileWork.parseFileExtensionsString(Properties.Settings.Default.DescriptionFileExtensions); //new String[] { ".txt", ".diz" };
            m_picExtensions = FileWork.parseFileExtensionsString(Properties.Settings.Default.PictureFileExtensions); //new String[] { ".gif", ".jpg", ".jpeg", ".tiff", ".png" };
            m_docExtensions = FileWork.parseFileExtensionsString(Properties.Settings.Default.DocumentFileExtensions); //new String[] { ".pdf", ".djv", ".djvu", ".doc", ".rtf", ".xls", ".zip", ".chm", ".7z" };
            this.m_ShowDescriptionForm = Properties.Settings.Default.ShowDescriptionForm_Flag;
            
            //установить обычное название формы
            SetFormTitle(null, null, null, -1);
            
            return;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //write settings
            Properties.Settings.Default.Save();
        }

        private void создатьХранилищеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //указать каталог для Хранилища
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            fb.ShowNewFolderButton = true;
            if (!String.IsNullOrEmpty(m_storageFolder) && Directory.Exists(m_storageFolder))
                fb.SelectedPath = m_storageFolder;
            fb.Description = "Выберите каталог для создаваемого Хранилища";
            if (fb.ShowDialog() != DialogResult.OK) return;
            m_storageFolder = fb.SelectedPath;
            fb = null;

            создатьХранилище(this.m_storageFolder);

            return;
        }

        private void addBooksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //указать каталог с книгами
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_sourceFolder) && Directory.Exists(m_sourceFolder))
                fbd.SelectedPath = m_sourceFolder;
            fbd.Description = "Выберите папку с подготовленными книгами";
            if (fbd.ShowDialog() != DialogResult.OK) return;
            this.m_sourceFolder = fbd.SelectedPath;
            fbd = null;
            //указать каталог с Хранилищем
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_storageFolder) && Directory.Exists(m_storageFolder))
                fb.SelectedPath = m_storageFolder;
            fb.Description = "Выберите каталог Хранилища";
            if (fb.ShowDialog() != DialogResult.OK) return;
            m_storageFolder = fb.SelectedPath;
            fb = null;

            //выполнить работу
            AddBooksToStorage(m_storageFolder, m_sourceFolder);

            return;
        }
        /// <summary>
        /// NR- Добавить текстовые файлы как документы, без описаний и картинок.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void добавитьДокументыTXTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //указать каталог с книгами
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_sourceFolder) && Directory.Exists(m_sourceFolder))
                fbd.SelectedPath = m_sourceFolder;
            fbd.Description = "Выберите папку с подготовленными книгами";
            if (fbd.ShowDialog() != DialogResult.OK) return;
            this.m_sourceFolder = fbd.SelectedPath;
            fbd = null;
            //указать каталог с Хранилищем
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_storageFolder) && Directory.Exists(m_storageFolder))
                fb.SelectedPath = m_storageFolder;
            fb.Description = "Выберите каталог Хранилища";
            if (fb.ShowDialog() != DialogResult.OK) return;
            m_storageFolder = fb.SelectedPath;
            fb = null;

            //выполнить работу
            AddTxtDocToStorage(m_storageFolder, m_sourceFolder);

            return;
        }


        private void проверитьХранилищеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get Storage folder
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_storageFolder) && Directory.Exists(m_storageFolder))
                fb.SelectedPath = m_storageFolder;
            fb.Description = "Выберите каталог Хранилища для проверки:";
            if (fb.ShowDialog() != DialogResult.OK) return;
            m_storageFolder = fb.SelectedPath;
            fb = null;

            TestingStorage(m_storageFolder, "C:\\Temp");

            return;
        }

        private void добавитьДаташитыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //указать каталог с книгами
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_sourceFolder) && Directory.Exists(m_sourceFolder))
                fbd.SelectedPath = m_sourceFolder;
            fbd.Description = "Выберите папку с подготовленными даташитами";
            if (fbd.ShowDialog() != DialogResult.OK) return;
            this.m_sourceFolder = fbd.SelectedPath;
            fbd = null;
            //указать каталог с Хранилищем
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_storageFolder) && Directory.Exists(m_storageFolder))
                fb.SelectedPath = m_storageFolder;
            fb.Description = "Выберите каталог Хранилища";
            if (fb.ShowDialog() != DialogResult.OK) return;
            m_storageFolder = fb.SelectedPath;
            fb = null;

            //CSV-файл описания деталей нужно получить утилитой.
            //Отредактировать (убрать мусор) и поместить в каталог с даташитами
            //как result.csv
            string csvfile = Path.Combine(this.m_sourceFolder, "result.csv");
            if (!File.Exists(csvfile))
            {
                DialogResult dr = MessageBox.Show(this, "Файл описания даташитов result.csv не найден в каталоге с даташитами!\nДля даташитов не будут добавлены описания.\n Продолжить выполнение задачи?", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (dr != DialogResult.OK)
                    return;
            }
            AddDatasheetsToStorage(m_storageFolder, m_sourceFolder, csvfile);
            return;
        }

        private void импортироватьМодульИнвентарьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //указать каталог модуля свойств Инвентарь
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_sourceFolder) && Directory.Exists(m_sourceFolder))
                fbd.SelectedPath = m_sourceFolder;
            fbd.Description = "Выберите каталог модуля свойств Инвентарь";
            if (fbd.ShowDialog() != DialogResult.OK) return;
            this.m_sourceFolder = fbd.SelectedPath;
            fbd = null;
            //указать каталог с Хранилищем
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_storageFolder) && Directory.Exists(m_storageFolder))
                fb.SelectedPath = m_storageFolder;
            fb.Description = "Выберите каталог Хранилища";
            if (fb.ShowDialog() != DialogResult.OK) return;
            m_storageFolder = fb.SelectedPath;
            fb = null;

            ImportFromInventoryModule(m_storageFolder, m_sourceFolder);

            return;
        }



        #region Service functions

        /// <summary>
        /// Добавить строку текста в текстбокс лога.
        /// Не забудьте вызвать Application.DoEvents() после.
        /// </summary>
        /// <param name="text"></param>
        private void AddLogString(String text)
        {
            StringBuilder sb = new StringBuilder(textBox_Log.Text);
            sb.AppendFormat("{0}  {1}", DateTime.Now.ToShortTimeString(), text);
            sb.AppendLine();
            textBox_Log.Clear();
            textBox_Log.AppendText(sb.ToString());
            textBox_Log.ScrollToCaret();
            Application.DoEvents();
            return;
        }

        private void AddDatasheetsToStorage(string storageFolder, string sourceFolder, string csvfile)
        {

            Manager man = null;
            try
            {
                //1 загрузить словарь с описаниями из ксв-файла
                Dictionary<String, String> descriptionDictionary = loadDescriptions(csvfile);
                //2 найти файлы книг для переработки
                //поскольку расширения файлов djv djvu обрабатываются фреймворком неправильно, 
                //в списке эти файлы встречаются дважды
                //поэтому вместо списка файлов применен словарь, чтобы файлы были уникальными
                String[] files = FileWork.getFolderFilesByExts(sourceFolder, m_docExtensions, SearchOption.AllDirectories);
                //если файлов нет, вывести сообщение и закончить работу.
                if (files.Length == 0)
                {
                    AddLogString("Файлы даташитов не найдены");
                    AddLogString("Добавление даташитов завершено");
                    return;
                }
                //вывести количество найденных файлов книг
                AddLogString("Найдено файлов даташитов: " + files.Length);

                //3 добавить книги в Хранилище
                //открыть хранилище
                man = new Manager();
                man.Open(storageFolder);
                AddLogString("Хранилище открыто: " + storageFolder);

                //подготовить файлы для загрузки
                //текстовый файл надо вывести на экран, чтобы убедиться в правильной его кодировке
                String BookCategory = String.Empty;//класс документа будет здесь храниться на время операции

                Char[] splitter2 = new char[] { ' ' };

                foreach (String docFile in files)
                {
                    //загружаем файлы-компаньоны, если они есть вдруг случайно
                    String picFile = findFirstCompanionFile(docFile, m_picExtensions);//загружаем компаньон если он есть
                    String textFile = findFirstCompanionFile(docFile, m_textExtensions);
                    //загружаем текст описания из словаря описаний 
                    String description = String.Empty;
                    string fname = docFile.ToLowerInvariant().Trim();
                    if (descriptionDictionary.ContainsKey(fname))
                        description = descriptionDictionary[fname];
                    //имя файла может содержать более 1 названия детали
                    //надо разобрать имя на названия деталей по пробелам
                    String datasheetTitle = Path.GetFileNameWithoutExtension(docFile);
                    String[] names = datasheetTitle.Split(splitter2, StringSplitOptions.RemoveEmptyEntries);
                    //избавиться от коротких 1 в конце файла если они есть - изменяет размерость массива.
                    names = MakeUpperCaseNames(names);

                    //автоматически назначить категорию на основании имен каталогов
                    BookCategory = makeCategory(sourceFolder, docFile);

                    foreach (String ds in names)
                    {
                        String descr007 = description + LoadDescription(textFile);
                        //тут управляем показом формы при добавлении.
                        if (this.m_ShowDescriptionForm == true)
                        {
                            //показать диалог добавления одной книги
                            //... тут создать диалог свойств книги и показать его
                            //и сохранить новую категорию в переменную
                            //и добавить одобренную пользователем книгу в хранилище
                            //show dialog for adding
                            BookInfo bi = new BookInfo();
                            bi.m_picturePath = picFile;
                            bi.m_title1 = ds;
                            bi.m_title2 = String.Empty;//Нет второго вартанта названия
                            bi.m_description = descr007;
                            bi.m_category = String.Copy(BookCategory);

                            if (bi.ShowDialog() == DialogResult.OK)
                            {
                                //add book to storage
                                BookCategory = bi.m_category;
                                AddEntity(man, false, bi.m_category, bi.m_title1, bi.m_description, docFile, picFile);
                                AddLogString("Добавлен: " + docFile);
                            }
                            else
                                AddLogString("Пропущен пользователем: " + docFile);
                        }
                        else
                        {
                            //automatic added without dialog box
                            AddEntity(man, false, BookCategory, ds, descr007, docFile, picFile);
                            AddLogString("Добавлен: " + docFile);

                        }
                        //обновить окно приложения
                        Application.DoEvents();
                    }
                }
                //завершить операцию
                man.Close();
                AddLogString("Хранилище закрыто");
                AddLogString("Добавление даташитов завершено");
                AddLogString("");
                AddLogString("");
            }
            catch (Exception ex)
            {
                AddLogString("Ошибка: " + ex.ToString());
                if (man != null)
                    man.Close();
            }
            return;

        }
        /// <summary>
        /// автоматически назначить категорию на основании имен каталогов
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="docFile"></param>
        /// <returns></returns>
        private static string makeCategory(string sourceFolder, string docFile)
        {
            Char[] splitter = new char[] { '\\' };
            String folder = Path.GetDirectoryName(docFile);
            String classpath = String.Empty;
            if (folder.StartsWith(sourceFolder))
            {
                //remove source folder path
                String dec = folder.Substring(sourceFolder.Length);
                //split to folder names
                String[] dirs = dec.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                //make classpath
                classpath = String.Join("::", dirs);
            }

            return classpath;
        }
        /// <summary>
        /// NT-перевести имена в верхний регистр и удалить имена короче 2 символов
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        private string[] MakeUpperCaseNames(string[] names)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < names.Length; i++)
            {
                String name = names[i];
                //check length of part name
                if (name.Length < 2)
                    continue;
                //process
                String newname = name.ToUpperInvariant();
                //check uP pattern
                if ((name[0] == 'u'))
                    newname = 'u' + newname.Substring(1);
                //add to output list
                result.Add(newname);
            }
            //return array
            return result.ToArray();
        }


        /// <summary>
        /// Добавить файлы pdf djv djvu doc как книги в Хранилище.
        /// Файлы ищутся в каталоге и подкаталогах.
        /// Изображения и описания должны совпадать по названию с документами.
        /// </summary>
        /// <param name="storageFolder">Каталог Хранилища</param>
        /// <param name="sourceFolder">Каталог, в котором содержатся файлы книг.</param>
        private void AddBooksToStorage(string storageFolder, string sourceFolder)
        {
            Manager man = null;
            try
            {
                //1 найти файлы книг для переработки
                //поскольку расширения файлов djv djvu обрабатываются фреймворком неправильно, 
                //в списке эти файлы встречаются дважды
                //поэтому вместо списка файлов применен словарь, чтобы файлы были уникальными
                String[] files = FileWork.getFolderFilesByExts(sourceFolder, m_docExtensions, SearchOption.AllDirectories);
                //если файлов нет, вывести сообщение и закончить работу.
                if (files.Length == 0)
                {
                    AddLogString("Файлы книг не найдены");
                    AddLogString("Добавление книг завершено");
                    return;
                }
                //вывести количество найденных файлов книг
                AddLogString("Найдено файлов книг: " + files.Length);

                //2 добавить книги в Хранилище
                //открыть хранилище
                man = new Manager();
                man.Open(storageFolder);
                AddLogString("Хранилище открыто: " + storageFolder);

                //подготовить файлы для загрузки
                //текстовый файл надо вывести на экран, чтобы убедиться в правильной его кодировке
                String BookCategory = String.Empty;//класс документа будет здесь храниться на время операции

                foreach (String docFile in files)
                {
                    String picFile = findFirstCompanionFile(docFile, m_picExtensions);
                    String textFile = findFirstCompanionFile(docFile, m_textExtensions);

                    //показать диалог добавления одной книги
                    //... тут создать диалог свойств книги и показать его
                    //и сохранить новую категорию в переменную
                    //и добавить одобренную пользователем книгу в хранилище
                    //show dialog for adding
                    BookInfo bi = new BookInfo();
                    bi.m_picturePath = picFile;
                    bi.m_title1 = Path.GetFileNameWithoutExtension(docFile);
                    bi.m_title2 = String.Empty;//Нет второго варианта названия
                    bi.m_description = LoadDescription(textFile);
                    bi.m_category = String.Copy(BookCategory);
                    //установить позицию формы, чтобы она не ползла по экрану как обычно, и можно было тупо кликать мышкой по выбранной кнопке не глядя.
                    bi.Location = new Point(100, 100);
                    if (bi.ShowDialog() == DialogResult.OK)
                    {
                        //add book to storage
                        BookCategory = bi.m_category;
                        AddEntity(man, false, bi.m_category, bi.m_title1, bi.m_description, docFile, picFile);
                        AddLogString("Добавлен: " + docFile);
                    }
                    else
                        AddLogString("Пропущен пользователем: " + docFile);
                    //обновить окно приложения
                    Application.DoEvents();
                }
                //завершить операцию
                man.Close();
                AddLogString("Хранилище закрыто");
                AddLogString("Добавление книг завершено");
                AddLogString("");
                AddLogString("");
            }
            catch (Exception ex)
            {
                AddLogString("Ошибка: " + ex.ToString());
                if (man != null)
                    man.Close();
            }
            return;
        }

        /// <summary>
        /// NR- Добавить из текущей директории текстовые файлы .txt как документы, без описаний и картинок.
        /// </summary>
        /// <param name="storageFolder">Каталог Хранилища</param>
        /// <param name="sourceFolder">Каталог, в котором содержатся файлы документов.</param>
        private void AddTxtDocToStorage(string storageFolder, string sourceFolder)
        {
            Manager man = null;
            try
            {
                //1 найти файлы книг для переработки
                //поскольку расширения файлов djv djvu обрабатываются фреймворком неправильно, 
                //в списке эти файлы встречаются дважды
                //поэтому вместо списка файлов применен словарь, чтобы файлы были уникальными
                String[] files = FileWork.getFolderFilesByExts(sourceFolder, new String[] { ".txt" }, SearchOption.TopDirectoryOnly);
                //если файлов нет, вывести сообщение и закончить работу.
                if (files.Length == 0)
                {
                    AddLogString("Файлы книг не найдены");
                    AddLogString("Добавление книг завершено");
                    return;
                }
                //вывести количество найденных файлов книг
                AddLogString("Найдено файлов книг: " + files.Length);

                //2 добавить книги в Хранилище
                //открыть хранилище
                man = new Manager();
                man.Open(storageFolder);
                AddLogString("Хранилище открыто: " + storageFolder);

                //подготовить файлы для загрузки
                //текстовый файл надо вывести на экран, чтобы убедиться в правильной его кодировке
                String BookCategory = man.StorageBaseClass;//класс документа будет здесь храниться на время операции
                
                bool AutoProcess = false;//галочка на форме описания книги "Выполнять дальше автоматически"
                for(int fileindex = 0; fileindex < files.Length; fileindex++)
                {
                    if ((fileindex % 8) == 0)
                    {
                        SetFormTitle("Добавление TXT", man.StorageTitle, sourceFolder, getIntPercent(files.Length, fileindex));
                    }
                    
                    string docFile = files[fileindex];
                    string title = Path.GetFileNameWithoutExtension(docFile);
                    string category = String.Copy(BookCategory);//копируем строку категории, она должна быть уже задана ранее в первом показе форме BookInfo
                    if (AutoProcess == false)
                    {
                        //показать диалог добавления одной книги
                        //... тут создать диалог свойств книги и показать его
                        //и сохранить новую категорию в переменную
                        //и добавить одобренную пользователем книгу в хранилище
                        //show dialog for adding
                        BookInfo bi = new BookInfo();
                        bi.m_picturePath = String.Empty; //картинки не нужны
                        bi.m_title1 = title;
                        bi.m_title2 = String.Empty;//Нет второго варианта названия
                        bi.m_description = String.Empty;//нет файла описания
                        bi.m_category = category;
                        //установить позицию формы, чтобы она не ползла по экрану как обычно, и можно было тупо кликать мышкой по выбранной кнопке не глядя.
                        bi.Location = new Point(100, 100);
                        if (bi.ShowDialog() == DialogResult.OK)
                        {
                            //add book to storage
                            BookCategory = bi.m_category;//передать значение категории документа из формы сюда
                            AutoProcess = bi.m_AllowAuto;//передать значение галочки из формы сюда
                            AddEntity(man, false, bi.m_category, bi.m_title1, bi.m_description, docFile, String.Empty);
                            AddLogString("Добавлен: " + docFile);
                        }
                        else
                            AddLogString("Пропущен пользователем: " + docFile);

                    }
                    else
                    {
                        //автоматически обрабатывать файлы без участия пользователя.
                        //поскольку этих книг 16000 штук в каталоге, глупо показывать для них форму свойств.
                        //но хотя бы в первый раз ее показать надо - там надо указать категорию документа, а больще этого сделать и негде.
                        //а если файлов мало - то галочку не ставить, и тогда - добавлять файлы через форму свойств.
                        AddEntity(man, true, category, title, String.Empty, docFile, String.Empty);
                        AddLogString("Добавлен автоматически: " + docFile);
                    }
                    //обновить окно приложения
                    Application.DoEvents();
                }
                //завершить операцию
                man.Close();
                AddLogString("Хранилище закрыто");
                AddLogString("Добавление книг завершено");
                AddLogString("");
                AddLogString("");
            }
            catch (Exception ex)
            {
                AddLogString("Ошибка: " + ex.ToString());
                if (man != null)
                    man.Close();
            }
            //заменить надпись формы на обычную
            SetFormTitle(null, null, null, -1);

            return;
        }
        /// <summary>
        /// NT-вычислить процент завершенности процесса
        /// </summary>
        /// <param name="full">значение для 100%</param>
        /// <param name="part">значение текущее</param>
        /// <returns>Функция возвращает степерь завершенности процесса в процентах</returns>
        private int getIntPercent(int full, int part)
        {
            Double f = (Double)full;
            Double p = (Double)part;
            Double result = (p / f) * 100.0d;// or 100*p / f;

            return (Int32)result;
        }

        ///// <summary>
        ///// NT-загрузить файлы документов из исходного каталога - заменена на функции FileWork
        ///// </summary>
        ///// <param name="sourceFolder">Путь к исходному каталогу</param>
        ///// <param name="docExtensions">Массив расширений файлов документов</param>
        ///// <returns></returns>
        //private static Dictionary<String, String> LoadDocumentFiles(string sourceFolder, String[] docExtensions)
        //{
        //    Dictionary<String, String> files = new Dictionary<string, string>();
        //    foreach (String ext in docExtensions)
        //    {
        //        String[] sar = Directory.GetFiles(sourceFolder, "*" + ext, SearchOption.AllDirectories);
        //        foreach (String sa in sar)
        //        {
        //            if (!files.ContainsKey(sa))
        //                files.Add(sa, sa);
        //        }
        //    }
        //    return files;
        //}

        /// <summary>
        /// NR-Загрузить словарь описаниями даташитов
        /// </summary>
        /// <param name="csvfile"></param>
        /// <returns></returns>
        private static Dictionary<string, string> loadDescriptions(string csvfile)
        {
            Dictionary<String, String> dict = new Dictionary<string, string>();
            //check file exists
            if (!File.Exists(csvfile))
                return dict;
            //read file
            StreamReader sr = new StreamReader(csvfile, Encoding.GetEncoding(1251));
            Char[] splitter = new Char[] { ';' };
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                String[] sar = line.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (sar.Length < 2)
                    continue;
                String filepath = sar[0].Trim().ToLowerInvariant();//в нижнем регистре чтобы ключи всегда были одинаковые
                //собрать строку для описания даташита из всех столбцов ксв-файла в строке
                String descr = String.Empty;
                for (int i = 1; i < sar.Length; i++)
                    descr = descr + sar[i] + Environment.NewLine;
                descr = descr.Trim();
                //добавить в словарь если еще нет такого ключа
                //если уже есть, новые данные будут потеряны, но этого не может быть, так как путь уникальный.
                if (!dict.ContainsKey(filepath))
                    dict.Add(filepath, descr);
            }
            sr.Close();
            return dict;
        }

        /// <summary>
        /// Добавить запись сущности. В описание файла документа добавляется описание сущности. В описание изображения сущности добавляется название сущности.
        /// </summary>
        /// <param name="man"></param>
        /// <param name="cat"></param>
        /// <param name="title"></param>
        /// <param name="descr"></param>
        /// <param name="docpath"></param>
        /// <param name="picpath"></param>
        /// <param name="buffered">Буферировать добавляемые файлы?</param>
        private void AddEntity(Manager man, Boolean buffered, string cat, string title, string descr, string docpath, string picpath)
        {
            //4 создать запись сущности
            //создаем полную запись с файлами документа и картинки
            EntityRecord e = new EntityRecord();
            e.Title = title;
            e.EntityType = new ClassItem(man.StorageBaseClass, cat);
            e.Description = descr;
            if (!String.IsNullOrEmpty(docpath))
            {
                if (File.Exists(docpath))
                {
                    FileRecord frd = new FileRecord(docpath);
                    frd.Description = descr;
                    frd.DocumentType = "Document" + FileWork.createDocTypeFromFileExtension(Path.GetExtension(docpath));//указывать только конечный тип сущности
                    e.Document = frd;
                }
                else
                    AddLogString("Error: " + docpath); //log file access error
            }
            if (!String.IsNullOrEmpty(picpath))
            {
                if (File.Exists(picpath))
                {
                    FileRecord frp = new FileRecord(picpath);
                    frp.Description = title;
                    frp.DocumentType = "Picture" + FileWork.createDocTypeFromFileExtension(Path.GetExtension(picpath));//указывать только конечный тип сущности
                    e.Picture = frp;
                }
                else
                    AddLogString("Error: " + picpath); //log file access error
            }
            //5 добавить сущность в менеджер хранилища
            if (buffered == true)
                man.AddEntityBuffered(e);
            else
                man.AddEntity(e);//небуферизованное, медленное добавление, поскольку буферизованное дает ошибки с одинаковыми именами файлов.
            //6 закончить цикл
            return;
        }

        private static string LoadDescription(string txtfile)
        {
            String result = String.Empty;
            if (!String.IsNullOrEmpty(txtfile))
            {
                StreamReader sr = new StreamReader(txtfile, Encoding.GetEncoding(1251));
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }



        /// <summary>
        /// NT-Вернуть путь к существующему файлу с тем же именем и одним из допустимых расширений
        /// Или вернуть пустую строку если файлов не найдено
        /// </summary>
        /// <param name="fi">Путь к файлу, для которого ищутся компаньоны</param>
        /// <param name="textExtensions">Расширения для файлов-компаньонов</param>
        /// <returns></returns>
        private string findFirstCompanionFile(string file, string[] exts)
        {
            foreach (String s in exts)
            {
                String name = Path.ChangeExtension(file, s);
                if (File.Exists(name))
                    return name;
            }
            return String.Empty;
        }

        /// <summary>
        /// NFT - Создать новое Хранилище через форму Свойств Хранилища
        /// </summary>
        /// <param name="storageRootFolder">Начальный путь к каталогу Хранилища, можно изменить в процессе создания</param>
        private void создатьХранилище(String storageRootFolder)
        {
            AddLogString("Создается Хранилище: " + storageRootFolder);
            Manager man = null;
            try
            {
                //показать форму свойств нового хранилища
                StorageInfo si = new StorageInfo();
                //si.Creator = "Я";//это не обязательное поле, можно прописать значение в конструкторе.
                si.Title = "КнигаЭкономика"; //Название Хранилища,
                si.QualifiedName = "Книга.КнигаЭкономика";//Квалифицированное имя хранилища
                si.Description = "Книги по Экономике";
                si.StorageType = "Книга::КнигаЭкономика<Документ, Изображение>";//Тип Хранилища или его схема в нотации типов Оператора
                si.StoragePath = storageRootFolder;//Это путь к КаталогХранилища.

                StorageInfoForm frm = new StorageInfoForm();
                frm.Info = si;
                if (frm.ShowDialog() != DialogResult.OK)
                    return;
                StorageInfo si2 = frm.Info;
                //Создаем Хранилище
                man = Manager.CreateStorage(si2);
                //хранилище уже открыто, закрыть его
                man.Close();
                //запомнить путь к хранилищу
                m_storageFolder = si2.StoragePath;

                AddLogString("Создано Хранилище: " + m_storageFolder);
                AddLogString("");
                AddLogString("");
            }
            catch (Exception ex)
            {
                AddLogString("Ошибка: " + ex.ToString());
                if (man != null)
                    man.Close();
            }
            return;
        }

        /// <summary>
        /// Проверить одно хранилище на читаемость файлов документов и изображений
        /// </summary>
        /// <param name="storageFolder">Каталог проверяемого Хранилища</param>
        /// <param name="tempFolderPath">Временная папка для проверки файлов</param>
        private void TestingStorage(string storageFolder, string tempFolderPath)
        {

            AddLogString("Проверка Хранилища " + storageFolder);
            Manager man = null;
            try
            {
                //Открыть хранилище для проверки возможности его открытия
                man = new Manager();
                man.Open(storageFolder);
                //далее работаем с хранилищем
                AddLogString("Хранилище открыто");

                //create temp folder
                String tempfolder = Path.Combine(tempFolderPath, "t" + DateTime.Now.ToBinary().ToString());
                if (!Directory.Exists(tempfolder))
                    Directory.CreateDirectory(tempfolder);
                String tempDocPath = Path.Combine(tempfolder, "doc.pdf");
                String tempPicPath = Path.Combine(tempfolder, "pic.jpg");
                if (File.Exists(tempDocPath))
                    File.Delete(tempDocPath);
                if (File.Exists(tempPicPath))
                    File.Delete(tempPicPath);
                //получить число документов из свойств Хранилища
                StorageInfo si = man.GetStorageInfo();
                String docS = si.DocsCount.ToString();
                String picS = si.PicsCount.ToString();
                si = null;
                //
                AddLogString("Проверка файлов документов (" + docS + "шт.)...");
                Int32 docCnt = 0;
                checkStorageFiles(man.Documents, tempDocPath, ref docCnt);
                AddLogString("Проверка файлов изображений (" + picS + "шт.)...");
                Int32 picCnt = 0;
                checkStorageFiles(man.Pictures, tempPicPath, ref picCnt);
                //закрыть хранилище
                man.Close();
                AddLogString("Хранилище закрыто");
                //delete temp directory
                Directory.Delete(tempfolder, true);
                //отчет о количестве файлов
                AddLogString("Файлов Документов найдено: " + docCnt.ToString() + " из " + docS);
                AddLogString("Файлов Изображений найдено: " + picCnt.ToString() + " из " + picS);
                //отчет о результате проверки
                AddLogString("Файлы Хранилища прочитаны успешно");
                AddLogString("");
                AddLogString("");
            }
            catch (Exception ex)
            {
                AddLogString("Ошибка: " + ex.ToString());
                if (man != null)
                    man.Close();
            }
            return;
        }
        /// <summary>
        /// NT-Проверить файлы указанной коллекции Хранилища
        /// Если размер файла меньше 10МБ, файл распаковывается в память, так быстрее.
        /// </summary>
        /// <param name="collection">Коллекция Документов или Изображений</param>
        /// <param name="tempFilePath">Путь временного файла для извлечения на диск</param>
        /// <param name="cnt"></param>
        private void checkStorageFiles(DocumentCollection collection, string tempFilePath, ref int cnt)
        {
            String archfilename;
            const int memFileSize = 10 * 1024 * 1024;//memory file size max=10mb
            foreach (FileRecord fr in collection)
            {
                cnt++;
                archfilename = fr.StoragePath;
                //просто проверим что что-то доступно из файла
                if (fr.Length < memFileSize)//if file size < 10 mb
                {
                    //read to memory stream
                    MemoryStream ms = new MemoryStream(memFileSize);
                    fr.GetFile(ms);
                    if (ms.Length != fr.Length)
                        throw new Exception(String.Format("Файл {0} неисправен", archfilename));
                    else
                        ms.Close();
                }
                else
                {
                    fr.GetFile(tempFilePath);
                    FileInfo fi = new FileInfo(tempFilePath);
                    if (fi.Exists)
                    {
                        //check file size
                        if (fi.Length != fr.Length)
                            throw new Exception(String.Format("Файл {0} имеет неверный размер", archfilename));
                        fi.Delete();
                    }
                    else throw new Exception(String.Format("Файл {0} не существует", archfilename));
                }
                //print 
                AddLogString(String.Format("№{0} {1}", cnt, archfilename));
            }
            return;
        }

        /// <summary>
        /// NT-Импорт данных из модуля свойств Инвентарь в Хранилище.
        /// </summary>
        /// <param name="storageFolder"></param>
        /// <param name="propModuleFolder"></param>
        private void ImportFromInventoryModule(string storageFolder, string propModuleFolder)
        {
            AddLogString(String.Format("Импорт модуля свойств {0} в Хранилище {1}", propModuleFolder, storageFolder));
            Manager man = null;
            OleDbConnection con = null;
            try
            {
                //1 открыть БД модуля
                String dbname = Path.GetFileName(propModuleFolder) + ".mdb";
                String dbpath = Path.Combine(propModuleFolder, dbname);
                String constring = createConnectionString(dbpath);
                //create connection
                con = new OleDbConnection(constring);

                //2 Открываем Хранилище
                man = new Manager();
                man.Open(storageFolder);
                AddLogString("Хранилище открыто: " + storageFolder);
                //далее работаем с хранилищем
                //3 считать каждую запись из БД модуля 
                //open new connection and set as primary
                con.Open();
                String query = "SELECT * FROM info;";
                OleDbCommand cmd = new OleDbCommand(query, con);
                cmd.CommandTimeout = 1000;
                OleDbDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        int id = rdr.GetInt32(0);
                        String cat = getDbString(rdr, 1);
                        String title = getDbString(rdr, 2);
                        String descr = getDbString(rdr, 3);
                        String doc = getDbString(rdr, 4);
                        String pic = getDbString(rdr, 5);

                        //make file pathes
                        String docfile = String.Empty;
                        String picfile = String.Empty;
                        if (!String.IsNullOrEmpty(doc))
                            docfile = Path.Combine(propModuleFolder, doc);
                        if (!String.IsNullOrEmpty(pic))
                            picfile = Path.Combine(propModuleFolder, pic);

                        //add entity
                        AddEntity(man, false, cat, title, descr, docfile, picfile);

                        //print Entity title to user
                        AddLogString("Добавлена запись: " + title);

                        Application.DoEvents();
                    }
                //завершить операцию
                rdr.Close();
                //7 закрыть БД
                con.Close();
                //8 закрыть менеджер
                man.Close();
                AddLogString("Хранилище закрыто");
                AddLogString("Добавление документов завершено");
                AddLogString("");
                AddLogString("");

            }
            catch (Exception ex)
            {
                AddLogString("Ошибка: " + ex.ToString());
                if (man != null)
                    man.Close();
                if (con != null)
                    if (con.State != System.Data.ConnectionState.Closed)
                        con.Close();
            }
            return;
        }

        /// <summary>
        /// Получить строку из ридера таблицы или пустую строку
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string getDbString(OleDbDataReader rdr, int p)
        {
            //эта функция уже добавлена в мою библиотеку классов
            if (rdr.IsDBNull(p))
                return String.Empty;
            else return rdr.GetString(p).Trim();
        }

        /// <summary>
        /// NT-Создать строку соединения с БД
        /// </summary>
        /// <param name="dbFile">Путь к файлу БД</param>
        public static string createConnectionString(string dbFile)
        {
            //Provider=Microsoft.Jet.OLEDB.4.0;Data Source="C:\Documents and Settings\salomatin\Мои документы\Visual Studio 2008\Projects\RadioBase\радиодетали.mdb"
            OleDbConnectionStringBuilder b = new OleDbConnectionStringBuilder();
            b.Provider = "Microsoft.Jet.OLEDB.4.0";
            b.DataSource = dbFile;
            //user id and password can specify here
            return b.ConnectionString;
        }

        #endregion

        #region Конвертер: Добавить документы и даташиты в Хранилища через CSV FDT формат


        /// <summary>
        /// Добавить в Хранилище даташиты и документы из файла CSV в формате Файл-Описание-Название
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void добавитьДаташитыКакФайлОписаниеНазваниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //указать каталог с файлом КСВ
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            ofd.Title = "Выберите CSV файл с данными документов";
            if (!String.IsNullOrEmpty(m_sourceFolder) && Directory.Exists(m_sourceFolder))
                ofd.InitialDirectory = m_sourceFolder;
            if (ofd.ShowDialog() != DialogResult.OK) return;
            String csvFile = ofd.FileName;
            ofd = null;

            //указать каталог с Хранилищем
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_storageFolder) && Directory.Exists(m_storageFolder))
                fb.SelectedPath = m_storageFolder;
            fb.Description = "Выберите каталог Хранилища";
            if (fb.ShowDialog() != DialogResult.OK) return;
            m_storageFolder = fb.SelectedPath;
            fb = null;

            //Добавить даташиты в Хранилище File-Description-Title
            AddDatasheetFromCsvOnFDTformat(this.m_storageFolder, csvFile);
        }

        private void добавитДьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //указать каталог с файлом КСВ
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            ofd.Title = "Выберите CSV файл с данными документов";
            if (!String.IsNullOrEmpty(m_sourceFolder) && Directory.Exists(m_sourceFolder))
                ofd.InitialDirectory = m_sourceFolder;
            if (ofd.ShowDialog() != DialogResult.OK) return;
            String csvFile = ofd.FileName;
            ofd = null;

            //указать каталог с Хранилищем
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrEmpty(m_storageFolder) && Directory.Exists(m_storageFolder))
                fb.SelectedPath = m_storageFolder;
            fb.Description = "Выберите каталог Хранилища";
            if (fb.ShowDialog() != DialogResult.OK) return;
            m_storageFolder = fb.SelectedPath;
            fb = null;
            //Добавить даташиты в Хранилище File-Description-Title
            AddDocumentsFromCsvOnFDTformat(this.m_storageFolder, csvFile);

            return;
        }
        
        /// <summary>
        /// NT-Добавить даташиты  в указанное Хранилище из CSV-файла формата File-Description-Title 
        /// </summary>
        /// <param name="storagePath">Путь к папке Хранилища</param>
        /// <param name="csvFile">Путь к CSV-файлу описания документов в FDT формате</param>
        private void AddDatasheetFromCsvOnFDTformat(string storagePath, string csvFile)
        {

            
            Manager man = null;
            try
            {
                //1 загрузить все записи из ксв-файла в память ( и распарсить их), чтобы убедиться, что нет ошибок в исходных файлах.
                //- убедиться, что исходные файлы документов существуют.
                //загрузить данные записей из ксв-файла как массивы String[3], по одному массиву на одну деталь, а деталей в строке может быть несколько
                List<String[]> docsar = loadFDTAsArrays(csvFile, true);//несколько Сущностей в одном названии разделены пробелом
                //- вывести количество найденных файлов документов
                //если файлов нет, вывести сообщение и закончить работу.
                if (docsar.Count == 0)
                {
                    AddLogString("Нет даташитов для добавления");
                    AddLogString("Добавление даташитов завершено");
                    return;
                }
                //вывести количество найденных файлов книг
                AddLogString("Найдено даташитов: " + docsar.Count);

                //2 Добавить документы в Хранилище
                //открыть хранилище
                man = new Manager();
                man.Open(storagePath);
                AddLogString("Хранилище открыто: " + storagePath);

                //подготовить файлы для загрузки
                //описание надо бы вывести на экран, чтобы убедиться в правильной его кодировке
                String BookCategory = man.StorageBaseClass;//класс документа будет здесь храниться на время операции
                this.m_ShowDescriptionForm = false;

                foreach (String[] sar in docsar)
                {
                    String title = sar[2];
                    String description = sar[1];
                    String docFile = sar[0];
                    //тут управляем показом формы при добавлении.
                    if (this.m_ShowDescriptionForm == true)
                    {
                        //показать диалог добавления одной книги
                        //... тут создать диалог свойств книги и показать его
                        //и сохранить новую категорию в переменную
                        //и добавить одобренную пользователем книгу в хранилище
                        //show dialog for adding
                        BookInfo bi = new BookInfo();
                        bi.m_picturePath = String.Empty;
                        bi.m_title1 = title;
                        bi.m_title2 = String.Empty;//Нет второго вартанта названия
                        bi.m_description = description;
                        bi.m_category = String.Copy(BookCategory);

                        if (bi.ShowDialog() == DialogResult.OK)
                        {
                            //add book to storage
                            BookCategory = bi.m_category;
                            AddEntity(man, false, BookCategory, bi.m_title1, bi.m_description, docFile, String.Empty);
                            AddLogString("Добавлен: " + docFile);
                        }
                        else
                            AddLogString("Пропущен пользователем: " + docFile);
                    }
                    else
                    {
                        //automatic added without dialog box
                        AddEntity(man, false, BookCategory, title, description, docFile, String.Empty);
                        AddLogString("Добавлен: " + docFile);

                    }
                    //обновить окно приложения
                    Application.DoEvents();
                }
                //завершить операцию
                man.Close();
                AddLogString("Хранилище закрыто");
                AddLogString("Добавление даташитов завершено");
                AddLogString("");
                AddLogString("");
            }
            catch (Exception ex)
            {
                AddLogString("Ошибка: " + ex.ToString());
                if (man != null)
                    man.Close();
            }
            return;
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        private List<string[]> loadFDTAsArrays(string csvFile, bool НазваниеЭтоСписокДеталей)
        {


            //1 загрузить все записи из ксв-файла в память ( и распарсить их), чтобы убедиться, что нет ошибок в исходных файлах.
            //- убедиться, что исходные файлы документов существуют.
            //загрузить данные записей из ксв-файла как массивы String[3], по одному массиву на одну деталь, а деталей в строке может быть несколько
            //Массивы тут только потому, что не хочется создавать класс для хранеия данных записи о сущностях для одного лишь только временного конвертера 

            Char[] CsvDelimiter = new Char[] { ';' };
            Char[] TitleDelimiter = new Char[] { ' ' };
            StreamReader sr = new StreamReader(csvFile, Encoding.GetEncoding(1251));
            int rowcounter = 0;
            //create local entity record list
            List<string[]> result = new List<String[]>();
            //read lines
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                rowcounter++;
                line = line.Trim();
                //check empty lines
                if (String.IsNullOrEmpty(line))
                    continue;
                //parse lines
                String[] sar = line.Split(CsvDelimiter, StringSplitOptions.None);
                if (sar.Length < 3)
                    throw new FormatException(String.Format("Неправильный формат входного CSV-файла: строка {0}: {1}", rowcounter, line));
                else
                {
                    //проверить что файл существует и выдать исключение
                    String docfile = sar[0].Trim();
                    if (!File.Exists(docfile))
                        throw new FileNotFoundException("Файл документа не найден", docfile);
                    //разобрать строку названия на отдельные названия деталей
                    String descr = sar[1].Trim().Replace("  ", " ").Replace("  ", " ");
                    String titles = sar[2].Trim().Replace(',', ' ').Replace("  ", " ").Replace("  ", " ");
                    if (НазваниеЭтоСписокДеталей == true)
                    {
                        String[] tar = titles.Split(TitleDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        foreach (String partname in tar)
                        {
                            String parttitle = partname.Trim();
                            //если название все-таки пустое, или короче двух символов, просто игнорируем его
                            if (parttitle.Length < 2)
                                continue;
                            //создаем массв значений и добавляем его в выходной список 
                            String[] ssr = new String[3];
                            ssr[0] = docfile;//путь файла
                            ssr[1] = descr;//описание
                            ssr[2] = parttitle;//название одной детали
                            result.Add(ssr);
                        }
                    }
                    else
                    {
                        //название это название одного документа
                        //создаем массив значений и добавляем его в выходной список 
                        String[] ssr = new String[3];
                        ssr[0] = docfile;//путь файла
                        ssr[1] = descr;//описание
                        ssr[2] = titles;//название одной детали
                        result.Add(ssr);
                    }
                }
            }//end while
            sr.Close();

            return result;
        }




        /// <summary>
        /// NT-Добавить документы в указанное Хранилище из CSV-файла формата File-Description-Title 
        /// </summary>
        /// <param name="storagePath">Путь к папке Хранилища</param>
        /// <param name="csvFile">Путь к CSV-файлу описания документов в FDT формате</param>
        private void AddDocumentsFromCsvOnFDTformat(string storagePath, string csvFile)
        {
            
            Manager man = null;
            try
            {
                //1 загрузить все записи из ксв-файла в память ( и распарсить их), чтобы убедиться, что нет ошибок в исходных файлах.
                //- убедиться, что исходные файлы документов существуют.
                //загрузить данные записей из ксв-файла как массивы String[3], по одному массиву на одну деталь, а деталей в строке может быть несколько
                List<String[]> docsar = loadFDTAsArrays(csvFile, false);//один документ на название
                //- вывести количество найденных файлов документов
                //если файлов нет, вывести сообщение и закончить работу.
                if (docsar.Count == 0)
                {
                    AddLogString("Нет документов для добавления");
                    AddLogString("Добавление документов завершено");
                    return;
                }
                //вывести количество найденных файлов книг
                AddLogString("Найдено документов: " + docsar.Count);

                //2 Добавить документы в Хранилище
                //открыть хранилище
                man = new Manager();
                man.Open(storagePath);
                AddLogString("Хранилище открыто: " + storagePath);

                //подготовить файлы для загрузки
                //описание надо вывести на экран, чтобы убедиться в правильной его кодировке
                String BookCategory = "";//класс документа будет здесь храниться на время операции

                foreach (String[] sar in docsar)
                {
                    String title = sar[2];
                    String description = sar[1];
                    String docFile = sar[0];
                    //тут обязательно показываем форму при добавлении.
                        //показать диалог добавления одной книги
                        //... тут создать диалог свойств книги и показать его
                        //и сохранить новую категорию в переменную
                        //и добавить одобренную пользователем книгу в хранилище
                        //show dialog for adding
                        BookInfo bi = new BookInfo();
                        bi.m_picturePath = String.Empty;
                        bi.m_title1 = title;
                        bi.m_title2 = String.Empty;//Нет второго варианта названия
                        bi.m_description = description;
                        bi.m_category = String.Copy(BookCategory);

                        if (bi.ShowDialog() == DialogResult.OK)
                        {
                            //add book to storage
                            BookCategory = bi.m_category;
                            AddEntity(man, false, BookCategory, bi.m_title1, bi.m_description, docFile, String.Empty);
                            AddLogString("Добавлен: " + docFile);
                        }
                        else
                            AddLogString("Пропущен пользователем: " + docFile);

                    //обновить окно приложения
                    Application.DoEvents();
                }
                //завершить операцию
                man.Close();
                AddLogString("Хранилище закрыто");
                AddLogString("Добавление документов завершено");
                AddLogString("");
                AddLogString("");
            }
            catch (Exception ex)
            {
                AddLogString("Ошибка: " + ex.ToString());
                if (man != null)
                    man.Close();
            }
            return;
        }

        #endregion
        /// <summary>
        /// Очистить окно лога чтобы повысить скорость на больших количествах файлов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void очиститьОкноЛогаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Очистить окно лога чтобы повысить скорость на больших количествах файлов
            this.textBox_Log.Clear();
            AddLogString("Strings removed by User Clear command.");
            
            return;
        }







    }
}
