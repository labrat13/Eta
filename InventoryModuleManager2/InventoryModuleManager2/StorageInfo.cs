using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Threading;
using InventoryModuleManager2.ClassificationEntity;

namespace InventoryModuleManager2
{
    /// <summary>
    /// Предоставляет сводную информацию о Хранилище.
    /// Используется только вне этой сборки, внешними потребителями.
    /// </summary>
    /// <remarks>
    /// Содержит словарь, в котором хранятся все значения свойств хранилища, 
    /// функции для работы с файлом описания хранилища и проперти для доступа к свойствам хранилища.
    /// Читает или создает файл описания хранилища.
    /// </remarks>
    public class StorageInfo: RecordBase
    {
        
        #region Константы названий тегов словаря
        //они же используются в БД как имена свойств в таблице свойств хранилища.
        internal const string tagStoragePath  = "StoragePath";
        internal const string tagReadOnly  = "ReadOnly";
        internal const string tagStorageVersion  = "StorageVersion";
        internal const string tagEngineVersion  = "EngineVersion";
        internal const string tagCreator  = "Creator";
        internal const string tagTitle  = "Title";
        internal const string tagQualifiedName  = "QualifiedName";
        internal const string tagDescription  = "Description";
        internal const string tagStorageType  = "StorageType";
        internal const string tagPicsSize  = "PicsSize";
        internal const string tagDocsCount  = "DocsCount";
        internal const string tagDocsSize  = "DocsSize";
        internal const string tagPicsCount  = "PicsCount";
        internal const string tagEntityCount  = "EntityCount";
        internal const string tagDatabaseSize  = "DatabaseSize";
        internal const string tagUsingCounter  = "UsingCounter";
        //всего 16 штук
        #endregion

        internal const string StorageVersionString = "1.0";

        public StorageInfo()
        {
            //fill dictionary with default items
            m_dictionary.Clear();
            m_dictionary.Add(tagStoragePath, String.Empty);
            m_dictionary.Add(tagReadOnly, Boolean.FalseString);
            m_dictionary.Add(tagStorageVersion, StorageVersionString);//THEME: QNAME - если формат хранилища изменится, то и здесь надо изменить значение. И проверку совместимости сделать.
            m_dictionary.Add(tagEngineVersion, Manager.getEngineVersionString());
            m_dictionary.Add(tagCreator, "Павел Селяков");
            m_dictionary.Add(tagTitle, String.Empty);
            m_dictionary.Add(tagQualifiedName, String.Empty);//THEME: QNAME - переработать все места по этой теме
            m_dictionary.Add(tagDescription, String.Empty);
            m_dictionary.Add(tagStorageType, String.Empty);
            m_dictionary.Add(tagPicsSize, "0");
            m_dictionary.Add(tagDocsCount, "0");
            m_dictionary.Add(tagDocsSize, "0");
            m_dictionary.Add(tagPicsCount, "0");
            m_dictionary.Add(tagEntityCount, "0");
            m_dictionary.Add(tagDatabaseSize, "0");
            m_dictionary.Add(tagUsingCounter, "0");
            //total 15 items

        }

        //TODO: переделать все эти проперти на вызовы класса RecordBase
        //TODO: добавить еще свойство язык описания сущностей. Хотя обычно используется несколько языков, англ + русский.
        //Но как их оба вписать? я пишу русский.

        #region *** Properties ***
        /// <summary>
        /// Путь к корневому каталогу Хранилища
        /// </summary>
        public string StoragePath
        {
            get
            {
                return this.m_dictionary[tagStoragePath];
            }
            set
            {
                m_dictionary[tagStoragePath] = value;
            }
        }

        /// <summary>
        /// read-only flag
        /// </summary>
        /// <remarks>По умолчанию значение false.</remarks>
        public bool ReadOnly
        {
            get
            {
                //return Boolean.Parse(this.m_dictionary[]);
                return this.getValueAsBoolean(tagReadOnly);
            }
            internal set
            {
                this.setValue(tagReadOnly, value);
                //m_dictionary["ReadOnly"] = value.ToString();
            }
        }

        /// <summary>
        /// Версия структуры Хранилища
        /// </summary>
        public string StorageVersion
        {
            get
            {
                return this.m_dictionary[tagStorageVersion];
            }
            set
            {
                m_dictionary[tagStorageVersion] = value;
            }
        }

        /// <summary>
        /// Версия менеджера Хранилища
        /// </summary>
        public string EngineVersion
        {
            get
            {
                return this.m_dictionary[tagEngineVersion];
            }
            //internal set
            //{
            //    m_dictionary["EngineVersion"] = value;
            //}
            //тут вообще незачем его устанавливать вне собственного класса
        }

        /// <summary>
        /// Создатель Хранилища
        /// </summary>
        public string Creator
        {
            get
            {
                return this.m_dictionary[tagCreator];
            }
            set
            {
                m_dictionary[tagCreator] = value;
            }
        }

        /// <summary>
        /// Название Хранилища краткое, вроде "Микросхемы"
        /// </summary>
        /// <remarks>
        /// Не должно содержать символы :. так как используется в КвалифицированноеИмяХранилища.
        /// </remarks>
        public string Title
        {
            get
            {
                return this.m_dictionary[tagTitle];
            }
            set
            {
                m_dictionary[tagTitle] = value;
            }
        }



        /// <summary>
        /// Полное квалификационное имя Хранилища, вроде "Radiodata.Микросхемы"
        /// </summary>
        public string QualifiedName
        {
            //Также DbAdapter.getStorageQName() использует это название тега для доступа к значению. 
            get
            {
                return this.m_dictionary[tagQualifiedName];
            }
            set
            {
                m_dictionary[tagQualifiedName] = value;
            }
        }

        /// <summary>
        /// Текст описания Хранилища
        /// </summary>
        public string Description
        {
            get
            {
                return this.m_dictionary[tagDescription];
            }
            set
            {
                m_dictionary[tagDescription] = value;
            }
        }

        /// <summary>
        /// Тип данных Хранилища в нотации типов Оператора.
        /// </summary>
        public string StorageType
        {
            get
            {
                return this.m_dictionary[tagStorageType];
            }
            set
            {
                m_dictionary[tagStorageType] = value;
            }
        }

        /// <summary>
        /// Размер архивов документов
        /// </summary>
        public Int64 DocsSize
        {
            get
            {
                return Int64.Parse(this.m_dictionary[tagDocsSize], CultureInfo.InvariantCulture);
            }
            internal set
            {
                m_dictionary[tagDocsSize] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Число файлов в архивах документов
        /// </summary>
        public int DocsCount
        {
            get
            {
                return Int32.Parse(this.m_dictionary[tagDocsCount], CultureInfo.InvariantCulture);
            }
            internal set
            {
                m_dictionary[tagDocsCount] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Размер архивов изображений
        /// </summary>
        public long PicsSize
        {
            get
            {
                return Int64.Parse(this.m_dictionary[tagPicsSize], CultureInfo.InvariantCulture);
            }
            internal set
            {
                m_dictionary[tagPicsSize] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Число файлов в архивах изображений
        /// </summary>
        public int PicsCount
        {
            get
            {
                return Int32.Parse(this.m_dictionary[tagPicsCount], CultureInfo.InvariantCulture);
            }
            internal set
            {
                m_dictionary[tagPicsCount] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Число записей сущностей в БД
        /// </summary>
        public int EntityCount
        {
            //TODO: переделать все эти проперти на вызовы класса RecordBase
            get
            {
                return Int32.Parse(this.m_dictionary[tagEntityCount], CultureInfo.InvariantCulture);
            }
            internal set
            {
                m_dictionary[tagEntityCount] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Размер файла БД.
        /// </summary>
        public long DatabaseSize
        {
            get
            {
                return this.getValueAsInt64(tagDatabaseSize);
                //return Int64.Parse(this.m_dictionary["DatabaseSize"], CultureInfo.InvariantCulture); 
            }
            internal set
            {
                this.setValue(tagDatabaseSize, value);
                //m_dictionary["DatabaseSize"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
        /// <summary>
        /// Размер файла БД.
        /// </summary>
        public long UsingCounter
        {
            get
            {
                return this.getValueAsInt64(tagUsingCounter);

            }
            internal set
            {
                this.setValue(tagUsingCounter, value);
            }
        }

#endregion

        #region *** Functions ***
        /// <summary>
        /// NT-Получить информацию о хранилище
        /// </summary>
        /// <param name="storagePath">Корневой каталог хранилища</param>
        /// <returns>Объект информации о хранилище</returns>
        public static StorageInfo GetInfo(String storagePath)
        {
            String p = Path.Combine(storagePath, Manager.DescriptionFileName);
            StorageInfo si = new StorageInfo();
            si.LoadFromFile(p);
            return si;
        }

        internal static bool StoreInfo(StorageInfo si)
        {
            //1 create new file or replace existing file
            //это надо собирать здесь, так как менеджер не запущен.
            string file = Path.Combine(si.StoragePath, Manager.DescriptionFileName); 

            //TODO: Тут надо быстро подменить файл если он не заблокирован.
            //несколько раз пытаемся его удалить, делая перерывы по 100мс.
            //если все же не удалось, то возвращаем неудачу.
            //но не исключение. - все же это не критическая ошибка.
            //если кто-то занял файл так надолго, то он его и обновит наверно тогда.

            //это одинаковый алгоритм для первого создания файла и для обновления файла
            for (int i = 0; i < 5; i++)
            {
                if (!File.Exists(file))
                    break;
                else
                {
                    try
                    {
                        File.Delete(file);
                        break;
                    }
                    catch (Exception)
                    {
                        ;
                    }
                    Thread.Sleep(100);
                }
            }
            //тут если удаление удалось, то надо перезаписать файл.
            if (!File.Exists(file))
            {
                //create file
                si.StoreToFile(file);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// NFT-Записать данные в файл описания хранилища
        /// </summary>
        /// <param name="filepath"></param>
        public void StoreToFile(String filepath)
        {
            
            //1 write info to string builder first 
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings s = new XmlWriterSettings();
            s.Encoding = Encoding.Unicode;
            s.Indent = true;
            XmlWriter wr = XmlWriter.Create(sb, s);
            wr.WriteStartElement("StorageInfo");
            //wr.WriteAttributeString("EngineVersion", m_dictionary["EngineVersion"]); - это нельзя потом прочитать!
            //Записать предупреждение пользователю о бесполезности редактирования файла
            wr.WriteElementString("DoNotEdit", "Не редактировать! Этот файл автоматически перезаписывается.");// - прочие свойства заменим на более простой синтаксис ниже
            //вывести остальные свойства хранилища
            _writeElement(wr, tagEngineVersion);
            _writeElement(wr, tagStorageVersion);
            _writeElement(wr, tagCreator);
            _writeElement(wr, tagTitle);
            _writeElement(wr, tagStorageType);
            _writeElement(wr, tagStoragePath);
            _writeElement(wr, tagQualifiedName);//THEME: QNAME - переработать все места по этой теме
            _writeElement(wr, tagDescription);
            _writeElement(wr, tagReadOnly);
            _writeElement(wr, tagUsingCounter);
            _writeElement(wr, tagEntityCount);
            _writeElement(wr, tagDocsCount);
            _writeElement(wr, tagPicsCount);
            _writeElement(wr, tagDatabaseSize);
            _writeElement(wr, tagDocsSize);
            _writeElement(wr, tagPicsSize);
            wr.WriteEndElement();
            wr.Flush();
            //2 get file and store to it fast
            //TODO: тут нужен стойкий многопроцессный код с учетом возможных блокировок файла.
            //а пока тут простое решение для отладки тестов.
            StreamWriter sw = new StreamWriter(filepath, false, Encoding.Unicode);//пишем в юникоде а то все жалуются.
            sw.Write(sb.ToString());
            sw.Close();

            return;
        }
        /// <summary>
        /// RT-вывести свойство в xml-поток. Служебная функция для более простого синтаксиса.
        /// </summary>
        /// <param name="wr">Xml-писатель</param>
        /// <param name="p">Название элемента словаря и свойства</param>
        private void _writeElement(XmlWriter wr, string p)
        {
            wr.WriteElementString(p, m_dictionary[p]);
        }


        /// <summary>
        /// RT-Загрузить данные из файла описания хранилища
        /// </summary>
        /// <param name="filepath">Путь к файлу описания хранилища</param>
        public void LoadFromFile(String filepath)
        {
            //1 - read file to string buffer
            StreamReader sr = new StreamReader(filepath, Encoding.Unicode);
            String xml = sr.ReadToEnd();
            sr.Close();
            //2 - parse xml to data
            XmlReaderSettings s = new XmlReaderSettings();
            s.CloseInput = true;
            s.IgnoreWhitespace = true;
            XmlReader rd = XmlReader.Create(new StringReader(xml), s);
            rd.Read();
            rd.ReadStartElement("StorageInfo");

            //read file version info - Облом. XmlReader атрибуты не читает.
            //rd.MoveToNextAttribute();
            //if (rd.Name != "EngineVersion")
            //    throw new Exception("Error: Invalid format of description file!");
            //String fileversion = rd.Value;
            //rd.MoveToElement(); // move back to element node

            //skip message for user
            rd.ReadStartElement("DoNotEdit");
            rd.ReadString();
            rd.ReadEndElement();
            //read list of properties
            _readElement(rd, tagEngineVersion);
            _readElement(rd, tagStorageVersion);
            _readElement(rd, tagCreator);
            _readElement(rd, tagTitle);
            _readElement(rd, tagStorageType);
            _readElement(rd, tagStoragePath);
            _readElement(rd, tagQualifiedName);//THEME: QNAME - переработать все места по этой теме
            _readElement(rd, tagDescription);
            _readElement(rd, tagReadOnly);
            _readElement(rd, tagUsingCounter);
            _readElement(rd, tagEntityCount);
            _readElement(rd, tagDocsCount);
            _readElement(rd, tagPicsCount);
            _readElement(rd, tagDatabaseSize);
            _readElement(rd, tagDocsSize);
            _readElement(rd, tagPicsSize);
            //end of properties list
            rd.ReadEndElement();
            rd.Close();

            return;
        }

        /// <summary>
        /// NT-извлечь свойство из xml-потока. Служебная функция для более простого синтаксиса.
        /// </summary>
        /// <param name="rd"></param>
        /// <param name="p"></param>
        private void _readElement(XmlReader rd, string p)
        {
            rd.ReadStartElement(p);
            String s = rd.ReadString();
            m_dictionary[p] = s;
            rd.ReadEndElement();
            return;
        }


        /// <summary>
        /// RT- fill object with test values
        /// </summary>
        public void fillTestValues()
        {
            DatabaseSize = 777;
            Description = "test storage info";
            DocsCount = 7;
            DocsSize = 32768;
            EntityCount = 456;
            PicsCount = 99;
            PicsSize = 456;
            QualifiedName = "text.texst.Мурзилка 36";
            ReadOnly = false;
            StoragePath = "storage test path";
            StorageType = "storage type";
            StorageVersion = "1";
            Title = "Тест журнал мурзилка";
            UsingCounter = 445;

        }

        public override string ToString()
        {
            return String.Format("name={0};docs={1};pics={2}", this.QualifiedName, this.DocsCount, this.PicsCount);
        }
        #endregion



    }
}
