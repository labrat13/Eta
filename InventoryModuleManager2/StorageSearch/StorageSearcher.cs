using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using InventoryModuleManager2;
using System.Xml;
using System.Collections.Specialized;
using System.Drawing;
using InventoryModuleManager2.ClassificationEntity;

namespace StorageSearch
{
    /// <summary>
    /// Выполняет поиск в хранилищах и все нужные для этого работы
    /// </summary>
    internal class StorageSearcher
    {
        /// <summary>
        /// Название корневой ноды деревьев мест и классов
        /// </summary>
        internal const string RootNodeTitle = "All";


        /// <summary>
        /// Список последних использованных паттернов поиска
        /// </summary>
        private List<String> m_lastSearchPatterns;

        /// <summary>
        /// Список мест поиска Хранилищ, определенный пользователем.
        /// </summary>
        internal List<String> m_searchPlacesUserDefined;

        /// <summary>
        /// Список путей Хранилищ, найденных при сканировании 
        /// определенных пользователем мест поиска. Для ускорения поиска.
        /// </summary>
        private List<String> m_storagePathes;

        /// <summary>
        /// Список классов, извлеченных из хранилищ при просмотре мест поиска
        /// </summary>
        private List<String> m_storageClasses;

        public StorageSearcher()
        {
            this.m_lastSearchPatterns = new List<string>();
            this.m_searchPlacesUserDefined = new List<string>();
            this.m_storageClasses = new List<string>();
            this.m_storagePathes = new List<string>();

        }


        /// <summary>
        /// Получить список последних использованных паттернов поиска
        /// </summary>
        public String[] LastQueries
        {
            get { return m_lastSearchPatterns.ToArray(); }
        }

        #region *** Search functions ***

        /// <summary>
        /// NR-Искать по запросу
        /// </summary>
        /// <param name="query">Текст запроса</param>
        /// <param name="fields">Состояние чекбоксов - полей в которых надо искать</param>
        /// <param name="storages">Массив путей хранилищ, в которых надо искать</param>
        /// <param name="classes">Массив классов, которые надо учитывать при поиске</param>
        /// <returns></returns>
        internal List<ResultItem> Search(String query, SearchCheckboxStates fields, String storage, String[] classes)
        {
            //TODO: add code here    
            //добавить запрос в список последних запросов, если его там еще нет.
            //а если есть, то переместить на верх списка, чтобы он оказался в видимой строке комбобокса - он не окажется там!

            //Если стоит флаг SearchCheckboxStates.Previous, искать не в хранилищах, а в предыдущих результатах.
            //А если этих результатов нет, то искать в хранилищах. 
            //поэтому предыдущие результаты придется хранить в памяти, даже если их много.
            //поэтому надо отслеживать объем памяти до и после поиска, чтобы не занимать много этими результатами.

            //кстати о результатах: если хранилище закрыто, толку от объектов никакого. 
            //Значит, надо либо хранить ссылки на сущности хранилищ, и потом заново их получать.
            //Либо надо повторить запрос и в его результатах сразу уже искать новые. Но это тупик.
            //Поэтому будем как-то записывать ссылки на сущности хранилищ, а потом в них искать.
            //А можно не так - можно хранить данные сущностей, и в них искать.
            //Но это фигня - пользователю нужно показать карточку сущности, и тут ничего не получится, кроме как открыть хранилище по ссылке на сущность.

            //TODO: Короче, тут у меня бардак в проекте - я разрабатываю концепцию прямо в процессе написания кода, без предварительного понимания что вообще делать.
            //Это генерация мусора, проект нежизнеспособен из-за этого. Да еще прямо в коде функций обсуждение - надо останавливаться и все это переделывать, перепроектировать.
            Manager man = null;

            bool searchInTitle = ((fields & SearchCheckboxStates.Title) != SearchCheckboxStates.None);
            bool searchInDescr = ((fields & SearchCheckboxStates.Descr) != SearchCheckboxStates.None);
            bool searchInDocDescr = ((fields & SearchCheckboxStates.DocDescr) != SearchCheckboxStates.None);
            bool searchInPicDescr = ((fields & SearchCheckboxStates.PicDescr) != SearchCheckboxStates.None);

            List<ResultItem> result = new List<ResultItem>();
            try
            {
                man = new Manager();
                man.Open(storage);
                //далее работаем с хранилищем
                if ((searchInDocDescr == false) && (searchInPicDescr == false))
                {
                    //искать по LIKE
                    EntityRecord[] records = man.FindEntity(query, searchInTitle, searchInDescr, false, 4096);
                    //TODO: здесь отобрать результаты по требуемым классам
                    CreateResultItems(result, records);
                    records = null;
                }
                else
                {
                    //искать полным перебором элементов
                    EntityRecord[] records = SearchFullEnumeration(query, man, classes, searchInTitle, searchInDescr, searchInDocDescr, searchInPicDescr);
                    CreateResultItems(result, records);
                    records = null;
                }
                //close storage
                man.Close();
            }
            catch (Exception ex)
            {
                man.Close();
                result = null;
            }

            return result;
        }
        /// <summary>
        /// NT-Превратить записи сущностей в результаты поиска и добавить в список результатов
        /// </summary>
        /// <param name="result">Список результатов поиска</param>
        /// <param name="records">Массив записей сущностей</param>
        private void CreateResultItems(List<ResultItem> result, EntityRecord[] records)
        {
            foreach (EntityRecord er in records)
                result.Add(new ResultItem(er));

            return;
        }
        /// <summary>
        /// NR-Искать полным перебором сущностей Хранилища, чисто кодом.
        /// </summary>
        /// <param name="query">Текст запроса</param>
        /// <param name="man">Объект менеджера Хранилища</param>
        /// <param name="classes">Массив названий классов</param>
        /// <param name="searchInTitle">Флаг Искать в названии</param>
        /// <param name="searchInDescr">Флаг Искать в описании</param>
        /// <param name="searchInDocDescr">Флаг Искать в описании документа</param>
        /// <param name="searchInPicDescr">Флаг Искать в описании изображения</param>
        /// <returns>Возвращает список найденных сущностей</returns>
        private static EntityRecord[] SearchFullEnumeration(String query, Manager man, string[] classes, bool searchInTitle, bool searchInDescr, bool searchInDocDescr, bool searchInPicDescr)
        {
            List<EntityRecord> results = new List<EntityRecord>();
            foreach (EntityRecord er in man.Entities)
            {
                //TODO: здесь отобрать результаты по требуемым классам
                if (searchInTitle)
                {
                    //if found, add to results list and "continue"
                    if (SearchInString(query, er.Title) == true)
                    {
                        results.Add(er);
                        continue;
                    }
                }
                if (searchInDescr)
                {
                    //if found, add to results list and "continue"
                    if (SearchInString(query, er.Description) == true)
                    {
                        results.Add(er);
                        continue;
                    }
                }
                if (searchInDocDescr)
                {
                    //искать в описании документа, но возвращать сущность с этим документом.
                    //if found, add to results list and "continue"
                    if (er.Document != null)
                        if (SearchInString(query, er.Document.Description) == true)
                        {
                            results.Add(er);
                            continue;
                        }
                }
                if (searchInPicDescr)
                {
                    //искать в описании изображения, но возвращать сущность с этим изображением.
                    //if found, add to results list and "continue"
                    if (er.Document != null)
                        if (SearchInString(query, er.Picture.Description) == true)
                        {
                            results.Add(er);
                            continue;
                        }
                }
            }
            return results.ToArray();
        }

        /// <summary>
        /// NT-Добавить результаты поиска в листвиев.
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="results"></param>
        internal void AddResults(ListView listView, List<ResultItem> results)
        {
            if (results.Count == 0) return;
            listView.BeginUpdate();
            foreach (ResultItem ri in results)
            {
                //create list item
                ListViewItem it = new ListViewItem();
                it.Text = ri.Title;
                it.ImageIndex = 0;
                it.ToolTipText = ri.ToString();
                it.SubItems.Add(ri.Category.ClassPath);
                it.SubItems.Add(makeDescriptionString(ri.Description));
                //показать что есть присоединенный документ
                string V = " V ";
                if(!ri.HaveDocument)
                    V = String.Empty;
                it.SubItems.Add(V);
                //показать что есть присоединенное изображение
                V = " V ";
                if (!ri.HavePicture)
                    V = String.Empty;
                it.SubItems.Add(V);
                //вставить ссылку на объект результата поиска для последуюшего использования.
                it.Tag = ri; 
                //добавить итем в листвиев.
                listView.Items.Add(it);
            }
            //сортировать список
            listView.Sort();

            listView.EndUpdate();

            return;
        }
        /// <summary>
        /// NT-Подготовить строку для показа в списке результатов
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string makeDescriptionString(string p)
        {
            //limit length
            if (p.Length > 128)
                p = p.Substring(0, 128);
            //remove CR LF
            p = p.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').Trim();
            //remove space chains
            p = p.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");

            return p;
        }

        /// <summary>
        /// NR-Искать в результатах предыдущего поиска, в листвиеве.
        /// </summary>
        /// <param name="lw">ListView control</param>
        /// <param name="query">Текст запроса</param>
        /// <param name="flags">Флаги поиска</param>
        /// <param name="classes">Классы для поиска</param>
        internal void SearchInResults(ListView lw, string query, SearchCheckboxStates flags, string[] classes)
        {
            bool searchInTitle = ((flags & SearchCheckboxStates.Title) != SearchCheckboxStates.None);
            bool searchInDescr = ((flags & SearchCheckboxStates.Descr) != SearchCheckboxStates.None);
            bool searchInDocDescr = ((flags & SearchCheckboxStates.DocDescr) != SearchCheckboxStates.None);
            bool searchInPicDescr = ((flags & SearchCheckboxStates.PicDescr) != SearchCheckboxStates.None);

            //get from listview to list
            List<ListViewItem> results = new List<ListViewItem>();
            foreach (ListViewItem lvi in lw.Items)
            {
                if (lvi.Tag == null) continue;

                ResultItem ri = (ResultItem)lvi.Tag;
                //TODO: filter items by query and classes here
                //TODO: здесь отобрать результаты по требуемым классам
                if (searchInTitle)
                {
                    //if found, add to results list and "continue"
                    if (SearchInString(query, ri.Title) == true)
                    {
                        results.Add(lvi);
                        continue;
                    }
                }
                if (searchInDescr)
                {
                    //if found, add to results list and "continue"
                    if (SearchInString(query, ri.Description) == true)
                    {
                        results.Add(lvi);
                        continue;
                    }
                }
                if (searchInDocDescr)
                {
                    //искать в описании документа, но возвращать сущность с этим документом.
                    //if found, add to results list and "continue"
                    if (SearchInString(query, ri.DocDescription) == true)
                    {
                        results.Add(lvi);
                        continue;
                    }
                }
                if (searchInPicDescr)
                {
                    //искать в описании изображения, но возвращать сущность с этим изображением.
                    //if found, add to results list and "continue"
                    if (SearchInString(query, ri.PicDescription) == true)
                    {
                        results.Add(lvi);
                        continue;
                    }
                }
            }
            //insert results back to listview
            lw.BeginUpdate();
            lw.Clear();
            lw.Items.Clear();
            lw.Items.AddRange(results.ToArray());
            lw.Sort();
            lw.EndUpdate();

            return;
        }
        /// <summary>
        /// NR-Искать запрос в тексте
        /// </summary>
        /// <param name="query">Текст запроса</param>
        /// <param name="text">Текст для поиска</param>
        /// <returns></returns>
        private static bool SearchInString(string query, string text)
        {
            throw new NotImplementedException();//TODO: add code here
        }


        /// <summary>
        /// NT-Загрузить список последних паттернов из настроек приложения
        /// </summary>
        /// <param name="p">Список с последними запросами пользователя</param>
        internal void LoadLastUsedQueryies(StringCollection p)
        {
            foreach (String s in p)
                AddQueryToListUsedQueries(s);
            return;
        }

        /// <summary>
        /// NT-Добавить запрос в список запросов
        /// </summary>
        /// <remarks>Эта функция регулирует длину списка запросов и отсутствие дубликатов.
        /// </remarks>
        /// <param name="query">Строка запроса</param>
        private void AddQueryToListUsedQueries(String query)
        {
            //check query dublicate
            //если дубликат уже есть, то старый удалить а новый добавить в конец списка
            int index = m_lastSearchPatterns.IndexOf(query);
            if (index != -1)
                m_lastSearchPatterns.RemoveAt(index);
            m_lastSearchPatterns.Add(query);
            //ограничить длину 8 последними запросами
            //удалить самые старые, то есть, самые первые элементы
            if (this.m_lastSearchPatterns.Count > 8)
                m_lastSearchPatterns.RemoveAt(0);

            return;
        }



        #endregion

        #region *** Функции дерева мест ***
        private const int NodeIconError = 0;
        private const int NodeIconFolder = 1;
        private const int NodeIconFolderOpen = 2;
        private const int NodeIconRoot = 3;
        private const int NodeIconDb = 4;
        private const int NodeIconClass = 5;



        /// <summary>
        /// NT-Загрузить список мест из настроек приложения.
        /// Потом обновить дерево классов вызовом соответствующей функции.
        /// </summary>
        /// <param name="sc">Коллекция мест</param>
        /// <param name="tw">Контрол дерева мест</param>
        internal void LoadPlacesTree(StringCollection sc, System.Windows.Forms.TreeView tw)
        {
            //get user-defined places from xml string
            LoadPlacesTree(this.GetStringArrayFromSpecializeString(sc), tw);
            return;
        }
        /// <summary>
        /// NT-Загрузить список мест
        /// Потом обновить дерево классов вызовом соответствующей функции.
        /// </summary>
        /// <param name="places">Массив путей мест</param>
        /// <param name="tw">Контрол дерева мест</param>
        internal void LoadPlacesTree(string[] places, TreeView tw)
        {
            //TODO: add code here
            //создать заново список мест, загружая его данные из настроек
            m_searchPlacesUserDefined.Clear();
            m_storagePathes.Clear();
            //пользователь ввел пути - некоторые специально дублируются для удобного управления
            //теперь эти пути надо показать как дерево.
            //соотнести с реальной файловой системой
            //часть путей может отсутствовать в файловой системе - показывать это значком ошибки.
            //хранилища находить и показывать в дереве.
            //тут же и классы из хранилищ собирать для дерева классов.

            //В целом, это сканер для поиска хранилищ и классов в них. 
            //Ему надо скармливать пути и он в них ищет хранилища.
            //Но сначала пути надо проверить - что они существуют и не перекрываются.
            //а потом этот сканер заполняет m_storagePathes путями хранилищ.

            //вот это сканирование будет много времени занимать наверно при запуске приложения.

            //1 создаем корневую ноду мест
            TreeNode root = new TreeNode(RootNodeTitle, NodeIconRoot, NodeIconRoot);
            tw.Nodes.Clear();
            tw.Nodes.Add(root);
            //2 создаем дерево рекурсивно обходя каждый из путей
            // и заодно создаем дерево классов и списки хранилищ и классов
            foreach (String s in places)
            {
                this.addNewPlace(s, tw);
            }
            //expand all tree
            tw.CollapseAll();//TODO: Тут открывать последовательно вниз ноды, пока у них менее 2 дочерних нод. 
            //3 return
            return;
        }
        /// <summary>
        /// NT-Добавить место в список пользовательских мест
        /// </summary>
        /// <param name="newPlacePath"></param>
        /// <param name="tp"></param>
        /// <param name="tc"></param>
        internal void addNewPlace(string place, TreeView tp)
        {
            //если место уже в списке, выходим
            if (m_searchPlacesUserDefined.Contains(place))
                return;
            //получить корневую ноду из контрола дерева мест
            TreeNode root = null;
            foreach (TreeNode t in tp.Nodes)//TODO: Этот код надо обдумать! Он непонятно почему такой.
                if (t.Text == RootNodeTitle)
                    root = t;
            //сканировать путь и найденные в нем хранилища добавить в список хранилищ 
            //и извлечь из найденных хранилищ классы и добавить их в список классов.
            //дерево классов будем отдельно обновлять из списка классов.
            tp.BeginUpdate();
            root.Nodes.Add(GetPlaceNodes(place));
            tp.EndUpdate();

            //добавить путь места в список мест
            m_searchPlacesUserDefined.Add(place);
        }

        /// <summary>
        /// NT-рекурсивно собрать ноды с хранилищами в дерево
        /// </summary>
        /// <param name="path">Путь к начальному каталогу</param>
        /// <returns></returns>
        private TreeNode GetPlaceNodes(string path)
        {
            //получить нормальный путь
            String p = Path.GetFullPath(path);
            //если каталог не существует, вернуть ноду ошибки
            if (!Directory.Exists(p))
                return CreateErrorNode(p);
            //если каталог существует, начать обход каталога
            TreeNode tn = recursiveGetPlaceNodes(p);
            //если папка не содержит хранилищ, ее все равно надо показать
            if (tn == null)
                tn = new TreeNode("", NodeIconFolderOpen, NodeIconFolderOpen);
            //показать полный путь к папке только для верхней ноды рекурсии
            tn.Text = p;
            return tn;
        }
        /// <summary>
        /// NT-рекурсивно собрать ноды с хранилищами в дерево
        /// </summary>
        /// <param name="p">Путь к каталогу</param>
        /// <returns>Возвращает ноду каталога с субнодами или null, если в каталоге нет хранилищ.</returns>
        private TreeNode recursiveGetPlaceNodes(string p)
        {
            string name = Path.GetFileName(p);
            TreeNode tn;
            //если это папка хранилища, создать ноду хранилища
            if (Manager.IsStorageFolder(p))
            {
                tn = new TreeNode(name, NodeIconDb, NodeIconDb);
                //вписать путь к хранилищу в таг ноды
                tn.Tag = p;
                //вписать путь к хранилищу в список хранилищ менеджера, если еще там его нет
                //и извлечь классы сущностей сразу, заодно проверить исправность хранилища.
                bool success = AddStoragePathToList(p);
                //если с хранилищем произошла ошибка, показать иконку ошибки
                if (!success)
                {
                    tn.SelectedImageIndex = NodeIconError;
                    tn.ImageIndex = NodeIconError;
                    //неисправные хранилища не должны иметь пути в теге, чтобы не попадать в поиск.
                    tn.Tag = null;
                }
                //вернуть ноду хранилища
                return tn;
            }
            //иначе создать ноду папки
            tn = new TreeNode(name, NodeIconFolder, NodeIconFolder);
            string[] dirs = Directory.GetDirectories(p);
            foreach (string d in dirs)
            {
                TreeNode ctn = recursiveGetPlaceNodes(d);
                if (ctn != null)
                    tn.Nodes.Add(ctn);
            }
            //если дочерних нод-хранилищ нет, то и эту ноду удалить
            if (tn.Nodes.Count == 0)
                tn = null;
            return tn;
        }
        /// <summary>
        /// NT-Вписать путь к хранилищу в список хранилищ менеджера
        /// И прочитать хранилище чтобы извлечь классы
        /// </summary>
        /// <param name="p">путь к хранилищу</param>
        private bool AddStoragePathToList(string path)
        {
            if (!m_storagePathes.Contains(path))
            {
                //TODO: тут открыть хранилище и извлечь классы в список классов, но пока не в дерево классов
                //классы надо сразу привести к нормальной форме, то есть вида Предмет.Класс.Класс.

                //если хранилище не открывается, или произошла любая ошибка, вернуть false
                List<string> classes = GetClassesFromStorage(path);
                if (classes == null)
                    return false;
                //И если класс уже есть в списке, не добавлять его.
                //TODO: Тут для списка классов нужен отдельный объект-словарь. А сейчас здесь не проверяется регистр символов.
                foreach (String s in classes)
                    if (!m_storageClasses.Contains(s))
                        m_storageClasses.Add(s);

                //если хранилище открывается успешно, добавить его в список хранилищ
                m_storagePathes.Add(path);
            }
            return true;
        }

        /// <summary>
        /// NT-Извлечь из Хранилища список классов сущностей
        /// </summary>
        /// <param name="path">Путь к каталогу Хранилища</param>
        /// <returns>
        /// Вернуть null если хранилище не удалось открыть. 
        /// Иначе вернуть список классов, которые есть в Хранилище.
        /// </returns>
        private List<string> GetClassesFromStorage(string path)
        {
            List<string> result = null; // new List<string>();
            Manager man = null;
            try
            {
                //Извлечь из Хранилища список классов сущностей
                man = new Manager();
                man.Open(path);

                //Это теперь делается в самом менеджере - он возвращает полные пути сам.

                //далее работаем с хранилищем
                //получить базовый класс Хранилища
                //StorageInfo si = man.GetStorageInfo();
                //String storageType = si.StorageType;//Радиодеталь<Документ, Изображение>
                //String qualifName = si.QualifiedName;//Предмет.Радиодеталь - не использовать.
                //si = null;
                //тут надо разобрать типы хранилища и извлечь из него классы.
                //парсер для этого где-то у меня есть в проектах, но я не помню где
                //String[] baseClasses = ClassItem.ParseClassPathFromClassExpression(storageType);//THEME: QNAME - переработать все места по этой теме
                //теперь надо получить список классов из БД Хранилища
                //List<String> classes = man.GetEntityCategoryNames();
                //теперь надо привести все классы к нормальной форме
                //для этого StorageBaseClass спереди прицепляем к каждому классу, если он его уже не содержит.
                //так, чтобы пути были правильными. А это непросто, пока в этих данных бардак в хранилищах.
                //Но там это просто можно исправить, и руководство себе написать, что как и зачем должно быть в этих именах. 
                //вот это преобразование путей надо сделать в самом движке. Но потом, когда все это уже будет отлажено здесь.

                ////берем последний класс из базовых
                //String lastBaseClass = baseClasses[baseClasses.Length - 1];

                ////ищем его в классах здесь
                //String basepart = String.Join("::", baseClasses) + "::";//loop optimization
                //foreach (String s in classes)
                //{
                //    //разберем на имена классов
                //    String[] sar = ClassItem.SplitClassPath(s);
                //    String tmp = null;
                //    //если первый класс это последний базовый класс, то собираем цепочку без первого класса
                //    if (String.Equals(lastBaseClass, sar[0], StringComparison.OrdinalIgnoreCase))
                //    {
                //        tmp = basepart + String.Join("::", sar, 1, sar.Length - 1);
                //    }
                //    //иначе собираем цепочку простым соединением строк
                //    else
                //    {
                //        tmp = basepart + String.Join("::", sar);
                //    }
                //    //отправляем в список классов
                //    result.Add(tmp);
                //}

                result = man.GetEntityCategoryNames();

                //close storage
                man.Close();
            }
            catch (Exception ex)
            {
                man.Close();
                result = null;
            }

            return result;
        }

        private bool IsStorageFolder(string path)
        {
            //критерии:
            //папка должна содержать файл "description.xml"
            //папка должна содержать файл db.mdb
            //папка должна содержать папки docs pics
            //файл "description.xml" должен читаться без проблем
            //TODO: переместить функцию после тестирования в класс Manager движка и заменить эти строковые значения на константы движка.
            String p = Path.Combine(path, "docs");
            if (!Directory.Exists(p)) return false;
            p = Path.Combine(path, "pics");
            if (!Directory.Exists(p)) return false;
            p = Path.Combine(path, "db.mdb");
            if (!File.Exists(p)) return false;
            p = Path.Combine(path, "description.xml");
            if (!File.Exists(p)) return false;
            //try load descr file
            bool result = true;
            try
            {
                StorageInfo si = StorageInfo.GetInfo(path);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// NT-Создает ноду ошибки
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private TreeNode CreateErrorNode(String name)
        {
            TreeNode tn = new TreeNode(name, NodeIconError, NodeIconError);
            return tn;
        }

        #endregion

        #region *** Функции дерева классов ***

        /// <summary>
        /// NT-Загрузить список классов из настроек приложения
        /// </summary>
        /// <param name="p">XML-строка со списком классов</param>
        /// <param name="treeView">Контрол дерева классов</param>
        internal void LoadClassesTree(TreeView tc)
        {
            //1 создаем корневую ноду классов
            TreeNode root = new TreeNode(RootNodeTitle, NodeIconRoot, NodeIconRoot);
            tc.Nodes.Clear();
            tc.Nodes.Add(root);
            //классы из списка найденных классов должны уже быть в нормальной форме.
            //и каждый в единственном экземпляре
            //Надо их отобразить в дереве классов - собрать из списка дерево.
            //1) сортировать список. Дерево нод автосортировать нельзя из-за постобработки - надо соблюдать формат. 
            //а по сортированному списку и ноды создаются уже сортированными.
            this.m_storageClasses.Sort();
            tc.BeginUpdate();
            //2) каждую строку списка превратить в массив классов
            foreach (String s in m_storageClasses)
            {
                String[] classes = ClassItem.SplitClassPath(s);
                //Тут превратить рекурсию в цикл - так проще, когда путь только один.
                TreeNode tparent = root;
                for (int i = 0; i < classes.Length; i++)
                {
                    //первый класс это первая нода дерева. Ищем ее в дереве сразу после корня.
                    TreeNode tchild = null;
                    foreach (TreeNode tnn in tparent.Nodes)
                    {
                        if (String.Equals(tnn.Text, classes[i], StringComparison.OrdinalIgnoreCase))
                        {
                            tchild = tnn;
                            break;
                        }
                    }
                    if (tchild == null)
                    {
                        //если не нашли, создаем новую, и в ней новые сразу без проверок, так до конца массива классов.
                        for (int j = i; j < classes.Length; j++)
                        {
                            tchild = new TreeNode(classes[j], NodeIconClass, NodeIconClass);
                            tparent.Nodes.Add(tchild);
                            tparent = tchild;
                        }
                        //все ноды были добавлены - прервать цикл
                        break;
                    }
                    else
                    {
                        //переходим к найденной дочерней ноде, делаем ее текущей родительской чтобы искать в ней.
                        tparent = tchild;
                    }
                }
            }
            //постобработка - вставить ноды собственно классов для выборки их
            foreach (TreeNode nd in root.Nodes)
                PostClassing(nd);

            //закончить обновление дерева
            tc.EndUpdate();
            //TODO: expand classes tree here - only if needed!!!
            //tc.ExpandAll();
            return;
        }



        /// <summary>
        /// NT-постобработка дерева классов. Имена классов из нод внести в поля Tag.
        /// Добавить в дерево ноды экземпляров классов визуально выделенные.
        /// Чтобы можно было их выбирать для поиска исключительно, бех их субклассов.
        /// </summary>
        /// <param name="node"></param>
        private void PostClassing(TreeNode node)
        {
            //рекурсивная функция
            //если это конечная нода, вставить ей в тег ее название класса
            //чтобы извлекать названия классов только из тегов и они там были чистые.
            if (node.Nodes.Count == 0)
            {
                node.Tag = node.Text;
                return;
            }
            //а если это узел, то тег не вставлять, а вставлять конечную ноду с тем же названием
            foreach (TreeNode tn in node.Nodes)
            {
                PostClassing(tn);
            }
            //insert new node
            TreeNode t = new TreeNode(node.Text, NodeIconClass, NodeIconClass);
            t.Tag = node.Text;
            node.Nodes.Insert(0, t);
            //rename current node
            node.Text = "[" + node.Text + "]";
            makeBoldNodeFont(node);

            return;
        }

        private Font NodeFont = null;
        /// <summary>
        /// NR-установить для ноды класса жирный шрифт
        /// </summary>
        /// <param name="node"></param>
        private void makeBoldNodeFont(TreeNode node)
        {
            if (node == null) return;
            //create node font bold
            if (NodeFont == null)
            {
                NodeFont = new Font(FontFamily.GenericSansSerif, 8.25F, FontStyle.Bold);
            }
            node.NodeFont = NodeFont;
            return;
        }

        #endregion

        /// <summary>
        /// NT-Получить список паттернов поиска как XML строку
        /// </summary>
        /// <returns></returns>
        internal StringCollection GetSearchPatternsCollection()
        {
            StringCollection sc = new StringCollection();
            sc.AddRange(this.m_lastSearchPatterns.ToArray());
            return sc;
        }
        /// <summary>
        /// NT-Получить список мест поиска хранилищ от пользователя как XML строку
        /// </summary>
        /// <returns></returns>
        internal StringCollection GetStoragePathesCollection()
        {
            StringCollection sc = new StringCollection();
            sc.AddRange(this.m_searchPlacesUserDefined.ToArray());
            return sc;
        }
        /// <summary>
        /// NT-Получить список классов сущностей Хранилищ как XML строку
        /// </summary>
        /// <returns></returns>
        internal StringCollection GetStorageClassesCollection()
        {
            StringCollection sc = new StringCollection();
            sc.AddRange(this.m_storageClasses.ToArray());
            return sc;
        }

        /// <summary>
        /// NT-Извлечь все строки переданного массива из XML-фрагмента.
        /// </summary>
        /// <param name="xml">Строка XML-формата, содержащая список строк</param>
        /// <returns>Возвращает массив строк</returns>
        private String[] GetStringArrayFromSpecializeString(StringCollection sc)
        {
            String[] sar = new string[sc.Count];
            for (int i = 0; i < sc.Count; i++)
                sar[i] = sc[i];
            return sar;
        }



        /// <summary>
        /// NR-Собрать список выбранных для поиска хранилищ.
        /// </summary>
        /// <param name="treeView"></param>
        /// <returns></returns>
        internal List<string> MakeSelectedStoragesList(TreeView tw)
        {
            //путь к хранилищу вписан в поле Tag ноды хранилища
            //Не надо проверять, что нода не ошибочная.
            List<String> results = new List<string>();
            recursiveGetSelectedStorages(results, tw.Nodes);

            return results;
        }

        private void recursiveGetSelectedStorages(List<string> results, TreeNodeCollection tnc)
        {
            foreach (TreeNode tn in tnc)
            {
                if ((tn.Checked == true) && (tn.Tag != null))
                {
                    String s = tn.Tag.ToString();
                    if (!results.Contains(s))
                        results.Add(s);
                }
                recursiveGetSelectedStorages(results, tn.Nodes);
            }
            return;
        }
        /// <summary>
        /// NR-Собрать список выбранных для поиска классов
        /// </summary>
        /// <param name="treeView"></param>
        /// <returns></returns>
        internal List<string> MakeSelectedClassesList(TreeView treeView)
        {
            //throw new NotImplementedException();//TODO: add code here

            //имя конечного класса вписано в тег. А вот путь не получится просто собрать из-за постобработки.
            //Он будет, во-первых, разделен / вместо ::
            //во-вторых, имена классов в []
            //в третьих, имена экземпляров классов и классов совпадают, не считая []
            //вот из этого надо восстановить имена классов?

            //а нельзя просто в тег их вписать сразу при сборке дерева?
            //--конечные классы можно, а промежуточные - как? Собирать на месте?

            //а как вообще потом их искать в хранилищах?
            //пока не придумал, но потом придумаю.
            //тогда сами эти классы оставить на потом, когда все остальное будет закончено.

            List<String> results = new List<string>();
            return results;
        }


        /// <summary>
        /// NT-Показать форму с сущностью
        /// </summary>
        /// <param name="link"></param>
        internal void ShowEntityForm(IWin32Window owner, ResultItem ri)
        {
            //1 распарсить ссылку, извлечь имя хранилища
            //String link = ri.Link;
            //LinkBuilder lb = new LinkBuilder(link);
            //String qname = lb.StorageQName;
            //2 найти и загрузить хранилище
            //у меня нет списка хранилищ по qname - есть только список путей хранилищ.
            //можно передавать сюда не линку, а объект записи с путем хранилища
            //или можно переделать список хранилищ так чтобы он и qname содержал.
            //- это если qname еще где-то используется
            String storagePath = ri.StoragePath;
            //3 получить объект записи сущности
            Manager man = new Manager();
            man.Open(storagePath);
            //отдаем ссылку в виде билдера, чтобы исключения формата ссылки остались тут а не в менеджере.
            EntityRecord entity = man.GetEntityByLink(new LinkBuilder(ri.Link));
            //4 показать форму
            EntityForm.ShowEntityForm(owner, entity);
            //5 записать изменения обратно в хранилище, если они были - пока не делаем это
            //-нет таких функций в менеджере и в адаптере БД пока.
            //TODO: Add code here...
            //6 закрыть хранилище
            man.Close();
            return;
        }
        /// <summary>
        /// NR-Вывести дерево классов в текстовый файл для анализа иерархии классов хранилищ.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tw"></param>
        internal void exportClassTree(string filename, TreeView tw)
        {
            //create writer
            StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8);
            ////если нода без дочерних нод, то выводим ее путь от корня 
            ////а если нет, то спускаемся ниже.
            //foreach(TreeNode tn in tw.Nodes)
            //    recursiveExportClassTree(tn, sw);

            this.m_storageClasses.Sort();
            foreach (String s in m_storageClasses)
                sw.WriteLine(s);

            sw.Close();
            return;
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="treeNodeCollection"></param>
        ///// <param name="sw"></param>
        //private void recursiveExportClassTree(TreeNode tn, StreamWriter sw)
        //{
        //    //если нода без дочерних нод, то выводим ее путь от корня 
        //    //а если нет, то спускаемся ниже.
        //    if (tn.Nodes.Count == 0)
        //    {
        //        sw.WriteLine(tn.FullPath);
        //    }
        //    else
        //    {
        //        foreach (TreeNode tnt in tn.Nodes )
        //            recursiveExportClassTree(tnt, sw);

        //    }
        //}




    }
}
