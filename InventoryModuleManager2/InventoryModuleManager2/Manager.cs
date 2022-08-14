using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;
using InventoryModuleManager2.ClassificationEntity;

namespace InventoryModuleManager2
{
    /// <summary>
    /// Менеджер Модуля свойств
    /// </summary>
    public class Manager
    {
        #region *** Переменные и константы ***

        /// <summary>
        /// Название файла описания хранилища
        /// </summary>
        internal const string DescriptionFileName = "description.xml";

        /// <summary>
        /// Контроллер архива файлов документов
        /// </summary>
        private ArchiveController m_docController;

        /// <summary>
        /// Контроллер архива файлов изображений
        /// </summary>
        private ArchiveController m_picController;

        /// <summary>
        /// Адаптер БД модуля свойств
        /// </summary>
        private DbAdapter m_db;

        /// <summary>
        /// Объект Сессия менеджера Хранилища
        /// </summary>
        private Session m_Session;

        /// <summary>
        /// путь к корневому каталогу хранилища.
        /// </summary>
        private string m_storageRootFolder;//TODO: переместить это поле в объект сеанса?
        /// <summary>
        /// флаг, что хранилище не поддерживает запись изменений.
        /// </summary>
        private bool m_ReadOnly;
        /// <summary>
        /// Квалифицированное имя открытого хранилища
        /// </summary>
        private string m_qualifiedStorageName;
        /// <summary>
        /// БазовыйКлассХранилища открытого хранилища
        /// </summary>
       private string m_storageBaseClass;//
        /// <summary>
        /// Название Хранилища для показа пользователю.
        /// </summary>
       private string m_storageTitle;
        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public Manager()
        {
            //инициализация переменных для нового сеанса производится в функции Open
            this.m_ReadOnly = false;
            this.m_qualifiedStorageName = String.Empty;
            this.m_storageBaseClass = String.Empty;
            this.m_storageRootFolder = String.Empty;
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        ~Manager()
        {

        }


        #region *** Проперти ***
        //TODO: после релиза: подумать как с этим быть.
        //не используется. Он должен по идее использоваться перед Open(), 
        //но сейчас используется временный каталог пользователя, по умолчанию.
        //пока имена файлов короткие, и дисковый том тот же, вроде все хорошо.
        //Это надо, если дисковый том с Хранилищем не С: - тогда перемещение файлов очень медленное
        //и вот тут можно установить другой временный каталог, на том же томе, что и хранилище.
        //тогда и перемещение файлов будет почти мгновенным.
        //- можно в Опен параметром его передавать, еще один Опен функцию сделать. 
        ///// <summary>
        ///// NT-Каталог временных файлов для работы Менеджера
        ///// </summary>
        //public String FolderForTemporaryFiles
        //{
        //    get
        //    {
        //        return m_Session.TempFolder;
        //    }
        //    set
        //    {
        //        m_Session.TempFolder = value;
        //    }
        //}

        /// <summary>
        /// Получить квалифицированное имя хранилища
        /// </summary>
        public String QualifiedName
        {
            get
            {
                return m_qualifiedStorageName;//
            }
        }

        /// <summary>
        /// Получить БазовыйКлассХранилища
        /// </summary>
        public String StorageBaseClass
        {
            get
            {
                return m_storageBaseClass;
            }
        }

        /// <summary>
        /// Корневой каталог Хранилища
        /// </summary>
        public string StorageFolder
        {
            get
            {
                return m_storageRootFolder;
            }
            //set
            //{
            //    m_storageRootFolder = value;
            //}
        }
        /// <summary>
        /// Название текущего открытого Хранилища для показа пользователю.
        /// </summary>
        public string StorageTitle
        {
            get { return m_storageTitle; }
        }

        /// <summary>
        /// Получить флаг что хранилище может изменяться
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return isReadOnly(this.m_storageRootFolder);
            }
        }



        /// <summary>
        /// NR-Енумератор Сущностей Хранилища
        /// </summary>
        public EntityCollection Entities
        {
            get
            {
                return new EntityCollection(this.m_db);
            }

        }

        /// <summary>
        /// NT-Енумератор Документов Хранилища
        /// </summary>
        public DocumentCollection Documents
        {
            get
            {
                return new DocumentCollection(DbAdapter.DocumentTableName, this.m_db);
            }

        }

        /// <summary>
        /// NT-Енумератор Изображений Хранилища
        /// </summary>
        public DocumentCollection Pictures
        {
            get
            {
                return new DocumentCollection(DbAdapter.PictureTableName, this.m_db);
            }
        }

        #endregion

        #region *** Основные Функции ***

        /// <summary>
        /// NT-Начать сеанс работы с Хранилищем
        /// </summary>
        /// <param name="storagePath">Путь к каталогу Хранилища</param>
        public void Open(String storagePath)
        {
            //TODO: проверить что этот каталог содержит хранилище.
            //но это долго будет, проще сразу попытаться открыть.

            //check disk writable
            this.m_storageRootFolder = storagePath;
            this.m_ReadOnly = isReadOnly(storagePath);

            //init session object
            this.m_Session = new Session();
            this.m_Session.StorageFolder = storagePath;
            m_Session.Open();//initialize session

            //init database
            string db = Path.Combine(storagePath, DbAdapter.DatabaseFileName);
            this.m_db = DbAdapter.SetupDbAdapter(this, db, this.m_ReadOnly);
            
            //init archive controllers
            this.m_docController = new ArchiveController(ArchiveController.DocumentsDir, m_Session, m_db);
            this.m_picController = new ArchiveController(ArchiveController.ImagesDir, m_Session, m_db);
            
            //get storage qualified name  
            this.m_qualifiedStorageName = this.m_db.getStorageInfoValue(StorageInfo.tagQualifiedName);
            //TODO: Это выдает исключение, когда вызывается при создании Хранилища
            //Поскольку этих двух значений нет еще в БД Хранилища, то вместо них возвращаются пустые строки
            this.m_storageBaseClass = ClassItem.GetClassPathFromClassExpression(this.m_db.getStorageInfoValue(StorageInfo.tagStorageType));

            this.m_storageTitle = this.m_db.getStorageInfoValue(StorageInfo.tagTitle);

            // add code here ?

            return;
        }
        /// <summary>
        /// NT-Открыть хранилище один раз при его создании, 
        /// когда БД еще не содержит всех нужных данных, а ее уже надо открыть.  
        /// </summary>
        /// <param name="si"></param>
        private void openIfNewOnly(StorageInfo si)
        {
            String storagePath = si.StoragePath;

            //check disk writable
            this.m_storageRootFolder = storagePath;
            this.m_ReadOnly = isReadOnly(storagePath);

            //init session object
            this.m_Session = new Session();
            this.m_Session.StorageFolder = storagePath;
            m_Session.Open();//initialize session

            //init database
            string db = Path.Combine(storagePath, DbAdapter.DatabaseFileName);
            this.m_db = DbAdapter.SetupDbAdapter(this, db, this.m_ReadOnly);

            //18112018: write values to new database  
            //TODO: я не хочу разбираться сейчас, что тут в каком порядке должно делаться
            //но это все же надо проектировать все, по-хорошему.
            this.m_db.storeStorageInfo(si);

            //init archive controllers
            this.m_docController = new ArchiveController(ArchiveController.DocumentsDir, m_Session, m_db);
            this.m_picController = new ArchiveController(ArchiveController.ImagesDir, m_Session, m_db);

            //get storage qualified name  
            this.m_qualifiedStorageName = this.m_db.getStorageInfoValue(StorageInfo.tagQualifiedName);
            //TODO: Это выдает исключение, когда вызывается при создании Хранилища
            //Поскольку этих двух значений нет еще в БД Хранилища, то вместо них возвращаются пустые строки
            this.m_storageBaseClass = ClassItem.GetClassPathFromClassExpression(this.m_db.getStorageInfoValue(StorageInfo.tagStorageType));

            this.m_storageTitle = this.m_db.getStorageInfoValue(StorageInfo.tagTitle);
            // add code here ?

            return;
        }

        /// <summary>
        /// NT-Завершить сеанс работы с Хранилищем
        /// </summary>
        public void Close()
        {
            //store Storage statistics
            this.updateStorageInfo();//check read-only included
            //close storage
            m_Session.Close();//close current session
            m_db.Disconnect();
            m_db = null;
            m_docController = null;
            m_picController = null;
            m_qualifiedStorageName = String.Empty;
            m_storageBaseClass = String.Empty;
            this.m_storageTitle = String.Empty;

            return;
        }

        //TODO: Надо определять как-то, что класс Сущности является, или является производным от,  БазовыйКлассХранилища. 
        //Если это не так, Сущность не может быть добавлена в это Хранилище.
        //- это можно только при добавлении сущностей, тогда указывается полный путь класса.
        //только по нему можно определить, принадлежит или нет. А вообще, надо думать.
        //если имена классов уникальные, то это несложно.

        /// <summary>
        /// NT-Добавить Сущность
        /// </summary>
        public void AddEntity(EntityRecord entity)
        {
            CheckReadOnly();
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();

            //add to storage document file if exists
            //TODO: тут хорошо бы начать транзакцию БД - но созданные при этом файлы не удалятся, а только записи о них.
            //а файлы мы удалить не можем пока из архивов. Но можем специальный обработчик отката транзакций сделать.
            //Но он тоже не может просто удалить файл из архива - вдруг этот файл еще где-то используется...
            //Тут вся надежда на оптимизатор - он и должен удалять непривязанные и отсутствующие в БД файлы из архивов.
            //Но оптимизатор пока не готов.
            try
            {
                m_db.TransactionBegin();
                m_docController.TransactionBegin();
                m_picController.TransactionBegin();
                //сначала добавить в БД файлы, а потом сущность. Иначе нельзя - в сущность надо вписать ид записей файлов.
				this.addDocument(entity);
                //add to storage picture file if exists
                this.addPicture(entity);
                //add entity to db
                this.m_db.InsertEntityRecord(entity);
                //тут принять транзакцию БД
				//TODO: если позже произошла ошибка, то как откатить изменения в архивах?
				//сейчас пока никак - файлы остаются в архивах, и либо удаляются оттуда оптимизатором
				//либо все же используются при последующей успешной транзакции
                m_picController.TransactionCommit();
                m_docController.TransactionCommit();
                m_db.TransactionCommit();
            }
            catch (Exception ex)
            {
                m_db.TransactionRollback();
                m_picController.TransactionRollback();
                m_docController.TransactionRollback();
                throw new StorageException("AddEntity failed", ex);
            }
            
            return;
        }


        /// <summary>
        /// NFT-Получить Сущность по названию.
        /// </summary>
        /// <param name="entityName">Название сущности</param>
        /// <returns>Возвращает массив найденных записей сущностей или пустой массив, если ничего не найдено</returns>
        public EntityRecord[] GetEntity(String entityName)
        {
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            //1 get entity record array
            EntityRecord[] recs = m_db.getEntityByName(entityName);
            return recs;
        }

        /// <summary>
        /// NFT-Найти Сущность
        /// </summary>
        ///<param name="searchPattern">
        ///Шаблон для поиска в формате со звездочками.
        ///Примеры: *, кт315*, *кт315* 
        ///Для радиодеталей надо искать запросом вида *кт315*, поскольку их система наименований сложная и данные слишком грязные.</param>
        ///<param name="searchTitle">Искать в поле названия сущности</param>
        ///<param name="searchDescription">Искать в поле описания сущности</param>
        ///<param name="searchType">Искать в поле класса сущности</param>
        ///<param name="limit">Ограничение размера выборки, записей</param>
        ///<returns>Возвращает массив найденных записей сущностей или пустой массив, если ничего не найдено</returns>
        public EntityRecord[] FindEntity(String searchPattern, bool searchTitle, bool searchDescription, bool searchType, int limit)
        {
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            EntityRecord[] recs = m_db.getEntityLike(searchPattern, searchTitle, searchDescription, searchType, limit);
            return recs;
        }

        /// <summary>
        /// NR-Удалить Сущность
        /// </summary>
        public void RemoveEntity(EntityRecord entity)
        {
            CheckReadOnly();
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Изменить Сущность
        /// </summary>
        public void ChangeEntity(EntityRecord entity)
        {
            CheckReadOnly();
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            //TODO: добавить код изменения сущности здесь 
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NT-Найти аналогичный файл в хранилище
        /// </summary>
        /// <param name="file">Путь к внешнему файлу</param>
        /// <returns>Возвращает FileRecord аналогичного файла в Хранилище или null если в хранилище нет аналогичного файла.</returns>
        public FileRecord FindDocumentDublicate(String file)
        {
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            FileRecord src = new FileRecord(file);
            FileRecord result = this.findDublicate(DbAdapter.DocumentTableName, src);
            return result;
        }
        /// <summary>
        /// NT-Найти аналогичный файл в хранилище
        /// </summary>
        /// <param name="file">Путь к внешнему файлу</param>
        /// <returns>Возвращает FileRecord аналогичного файла в Хранилище или null если в хранилище нет аналогичного файла.</returns>
        public FileRecord FindPictureDublicate(String file)
        {
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            FileRecord src = new FileRecord(file);
            FileRecord result = this.findDublicate(DbAdapter.PictureTableName, src);
            return result;
        }

        /// <summary>
        /// NT-Проверить, что указанный каталог является каталогом Хранилища
        /// </summary>
        /// <param name="path">Путь к каталогу</param>
        /// <returns>Возвращает true, если каталог является каталогом Хранилища. В противном случае возвращает false</returns>
        public static bool IsStorageFolder(string path)
        {
            //критерии:
            //папка должна содержать файл "description.xml"
            //папка должна содержать файл db.mdb
            //папка должна содержать папки docs pics
            //файл "description.xml" должен читаться без проблем
            
            String p = Path.Combine(path, ArchiveController.DocumentsDir);
            if (!Directory.Exists(p)) return false;
            p = Path.Combine(path, ArchiveController.ImagesDir);
            if (!Directory.Exists(p)) return false;
            p = Path.Combine(path, DbAdapter.DatabaseFileName);
            if (!File.Exists(p)) return false;
            p = Path.Combine(path, Manager.DescriptionFileName);
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
        /// NR-Оптимизация Хранилища - незакончено, неясно как сделать и как использовать потом
        /// </summary>
        public void OptimizeStorage()
        {
            CheckReadOnly();
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            //TODO: добавить код оптимизаци Хранилища здесь
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NFT-Очистить Хранилище
        /// </summary>
        /// <returns>Return True if success, False otherwise</returns>
        public bool ClearStorage()
        {
            CheckReadOnly();
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            //Тут очищаем все таблицы БД кроме таблицы свойств, удаляем все архивы, пересчитываем статистику и вносим ее в БД.
            //в результате должно получиься пустое Хранилище, сохранившее свойства - имя, квалифицированное имя, путь итп.
            if (m_db.ClearDb() == true)
            {
                m_docController.Clear();
                m_picController.Clear();
                this.updateStorageInfo();//это будет выполнено также при закрытии менеджера.
                return true;
            }
            else return false;
        }
        /// <summary>
        /// NFT-Создать Хранилище
        /// </summary>
        /// <returns>Возвращает готовый к работе, открытый менеджер Хранилища</returns>
        public static Manager CreateStorage(StorageInfo si)
        {
            //регистрировать Хранилище будет сам РеестрХранилищ
            //1) создать каталог хранилища
            //создать корневой каталог хранилища
            if (Directory.Exists(si.StoragePath))
                throw new Exception("Storage already exists!");
            DirectoryInfo di = new DirectoryInfo(si.StoragePath);
            di.Create();
            //установить атрибуты, запрещающие индексацию, архивацию и прочее в том же духе
            di.Attributes = (di.Attributes | FileAttributes.NotContentIndexed);
            di = null;
            //создать файл описания хранилища
            StorageInfo.StoreInfo(si);
            //создать подкаталоги хранилища docs pics
            String pics = Path.Combine(si.StoragePath, ArchiveController.ImagesDir);
            Directory.CreateDirectory(pics);
            String docs = Path.Combine(si.StoragePath, ArchiveController.DocumentsDir);
            Directory.CreateDirectory(docs);
            //извлечь из ресурсов сборки в корневой каталог шаблон БД хранилища
            String db = Path.Combine(si.StoragePath, DbAdapter.DatabaseFileName);
            extractDbFile(db);
            //2) открыть менеджер хранилища
            Manager man = new Manager();
            man.openIfNewOnly(si);//записать данные о хранилище в БД, кроме пути к хранилищу
            man.Close();
            //вернуть открытый менеджер хранилища
            man.Open(si.StoragePath);
            return man;
        }



        /// <summary>
        /// NFT-Удалить Хранилище
        /// </summary>
        /// <param name="storagePath">Путь к каталогу Хранилища</param>
        /// <returns>Возвращает true, если Хранилище успешно удалено или его каталог не существует.
        /// Возвращает false, если удалить Хранилище не удалось по какой-либо причине.</returns>
        public static bool DeleteStorage(String storagePath)
        {
            //Из реестра Хранилищ удалять будет сам РеестрХранилищ.
            
            //если каталог не существует, возвращаем  true
            if(!Directory.Exists(storagePath)) return true;
            //1) если Хранилище на диске только для чтения, то вернуть false.
            if (isReadOnly(storagePath)) return false;
            //2) пробуем переименовать каталог Хранилища
            //если получится, то каталог никем не используется. удалим каталог и вернем true.
            //иначе будет выброшено исключение - перехватим его и вернем false
            //сначала еще надо сгенерировать такое новое имя каталога, чтобы незанятое было.
            String newName = String.Empty;
            String preRoot = Path.GetDirectoryName(storagePath);
            for (int i = 0; i < 16384; i++)
            {
                newName = Path.Combine(preRoot, "tmp" + i.ToString());
                if (!Directory.Exists(newName)) break;
            }
            //тут мы должны оказаться с уникальным именем
            if (Directory.Exists(newName))
                throw new Exception("Error! Cannot create temp directory name!");
            //пробуем переименовать каталог
            try
            {
                Directory.Move(storagePath, newName);
            }
            catch (Exception)
            {
                return false;
            }
            //каталог не используется, удалим его
            //TODO: вот лучше бы его через шелл и в корзину удалить. Хотя... Удалять же будет не приложение. Некому показывать шелл.
            //Надо это решить как удобнее будет. Может, через аргументы передавать способ - с гуем в корзину или нет.
            Directory.Delete(newName, true);
            //если тут возникнут проблемы, то хранилище все равно уже будет повреждено.
            //так что выброшенное исключение достанется вызывающему коду.
            return true;
        }

        /// <summary>
        /// NT-Получить информацию о Хранилище
        /// </summary>
        public StorageInfo GetStorageInfo()
        {
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();

            StorageInfo si = new StorageInfo();

            //si.QualifiedName
            //si.StorageType
            //si.StorageVersion
            //si.Title
            //si.UsingCounter
            //si.Creator
            //si.DatabaseSize
            //si.Description 
            //si.EntityCount
            m_db.getStorageInfo(si);//fill object with storage info from db
            
            //store local values
            si.StoragePath = this.m_storageRootFolder; //Путь к корневому каталогу Хранилища
            si.ReadOnly = isReadOnly(this.m_storageRootFolder);//read-only flag
            //si.EngineVersion = getEngineVersionString(); //It's added to StorageInfo constructor
            si.DocsSize = m_docController.getFolderSize();//get size of all archives in docs folder
            si.DocsCount = m_docController.getFilesCount();//get count of files in all docs archives
            si.PicsSize = m_picController.getFolderSize();//get size of all archives in pics folder
            si.PicsCount = m_picController.getFilesCount();//get count of files in all pics archives
            
            //Размер файла БД, байт - для контроля предела размера бд.
            string db = Path.Combine(this.m_storageRootFolder, DbAdapter.DatabaseFileName);
            FileInfo fi = new FileInfo(db);
            si.DatabaseSize = fi.Length;
            fi = null;

            return si;
        }
        /// <summary>
        /// NFT-Изменить свойства Хранилища.
        /// Только некоторые свойства Хранилища могут быть изменены таким образом:
        /// Title
        /// Creator
        /// Description
        /// QualifiedName
        /// StorageType
        /// StorageVersion
        /// EngineVersion
        /// UsingCounter
        /// </summary>
        /// <param name="si">Объект СвойстваХранилища</param>
        public void ChangeStorageInfo(StorageInfo si)
        {
            //silent skip if read-only
            if (m_ReadOnly)
                return;
            //сбросить буферизированные файлы в архивы, чтобы они не портили мне всю схему
            this.FlushBuffered();
            //update database storage info
            this.m_db.storeStorageInfo(si);
            //изменения будут записаны в файл описания Хранилища при закрытии менеджера Хранилища.
            return;
        }

        /// <summary>
        /// NT-Записать в файл описания хранилища данные о состоянии хранилища
        /// </summary>
        private void updateStorageInfo()
        {
            //silent skip if read-only
            if (m_ReadOnly) 
                return;
            //get storage statistics
            StorageInfo si = GetStorageInfo();
            //write storage description file
            StorageInfo.StoreInfo(si);
            //update database storage info
            this.m_db.storeStorageInfo(si);

            return;
        }

        public override string ToString()
        {
            return base.ToString();
        }


        #endregion



        #region *** Вспомогательные функции ***





        /// <summary>
        /// NFT-Добавить изображение в хранилище
        /// </summary>
        /// <param name="entity"></param>
        /// <remarks>Одинаковая с addDocument()</remarks>
        private void addPicture(EntityRecord entity)
        {
            //подобно коду для документов после его отладки
            //если в данных указан не новый документ, то выйти.
            if (!entity.HaveNewPic()) return;
            //Тут надо пересчитать свойства файла - контрольную сумму, длину итд.
            FileRecord fr = entity.Picture; //должно быть уже посчитано в конструкторе
            fr.calculateChecksum();//если не посчитано, тут и посчитаем
            //Тут надо по БД поискать аналоги файла, потом проверить их, если найдутся схожие.
            FileRecord dublicate = findDublicate(DbAdapter.PictureTableName, fr);
            if (dublicate == null)
            {
                //Тут надо подготовить новое имя файла, уникальное для всего хранилища.
                //искать просмотром БД
                String unicalName = findUnicalFileName(DbAdapter.PictureTableName, fr.ExternalPath);
                //Тут надо вызвать контроллер архивов, чтобы он добавил файл и вернул новое имя файла.
                String archpath = this.m_picController.addFile(unicalName, fr.ExternalPath);
                //Тут надо все вписать в запись файла
                fr.StoragePath = archpath;
                //записать файл в БД. И сразу же получить и вписать его ИД в объект.
                m_db.InsertFileRecord(DbAdapter.PictureTableName, fr);
                //тут вписать имя добавленного файла в кеш-словарь имен файлов.
                this.m_picController.addFileNameToDictionary(fr.StoragePath);
                //вписать новый файл в объект сущности
                entity.Picture = fr;
            }
            else
            {
                //вписать дубликат вместо нового файла
                entity.Picture = dublicate;
            }

            return;
        }

        /// <summary>
        /// NFT-добавить документ в БД и архивы
        /// </summary>
        /// <param name="entity"></param>
        /// <remarks>Одинаковая с addPicture()</remarks>
        private void addDocument(EntityRecord entity)
        {
            //если в данных указан не новый документ, то выйти.
            if (!entity.HaveNewDoc()) return;
            //Тут надо пересчитать свойства файла - контрольную сумму, длину итд.
            FileRecord fr = entity.Document; //должно быть уже посчитано в конструкторе
            fr.calculateChecksum();//если не посчитано, тут и посчитаем
            //Тут надо по БД поискать аналоги файла, потом проверить их, если найдутся схожие.
            FileRecord dublicate = findDublicate(DbAdapter.DocumentTableName, fr);
            if (dublicate == null)
            {
                //Тут надо подготовить новое имя файла, уникальное для всего хранилища.
                //искать просмотром БД
                String unicalName = findUnicalFileName(DbAdapter.DocumentTableName, fr.ExternalPath);
                //Тут надо вызвать контроллер архивов, чтобы он добавил файл и вернул новое имя файла.
                String archpath = this.m_docController.addFile(unicalName, fr.ExternalPath);
                //Тут надо все вписать в запись файла
                fr.StoragePath = archpath;
                //записать файл в БД. И сразу же получить и вписать его ИД в объект.
                m_db.InsertFileRecord(DbAdapter.DocumentTableName, fr);
                //тут вписать имя добавленного файла в кеш-словарь имен файлов.
                this.m_docController.addFileNameToDictionary(fr.StoragePath);
                //вписать новый файл в объект сущности
                entity.Document = fr;
            }
            else
            {
                //вписать дубликат вместо нового файла
                entity.Document = dublicate;
            }

            return;
        }
        /// <summary>
        /// NFT-Создать уникальное в хранилище имя файла, не содержащее неподходящих символов
        /// </summary>
        /// <param name="filepath">Исходный путь файла</param>
        /// <param name="table">Document or picture table name</param>
        /// <returns>Возвращает уникальное имя файла с расширением но без пути</returns>
        private string findUnicalFileName(String table, string filepath)
        {
            //1 выделить из пути имя файла и расширение отдельно
            String ext = Path.GetExtension(filepath);
            String name = Path.GetFileNameWithoutExtension(filepath);
            //2 убрать из имени файла символы недопустимые в веб-пути подобно Инвентарь - заменить на _
            //это обрезает имя до 16 символов плюс числовой индекс иногда.
            name = Utility.RemoveWrongSymbols(name);
            //3 проверить отсутствие такого имени в БД, отправив туда имя без расширения.
            //Но в таблице хранятся не имена файлов, а пути в архивах. Вида doc1/file.ext.
            //Поэтому ускорения поиска не получится - от имени файла надо еще отделять расширение и каталог.
            //Поэтому LIKE здесь тормозит всю работу, а заменить его нечем.
            Dictionary<String, Int32> dubdict = m_db.GetFileNamesLike(table, name);
            //TODO: Optimization - Если много одноименных файлов добавляется в Хранилище
            //то нужно искать свободное имя перебором вариантов еще при получении выборки в таблице.
            //иначе массив dubnames займет всю свободную память.
            name = ArchiveController.MakeUnicalNameSub(name, dubdict); 
            //это запрос вроде LIKE %name% вернет все похожие варианты, и вот из них надо распарсить и выявить аналоги.
            //Это одним запросом получится сделать, но это медленная операция.
            // а сейчас мы завершаем функцию,  и этот большой массив дубнеймов будет уничтожен.
            //Хорошо что он недолго существует.

            //4 вернуть уникальное имя файла с расширением
            return name + ext;
        }

      
        /// <summary>
        /// NT-Найти в БД дубликат для нового файла
        /// </summary>
        /// <param name="table">Document or picture table name</param>
        /// <param name="fr">Запись нового файла</param>
        /// <returns>Возвращаем дубликат нового файла или null если не найден дубликат.</returns>
        private FileRecord findDublicate(String table, FileRecord fr)
        {

            //запросить из БД записи сходные по КС и размеру
            //файлы, добавленные в пакетном режиме, все же записываются в БД, хотя не записываются в архив.
            FileRecord[] dublicates = m_db.GetFileDublicates(table, fr.Length, fr.Crc);
            //если дубликатов не найдено, возвращаем null
            FileRecord result = null;
            //если найден один и более дубликатов, сравниваем их побайтно.
            //если есть первое совпадение, то его и возвращаем вместо оригинала.

            //select controller
            ArchiveController controller;
            if (table == ArchiveController.DocumentsDir)
                controller = m_docController;
            else controller = m_picController;

            //find dublicates
            foreach (FileRecord f in dublicates)
            {
                bool res = controller.CompareFile(fr.ExternalPath, f.StoragePath);
                if (res == true)
                {
                    result = f;
                    break;
                }
            }
            //
            return result;
        }

        


        ///// <summary>
        ///// NT-Возвращает флаг что Хранилище может обновляться.
        ///// </summary>
        ///// <returns></returns>
        //private bool isReadOnly()
        //{
        //    return isReadOnly(this.m_storageRootFolder);
        //}
        /// <summary>
        /// NT-Возвращает флаг что указанное Хранилище может обновляться.
        /// </summary>
        /// <returns>Returns True if storage is ReadOnly, False otherwise</returns>
        private static bool isReadOnly(String storageRootFolder)
        {
            bool ro = false;
            //generate test file name
            String test = Path.Combine(storageRootFolder, "writetest.txt");
            
            try
            {
                //if test file already exists, try remove it
                if (File.Exists(test))
                    File.Delete(test);//тут тоже будет исключение, если каталог read-only
                //test creation 
                FileStream fs = File.Create(test);
                fs.Close();
            }
            catch (Exception)
            {
                ro = true;
            }
            finally
            {
                File.Delete(test);
            }
            return ro;
        }
        /// <summary>
        /// NT-Получить строку версии текущей сборки
        /// </summary>
        /// <returns></returns>
        public static string getEngineVersionString()
        {
            //DONE: Убедиться что это возвращает версию текущей сборки а не приложения.
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        /// <summary>
        /// NT-Извлечь файл шаблона базы данных из ресурсов сборки
        /// </summary>
        /// <param name="filepath">Путь к итоговому файлу</param>
        private static void extractDbFile(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Create);
            byte[] content = Properties.Resources._base;//database template name
            fs.Write(content, 0, content.Length);
            fs.Close();
        }
        /// <summary>
        /// NT-Проверить режим read-only и выбросить исключение
        /// </summary>
        private void CheckReadOnly()
        {
            if (this.m_ReadOnly == true)
                throw new Exception("Error: Writing to read-only storage!");
        }



        #endregion

        #region *** Buffered-Bulk adding functions ***

        /// <summary>
        /// NR-Добавить Сущность используя буферизацию записи файлов
        /// </summary>
        public void AddEntityBuffered(EntityRecord entity)
        {
            //TODO: Bulk adding - код тут вроде соответствует, но вдруг...
            CheckReadOnly();

            //add to storage document file if exists
            //TODO: тут хорошо бы начать транзакцию БД - но созданные при этом файлы не удалятся, а только записи о них.
            //а файлы мы удалить не можем пока из архивов. Но можем специальный обработчик отката транзакций сделать.
            //Но он тоже не может просто удалить файл из архива - вдруг этот файл еще где-то используется...
            //Тут вся надежда на оптимизатор - он и должен удалять непривязанные и отсутствующие в БД файлы из архивов.
            //Но оптимизатор пока не готов.
            try
            {
                m_db.TransactionBegin();
                //сначала добавить в БД файлы, а потом сущность. Иначе нельзя - в сущность надо вписать ид записей файлов.
                this.addDocumentBuffered(entity);
                //add to storage picture file if exists
                this.addPictureBuffered(entity);
                //add entity to db
                this.m_db.InsertEntityRecord(entity);
                //тут принять транзакцию БД
                m_db.TransactionCommit();
            }
            catch (Exception ex)
            {
                m_db.TransactionRollback();
                throw new Exception("Error!", ex);
            }

            return;
        }
        /// <summary>
        /// NR-Добавить изображение в хранилище через буфер файлов
        /// </summary>
        /// <param name="entity"></param>
        private void addPictureBuffered(EntityRecord entity)
        {
            //если в данных указан не новый документ, то выйти.
            if (!entity.HaveNewPic()) return;
            //Тут надо пересчитать свойства файла - контрольную сумму, длину итд.
            FileRecord fr = entity.Picture; //должно быть уже посчитано в конструкторе
            fr.calculateChecksum();//если не посчитано, тут и посчитаем
            //Тут надо по БД поискать аналоги файла, потом проверить их, если найдутся схожие.
            FileRecord dublicate = this.m_picController.findDublicate(fr);
            if (dublicate == null)
            {
                //Тут надо подготовить новое имя файла, уникальное для всего хранилища.
                //искать просмотром БД
                String unicalName = this.m_picController.findUnicalFileName( fr.ExternalPath);
                //Тут надо вызвать контроллер архивов, чтобы он добавил файл и вернул новое имя файла.
                //TODO: Bulk adding - пример вызова функции
                //тут надо передать объект FileRecord, чтобы сделать его копию внутри функции и иметь длину файла также
                //но тут у объекта еще нет ИД из БД, и поэтому дубликат не имеет этого ИД.
                FileRecord copyFr = null;
                String archpath = this.m_picController.addFileBuffered(unicalName, fr, out copyFr);
                //Тут надо все вписать в запись файла
                //fr.StoragePath = archpath; - уже вписано
                //записать файл в БД. И сразу же получить и вписать его ИД в объект.
                m_db.InsertFileRecord(DbAdapter.PictureTableName, fr);
                //HACK: 26102017: вписать ИД таблицы файлов в запись файла в пакетном буфере.
                //так как там копия записи, то в нее ИД сам не передается.
                copyFr.TableId = fr.TableId;
                //тут вписать имя добавленного файла в кеш-словарь имен файлов.
                this.m_docController.addFileNameToDictionary(fr.StoragePath);
                //вписать новый файл в объект сущности
                entity.Picture = fr;
            }
            else
            {
                //вписать дубликат вместо нового файла
                entity.Picture = dublicate;
            }

            return;
        }
        /// <summary>
        /// NT-добавить документ в БД и архивы через буфер файлов
        /// </summary>
        /// <param name="entity"></param>
        private void addDocumentBuffered(EntityRecord entity)
        {
           
            //этот же код применен для изображений.

            //если в данных указан не новый документ, то выйти.
            if (!entity.HaveNewDoc()) return;
            //Тут надо пересчитать свойства файла - контрольную сумму, длину итд.
            FileRecord fr = entity.Document; //должно быть уже посчитано в конструкторе
            fr.calculateChecksum();//если не посчитано, тут и посчитаем
            //Тут надо по БД поискать аналоги файла, потом проверить их, если найдутся схожие.
            FileRecord dublicate = this.m_docController.findDublicate(fr);
            //если ничего не нашли, то создаем новое имя файла
            if (dublicate == null)
            {
                //Тут надо подготовить новое имя файла, уникальное для всего хранилища.
                //искать просмотром БД
                String unicalName = this.m_docController.findUnicalFileName( fr.ExternalPath);
                //Тут надо вызвать контроллер архивов, чтобы он добавил файл и вернул новое имя файла.
                //TODO: Bulk adding - пример вызова функции
                //тут надо передать объект FileRecord, чтобы сделать его копию внутри функции и иметь длину файла также
                //но тут у объекта еще нет ИД из БД, и поэтому дубликат не имеет этого ИД.
                FileRecord copyFr = null;
                String archpath = this.m_docController.addFileBuffered(unicalName, fr, out copyFr);
                //Тут надо все вписать в запись файла
                //fr.StoragePath = archpath; - уже вписано
                //записать файл в БД. И сразу же получить и вписать его ИД в объект.
                m_db.InsertFileRecord(DbAdapter.DocumentTableName, fr);
                //HACK: 26102017: вписать ИД таблицы файлов в запись файла в пакетном буфере.
                //так как там копия записи, то в нее ИД сам не передается.
                copyFr.TableId = fr.TableId;
                //тут вписать имя добавленного файла в кеш-словарь имен файлов.
                this.m_docController.addFileNameToDictionary(fr.StoragePath);
                //вписать новый файл в объект сущности
                entity.Document = fr;
            }
            else
            {
                //вписать дубликат вместо нового файла
                entity.Document = dublicate;
            }

            return;
        }
        /// <summary>
        /// NT-Записать файлы из буфера на диск в архив
        /// </summary>
        public void FlushBuffered()
        {
            //write bufferd files to archives
            this.m_docController.FlushBuffered();
            this.m_picController.FlushBuffered();
        }
        #endregion

        #region *** Bulk adding functions ***
        /// <summary>
        /// NR-Добавить массив сущностей
        /// </summary>
        /// <param name="items">Массив сущностей</param>
        public void AddEntities(EntityRecord[] items)
        {
            throw new NotImplementedException(); //TODO: add code here
        }
        #endregion
        #region *** GetFile functions ***

        /// <summary>
        /// NT-Копировать содержимое файла из архива в указанный поток.
        /// </summary>
        /// <param name="qualifiedName">Квалифицированный путь файла из хранилища</param>
        /// <param name="s">Поток, в который нужно записать содержимое файла</param>
        public void GetFile(String qualifiedName, Stream s)
        {
            
            //do not close stream!
            //1 проверить что в пути указано именно это хранилище
            String storageqname = QualifiedNameManager.getStorageQName(qualifiedName);
            if (this.QualifiedName != storageqname)
                throw new Exception(String.Format("Path {0} not belongs to {1} Storage", qualifiedName, this.QualifiedName));
            //2 получить архивное имя файла и извлечь из него тип контроллера архивов
            String archpath = QualifiedNameManager.getFileQName(qualifiedName);
            //извлечь файл
            intGetFile(archpath, s);

            return;
        }
        /// <summary>
        /// NR-Записать содержимое файла из архива в указанный поток.
        /// </summary>
        /// <param name="archpath">Путь файла внутри Хранилища</param>
        /// <param name="s">Поток для записи</param>
        internal void intGetFile(String archpath, Stream s)
        {
            ArchiveController ac = null;
            if (archpath.StartsWith(ArchiveController.DocumentsDir))
                ac = this.m_docController;
            else if (archpath.StartsWith(ArchiveController.ImagesDir))
                ac = this.m_picController;
            else throw new Exception(String.Format("Invalid qualified path: {0}", archpath));
            //3 get file
            ac.getFile(archpath, s);

            return;
        }

        /// <summary>
        /// NT-Записать содержимое файла из архива в указанный файл.
        /// </summary>
        /// <param name="archpath">Путь файла внутри Хранилища</param>
        /// <param name="filepath">Путь файла</param>
        internal void intGetFile(String archpath, String filepath)
        {
            //create specified file
            FileStream fs = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            //write data to stream
            intGetFile(archpath, fs);
            //close stream
            fs.Close();
            return;
        }

        /// <summary>
        /// NT-Копировать содержимое файла из архива в указанный файл.
        /// </summary>
        /// <param name="qualifiedName">Квалифицированный путь файла из хранилища</param>
        /// <param name="filepath">Путь файла</param>
        public void GetFile(String qualifiedName, String filepath)
        {
            
            //create specified file
            FileStream fs = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            //write data to stream
            GetFile(qualifiedName, fs);
            //close stream
            fs.Close();
            return;
        }

        #endregion

        /// <summary>
        /// NT-получить список классов из БД Хранилища
        /// </summary>
        /// <returns>Возвращает список полных путей уникальных значений категорий сущностей</returns>
        public List<string> GetEntityCategoryNames()
        {
            List<string> classes = this.m_db.GetEntityCategoryValues();
            //return classes; - старая версия возвращала короткие пути классов

            //Прицепить всем классам базовый класс хранилища, чтбы возвращать полные пути классов.
            List<string> result = new List<string>();
            //для оптимизации разберем путь на классы здесь, а не каждый  раз в функции.
            string[] baseClasses = ClassItem.SplitClassPath(this.m_storageBaseClass);
            //конвертировать классы
            foreach (string s in classes)
            {
                String ss = ClassItem.MergeClassPath(baseClasses, s);
                result.Add(ss);
            }
            return result;
        }
        /// <summary>
        /// NT-Получить запись сущности по ссылке
        /// </summary>
        /// <param name="lb">Билдер распарсенной ссылки для изоляции от исключений и удобства</param>
        /// <returns>Возвращает запись сущности или null, если записи с таким идентификатором не существует.</returns>
        public EntityRecord GetEntityByLink(LinkBuilder lb)
        {
            if (lb.Linktype != LinkType.EntityRecordLink)
                throw new ArgumentException("Invalid link type", "lb");
            //check storage qname
            if(String.Equals(this.m_qualifiedStorageName, lb.StorageQName, StringComparison.OrdinalIgnoreCase) == false)
                throw new ArgumentException("Storage qualified name mismatched", "lb");
            //get entity table id
            int tableid = lb.TableId;
            //load from database
            EntityRecord result = this.m_db.getEntityById(tableid);
            return result;
        }


    }
}
