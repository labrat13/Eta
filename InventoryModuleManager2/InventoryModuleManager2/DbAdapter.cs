using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Text;
using System.IO;
using InventoryModuleManager2.ClassificationEntity;

namespace InventoryModuleManager2
{
    /// <summary>
    /// Клас адаптера БД
    /// </summary>
    public class DbAdapter
    {
        #region Fields
        /// <summary>
        /// Имя файла базы данных хранилища
        /// </summary>
        public const string DatabaseFileName = "db.mdb";
        /// <summary>
        /// Название таблицы документов
        /// </summary>
        internal const string DocumentTableName = "docs";
        /// <summary>
        /// Название таблицы изображений
        /// </summary>
        internal const string PictureTableName = "pics";
        /// <summary>
        /// Название таблицы сущностей
        /// </summary>
        internal const string ContentTableName = "content";
        /// <summary>
        /// Название таблицы описания хранилища
        /// </summary>
        internal const string AboutTableName = "about";


        /// <summary>
        /// database connection string
        /// </summary>
        public static String ConnectionString;
        /// <summary>
        /// database connection
        /// </summary>
        private System.Data.OleDb.OleDbConnection m_connection;
        /// <summary>
        /// Transaction for current connection
        /// </summary>
        private OleDbTransaction m_transaction;
        /// <summary>
        /// Database is read-only
        /// </summary>
        private bool dbReadOnly;

        /// <summary>
        /// Ссылка на менеджер для поддержки некоторых функций
        /// </summary>
        private Manager m_manager;

        //все объекты команд сбрасываются в нуль при отключении соединения с БД
        //TODO: Новые команды внести в ClearCommands()
        /// <summary>
        /// Команда без параметров, используемая во множестве функций
        /// </summary>
        private OleDbCommand m_cmdWithoutArguments;
        private OleDbCommand m_cmdUpdateUsings;
        private OleDbCommand m_cmdGetDublicates;
        private OleDbCommand m_cmdInsertFileRecord;
        private OleDbCommand m_cmdInsertEntityRecord;
        private OleDbCommand m_cmdGetEntityByName;
        private OleDbCommand m_cmdGetEntityByLike;

        #endregion

        public DbAdapter(Manager man)
        {
            m_manager = man;
        }

        /// <summary>
        /// Database is read-only
        /// </summary>
        public bool ReadOnly
        {
            get { return dbReadOnly; }
            //set { dbReadOnly = value; }
        }

        #region Service functions

        /// <summary>
        /// NT-все объекты команд сбросить в нуль
        /// </summary>
        private void ClearCommands()
        {
            m_cmdWithoutArguments = null;
            m_cmdUpdateUsings = null;
            m_cmdGetDublicates = null;
            m_cmdInsertFileRecord = null;
            m_cmdInsertEntityRecord = null;
            m_cmdGetEntityByName = null;
            m_cmdGetEntityByLike = null;
            return;
        }

        /// <summary>
        /// NT-Создать и инициализировать объект БД
        /// </summary>
        /// <returns></returns>
        public static DbAdapter SetupDbAdapter(Manager man, String dbFile, bool ReadOnly)
        {
            //теперь создать новый интерфейс БД
            DbAdapter dblayer = new DbAdapter(man);
            dblayer.dbReadOnly = ReadOnly;
            String constr = createConnectionString(dbFile, ReadOnly);
            //connect to database
            dblayer.Connect(constr);
            return dblayer;
        }


        /// <summary>
        /// NT-Открыть соединение с БД
        /// </summary>
        /// <param name="connectionString">Строка соединения с БД</param>
        public void Connect(String connectionString)
        {
            //create connection
            OleDbConnection con = new OleDbConnection(connectionString);
            //try open connection
            con.Open();
            con.Close();
            //close existing connection
            Disconnect();
            //open new connection and set as primary
            this.m_connection = con;
            ConnectionString = connectionString;
            this.m_connection.Open();

            return;
        }


        /// <summary>
        /// NT-Закрыть соединение с БД
        /// </summary>
        public void Disconnect()
        {
            if (m_connection != null)
            {
                if (m_connection.State == System.Data.ConnectionState.Open)
                    m_connection.Close();
                m_connection = null;
            }

            //все объекты команд сбросить в нуль при отключении соединения с БД, чтобы ссылка на объект соединения при следующем подключении не оказалась устаревшей
            ClearCommands();

            return;
        }



        /// <summary>
        /// NT-Начать транзакцию. 
        /// </summary>
        public void TransactionBegin()
        {
            m_transaction = m_connection.BeginTransaction();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
        }
        //NT-Подтвердить транзакцию Нужно закрыть соединение после этого!
        public void TransactionCommit()
        {
            m_transaction.Commit();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands(); //надо ли сбросить m_transactions = null?
            m_transaction = null;

        }
        //NT-Отменить транзакцию. Нужно закрыть соединение после этого!
        public void TransactionRollback()
        {
            m_transaction.Rollback();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
            m_transaction = null;
        }

        /// <summary>
        /// NT-Создать строку соединения с БД
        /// </summary>
        /// <param name="dbFile">Путь к файлу БД</param>
        public static string createConnectionString(string dbFile, bool ReadOnly)
        {
            //Provider=Microsoft.Jet.OLEDB.4.0;Data Source="C:\Documents and Settings\salomatin\Мои документы\Visual Studio 2008\Projects\RadioBase\радиодетали.mdb"
            OleDbConnectionStringBuilder b = new OleDbConnectionStringBuilder();
            b.Provider = "Microsoft.Jet.OLEDB.4.0";
            b.DataSource = dbFile;
            //это только для БД на незаписываемых дисках
            if (ReadOnly)
            {
                b.Add("Mode", "Share Deny Write");
            }
            //user id and password can specify here
            return b.ConnectionString;
        }
        #endregion


        #region *** Для всех таблиц ***
        /// <summary>
        /// NT- get max of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>Value</returns>
        internal int getTableMaxId(string table)
        {
            //SELECT MAX(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT MAX(id) FROM {0};", table);
            m_cmdWithoutArguments.CommandText = query;
            Object ob = m_cmdWithoutArguments.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);


        }
        /// <summary>
        /// NT-get min of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>Value</returns>
        internal int getTableMinId(string table)
        {
            //SELECT MIN(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT MIN(id) FROM {0};", table);
            m_cmdWithoutArguments.CommandText = query;
            Object ob = m_cmdWithoutArguments.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);


        }

        /// <summary>
        /// NT-Получить число записей в таблице
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <returns></returns>
        internal int GetRowCount(string table)
        {
            //SELECT COUNT(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT COUNT(id) FROM {0};", table);
            m_cmdWithoutArguments.CommandText = query;
            int result = (Int32)m_cmdWithoutArguments.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных

            return result;
        }

        /// <summary>
        /// NFT-Очистить БД Хранилища
        /// </summary>
        /// <returns>True if Success, False otherwise</returns>
        internal bool ClearDb()
        {
            bool result = false;
            try
            {
                this.TransactionBegin();
                this.ClearTable(DbAdapter.ContentTableName);
                this.ClearTable(DbAdapter.DocumentTableName);
                this.ClearTable(DbAdapter.PictureTableName);
                this.TransactionCommit();
                result = true;
            }
            catch (Exception)
            {
                this.TransactionRollback();
                result = false;
            }
            return result;
        }

        /// <summary>
        /// RT-Удалить все строки из указанной таблицы
        /// </summary>
        /// <param name="table">Название таблицы</param>
        internal void ClearTable(string table)
        {
            //DELETE FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 600;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "DELETE FROM {0};", table);
            m_cmdWithoutArguments.CommandText = query;
            m_cmdWithoutArguments.ExecuteNonQuery(); 

            return;
        }

        /// <summary>
        /// NT-Update UsingCounter in any table
        /// </summary>
        /// <param name="table">table name</param>
        /// <param name="id">record id</param>
        internal void updateUsingCounter(String table, int id)
        {
            //UPDATE pics SET pics.cnt = pics.cnt+1 WHERE (((pics.id)=1));
            if (this.dbReadOnly == true) return;
            if (m_cmdUpdateUsings == null)
            {
                m_cmdUpdateUsings = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdUpdateUsings.CommandTimeout = 60;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET {0}.cnt = {0}.cnt+1 WHERE ((({0}.id)={1}));", table, id);
            m_cmdWithoutArguments.CommandText = query;
            m_cmdUpdateUsings.ExecuteNonQuery(); //Тут могут быть исключения из-за другого типа данных

            return;
        }
        #endregion

        #region *** Для таблиц Документов и изображений ***
        /// <summary>
        /// NT-Get list of file records in range of id's
        /// </summary>
        /// <param name="table">Documents table name</param>
        /// <param name="idFrom">start id for SELECT</param>
        /// <param name="idTo">end (non-included) id for SELECT</param>
        /// <returns>List of records</returns>
        internal List<FileRecord> getFileRecordsBetween(string table, int idFrom, int idTo)
        {
            //SELECT * FROM table WHERE((id >= idFrom) AND (id < idTo));

            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
            }
            //create command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE((id >= {1}) AND (id < {2}));", table, idFrom, idTo);
            m_cmdWithoutArguments.CommandText = query;
            //execute command
            //read rows
            OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
            List<FileRecord> lir = new List<FileRecord>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                    lir.Add(readFileRow(rdr));
            }
            //close reader
            rdr.Close();
            return lir;
        }
        /// <summary>
        /// NT-получить запись файла по идентификатору записи
        /// </summary>
        /// <param name="table">Documents table name</param>
        /// <param name="id">Идентификатор записи</param>
        /// <returns>Возвращает объект записи файла или null</returns>
        internal FileRecord getFileRecordById(string table, int id)
        {
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
            }
            //create command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE (id = {1});", table, id);
            m_cmdWithoutArguments.CommandText = query;
            //execute command
            //read rows
            OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
            FileRecord fr = null;
            if (rdr.HasRows)
            {
                while (rdr.Read())
                    fr = readFileRow(rdr);
            }
            //close reader
            rdr.Close();
            return fr;
        }

        /// <summary>
        /// NT-Read one file row
        /// </summary>
        /// <param name="rdr">Data reader object</param>
        /// <returns></returns>
        private FileRecord readFileRow(OleDbDataReader rdr)
        {
            FileRecord r = new FileRecord(this.m_manager);
            r.TableId = rdr.GetInt32(0);
            r.StoragePath = rdr.GetString(1);
            r.DocumentType = rdr.GetString(2);
            r.Description = rdr.GetString(3);
            r.UsingCounter = rdr.GetInt32(4);
            r.Crc = Utility.FromHexString(rdr.GetString(5));
            r.Length = Int64.Parse(rdr.GetString(6), CultureInfo.InvariantCulture);
            //r.ExternalPath - not for db

            return r;
        }

        /// <summary>
        /// NT-Получить список дубликатов файла 
        /// </summary>
        ///<param name="table">Имя таблицы</param>
        ///<param name="length">Значение длины файла</param>
        ///<param name="crc">Значение контрольной суммы</param>
        /// <returns>Вернуть набор найденных записей дубликатов</returns>
        internal FileRecord[] GetFileDublicates(string table, long length, UInt32 crc)
        {
            //SELECT * FROM table WHERE((len = length) AND (crc = crc));

            if (m_cmdGetDublicates == null)
            {
                m_cmdGetDublicates = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdGetDublicates.CommandTimeout = 60;
                //parameters
                m_cmdGetDublicates.Parameters.Add("@p0", OleDbType.VarWChar);
                m_cmdGetDublicates.Parameters.Add("@p1", OleDbType.VarWChar);
            }
            //refresh query for "table" value
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE((len = ?) AND (crc = ?));", table);
            m_cmdGetDublicates.CommandText = query;
            //execute command
            m_cmdGetDublicates.Parameters[0].Value = length.ToString();
            m_cmdGetDublicates.Parameters[1].Value = Utility.ToHexString(crc);
            OleDbDataReader rdr = m_cmdGetDublicates.ExecuteReader();
            List<FileRecord> lir = new List<FileRecord>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                    lir.Add(readFileRow(rdr));
            }
            //close reader
            rdr.Close();
            return lir.ToArray();
        }
        /// <summary>
        /// NT-Получить список путей файлов подобных указанному имени файла
        /// </summary>
        /// <param name="table">Имя таблицы файлов</param>
        /// <param name="name">Имя файла без расширения</param>
        /// <returns>Возвращает словарь имен файлов, подобных указанному</returns>
        internal Dictionary<String, int> GetFileNamesLike(string table, string name)
        {
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 300;
            }
            //create command
            String pattern = "%" + name + "%";
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT filepath FROM {0} WHERE(filepath LIKE '{1}');", table, pattern);
            m_cmdWithoutArguments.CommandText = query;
            //execute command
            OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
            Dictionary<String, int> lir = new Dictionary<String, int>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    String s = rdr.GetString(0);
                    s = Path.GetFileNameWithoutExtension(s).ToLowerInvariant();
                    lir.Add(s, 0);
                }
            }
            //close reader
            rdr.Close();
            return lir;
        }

        /// <summary>
        /// NT-Получить словарь уникальных имен файлов
        /// </summary>
        /// <param name="table">Имя таблицы файлов</param>
        /// <returns>Возвращает словарь уникальных имен файлов без расширений и каталогов.</returns>
        internal Dictionary<String, int> GetFileNamesDictionary(String table)
        {
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 300;
            }
            //create command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT filepath FROM {0};", table);
            m_cmdWithoutArguments.CommandText = query;
            //execute command
            OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
            Dictionary<String, int> result = new Dictionary<string, int>(64);
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    String n = rdr.GetString(0);
                    n = Path.GetFileNameWithoutExtension(n).ToLowerInvariant();
                    result.Add(n, 0);
                    //TODO: Это будет работать, если все имена файлов уникальные. Так и должно быть по замыслу, но, возможно, это не так из-за старого кода.
                    //который не проверял файлы внутри буфера. Из-за чего они могли иметь одинаковые имена. 
                    //Если такие ошибки есть в старых хранилищах, этот код тут будет выбрасывать исключение.
                    //Хотя старые хранилища незачем дополнять новыми файлами в пакетном режиме. Поэтому проблем не должно возникнуть.
                }
            }
            //close reader
            rdr.Close();
            return result;
        }
        /// <summary>
        /// NT-записать файл в БД. И сразу же получить и вписать его ИД в объект.
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="fr">File record object</param>
        internal void InsertFileRecord(string table, FileRecord fr)
        {
            //INSERT INTO table (filepath, type, descr, cnt, crc, len) VALUES (?,?,?,?,?,?);
            if (m_cmdInsertFileRecord == null)
            {
                m_cmdInsertFileRecord = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdInsertFileRecord.CommandTimeout = 60;
                //parameters
                m_cmdInsertFileRecord.Parameters.Add("@p0", OleDbType.VarWChar);
                m_cmdInsertFileRecord.Parameters.Add("@p1", OleDbType.VarWChar);
                m_cmdInsertFileRecord.Parameters.Add("@p2", OleDbType.VarWChar);
                m_cmdInsertFileRecord.Parameters.Add("@p3", OleDbType.Integer);
                m_cmdInsertFileRecord.Parameters.Add("@p4", OleDbType.VarWChar);
                m_cmdInsertFileRecord.Parameters.Add("@p5", OleDbType.VarWChar);
            }
            //refresh query for "table" value
            string query = String.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} (filepath, type, descr, cnt, crc, len) VALUES (?,?,?,?,?,?);", table);
            m_cmdInsertFileRecord.CommandText = query;
            //execute command
            m_cmdInsertFileRecord.Parameters[0].Value = fr.StoragePath;
            m_cmdInsertFileRecord.Parameters[1].Value = fr.DocumentType;
            m_cmdInsertFileRecord.Parameters[2].Value = fr.Description;
            m_cmdInsertFileRecord.Parameters[3].Value = fr.UsingCounter;
            m_cmdInsertFileRecord.Parameters[4].Value = Utility.ToHexString(fr.Crc);
            m_cmdInsertFileRecord.Parameters[5].Value = fr.Length.ToString();

            m_cmdInsertFileRecord.ExecuteNonQuery();

            //get record id from table
            GetFileRecordId(table, fr);

            return;
        }
        /// <summary>
        /// NT-Получить из таблицы TableId и вписать его в объект записи файла
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="fr">Объект записи файла</param>
        private void GetFileRecordId(string table, FileRecord fr)
        {
            //StoragePath должен оказаться уникальный для каждого файла - по нему и будем искать нашу запись и ид 
            String query = String.Format("SELECT id FROM {0} WHERE ( filepath = '{1}');", table, fr.StoragePath);
            OleDbCommand cmd = new OleDbCommand(query, m_connection, m_transaction);
            //execute command
            OleDbDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    fr.TableId = rdr.GetInt32(0);
                }
            }
            else throw new Exception(String.Format("Error: cannot determine TableId for {0}!", fr.StoragePath));
            //close reader
            rdr.Close();
            return;
        }

        #endregion

        #region *** Для таблицы сущностей ***
        /// <summary>
        /// NT-Получить массив сущностей в диапазоне идентификаторов
        /// </summary>
        /// <param name="idFrom">Начальный идентификатор диапазона</param>
        /// <param name="idTo">Конечный, не включаемый идентификатор диапазона</param>
        /// <returns></returns>
        internal EntityRecord[] getEntityRecordsBetween(int idFrom, int idTo)
        {
            //SELECT * FROM table WHERE((id >= idFrom) AND (id < idTo));

            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
            }
            //create command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE((id >= {1}) AND (id < {2}));", DbAdapter.ContentTableName, idFrom, idTo);
            m_cmdWithoutArguments.CommandText = query;
            //execute command
            //read rows
            OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
            List<EntityRecord> lir = new List<EntityRecord>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                    lir.Add(readEntityRow(rdr));
            }
            //close reader
            rdr.Close();
            //прицепить записи файлов
            return this.AddLinkedFileRecords(lir);
        }
        /// <summary>
        /// NT-Получить запись сущности по ее идентификатору в таблице сущностей
        /// </summary>
        /// <param name="tableid">Идентификатор записи сущности в таблице сущностей</param>
        /// <returns>Возвращает запись сущности или null если записи с таким идентификатором не существует.</returns>
        internal EntityRecord getEntityById(int tableid)
        {
            EntityRecord[] ers = this.getEntityRecordsBetween(tableid, tableid + 1);
            if (ers.Length == 0)
                return null;
            else 
                return ers[0];
        }

        /// <summary>
        /// NT-Получить массив сущностей по названию
        /// </summary>
        /// <param name="entityName">Название сущности</param>
        /// <returns></returns>
        internal EntityRecord[] getEntityByName(string entityName)
        {
            //SELECT * FROM content WHERE (name = ?)
            if (m_cmdGetEntityByName == null)
            {
                String query = "SELECT * FROM content WHERE (name = ?)";
                m_cmdGetEntityByName = new OleDbCommand(query, m_connection, m_transaction);
                m_cmdGetEntityByName.CommandTimeout = 60;
                //arguments
                m_cmdGetEntityByName.Parameters.Add(new OleDbParameter("@p0", OleDbType.VarWChar));
            }
            //execute
            m_cmdGetEntityByName.Parameters[0].Value = entityName;
            OleDbDataReader rdr = this.m_cmdGetEntityByName.ExecuteReader();
            List<EntityRecord> lir = new List<EntityRecord>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                    lir.Add(readEntityRow(rdr));
            }
            //close reader
            rdr.Close();
            //присоединить записи файлов
            return this.AddLinkedFileRecords(lir);
        }
        /// <summary>
        /// NFT-Выбрать записи сущностей по шаблону
        /// </summary>
        /// <param name="searchPattern">Шаблон для поиска в формате со звездочками</param>
        /// <param name="searchTitle">Искать в поле названия сущности</param>
        /// <param name="searchDescription">Искать в поле описания сущности</param>
        /// <param name="searchType">Искать в поле класса сущности</param>
        /// <param name="count">Ограничение размера выборки, записей</param>
        /// <returns></returns>
        internal EntityRecord[] getEntityLike(string searchPattern, bool searchTitle, bool searchDescription, bool searchType, int count)
        {
            //SELECT * FROM content WHERE(title LIKE 'pattern');
            if (m_cmdGetEntityByLike == null)
            {
                m_cmdGetEntityByLike = new OleDbCommand(String.Empty, m_connection, m_transaction);
                m_cmdGetEntityByLike.CommandTimeout = 600;
                //arguments
                m_cmdGetEntityByLike.Parameters.Add(new OleDbParameter("@p0", OleDbType.VarWChar));
                m_cmdGetEntityByLike.Parameters.Add(new OleDbParameter("@p1", OleDbType.VarWChar));
                m_cmdGetEntityByLike.Parameters.Add(new OleDbParameter("@p2", OleDbType.VarWChar));
            }
            //set query
            String query = makeLikeQuery(searchTitle, searchDescription, searchType, count);
            m_cmdGetEntityByLike.CommandText = query;
            //хотя аргумент всего один, повторим его трижды, поскольку в строке запроса он встречается от 1 до 3 раз.
            String patternDb = searchPattern.Replace('*', '%');
            m_cmdGetEntityByLike.Parameters[0].Value = patternDb;
            m_cmdGetEntityByLike.Parameters[1].Value = patternDb;
            m_cmdGetEntityByLike.Parameters[2].Value = patternDb;
            //execute
            OleDbDataReader rdr = this.m_cmdGetEntityByLike.ExecuteReader();
            List<EntityRecord> lir = new List<EntityRecord>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                    lir.Add(readEntityRow(rdr));
            }
            //close reader
            rdr.Close();
            //присоединить записи файлов
            return this.AddLinkedFileRecords(lir);
        }
        /// <summary>
        /// RT-собрать строку запроса для поиска элементов
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchTitle"></param>
        /// <param name="searchDescription"></param>
        /// <param name="searchType"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private string makeLikeQuery(bool searchTitle, bool searchDescription, bool searchType, int limit)
        {
            StringBuilder sb = new StringBuilder(256);
            String pat = " LIKE ?) OR ";
            sb.Append(String.Format("SELECT TOP {0} * FROM {1} WHERE (", limit, DbAdapter.ContentTableName));
            //check search flags
            if (searchTitle) { sb.Append(" (name "); sb.Append(pat); }
            if (searchDescription) { sb.Append(" (descr "); sb.Append(pat); }
            if (searchType) { sb.Append(" (cat "); sb.Append(pat); }
            sb.Append("(0=1));");

            return sb.ToString();
        }

        /// <summary>
        /// NT-Прицепить записи файлов к записям сущностей.
        /// </summary>
        /// <param name="lir">Список записей сущностей</param>
        /// <returns></returns>
        private EntityRecord[] AddLinkedFileRecords(List<EntityRecord> lir)
        {

            if (lir.Count > 0)
            {
                //2 get linked files for each entity
                //3 connect linked files for each entity 
                //выявляем и кешируем требуемые идентификаторы записей и сами записи
                //поочередно документы и изображения, чтобы не расходовать память на больших выборках
                //хотя они вряд ли будут, но все же.
                Dictionary<int, FileRecord> fileDict = new Dictionary<int, FileRecord>();
                int id;
                FileRecord fr;
                //for documents
                foreach (EntityRecord rec in lir)
                {
                    //for docs
                    id = rec.DocId;
                    if (id > 0)
                    {
                        //если запись файла уже есть в кэше, вставляем ее в запись сущности
                        if (fileDict.ContainsKey(id))
                            rec.Document = fileDict[id];
                        else //а если нет - получаем из БД и вставляем и в кэш и в запись сущности.
                        {
                            fr = this.getFileRecordById(DbAdapter.DocumentTableName, id);
                            if (fr == null)
                                throw new Exception(String.Format("Error: document file not found: id={0}", id));
                            //use record
                            rec.Document = fr;
                            fileDict.Add(id, fr);
                        }
                    }
                }
                //clear dictionary for new files processing
                fileDict.Clear();
                //for pictures
                foreach (EntityRecord rec in lir)
                {
                    //for pics
                    id = rec.PicId;
                    if (id > 0)
                    {
                        //если запись файла уже есть в кэше, вставляем ее в запись сущности
                        if (fileDict.ContainsKey(id))
                            rec.Picture = fileDict[id];
                        else //а если нет - получаем из БД и вставляем и в кэш и в запись сущности.
                        {
                            fr = this.getFileRecordById(DbAdapter.PictureTableName, id);
                            if (fr == null)
                                throw new Exception(String.Format("Error: picture file not found: id={0}", id));
                            //use record
                            rec.Picture = fr;
                            fileDict.Add(id, fr);
                        }
                    }
                }
            }
            return lir.ToArray();
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns></returns>
        private EntityRecord readEntityRow(OleDbDataReader rdr)
        {
            EntityRecord er = new EntityRecord(this.m_manager);
            er.TableId = rdr.GetInt32(0);
            //из короткого пути класса сделать полный, подставив БазовыйКлассХранилища, по правилам.
            er.EntityType = new ClassItem(this.m_manager.StorageBaseClass, rdr.GetString(1));//THEME: CLSIT - переработать все места по этой теме
            er.Title = rdr.GetString(2);
            er.Description = rdr.GetString(3);
            er.UsingCounter = rdr.GetInt32(4);
            er.DocId = rdr.GetInt32(5);
            er.PicId = rdr.GetInt32(6);
            return er;
        }
        /// <summary>
        /// NR-Вставить запись сущности в таблицу сущностей
        /// </summary>
        /// <param name="entity"></param>
        internal void InsertEntityRecord(EntityRecord er)
        {
            //INSERT INTO content (cat, name, descr, cnt, doc, pic) VALUES (?,?,?,?,?,?);
            if (m_cmdInsertEntityRecord == null)
            {
                String query = "INSERT INTO content (cat, name, descr, cnt, doc, pic) VALUES (?,?,?,?,?,?);";
                m_cmdInsertEntityRecord = new OleDbCommand(query, this.m_connection, m_transaction);
                m_cmdInsertEntityRecord.CommandTimeout = 60;
                //parameters
                m_cmdInsertEntityRecord.Parameters.Add("@p0", OleDbType.VarWChar);
                m_cmdInsertEntityRecord.Parameters.Add("@p1", OleDbType.VarWChar);
                m_cmdInsertEntityRecord.Parameters.Add("@p2", OleDbType.VarWChar);
                m_cmdInsertEntityRecord.Parameters.Add("@p3", OleDbType.Integer);
                m_cmdInsertEntityRecord.Parameters.Add("@p4", OleDbType.Integer);
                m_cmdInsertEntityRecord.Parameters.Add("@p5", OleDbType.Integer);
            }

            //execute command
            //из полного пути класса сделать короткий, обрезав по БазовыйКлассХранилища, по правилам.
            m_cmdInsertEntityRecord.Parameters[0].Value = er.EntityType.GetShortClassPath(this.m_manager.StorageBaseClass);//THEME: CLSIT - переработать все места по этой теме
            m_cmdInsertEntityRecord.Parameters[1].Value = er.Title;
            m_cmdInsertEntityRecord.Parameters[2].Value = er.Description;
            m_cmdInsertEntityRecord.Parameters[3].Value = er.UsingCounter;
            ////тут надо выявить идентификаторы файлов для сущности        
            m_cmdInsertEntityRecord.Parameters[4].Value = er.getDocumentId();
            m_cmdInsertEntityRecord.Parameters[5].Value = er.getPictureId();

            m_cmdInsertEntityRecord.ExecuteNonQuery();
            //TODO: а надо ли потом вернуть вписать идентификатор новой записи?
            return;
        }

        #endregion

        #region *** Для таблицы свойств хранилища ***
        /// <summary>
        /// NR-Вписать значения свойств Хранилища из таблицы БД
        /// </summary>
        /// <param name="si"></param>
        internal void getStorageInfo(StorageInfo si)
        {
            //si.Version = ""; //Версия Хранилища подобно версии 1.
            //si.Creator = ""; //Создатель хранилища подобно версии 1.
            //si.Title = "";   //Название хранилища вроде "Микросхемы" - для версии 2.
            //si.QualifiedName = ""; //Полное квалификационное имя Хранилища, вроде "Radiodata.Микросхемы" - для версии 2.
            //si.Description = ""; //Текст описания Хранилища подобно версии 1.
            //si.Type = ""; //Тип данных Хранилища в нотации как у Оператора. Хранилище это контейнер, так вот что он хранит?  - для версии 2.
            
            //create command
            String query = String.Format("SELECT * FROM {0};", AboutTableName);
            OleDbCommand cmd = new OleDbCommand(query, this.m_connection);
            cmd.CommandTimeout = 120;
            //execute command
            OleDbDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    //int id = rdr.GetInt32(0); //id not used
                    String param = rdr.GetString(1);
                    String val = rdr.GetString(2);
                    //store to StorageInfo object 
                    si.setValue(param, val);
                }
            }
            //close reader
            rdr.Close();

            //Эти поля надо переписать по актуальным значениям
            si.EntityCount = this.GetRowCount(DbAdapter.ContentTableName);//Число записей в таблице сущностей
            si.UsingCounter++; //число использований Хранилища - сейчас как число запусков Хранилища.
            // TODO: А вообще же, надо сделать поле в менеджере и отслеживать каждый запрос к хранилищу.
            return;
        }

  

        /// <summary>
        /// NT - обновить информацию о хранилище
        /// </summary>
        /// <param name="si">Новая информация о хранилище</param>
        internal void storeStorageInfo(StorageInfo si)
        {
            //TODO: В основном это работает, но на асус4 периодически выдает исключение
               //System.InvalidOperationException: Коллекция была изменена; невозможно выполнить операцию перечисления.
               //в System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
               //в System.Collections.Generic.Dictionary`2.Enumerator.MoveNext()
               //в InventoryModuleManager2.DbAdapter.storeStorageInfo(StorageInfo si)
            //причем эта функция вызывается каждый раз при закрытии хранилища, и перезаписывает его свойства.
            //из-за чего они теряются и хранилище в следующий раз не открывается.
            //Тут надо заменить код на обновление строк таблицы, а не пересоздание их
            //именно из-за косяков с АСУС4 ноутом.
            //Большая часть полей вообще не нуждается в перезаписи при каждом завершении работы,
            //а остальные - получаются из других источников, а не из БД.
            //Тут только UsingCounter вроде бы обновляется каждый раз и хранится в БД. А более ничего.
            //Вот надо это все переделать - поля создавать или при создании БД, или в шаблоне БД сразу.
            //А обновлять только здесь и только тогда, когда это нужно. Прямо по каждому полю отдельно сделать.
            //С набором полей я уже определился, и меняться они не будут, я думаю.
            //Поэтому можно все нужные поля явно указывать по именам.
            

            //1 - очистить таблицу
            String query = String.Format("DELETE * FROM {0};", AboutTableName);
            OleDbCommand cmd = new OleDbCommand(query, this.m_connection);
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            //2 - записать новые значения
            cmd.CommandText = String.Format("INSERT INTO {0} (param, val) VALUES (?, ?);", AboutTableName);
            cmd.Parameters.Add(new OleDbParameter("@p0", OleDbType.VarWChar));
            cmd.Parameters.Add(new OleDbParameter("@p1", OleDbType.VarWChar));
            //execute commands
            Dictionary<string, string> di = si.getBaseDictionary();
            foreach (KeyValuePair<String, String> kvp in di)
            {
                cmd.Parameters[0].Value = kvp.Key;
                cmd.Parameters[1].Value = kvp.Value;
                cmd.ExecuteNonQuery();
            }

            return;
        }

        /// <summary>
        /// NT-Получить значение свойства Хранилища. 
        /// </summary>
        /// <param name="keyname">Название свойства из StorageInfo.* констант</param>
        /// <returns></returns>
        internal string getStorageInfoValue(string keyname)
        {
            //THEME: QNAME - переработать все места по этой теме
            //create command
            String query = String.Format("SELECT * FROM {0} WHERE (param = '{1}' );", AboutTableName, keyname);
            OleDbCommand cmd = new OleDbCommand(query, this.m_connection);
            cmd.CommandTimeout = 120;
            //execute command
            OleDbDataReader rdr = cmd.ExecuteReader();
            String result = String.Empty;
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    //int id = rdr.GetInt32(0); //id not used
                    //String param = rdr.GetString(1);//param not used
                    result = rdr.GetString(2);
                }
            }
            //close reader
            rdr.Close();
            return result;
        }

        #endregion

        #region новое с 05 окт 2017


        /// <summary>
        /// NT-Получить список уникальных значений категорий сущностей
        /// </summary>
        /// <returns>Возвращает список уникальных значений категорий сущностей из БД</returns>
        internal List<string> GetEntityCategoryValues()
        {
            //SELECT DISTINCT content.cat FROM content ORDER BY cat;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
            }
            //create command
            string query = "SELECT DISTINCT content.cat FROM content ORDER BY cat;";
            m_cmdWithoutArguments.CommandText = query;
            //execute command
            //read rows
            OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
            List<String> lir = new List<String>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    lir.Add(rdr.GetString(0));
                }
            }
            //close reader
            rdr.Close();

            return lir;
        }

        #endregion


    }
}
