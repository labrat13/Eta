using System;
using System.Collections.Generic;
using System.Text;
using InventoryModuleManager2;
using System.Windows.Forms;
using InventoryModuleManager2.ClassificationEntity;

namespace StorageSearch
{
    internal class ResultItem
    {
#region Fields
        /// <summary>
        /// Название элемента
        /// </summary>
        private String m_Title;

        /// <summary>
        /// Описание элемента
        /// </summary>
        private String m_Description;

        /// <summary>
        /// Описание документа элемента
        /// </summary>
        private String m_DocDescription;

        /// <summary>
        /// Описание изображения элемента
        /// </summary>
        private String m_PicDescription;

        /// <summary>
        /// Категория элемента
        /// </summary>
        private ClassItem m_Category;

        /// <summary>
        /// Ссылка на элемент в хранилище
        /// </summary>
        private String m_Link;

        /// <summary>
        /// Флаг что итем имеет присоединенный документ
        /// </summary>
        private bool m_haveDocument;

        /// <summary>
        /// Флаг что итем имеет присоединенное изображение
        /// </summary>
        private bool m_havePicture;
        /// <summary>
        /// Путь к хранилищу
        /// </summary>
        private string m_storagePath;


#endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public ResultItem()
        {
            m_Category = new ClassItem();
            m_Description = String.Empty;
            m_DocDescription = String.Empty;
            m_Link = String.Empty;
            m_PicDescription = String.Empty;
            m_Title = String.Empty;
            m_haveDocument = false;
            m_havePicture = false;
        }
        /// <summary>
        /// Parameter constructor
        /// </summary>
        /// <param name="entity">Entity object</param>
        public ResultItem(EntityRecord entity)
        {
            this.m_Category = entity.EntityType;
            this.m_Description = entity.Description;
            //TODO: Тут надо вписать не ид таблицы а ссылку на сущность и хранилище
            this.m_Link = entity.GetEntityLink();
            this.m_Title = entity.Title;
            //получить путь к хранилищу для загрузки сущности для показа после поиска
            this.m_storagePath = entity.getStorageManager().StorageFolder;
            if (entity.Document != null)
            {
                this.m_DocDescription = entity.Document.Description;
                this.m_haveDocument = true;
            }
            if (entity.Picture != null)
            {
                this.m_PicDescription = entity.Picture.Description;
                this.m_havePicture = true;
            }
            return;
        }

#region Properties
        /// <summary>
        /// Название элемента
        /// </summary>
        public String Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        /// <summary>
        /// Описание элемента
        /// </summary>
        public String Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        
        /// <summary>
        /// Описание документа элемента
        /// </summary>
        public String DocDescription
        {
            get { return m_DocDescription; }
            set { m_DocDescription = value; }
        }

        /// <summary>
        /// Описание изображения элемента
        /// </summary>
        public String PicDescription
        {
            get { return m_PicDescription; }
            set { m_PicDescription = value; }
        }

        /// <summary>
        /// Категория элемента
        /// </summary>
        public ClassItem Category
        {
            get { return m_Category; }
            set { m_Category = value; }
        }

        /// <summary>
        /// Ссылка на элемент в хранилище
        /// </summary>
        public String Link
        {
            get { return m_Link; }
            set { m_Link = value; }
        }

        /// <summary>
        /// Флаг что итем имеет присоединенный документ
        /// </summary>
        public bool HaveDocument
        {
            get { return m_haveDocument; }
            set { m_haveDocument = value; }
        }
        /// <summary>
        /// Флаг что итем имеет присоединенное изображение
        /// </summary>
        public bool HavePicture
        {
            get { return m_havePicture; }
            set { m_havePicture = value; }
        }

        /// <summary>
        /// Путь к хранилищу
        /// </summary>
        public string StoragePath
        {
            get { return m_storagePath; }
            set { m_storagePath = value; }
        }

#endregion

        public override string ToString()
        {
            return m_Title + "; " + m_Link;
        }
    }
}
