using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace InventoryModuleManager2
{
    public class Session
    {
        /// <summary>
        /// Название каталога для временных файлов буфера
        /// </summary>
        public const String BufferTempFolderName = "Bulk"; 
        
        /// <summary>
        /// Состояние сессии менеджера Хранилища
        /// </summary>
        private SessionState m_SessionState;

        /// <summary>
        /// Каталог временных файлов для работы Менеджера
        /// </summary>
        private String m_TempFilesFolder;

        /// <summary>
        /// Корневой каталог Хранилища
        /// </summary>
        private String m_StorageFolder;
        /// <summary>
        /// Каталог сеанса
        /// </summary>
        private String m_SessionFolder;
        /// <summary>
        /// Путь к каталогу временных файлов буфера для ArchiveController
        /// </summary>
        private String m_BulkBufferFolderPath;

        public Session()
        {

            //set default temp directory as current user temp directory
            m_TempFilesFolder = Path.GetTempPath();
            m_SessionState = SessionState.Closed;

        }

        /// <summary>
        /// NT-Состояние сессии менеджера Хранилища
        /// </summary>
        public SessionState State
        {
            get
            {
                return m_SessionState;
            }
            set
            {
                m_SessionState = value;
            }
        }

        /// <summary>
        /// NT-Каталог временных файлов для работы Менеджера
        /// </summary>
        public String TempFolder
        {
            get
            {
                return m_TempFilesFolder;
            }
            set
            {
                m_TempFilesFolder = value;
            }
        }
        /// <summary>
        /// Корневой каталог Хранилища
        /// </summary>
        public String StorageFolder
        {
            get { return m_StorageFolder; }
            set { m_StorageFolder = value; }
        }

        /// <summary>
        /// NT-Собрать путь к рабочему каталогу для контроллера архивов
        /// </summary>
        /// <param name="archiveFolderName"></param>
        /// <returns></returns>
        internal string GetWorkFolder(string archiveFolderName)
        {
            return Path.Combine(this.m_StorageFolder, archiveFolderName);
        }

        public void Open()
        {
            //эта функция тут нужна потому что после создания объекта еще надо присвоить значения путей каталогов
            //а потом уже начинать сеанс. Но если ввести пути в конструктор, то эта функция не нужна.
            //Но параметры по умолчанию не поддерживаются тут, а TempFilesFolder нужно переопределять иногда пользователю.
            //Для этого нужна еще одна версия конструктора.
            //TODO: доопределиться с этим в конце разработки.

            //Создать новый каталог сеанса с уникальным именем
            createSessionFolder();
            m_SessionState = SessionState.Opened;//TODO: А это где-нибудь необходимо?
        }



        public void Close()
        {
            //не удалять каталог сеанса - файлы в нем еще могут использоваться приложениями.
            //очистить каталог буфера файлов контроллера архивов, если он не пустой.
            this.DeleteBulkBufferedFiles();
            m_SessionState = SessionState.Closed;

        }
        /// <summary>
        /// NT-Создать каталог сеанса
        /// </summary>
        private void createSessionFolder()
        {
            String name = "";
            //подбираем уникальное имя каталога
            for (int i = 0; i < 1024; i++)
            {
                name = Path.Combine(m_TempFilesFolder, Path.GetRandomFileName());
                if (!Directory.Exists(name))
                    break;
            }
            //создаем каталог
            if (!Directory.Exists(name))
            {
                Directory.CreateDirectory(name);
                m_SessionFolder = name;
                //TODO: Bulk adding - создаем подкаталог для Bulk buffer
                m_BulkBufferFolderPath = Path.Combine(name, Session.BufferTempFolderName);
                Directory.CreateDirectory(m_BulkBufferFolderPath);
            }
            else
                throw new Exception("Error: Cannot create session directory");

            return;
        }
        /// <summary>
        /// NT-создать временную копию файла и вернуть новый путь файла
        /// </summary>
        /// <param name="filepath">Путь к оригинальному файлу</param>
        /// <returns>Возвращает путь к копии файла</returns>
        internal string StoreBulkBufferedFile(string filepath)
        {
            String filename = Path.GetFileName(filepath);
            String copyPath = Path.Combine(m_BulkBufferFolderPath, filename);
            //check destination file not exists
            copyPath = Utility.createFreeFileName(copyPath);
            //copy file
            File.Copy(filepath, copyPath);
            return copyPath;
        }
        /// <summary>
        /// NT-Создать временную копию файла под архивныи именем и вернуть новый путь файла
        /// </summary>
        /// <param name="filepath">Путь к оригинальному файлу</param>
        /// <param name="fileArchName">Имя файла в архиве, уникальное.</param>
        /// <returns>Возвращает путь к копии файла</returns>
        internal string StoreBulkBufferedFile(string filepath, string fileArchName)
        {
            String copyPath = Path.Combine(m_BulkBufferFolderPath, fileArchName);
            //имя должно быть уникальным в папке - иначе где-то поизошла ошибка.
            //copy file
            File.Copy(filepath, copyPath);
            return copyPath;
        }
        /// <summary>
        /// NT-Удалить все временные файлы из папки буфера файлов
        /// </summary>
        internal void DeleteBulkBufferedFiles()
        {
            String[] sar = Directory.GetFiles(m_BulkBufferFolderPath);
            foreach (String s in sar)
                File.Delete(s);
            return;
        }


    }
}
