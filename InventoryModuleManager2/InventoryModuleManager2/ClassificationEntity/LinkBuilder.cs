using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryModuleManager2.ClassificationEntity
{
    //    * Решение: вариант В:
    //    * формат ссылки: префикс://КвалифицированноеИмяХранилища/Код_НазваниеСущности 
    //    * код состоит из буквы: e - entity; d - doc; p - pic; и 6 цифр от 000001 до 999999. 
    //      В хранилище, таким образом, не может быть более 999999 файлов или записей сущностей.
    //    * после 7 символов кода может идти название сущности. 
    //      Если так, то 8 символом должен идти символ-разделитель _ или - (как удобнее будет выглядеть). 
    //    * название сущности не должно содержать символов, запрещенных в ссылках. 
    //      Но может содержать пробелы и русские символы. (todo: уточнить это)   
    //* все форматы ссылок:
    //    * префикс://КвалифицированноеИмяХранилища - ссылка на хранилище 
    //    * префикс://КвалифицированноеИмяХранилища/docs1/file1.ext - ссылка на файл документа
    //    * префикс://КвалифицированноеИмяХранилища/pics1/file1.ext - ссылка на файл изображения
    //    * префикс://КвалифицированноеИмяХранилища/e000001-КТ315Б - ссылка на запись сущности
    //    * префикс://КвалифицированноеИмяХранилища/d123456-file1.ext - ссылка на запись документа
    //    * префикс://КвалифицированноеИмяХранилища/p123456-img2.ext - ссылка на запись изображения 
    
    /// <summary>
    /// Класс сборки-разборки ссылок Хранилища
    /// </summary>
    public class LinkBuilder
    {
        /// <summary>
        /// Префикс для ссылок Хранилищ по умолчанию. TODO: установить префикс ссылок здесь
        /// </summary>
        internal const string DefaultLinkPrefix = "imms";
        /// <summary>
        /// Отделитель префикса ://
        /// </summary>
        private const string prePrefix = "://";

        private const string pathDelimiter = "/";

        private const char tagTitleDelimiter = '-';
        private const char tagEntityRecord = 'e';
        private const char tagDocumentRecord = 'd';
        private const char tagPictureRecord = 'p';


        #region *** Fields ***
        /// <summary>
        /// Префикс ссылки
        /// </summary>
        private string m_prefix;

        /// <summary>
        /// Квалифицированное имя хранилища
        /// </summary>
        private string m_storageQname;

        /// <summary>
        /// Тип ссылки
        /// </summary>
        private LinkType m_linktype;

        /// <summary>
        /// Идентификатор записи в соответствующей таблице БД
        /// </summary>
        private int m_tableId;

        /// <summary>
        /// Название сущности
        /// </summary>
        private string m_title;

        /// <summary>
        /// Путь файла в хранилище
        /// </summary>
        private string m_filepath;

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public LinkBuilder()
        {
            m_filepath = String.Empty;
            m_linktype = LinkType.Unknown;
            m_prefix = DefaultLinkPrefix;
            m_storageQname = String.Empty;
            m_tableId = 0;
            m_title = String.Empty;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="linktext">Текст ссылки или пустая строка</param>
        public LinkBuilder(string linktext)
        {
            m_filepath = String.Empty;
            m_linktype = LinkType.Unknown;
            m_prefix = DefaultLinkPrefix;
            m_storageQname = String.Empty;
            m_tableId = 0;
            m_title = String.Empty;
            //parse link text
            parse(linktext);
        }

        #region *** Properties ***
        /// <summary>
        /// Префикс ссылки
        /// </summary>
        public string Prefix
        {
            get { return m_prefix; }
            set { m_prefix = value; }
        }
        /// <summary>
        /// Квалифицированное имя хранилища
        /// </summary>
        public string StorageQName
        {
            get { return m_storageQname; }
            set { m_storageQname = value; }
        }
        /// <summary>
        /// Тип ссылки
        /// </summary>
        public LinkType Linktype
        {
            get { return m_linktype; }
            set { m_linktype = value; }
        }
        /// <summary>
        /// Идентификатор записи в соответствующей таблице БД
        /// </summary>
        public int TableId
        {
            get { return m_tableId; }
            set { m_tableId = value; }
        }
        /// <summary>
        /// Название сущности
        /// </summary>
        public string Title
        {
            get { return m_title; }
            set { m_title = value; }
        }
        /// <summary>
        /// Путь файла в хранилище - архивный путь файла
        /// </summary>
        public string Filepath
        {
            get { return m_filepath; }
            set { m_filepath = value; }
        }
        #endregion


        /// <summary>
        /// NT-Удалить из текста символы запрещенные в ссылках
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string makeSafeLinkTitle(string title)
        {
            return Utility.makeSafeLinkTitle(title, 32);
        }

        /// <summary>
        /// NT-Собрать текст ссылки на Хранилище
        /// </summary>
        public string getStorageLink()
        {
            //TODO: check code here...
            return m_prefix + prePrefix + m_storageQname;
        }

        /// <summary>
        /// NT-Собрать текст ссылки на запись сущности
        /// </summary>
        public string getEntityRecordLink()
        {
            String p1 = m_prefix + prePrefix + m_storageQname + pathDelimiter 
                + tagEntityRecord + m_tableId.ToString("D6") + tagTitleDelimiter + makeSafeLinkTitle(m_title);
            return p1;
        }

        /// <summary>
        /// NT-Собрать текст ссылки на запись документа
        /// </summary>
        public string getDocumentRecordLink()
        {
            
            String p1 = m_prefix + prePrefix + m_storageQname + pathDelimiter
                + tagDocumentRecord + m_tableId.ToString("D6") + tagTitleDelimiter + makeSafeLinkTitle(m_title);
            return p1;
        }

        /// <summary>
        /// NT-Собрать текст ссылки на запись изображения
        /// </summary>
        public string getPictureRecordLink()
        {
            String p1 = m_prefix + prePrefix + m_storageQname + pathDelimiter
                + tagPictureRecord + m_tableId.ToString("D6") + tagTitleDelimiter + makeSafeLinkTitle(m_title);
            return p1;
        }

        /// <summary>
        /// NT-Собрать текст ссылки на файл документа или изображения - определяется путем файла
        /// </summary>
        public string getFileLink()
        {
            String p1 = m_prefix + prePrefix + m_storageQname;
            p1 = p1 +  pathDelimiter + m_filepath;
            return p1;
        }

        /// <summary>
        /// NFT-Распарсить текст ссылки на части
        /// </summary>
        /// <param name="text">Текст ссылки</param>
        public void parse(string text)
        {
            text = text.Trim();
            //если текст ссылки пустой, просто выходим.
            if(String.IsNullOrEmpty(text))
                return;
            //TODO: тут парсить текст ссылки

            //* все форматы ссылок:
            //    * префикс://КвалифицированноеИмяХранилища - ссылка на хранилище 
            //    * префикс://КвалифицированноеИмяХранилища/docs1/file1.ext - ссылка на файл документа
            //    * префикс://КвалифицированноеИмяХранилища/pics1/file1.ext - ссылка на файл изображения
            //    * префикс://КвалифицированноеИмяХранилища/e000001-КТ315Б - ссылка на запись сущности
            //    * префикс://КвалифицированноеИмяХранилища/d123456-file1.ext - ссылка на запись документа
            //    * префикс://КвалифицированноеИмяХранилища/p123456-img2.ext - ссылка на запись изображения 

            //отделим префикс
            string[] sar = text.Split(new String[] { prePrefix }, StringSplitOptions.RemoveEmptyEntries);
            if (sar.Length != 2)
                throw new ArgumentException("Invalid link text", "text");
            this.m_prefix = sar[0];
            String part2 = sar[1];
            //отделим qname по первой /
            int position = part2.IndexOf(pathDelimiter);
            if (position == -1)//не найдено
            {
                //весь остаток это ссылка на хранилище
                this.m_storageQname = part2;
                this.m_linktype = LinkType.StorageLink;
                return;
            }
            //else
            String qname = part2.Remove(position);
            this.m_storageQname = qname;
            if (part2.Length == position+1)
            {
                //весь остаток это ссылка на хранилище и / на конце
                this.m_linktype = LinkType.StorageLink;
                return;
            }
            //else
            part2 = part2.Substring(position + 1); //остаток для разбора
            //
            if (part2.StartsWith(ArchiveController.DocumentsDir))
            {
                this.m_filepath = part2;
                this.m_linktype = LinkType.DocumentFileLink;
                return;
            }
            
            if (part2.StartsWith(ArchiveController.ImagesDir))
            {
                this.m_filepath = part2;
                this.m_linktype = LinkType.PictureFileLink;
                return;
            }
            //check id format
            if(part2.Length < 7)
                throw new ArgumentException("Invalid link text", "text");
            //check digits
            if(!(Char.IsDigit(part2[1]) && Char.IsDigit(part2[2]) && Char.IsDigit(part2[3]) 
                && Char.IsDigit(part2[4]) && Char.IsDigit(part2[5]) && Char.IsDigit(part2[6])))
                throw new ArgumentException("Invalid link text", "text");
            //extract table id
            String t = part2.Substring(1, 6);
            Int32 id = Int32.Parse(t);
            m_tableId = id;
            //get title - его может и не быть
            if (part2.Length > 8)//если после - еще что-то есть
            {
                if(part2[7] != tagTitleDelimiter)
                    throw new ArgumentException("Invalid link text", "text");
                //else
                this.m_title = part2.Substring(8);
            }
            //get type
            if (part2[0] == tagEntityRecord)
                this.m_linktype = LinkType.EntityRecordLink;
            else if (part2[0] == tagDocumentRecord)
                this.m_linktype = LinkType.DocumentRecordLink;
            else if (part2[0] == tagPictureRecord)
                this.m_linktype = LinkType.PictureRecordLink;
            else 
                throw new ArgumentException("Invalid link text", "text");

            return;
        }

        /// <summary>
        /// NT-Распарсить ссылку не допуская исключений
        /// </summary>
        /// <param name="linktext">Текст ссылки</param>
        /// <returns>Возвращает объект ссылки или null если при разборе текста возникли ошибки</returns>
        public static LinkBuilder TryParseLink(string linktext)
        {
            LinkBuilder result = null;
            try
            {
                result = new LinkBuilder(linktext);
            }
            catch(Exception)
            {
                result = null;
            }
            return result;
        }
    }
}
