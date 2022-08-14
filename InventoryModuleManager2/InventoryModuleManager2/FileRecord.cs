using System;
using System.Globalization;
using System.IO;
using InventoryModuleManager2.ClassificationEntity;

namespace InventoryModuleManager2
{
    /// <summary>
    /// Представляет запись о файле для БД итд
    /// </summary>
    /// 
    public class FileRecord: RecordBase
    {
        #region *** Переменные ***
        /// <summary>
        /// счетчик первичный ключ таблицы
        /// </summary>
        private int m_id;
        /// <summary>
        /// путь к файлу внутри Хранилища.
        /// </summary>
        private string m_storagePath;
        /// <summary>
        /// путь к файлу вне Хранилища, для указания файлового пути к загружаемому в хранилище файлу.
        /// </summary>
        private string m_externalPath;
        /// <summary>
        /// описание документа.
        /// </summary>
        private string m_description;
        /// <summary>
        /// контрольная сумма файла в HEX-формате
        /// </summary>
        private UInt32 m_crc;
        /// <summary>
        /// длина файла в байтах - для более быстрого поиска дубликатов файла.
        /// </summary>
        private Int64 m_length;
        /// <summary>
        /// тип документа по НотацияТиповОператора.
        /// </summary>
        private string m_type;
        /// <summary>
        /// счетчик СтепеньИспользования документа
        /// </summary>
        private int m_usingCounter;

        /// <summary>
        /// Ссылка на менеджер для поддержки некоторых функций
        /// </summary>
        private Manager m_manager;
#endregion

#region *** Проперти ***

        /// <summary>
        /// счетчик СтепеньИспользования документа
        /// </summary>
        public int UsingCounter
        {
            get { return m_usingCounter; }
            set { m_usingCounter = value; }
        }

        /// <summary>
        /// тип документа по НотацияТиповОператора.
        /// </summary>
        public string DocumentType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        /// <summary>
        /// длина файла в байтах - для более быстрого поиска дубликатов файла.
        /// </summary>
        public Int64 Length
        {
            get { return m_length; }
            set { m_length = value; }
        }

        /// <summary>
        /// контрольная сумма файла в HEX-формате
        /// </summary>
        public UInt32 Crc
        {
            get { return m_crc; }
            set { m_crc = value; }
        }

        /// <summary>
        /// описание документа.
        /// </summary>
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// путь к файлу вне Хранилища, для указания файлового пути к загружаемому в хранилище файлу.
        /// </summary>
        public string ExternalPath
        {
            get { return m_externalPath; }
            set { m_externalPath = value; }
        }

        /// <summary>
        /// путь к файлу внутри Хранилища.
        /// </summary>
        public string StoragePath
        {
            get { return m_storagePath; }
            set { m_storagePath = value; }
        }

        /// <summary>
        /// счетчик первичный ключ таблицы
        /// </summary>
        public int TableId
        {
            get { return m_id; }
            set { m_id = value; }
        }
        /// <summary>
        /// NT-Получить квалифицированный путь файла в хранилище.
        /// Использовать для доступа к файлу извне хранилища. Например, через ссылки.
        /// </summary>
        public String QualifiedName
        {
            get
            {
                //throw new NotImplementedException(); //TODO: add code here...
                if (m_manager == null)
                    throw new Exception("File info object not connected to Storage");
                //call manager
                return QualifiedNameManager.CombineForFile(m_manager.QualifiedName, this.m_storagePath);//THEME: QNAME - переработать все места по этой теме
            }
        }

#endregion



        //TODO: Этот класс используется внутри и вне сборки.
        //Поэтому он должен и содержать переменные-свойства, и поддерживать строковый набор данных.
        //Чтобы выводить данные как строковый набор и загружать данные из строкового набора.
        //И быстро работать, как обычный класс с типизированными переменными.
        //Поэтому обычно словарь пустой, а работа идет через переменные
        //А при передаче из сборки словарь заполняется данными из полей - делается отдельная копия объекта записи с пустыми полями.
        //Хотя это все выглядит замороченно. Возможно, есть более простой способ...

        //Взять список полей из вики и сделать тут в виде проперти.


        /// <summary>
        /// NT-Конструктор.
        /// Словарь значений заполняется отдельным вызовом Load...()
        /// </summary>
        public FileRecord()
        {
            this.m_crc = 0;
            this.m_description = String.Empty;
            this.m_externalPath = String.Empty;
            this.m_id = 0;
            this.m_length = 0;
            this.m_storagePath = String.Empty;
            this.m_type = String.Empty;
            this.m_usingCounter = 0;

            this.m_manager = null;//manager reference
            return;
        }

        /// <summary>
        /// NT-Конструктор.
        /// Словарь значений заполняется отдельным вызовом Load...()
        /// </summary>
        public FileRecord(Manager man)
        {
            this.m_crc = 0;
            this.m_description = String.Empty;
            this.m_externalPath = String.Empty;
            this.m_id = 0;
            this.m_length = 0;
            this.m_storagePath = String.Empty;
            this.m_type = String.Empty;
            this.m_usingCounter = 0;

            this.m_manager = man;//manager reference
            return;
        }

        /// <summary>
        /// NT-Конструктор для автоматического заполнения свойств файла.
        /// Словарь значений заполняется отдельным вызовом Load...()
        /// </summary>
        /// <param name="filepath">Путь к файлу вне хранилища или в другом хранилище или еще как-то</param>
        public FileRecord(String filepath)
        {
            this.m_description = String.Empty;
            this.m_externalPath = filepath;
            this.m_id = 0;
            this.m_storagePath = String.Empty;
            this.m_type = String.Empty;//TODO: А что тут вписать?
            this.m_usingCounter = 0;
            //calc checksum and length
            this.calculateChecksum();
            this.m_manager = null;//manager reference
            return;
        }


        /// <summary>
        /// NT-Конструктор копирования.
        /// Словарь значений заполняется отдельным вызовом Load...()
        /// </summary>
        /// <param name="f">Объект для копирования</param>
        public FileRecord(FileRecord f)
        {
            this.m_description = String.Copy(f.m_description);
            this.m_externalPath = String.Copy(f.m_externalPath);
            this.m_id = f.m_id;
            this.m_storagePath = String.Copy(f.m_storagePath);
            this.m_type = String.Copy(f.m_type);
            this.m_usingCounter = f.m_usingCounter;
            this.m_crc = f.m_crc;
            this.m_id = f.m_id;
            this.m_length = f.m_length;
            this.m_manager = f.m_manager;//manager reference
            return;
        }

        /// <summary>
        /// NT
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0};{1};{2}", this.m_id, this.m_storagePath, this.m_length);
        }

        /// <summary>
        /// NT-Вписать значения в внутренний словарь для пересылки между приложениями.
        /// </summary>
        public void StoreToDictionary()
        {
            this.m_dictionary.Add("TableId", this.m_id.ToString(CultureInfo.InvariantCulture));
            this.m_dictionary.Add("StoragePath", this.m_storagePath);
            this.m_dictionary.Add("ExternalPath", this.m_externalPath);
            this.m_dictionary.Add("Description", this.m_description);
            this.m_dictionary.Add("Crc", this.m_crc.ToString(CultureInfo.InvariantCulture));
            this.m_dictionary.Add("Length", this.m_length.ToString(CultureInfo.InvariantCulture));
            this.m_dictionary.Add("Type", this.m_type);
            this.m_dictionary.Add("UsingCounter", this.m_usingCounter.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// NT-Извлечь значения из внутреннего словаря
        /// </summary>
        public void LoadFromDictionary()
        {
            this.m_crc = this.getValueAsUInt32("Crc");
            this.m_description = this.getValueAsString("Description");
            this.m_externalPath = this.getValueAsString("ExternalPath");
            this.m_id = this.getValueAsInt32("TableId");
            this.m_length = this.getValueAsInt64("Length");
            this.m_storagePath = this.getValueAsString("StoragePath");
            this.m_type = this.getValueAsString("Type");
            this.m_usingCounter = this.getValueAsInt32("UsingCounter");
        }

        /// <summary>
        /// NT-Если контрольная сумма и длина не были ранее подсчитаны, подсчитать сейчас. 
        /// </summary>
        internal void calculateChecksum()
        {
            FileInfo fi = new FileInfo(this.m_externalPath);
            if (!fi.Exists)
                throw new FileNotFoundException("Error: File not found!", this.m_externalPath);
            //проверить, если КС не вычислялась, то вычислить
            if(m_crc == 0)
                this.m_crc  = Utility.getFileCrc(this.m_externalPath);
            this.m_length = fi.Length;
            return;
        }

        /// <summary>
        /// NT-получить ссылку на объект сущности
        /// </summary>
        /// <returns>Возвращает ссылку вида префикс://КвалифицированноеИмяХранилища/docs1/file1.ext  или префикс://КвалифицированноеИмяХранилища/pics1/file1.ext</returns>
        public string GetFileLink()
        {
            if (this.m_manager == null)
                throw new Exception("Invalid Manager reference");//ссылка на менеджер не установлена

            LinkBuilder lb = new LinkBuilder();
            lb.StorageQName = this.m_manager.QualifiedName;
            lb.TableId = this.m_id;
            lb.Filepath = this.m_storagePath;//путь определяет, документ это или изображение
            //lb.Title = this.m_storagePath;//не используется

            return lb.getFileLink();
        }

//        /// <summary>
//        /// NR-Получить путь к файлу во временном каталоге
//        /// </summary>
//        /// <returns></returns>
//        public String GetFile()
//        {
//            //собственно, зачем это надо?
//            //если приложение хочет получить файл, пусть оно и ищет для него место
//            //и само удаляет файл, когда он уже не нужен.
//            //а тут это просто от набросков осталось.
//            //надо это обдумать.
//throw new NotImplementedException(); //TODO: add code here
//        }

        /// <summary>
        /// NT-Копировать содержимое файла из архива в указанный поток.
        /// </summary>
        /// <param name="s">Поток, в который нужно записать содержимое файла</param>
        public void GetFile(Stream s)
        {
            //throw new NotImplementedException(); //TODO: add code here
            //
            if(m_manager == null)
                throw new Exception("File info object not connected to Storage");
            //call manager
            m_manager.intGetFile(this.m_storagePath, s);
            
            return;
        }

        /// <summary>
        /// NT-Копировать содержимое файла из архива в указанный файл
        /// </summary>
        /// <param name="filename">Путь файла</param>
        public void GetFile(String filepath)
        {
            //throw new NotImplementedException(); //TODO: add code here
            if (m_manager == null)
                throw new Exception("File info object not connected to Storage");
            //call manager
            //это медленный способ, только для тестирования механизма квалифицированных имен
            m_manager.GetFile(this.QualifiedName, filepath);
            //m_manager.intGetFile(this.m_storagePath, filepath);//а это более быстрый способ
               
            return;
        }
            
    }
}
