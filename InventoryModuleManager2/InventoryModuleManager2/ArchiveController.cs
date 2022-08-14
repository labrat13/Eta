using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace InventoryModuleManager2
{
    /// <summary>
    /// Контроллер папки с архивами файлов
    /// </summary>
    internal class ArchiveController
    {
        #region *** Переменные и константы***
        /// <summary>
        /// Название каталога для Документов
        /// </summary>
        internal const String DocumentsDir = "docs";
        /// <summary>
        /// Название каталога для Изображений
        /// </summary>
        internal const String ImagesDir = "pics";
        /// <summary>
        /// Название каталога для Изображений
        /// </summary>
        private const String ArchiveExtension = ".zip";

        /// <summary>
        /// Максимальный размер архива, в байтах. Архивы, размером больше указанного предела, не пополняются.
        /// </summary>
        /// <remarks>Это значение влияет на скорость пакетного добавления сущностей. 
        /// Чем больше размер архива, тем медленнее в него добавляются файлы. 
        /// Файл с размером, большим указанного предела, может оказаться один в архиве.
        /// Слишком малый размер файла замедлит работу Хранилища из-за тысяч этих мелких файлов.</remarks>
        protected const Int64 MaxArchiveFileSize = 64 * 1024 * 1024;

        /// <summary>
        /// Название папки каталога архива. Определяет, что обслуживает данный контроллер: Документы или Изображения.
        /// Таже используется в качестве названия таблицы БД в запросах.
        /// </summary>
        private String m_WorkFolderName;
        /// <summary>
        /// Объект текущей сессии менеджера
        /// </summary>
        private Session m_Session;
        /// <summary>
        /// Шаблон поиска файлов в архивном каталоге
        /// </summary>
        private String m_archPattern;
        /// <summary>
        /// Путь папки рабочего каталога архива. 
        /// </summary>
        private String m_WorkFolderPath;
        /// <summary>
        /// Транзакция операции архива
        /// </summary>
        private ArchiveTransaction m_transaction;
        /// <summary>
        /// Обратная ссылка на объект менеджера
        /// </summary>
        private DbAdapter m_dbAdapter;

        #endregion
        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="archiveFolderName"></param>
        /// <param name="session"></param>
        public ArchiveController(String archiveFolderName, Session session, DbAdapter dba)
        {
            //set controller theme
            m_WorkFolderName = archiveFolderName;
            //set session object
            m_Session = session;
            //set manager backreference
            m_dbAdapter = dba;
            //set archive files search pattern
            m_archPattern = m_WorkFolderName + "*" + ArchiveExtension;
            //set work folder path
            m_WorkFolderPath = session.GetWorkFolder(archiveFolderName); // 
            return;
        }



        #region *** Main controller functions ***
        /// <summary>
        /// NT-Добавить файл в хранилище.
        /// </summary>
        /// <param name="filepath">Путь к добавляемому файлу</param>
        /// <param name="fileArchName">Имя файла в архиве. 
        /// Не должно содержать пробелов и неправильных символов.
        /// Должно быть уникальным во всем хранилище. 
        /// Использовать для этого БД.</param>
        /// <returns>Возвращает путь к файлу внутри хранилища.</returns>
        internal String addFile(String fileArchName, String filepath)
        {
            // описания не добавляются в архив - так как зип их не поддерживает сейчас.

            //Имя файла в архиве должно быть уникальным.
            //найти архив для добавления файла
            Int64 arch_size;
            String arch = findArchiveForAdding(out arch_size); //возвращает полный путь к архиву
            //создать путь файла внутри архива
            String archpath = Path.GetFileNameWithoutExtension(arch);
            archpath = archpath + ZipWorker.PathDelimiter + fileArchName;
            //добавить файл в архив
            ZipWorker.AddFile(arch, archpath, filepath);
            //вернуть путь к файлу в хранилище как папка/имяФайлаАрхиваКакПапка/имяФайла
            return archpath; //return docs1\file.ext
        }

        /// <summary>
        /// NT-Получить число файлов в архивах
        /// </summary>
        /// <returns></returns>
        internal int getFilesCount()
        {
            string[] files = this.getArchives();
            int result = 0;
            foreach (String f in files)
                result += ZipWorker.GetFilesCount(f);
            return result;
        }
        /// <summary>
        /// NT-Получить общий объем архивов в рабочей папке
        /// </summary>
        /// <returns></returns>
        internal long getFolderSize()
        {
            DirectoryInfo di = new DirectoryInfo(m_WorkFolderPath);
            FileInfo[] fis = di.GetFiles(this.m_archPattern, SearchOption.TopDirectoryOnly);
            long result = 0;
            foreach (FileInfo fi in fis)
                result += fi.Length;

            return result;
        }

        /// <summary>
        /// NFT-Сравнить содержимое файлов через их потоки
        /// </summary>
        ///<param name="filePath">Путь к файлу на диске</param>
        ///<param name="archivePath">Путь к файлу в архиве хранилища</param>
        /// <returns>Возвращает true  если содержимое файлов одинаковое, иначе false</returns>
        internal bool CompareFile(string filePath, string archivePath)
        {

            //1 получить имя файла архива
            String arch = Path.GetDirectoryName(archivePath);//получить имя папки - оно же имя файла архива
            arch = Path.Combine(m_WorkFolderPath, arch + ArchiveExtension);//собираем имя файла архива
            //TODO: Bulk adding: тут просматривать также буфер файлов, в первую очередь
            //если это архив, который буферизирован, то сначала просматриваем содержимое буфера
            //поскольку это быстрее, и более вероятно, что именно там находится требуемый файл 
            //но простое сравнение путей не поможет, если пути не чисто локальные, а например, сетевые.
            String file2 = null;
            if ((m_BulkFileBufferList != null) && (m_BulkFileBufferList.Count > 0))//эта проверка быстрее, поэтому выполняется раньше
            {
                if (String.Equals(m_BulkFilePathOfArchiveToWriting, arch))//эта проверка медленнее поэтому выполняется позже
                {
                    //ищем требуемый файл
                    foreach (FileRecord f in m_BulkFileBufferList)
                    {
                        if (f.StoragePath == archivePath)
                        {
                            file2 = f.ExternalPath;
                            break;
                        }
                    }
                }
            }
            //Stream s1, s2;
            bool result;
            //если файл не нашли в буфере, он либо уже записан в архив, либо этот буфер для другого архива, либо это ошибка и файла нигде нет.
            if (file2 != null)
            {
                //s1 = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //s2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read);
                //result = Utility.CompareStreams(s1, s2);
                //s1.Close();
                //s2.Close();
                //replaced by: 
                result = Utility.CompareFiles(filePath, file2);
            }
                
            else
                result = ZipWorker.CompareFiles(arch, archivePath, filePath);
            //return
            return result;
        }



        /// <summary>
        /// RT-Удалить все архивы и сбросить настройки контроллера
        /// </summary>
        internal void Clear()
        {
            //получить список архивов
            String[] files = getArchives();
            //delete files
            foreach (String f in files)
                File.Delete(f);
            return;
        }

        public override string ToString()
        {
            return m_WorkFolderPath;//пока только рабочий каталог покажем - число файлов неохота получать из ФС.
        }

        #endregion


        #region *** Вспомогательные функции контроллера архивов ***
        /// <summary>
        /// NT-Найти архив для добавления нового файла
        /// </summary>
        /// <param name="archiveLength">Возвращает текущую длину файла архива</param>
        /// <returns>Возвращает путь к архиву</returns>
        private String findArchiveForAdding(out Int64 archiveLength)
        {
            //Это рассчитано на редкие добавления файлов при более частых получениях и больших перерывах.
            //получить рабочий каталог архива
            //получить список архивов
            String[] files = getArchives();
            //отобрать из них первый пригодный для дополнения
            foreach (String file in files)
            {
                FileInfo fi = new FileInfo(file);
                //если архив найден, вернуть путь к архиву.
                if (fi.Length < ArchiveController.MaxArchiveFileSize)
                {
                    archiveLength = fi.Length;
                    return file;
                }
            }
            //если не найден, создать новый архив и вернуть путь к нему
            archiveLength = 0;
            return this.createNewArchive();
        }

        /// <summary>
        /// NFT-Создать новый архив
        /// </summary>
        /// <returns>Возвращает путь к созданному архиву</returns>
        private String createNewArchive()
        {
            //создать имя для нового архива
            String name = createNewArchiveName();
            //создать новый пустой архив с папкой в корне
            ZipWorker.CreateArchive(name);
            //вернуть путь к новому архиву
            return name;

        }

        /// <summary>
        /// NFT-Создать имя для нового файла архива
        /// </summary>
        /// <returns>Вернуть новое имя архива и путь к его файлу</returns>
        private String createNewArchiveName()
        {
            //TODO: можно кэшировать последний номер существующего архива, чтобы не вычислять его каждый раз при создании нового архива.
            //Сейчас тут все рассчитано на единичное добавление записи и большой перерыв потом.
            //Но если вводить этот кэш номера архива, то надо ввести функцию Reset() и в ней прописать сброс подобных переменных.
            //и эту Reset() вызывать в ClearStorage() итп.
            //если значение = 0, то надо найти актуальное значение. А если больше 0, то использовать это значение кэша.
            //Однако, архивы создаются ОЧЕНЬ редко, и тут не будет существенной экономии, а сложность системы увеличится.

            //получить список существующих архивов
            //сортировать список и отобрать файл с самым большим номером файла
            String[] files = getArchives();
            Int32 archNo = 0;
            //find max of archive number
            foreach (string f in files)
            {
                String archFileName = Path.GetFileName(f);
                int t = extractArchiveNum(archFileName);
                if (archNo < t)
                    archNo = t;
            }
            //создать новое имя архива
            String name = combineArchiveName(archNo + 1);
            //вернуть новое имя архива вместе с путем к файлу
            return Path.Combine(m_WorkFolderPath, name);
        }

        /// <summary>
        /// NT-Получить список путей файлов архива
        /// </summary>
        /// <returns></returns>
        private String[] getArchives()
        {
            String[] files = Directory.GetFiles(m_WorkFolderPath, m_archPattern, SearchOption.TopDirectoryOnly);
            return files;
        }
        /// <summary>
        /// NT-Собрать имя файла архива без пути
        /// </summary>
        /// <param name="p">Порядковый номер файла</param>
        /// <returns>Имя файла архива без пути</returns>
        private string combineArchiveName(int p)
        {
            //like docs176.zip
            return m_WorkFolderName + p.ToString() + ArchiveExtension;
        }
        /// <summary>
        /// NT-Извлечь номер файла архива из имени файла
        /// </summary>
        /// <param name="p">Имя файла без пути</param>
        /// <returns>Порядковый номер файла</returns>
        private int extractArchiveNum(string p)
        {
            //like docs176.zip
            String s = p.Replace(m_WorkFolderName, String.Empty);
            s = s.Replace(ArchiveExtension, String.Empty);
            //now here digits only must be
            return Int32.Parse(s);
        }
        #endregion


        #region *** Buffered-Bulk adding functions ***

        private List<FileRecord> m_BulkFileBufferList;

        private Int64 m_BulkFileBufferLength;

        private Int64 m_BulkArchiveFreePlaceSize;

        private String m_BulkFilePathOfArchiveToWriting;
        //07072017 - переделка пакетного добавления файлов
        private Dictionary<String, int> m_BulkFileTitleDictionary;

        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="fileArchName">Имя файла в архиве.</param>
        /// <param name="fr">Запись добавлемого файла</param>
        /// <returns></returns>
        internal string addFileBuffered(string fileArchName, FileRecord fr, out FileRecord copyFr)
        {
            //1 если буфер файлов не создан, создать его - это первый раз запуск функции
            if (m_BulkFileBufferList == null)
                m_BulkFileBufferList = new List<FileRecord>();
            //2 если буфер пустой, инициализировать его переменные
            if (m_BulkFileBufferList.Count == 0)
            {
                //- сбросить счетчик размера файлов буфера
                m_BulkFileBufferLength = 0;
                //- найти или создать архив для добавления файлов
                Int64 temp;
                m_BulkFilePathOfArchiveToWriting = findArchiveForAdding(out temp); //возвращает полный путь к архиву
                //- вычислить размер свободного места в архиве
                m_BulkArchiveFreePlaceSize = ArchiveController.MaxArchiveFileSize - temp;
                //07072017 - переделка пакетного добавления файлов
                //TODO: заполнить словарь именами из БД - ERROR: нельзя - нет имени таблицы для БД
                m_BulkFileTitleDictionary = this.m_dbAdapter.GetFileNamesDictionary(m_WorkFolderName);
            }
            //3 добавить файл в буфер
            //TODO: сразу переименовать файл в новое имя как в архиве, чтобы потом так и добавить в архив.
            //--создать путь файла внутри архива
            String archpath = Path.GetFileNameWithoutExtension(m_BulkFilePathOfArchiveToWriting);
            archpath = archpath + ZipWorker.PathDelimiter + fileArchName;
            //- создать копию файла в временном каталоге и сразу переименовать в архивное имя файла - для ZipFile
            String tmpPath = m_Session.StoreBulkBufferedFile(fr.ExternalPath, fileArchName);
            //- создать копию записи файла и переделать ее на путь к копии файла
            FileRecord tempfr = new FileRecord(fr);
            tempfr.ExternalPath = tmpPath;
            //- в копию записи файла и в запись файла добавить путь в архиве
            tempfr.StoragePath = archpath;
            fr.StoragePath = archpath;
            //HACK: 26102017: исправляем отсутствующий ИД - передать ссылку на этот временный объект наружу, чтобы и ему тоже присвоить ИД записи таблицы файлов.
            copyFr = tempfr;
            //- добавить запись файла в буфер файлов
            m_BulkFileBufferList.Add(tempfr);
            m_BulkFileBufferLength += tempfr.Length;
            //TODO: Bulk adding - что-то еще забыл?

            //4 проверить размер буфера и выгрузить файлы в архив
            if (m_BulkArchiveFreePlaceSize < m_BulkFileBufferLength)
            {
                //тут же сбросить переменные буфера или просто удалить список буфера.
                //удалить записанные временные файлы
                this.FlushBuffered();
            }
            //5 вернуть путь файла в архиве, как если бы он был уже записан в архив.
            //-можно добавить его сразу в переданный FileRecord - уже добавлен
            return archpath;
        }

        /// <summary>
        /// NT-Записать файлы из буфера файлов в архивы на диск
        /// </summary>
        internal void FlushBuffered()
        {
            //если буфер файлов пустой, выйти быстро
            if (m_BulkFileBufferList == null) return;
            if (m_BulkFileBufferList.Count == 0) return;
            //add files to archive
            ZipWorker.AddFiles(m_BulkFilePathOfArchiveToWriting, m_BulkFileBufferList);
            //удалить записанные временные файлы только в этом буфере.
            //а еще есть буфер для изображений - он своим контроллером обслуживается.
            foreach (FileRecord f in m_BulkFileBufferList)
                deleteFile(f.ExternalPath);
                
            //сбросить переменные буфера или очистить или удалить список буфера.
            m_BulkFileBufferList.Clear();
            //Также удалить словарь имен файлов, так как он используется снаружи контроллера.
            //Хотя это не обязательно - вне пакетного режима словарь не должен использоваться,
            //а внутри пакетного режима он пересоздается вместе с буфером пакета.
            //Но сейчас я подстрахуюсь.
            m_BulkFileTitleDictionary.Clear();
            m_BulkFileTitleDictionary = null;

            return;
        }

        private void deleteFile(string path)
        {
            //clear file attributes - чтобы избежать исключения при удалении файла
            File.SetAttributes(path, FileAttributes.Normal);
            //delete file
            File.Delete(path);
            return;
        }
        #endregion










        //internal Stream getStream(string archivePath)
        //{
        //    String arch = Path.GetDirectoryName(archivePath);//получить имя папки - оно же имя файла архива
        //    arch = Path.Combine(m_WorkFolderPath, arch + ArchiveExtension);//собираем имя файла архива
        //    return ZipWorker.getArchiveFileStream(arch, archivePath);
        //}
        /// <summary>
        /// NT-Записать содержимое файла из архива в указанный поток.
        /// </summary>
        /// <param name="archpath">Путь файла внутри Хранилища</param>
        /// <param name="s">Поток для записи</param>
        internal void getFile(string archpath, Stream s)
        {
            //do not close stream!
            //1 получить имя файла архива
            String arch = Path.GetDirectoryName(archpath);//получить имя папки - оно же имя файла архива
            arch = Path.Combine(m_WorkFolderPath, arch + ArchiveExtension);//собираем имя файла архива
            //2 получить данные
            ZipWorker.GetFile(arch, archpath, s);

            return;
        }

        #region *** Транзакции ***
        /// <summary>
        /// NR-Начать транзакцию.
        /// </summary>
        internal void TransactionBegin()
        {
            m_transaction = new ArchiveTransaction();
            m_transaction.m_controller = this;

            return;
        }
        /// <summary>
        /// NR-Подтвердить транзакцию.
        /// </summary>
        internal void TransactionRollback()
        {
            m_transaction.Rollback();

            return;
        }
        /// <summary>
        /// NR-Отменить транзакцию.
        /// </summary>
        internal void TransactionCommit()
        {
            m_transaction.Commit();

            return;
        }
        #endregion

        /// <summary>
        /// NT-вернуть уникальное имя для добавляемого в хранилище файла
        /// </summary>
        /// <param name="filepath">Исходное имя файла</param>
        /// <returns></returns>
        internal string findUnicalFileName(string filepath)
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
            Dictionary<String, Int32> dubdict = null;
            //если есть словарь имен файлов, то ищем по нему.
            if ((m_BulkFileTitleDictionary != null) && (m_BulkFileTitleDictionary.Count > 0))
            {
                dubdict = m_BulkFileTitleDictionary;
            }
            else
            {
                //если же его нет или он пустой, то ищем по БД.
                //это запрос вроде LIKE %name% вернет все похожие варианты, и вот из них надо распарсить и выявить аналоги.
                //Это одним запросом получится сделать, но это медленная операция.
                dubdict = this.m_dbAdapter.GetFileNamesLike(this.m_WorkFolderName, name);
            }
            name = MakeUnicalNameSub(name, dubdict);
            // а сейчас мы завершаем функцию,  и этот большой массив дубнеймов будет уничтожен.
            //Хорошо что он недолго существует.

            //4 вернуть уникальное имя файла с расширением
            return name + ext;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dubdict"></param>
        /// <returns></returns>
        internal static string MakeUnicalNameSub(string name, Dictionary<String, Int32> dubdict)
        {
            //2 проверить начальное имя файла на отсутствие в списке.
            //3 если есть дубликаты, то модифицировать имя файла. и так в цикле.
            //4 вернуть уникальное имя файла
            String result = name;
            //Если в хранилище кладутся только файлы с одинаковыми именами, то нужен
            //большой-большой счетчик. 
            //В этом случае текущее решение здесь будет работать очень медленно. 
            //Проще найти максимальный номер существующего файла.
            //А еще, можно сгенерировать случайное число. Но при большом числе аналогов это будет только хуже.
            for (int i = 0; i < Int32.MaxValue; i++)
            {
                ////перевести все в нижний регистр, чтобы не было проблем с файловой системой
                //String lowname = result.ToLowerInvariant();
                ////искать совпадение в списке вариантов
                ////CODE26052021: тут косяк в том, что строки имени файла в разном регистре: 
                ////- из БД приходят имена файлов в нижнем регистре
                ////- в пакетный буфер новые имена добавляются в натуральном виде
                ////- новое имя - в натуральном виде
                ////Файловая система игнорирует регистр, а словарь выбирает имена с учетом регистра.
                ////я должен опробовать поиск по ключу, перебором с игнорированием регистра сииволов.

                //if (!dubdict.ContainsKey(lowname)) //старый код, удалить после отладки
                //    return result;
                //else
                //    result = String.Format("{0}_{1}", name, i.ToString());

                //заменен на:
                if (!Utility.DictionaryContainsKeyIgnoreCase(dubdict, result)) 
                    return result;
                else
                    result = String.Format("{0}_{1}", name, i.ToString());

            }
            //исчерпаны все попытки создать имя файла. Этого конечно не может быть, но а вдруг...
            throw new Exception(String.Format("Error: Cannot create unical filename for {0}", name));
        }


        /// <summary>
        /// NT-Найти в БД дубликат для нового файла
        /// </summary>
        /// <param name="fr">Запись нового файла</param>
        /// <returns>Возвращаем дубликат нового файла или null если не найден дубликат.</returns>
        internal FileRecord findDublicate( FileRecord fr)
        {
            FileRecord result = null;

            //1 ищем дубликаты в пакетном буфере, если он есть и не пустой
            result = findDublicateInPacketBuffer(fr);
            if (result != null)
                return result;
            //2 ищем дубликаты в БД
            List<FileRecord>  dublicates = new List<FileRecord>(this.m_dbAdapter.GetFileDublicates(m_WorkFolderName, fr.Length, fr.Crc));
            foreach (FileRecord f in dublicates)
            {
                bool res = this.CompareFile(fr.ExternalPath, f.StoragePath);//тут проверяемый файл уже добавлен в архив
                if (res == true)
                {
                    result = f;
                    break;
                }
            }

            return result;
        }
        /// <summary>
        /// Найти в пакетном буфере дубликат для нового файла
        /// </summary>
        /// <param name="fr">Запись нового файла</param>
        /// <returns>Возвращаем дубликат нового файла или null если не найден дубликат.</returns>
        private FileRecord findDublicateInPacketBuffer(FileRecord fr)
        {
            FileRecord result = null;
            //ищем дубликаты в пакетном буфере, если он есть и не пустой
            if ((m_BulkFileBufferList != null) && (m_BulkFileBufferList.Count > 0))
            {
                foreach (FileRecord f in m_BulkFileBufferList)
                {
                    if ((f.Crc == fr.Crc) && (f.Length == fr.Length))
                    {
                        //тут проверяемый файл еще не добавлен в архив
                        bool res = Utility.CompareFiles(fr.ExternalPath, f.ExternalPath);
                        if (res == true)
                        {
                            result = f;
                            break;
                        }
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// Добавить имя свежедобавленного файла в кэш-словарь имен файлов для таблицы БД
        /// 
        /// </summary>
        /// <param name="p"></param>
        internal void addFileNameToDictionary(string p)
        {
            if (this.m_BulkFileTitleDictionary != null)
            {
                String filename = Path.GetFileNameWithoutExtension(p);
                this.m_BulkFileTitleDictionary.Add(filename, 0);
            }
            return;
        }


    }
}
