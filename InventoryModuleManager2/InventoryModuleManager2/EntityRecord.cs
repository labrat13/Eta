using System;
using System.Collections.Generic;
using System.Text;
using InventoryModuleManager2.ClassificationEntity;

namespace InventoryModuleManager2
{
    /// <summary>
    /// Представляет запись о Сущности для БД итд
    /// </summary>
    public class EntityRecord: RecordBase
    {
        //TODO: Этот класс используется внутри и вне сборки.
        //Поэтому он должен и содержать переменные-свойства, и поддерживать строковый набор данных.
        //Чтобы выводить данные как строковый набор и загружать данные из строкового набора.
        //И быстро работать, как обычный класс с типизированными переменными.
        //Поэтому обычно словарь пустой, а работа идет через переменные
        //А при передаче из сборки словарь заполняется данными из полей - делается отдельная копия объекта записи с пустыми полями.
        //Хотя это все выглядит замороченно. Возможно, есть более простой способ...

        #region *** Переменные ***
        /// <summary>
        /// первичный ключ ТаблицаСущностей
        /// </summary>
        private int m_id;
        /// <summary>
        /// Название категории сущности в нотации Оператора
        /// </summary>
        //private string m_type;//THEME: CLSIT - переработать все места по этой теме
        private ClassItem m_type;
        /// <summary>
        /// Название предмета, однозначно идентифицирующее предмет, например, марка детали
        /// </summary>
        private string m_title;
        /// <summary>
        /// Описание предмета - перечисление параметров предмета
        /// </summary>
        private string m_description;
        /// <summary>
        /// Счетчик СтепеньИспользования сущности
        /// </summary>
        private int m_usingCounter;
        /// <summary>
        /// присоединенный ЗаписьФайла
        /// </summary>
        private FileRecord m_document;
        /// <summary>
        /// присоединенный ЗаписьФайла
        /// </summary>
        private FileRecord m_picture;
        /// <summary>
        /// внутренний идентификатор записи документа
        /// </summary>
        private int m_docId;

        /// <summary>
        /// внутренний идентификатор записи изображения
        /// </summary>
        private int m_picId;

        /// <summary>
        /// Ссылка на менеджер для поддержки некоторых функций
        /// </summary>
        private Manager m_manager;
        #endregion

        /// <summary>
        /// Конструктор. Словарь не заполняется.
        /// </summary>
        public EntityRecord()
        {
            m_type = new ClassItem();
            m_description = string.Empty;
            m_document = null;
            m_id = 0;
            m_picture = null;
            m_title = String.Empty;
            m_usingCounter = 0;

            m_manager = null; //manager reference
            
            return;
        }
        /// <summary>
        /// Конструктор. Словарь не заполняется.
        /// </summary>
        public EntityRecord(Manager man)
        {
            m_type = new ClassItem();
            m_description = string.Empty;
            m_document = null;
            m_id = 0;
            m_picture = null;
            m_title = String.Empty;
            m_usingCounter = 0;

            m_manager = man; //manager reference
            return;
        }

        #region *** Проперти ***
        /// <summary>
        /// присоединенный ЗаписьФайла
        /// </summary>
        public FileRecord Picture
        {
            get { return m_picture; }
            set { m_picture = value; }
        }

        /// <summary>
        /// присоединенный ЗаписьФайла
        /// </summary>
        public FileRecord Document
        {
            get { return m_document; }
            set { m_document = value; }
        }

        /// <summary>
        /// Счетчик СтепеньИспользования сущности
        /// </summary>
        public int UsingCounter
        {
            get { return m_usingCounter; }
            internal set { m_usingCounter = value; }
        }

        /// <summary>
        /// Описание предмета - перечисление параметров предмета
        /// </summary>
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// Название предмета, однозначно идентифицирующее предмет, например, марка детали
        /// </summary>
        public string Title
        {
            get { return m_title; }
            set { m_title = value; }
        }

        /// <summary>
        /// Название категории предмета
        /// </summary>
        public ClassItem EntityType  //THEME: CLSIT - переработать все места по этой теме
        {
            get { return m_type; }
            set { m_type = value; }
        }

        /// <summary>
        /// первичный ключ ТаблицаСущностей
        /// </summary>
        public int TableId
        {
            get { return m_id; }
            internal set { m_id = value; }
        }

        /// <summary>
        /// внутренний идентификатор записи изображения
        /// </summary>
        internal int PicId
        {
            get { return m_picId; }
            set { m_picId = value; }
        }
        /// <summary>
        /// внутренний идентификатор записи документа
        /// </summary>
        internal int DocId
        {
            get { return m_docId; }
            set { m_docId = value; }
        }

        /// <summary>
        /// Получить ссылку на менеджер Хранилища
        /// </summary>
        /// <returns></returns>
        public Manager getStorageManager()
        {
            return this.m_manager;
        }

        #endregion

        #region *** Функции ***
        /// <summary>
        /// NT-
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}-{1}-{2}", this.m_id, this.m_title, this.m_type.ToString());
        }
        /// <summary>
        /// NT-получить ссылку на объект сущности
        /// </summary>
        /// <returns></returns>
        public string GetEntityLink()
        {
            if (this.m_manager == null)
                throw new Exception("Invalid Manager reference");//ссылка на менеджер не установлена

            LinkBuilder lb = new LinkBuilder();
            //lb.Linktype = LinkType.EntityRecordLink; не используется тут
            lb.StorageQName = this.m_manager.QualifiedName;
            lb.TableId = this.m_id;
            lb.Title = this.m_title;

            return lb.getEntityRecordLink();
        }

        /// <summary>
        /// NT-получить ссылку на объект сущности
        /// </summary>
        /// <returns></returns>
        public string GetDocumentLink()
        {
            if (this.m_manager == null)
                throw new Exception("Invalid Manager reference");//ссылка на менеджер не установлена
            //если документа нет, возвращаем пустую строку сейчас
            if (this.m_document == null)
                return String.Empty;
            //else
            LinkBuilder lb = new LinkBuilder();
            lb.StorageQName = this.m_manager.QualifiedName;
            lb.TableId = m_document.TableId;
            lb.Title = this.m_document.StoragePath;
        
            return lb.getDocumentRecordLink();
        }

        /// <summary>
        /// NT-получить ссылку на объект сущности
        /// </summary>
        /// <returns></returns>
        public string GetPictureLink()
        {
            if (this.m_manager == null)
                throw new Exception("Invalid Manager reference");//ссылка на менеджер не установлена
            //если изображения нет, возвращаем пустую строку сейчас
            if (this.m_picture == null)
                return String.Empty;
            //else
            LinkBuilder lb = new LinkBuilder();
            lb.StorageQName = this.m_manager.QualifiedName;
            lb.TableId = this.m_picture.TableId;
            lb.Title = this.m_picture.StoragePath;

            return lb.getPictureRecordLink();
        }

        /// <summary>
        /// NR-Присоединить внешний файл документа
        /// </summary>
        /// <param name="filepath">Путь к файлу</param>
        public void SetDocument(string filepath)
        {
            m_document = new FileRecord(filepath);
        }
        /// <summary>
        /// NR-Присоединить внешний файл изображения
        /// </summary>
        /// <param name="filepath">Путь к файлу</param>
        public void SetPicture(string filepath)
        {
            m_picture = new FileRecord(filepath);
        }
        /// <summary>
        /// NT-Отцепить присоединенный документ
        /// </summary>
        public void RemoveDocument()
        {
            m_document = null;
        }
        /// <summary>
        /// NT-Отцепить присоединенное изображение
        /// </summary>
        public void RemovePicture()
        {
            m_picture = null;
        }

        /// <summary>
        /// NT-Флаг что запись сущности имеет новый внешний файл
        /// </summary>
        /// <returns></returns>
        internal bool HaveNewDoc()
        {
            if (this.m_document != null)
                if (!String.IsNullOrEmpty(this.m_document.ExternalPath))
                    return true;
            return false;
        }
        /// <summary>
        /// NT-Флаг что запись сущности имеет новый внешний файл
        /// </summary>
        /// <returns></returns>
        internal bool HaveNewPic()
        {
            if (this.m_picture != null)
                if (!String.IsNullOrEmpty(this.m_picture.ExternalPath))
                    return true;
            return false;
        }

        /// <summary>
        /// NT-Получить идентификатор документа в таблице документов или 0
        /// </summary>
        /// <returns></returns>
        internal int getDocumentId()
        {
            int result = 0;
            if (this.m_document != null)
                result = this.m_document.TableId;
            return result;
        }
        /// <summary>
        /// NT-Получить идентификатор изображения в таблице изображений или 0
        /// </summary>
        /// <returns></returns>
        internal int getPictureId()
        {
            int result = 0;
            if (this.m_picture != null)
                result = this.m_picture.TableId;
            return result;
        }


        #endregion


    }
}
