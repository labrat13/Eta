using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zip;
using Ionic.Crc;


namespace InventoryModuleManager2
{
    public class ZipWorker
    {
        /// <summary>
        /// Разделитель пути внутри архива не такой как в Виндовс.
        /// </summary>
        internal const string PathDelimiter = "/"; 

        /// <summary>
        /// NT-Получить число файлов в архиве
        /// </summary>
        /// <param name="file">Путь к файлу архива</param>
        /// <returns>Число любых файлов в архиве</returns>
        internal static int GetFilesCount(string file)
        {
            int cnt = 0;
            using (ZipFile zip = new ZipFile(file))
            {
                //use field
                cnt = zip.Count - 1;// -1 for root folder
                if (cnt < 0) cnt = 0;

            }
            return cnt;
            
        }
        /// <summary>
        /// NT-Создать новый файл архива 
        /// </summary>
        /// <param name="name">Путь к новому файлу архива</param>
        internal static void CreateArchive(string name)
        {
            //получить имя для каталога в архиве
            String zipfolder = Path.GetFileNameWithoutExtension(name);
            //создать файл архива
            using (ZipFile zip = new ZipFile(name, Encoding.UTF8))
            {
                zip.AlternateEncoding = Encoding.UTF8;
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.UseZip64WhenSaving = Zip64Option.Always;
                //add root folder
                zip.AddDirectoryByName(zipfolder);
                //закончить
                zip.Save();
            }
            return;
            
        }
        /// <summary>
        /// NT-Добавить файл в архив
        /// </summary>
        /// <param name="archive">Имя файла архива</param>
        /// <param name="pathInArchive">Путь файла в архиве</param>
        /// <param name="filepath">Путь добавляемого внешнего файла</param>
        /// <exception cref="Exception">Zip error</exception>
        internal static void AddFile(string archive, string pathInArchive, string filepath)
        {
            try
            {            
                Stream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (ZipFile zip = new ZipFile(archive))
                {

                    //set up zip properties
                    zip.AlternateEncoding = Encoding.UTF8;
                    zip.AlternateEncodingUsage = ZipOption.Always;
                    zip.UseZip64WhenSaving = Zip64Option.Always;
                    //zip.ZipErrorAction = ZipErrorAction.Throw;//default

                    zip.AddEntry(pathInArchive, fs);
                    //вот тут на самом деле можно много файлов добавлять, чтобы все их за один раз потом записать.
                    //Но у меня все файлы поодиночке добавляются, к сожалению.
                    //finish
                    zip.Save();//тут открытый ранее поток записывается в архив

                }
                fs.Close();
            }
            catch (Exception ex)
            {
                
                throw new Exception(String.Format("Zip adding error: {0} {1} {2}", archive, pathInArchive, filepath), ex);
            }

            return;
            
        }

        /// <summary>
        /// NR-Добавить файлы из буфера файлов в архив
        /// </summary>
        /// <param name="archive">Имя файла архива</param>
        /// <param name="files">Список объектов кэшированных файлов из буфера файлов.</param>
        /// <exception cref="Exception">Zip error</exception>
        internal static void AddFiles(string archive, List<FileRecord> files)
        {
            try
            {
                using (ZipFile zip = new ZipFile(archive))
                {
                    //set up zip properties
                    zip.AlternateEncoding = Encoding.UTF8;
                    zip.AlternateEncodingUsage = ZipOption.Always;
                    zip.UseZip64WhenSaving = Zip64Option.Always;
                    //zip.ZipErrorAction = ZipErrorAction.Throw;//default
                    
                    String archfolder = Path.GetFileNameWithoutExtension(archive);
                    //Добавить файлы 
                    //файлы уже должны быть переименованы в архивные имена!!!
                    foreach (FileRecord fr in files)
                        zip.AddFile(fr.ExternalPath, archfolder);
                    //finish
                    zip.Save();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Zip adding error: {0}", archive), ex);
                //TODO: надо придумать что делать, если произойдет ошибка при дозаписи в архив 
                //- предыдущий менеджер сам все откатывал к прежней версии
                //Тут надо тотально перепроектировать все, чтобы учитывать, что архив может дать сбой при любой операции.

                //Тут есть идея что здесь надо выполнить локальную уборку, 
                //а восстановление из резервных копий архивов передать в механизм транзакций - в вызывающем коде.
                //Поскольку вызывающий код использует оба контроллера архивов и БД, 
                //и при ошибке в одном из них надо откатывать все эти элементы, а не только вызвавший ошибку.
            }

            return;
            
        }

        /// <summary>
        /// NT-Сравнить содержимое внешнего и сжатого файлов через их потоки
        /// </summary>
        /// <param name="arch">Путь к файлу архива</param>
        /// <param name="archivePath">путь к файлу в архиве</param>
        /// <param name="filePath">Путь к внешнему файлу</param>
        /// <returns>Возвращает true  если содержимое файлов одинаковое, иначе false</returns>
        /// <exception cref="Exception">Invalid CRC of entry</exception>
        /// <exception cref="Exception">Invalid length of entry</exception>
        /// <exception cref="Exception">Entry not found</exception>
        internal static bool CompareFiles(string arch, string archivePath, string filePath)
        {
            bool result = true;
            using (ZipFile zip = new ZipFile(arch))
            {
                ZipEntry e1 = zip[archivePath];
                //check entry is exists
                if(e1 == null)
                    throw new Exception(String.Format("Entry {0} not found in archive {1}", archivePath, arch));
                //do work
                using (CrcCalculatorStream s1 = e1.OpenReader())
                {
                    //Тут надо:
                    //пока не будет выявлено несоответствие, читать байты из архива и сравнивать с байтами из файла
                    //если несоответствие выявлено, продолжать чтение байт из архива до конца архива.
                    //затеи проверить длину архива и КС, выбросить исключение или нормально закончить алгоритм. 
                    
                    byte[] buf1 = new byte[4096];
                    byte[] buf2 = new byte[4096];
                    int rd1, rd2, totalBytesRead = 0;
                    Stream s2 = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096);
                    
                    do
                    {
                        //весь цикл читать файл из архива, чтобы получить его длину и КС, чтобы проверить, что файл распакован правильно.
                        rd1 = s1.Read(buf1, 0, buf1.Length);
                        totalBytesRead += rd1;
                        //пока несоответствие файлов не выявлено, читать и сравнивать проверяемый файл и файл из архива. 
                        if (result == true)
                        {
                            //read file
                            rd2 = s2.Read(buf2, 0, buf2.Length);
                            //check length
                            if (rd1 != rd2)
                            {
                                result = false;
                            }
                            else
                            {
                                for (int i = 0; i < rd1; i++)
                                    if (buf1[i] != buf2[i])
                                        result = false;
                            }
                        }
                    } while (rd1 > 0);
                    s2.Close();//bugfix 08102017
                    //check length and crc
                    if (s1.Crc != e1.Crc)
                        throw new Exception(string.Format("The Zip Entry failed the CRC Check. (0x{0:X8}!=0x{1:X8})", s1.Crc, e1.Crc));
                    if (totalBytesRead != e1.UncompressedSize)
                        throw new Exception(string.Format("We read an unexpected number of bytes. ({0}!={1})", totalBytesRead, e1.UncompressedSize));
                }
            }
            // вернуть результат
            return result;
            
        }




        /// <summary>
        /// NT-Загрузить данные файла архива в поток
        /// </summary>
        /// <param name="arch">Путь к файлу архива</param>
        /// <param name="archpath">Путь к файлу внутри архива</param>
        /// <param name="s">Поток для записи содержимого файла</param>
        internal static void GetFile(string arch, string archpath, Stream s)
        {
            //TODO: надо ли здесь перехватывать исключение или же его оставить для Менеджера?
            using (ZipFile zip = new ZipFile(arch))
            {
                ZipEntry e1 = zip[archpath];
                //check entry is exists
                if (e1 == null)
                    throw new Exception(String.Format("Entry {0} not found in archive {1}", archpath, arch));
                //extract entry to sprcified stream
                e1.Extract(s);

            }
            
            return;
        }
    }
}
