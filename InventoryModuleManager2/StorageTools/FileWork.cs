using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

//Для моей библиотеки функций:
//1. Написать функцию, которая возвращает список путей файлов в каталоге
//- принимает:
//-- путь к папке,
//-- строку расширения файлов
//-- флаг: искать только в верхней папке или также и во вложенных папках
//- возвращает:
//--список путей найденных файлов

//Из-за особенностей реализации надо файлы добавлять в словарь.
//А возвращать массив или список.
//Поэтому еще время на создание возвращаемого списка надо тратить. 

//2. Написать функцию. которая строку с расширениями файлов парсит на массив расширений файлов.
//Тоже для моей библиотеки и для использования в функции 1.

//3. Написать функцию, которая возвращает массив или список или словарь расширений файлов в указанном каталоге (и подкаталогах?)
//--надо получить все файлы в каталоге и извлечь из каждого расширение и добавить в словарь счетчиков.
//--вернуть словарь счетчиков.
//--можно сделать рекурсивной, это экономит память?

//4.Хорошо бы замерить производительность функций.


namespace StorageTools
{
    public class FileWork
    {
        /// <summary>
        /// NT-Получить словарь счетчиков расширений файлов, размещающихся в указанном каталоге.
        /// </summary>
        /// <param name="folder">Путь к каталогу, в котором производится поиск</param>
        /// <param name="option">Искать ли во вложенных каталогах</param>
        /// <returns>Возвращает словарь счетчиков расширений файлов</returns>
        public static Dictionary<String, int> getCounterDictionaryOfFileExtensions(String folder, SearchOption option)
        {
            Dictionary<String, int> dic = new Dictionary<string, int>();
            DirectoryInfo di = new DirectoryInfo(folder);
            recursiveGetExtensions(dic, di, option);

            return dic;
        }

        /// <summary>
        /// NT-Служебная функция рекурсивного обхода каталога
        /// </summary>
        /// <param name="dic">Словарь счетчиков расширений</param>
        /// <param name="di">DirectoryInfo объект просматриваемого каталога</param>
        /// <param name="option">Искать ли во вложенных каталогах</param>
        private static void recursiveGetExtensions(Dictionary<string, int> dic, DirectoryInfo di, SearchOption option)
        {
            //process files
            FileInfo[] fi = di.GetFiles();
            foreach (FileInfo f in fi)
            {
                String ext = f.Extension;//example: .txt
                if (dic.ContainsKey(ext))
                {
                    int t = dic[ext];
                    dic[ext] = t + 1;
                }
                else
                {
                    dic.Add(ext, 1);
                }
            }
            //process directories
            if (option != SearchOption.AllDirectories)
                return;

            DirectoryInfo[] dir = di.GetDirectories();
            foreach (DirectoryInfo d in dir)
                recursiveGetExtensions(dic, d, option);

            return;
        }
        
        /// <summary>
        /// NT-Возвращает массив путей файлов с указаными расширениями 
        /// </summary>
        /// <param name="extensions">
        /// Строка с расширениями файлов. Пример: ".txt .diz" 
        /// </param>
        /// <param name="folder">Путь к каталогу, в котором производится поиск</param>
        /// <param name="option">Искать ли во вложенных каталогах</param>
        /// <returns>Возвращает массив путей файлов с указаными расширениями </returns>
        public static string[] getFolderFilesByExts(String folder, string extensions, SearchOption option)
        {
            //тут надо предусмотреть случаи, когда строка расширений пустая
            return getFolderFilesByExts(folder, parseFileExtensionsString(extensions), option);
        }

        /// <summary>
        /// NT-Возвращает массив путей файлов с указанными расширениями 
        /// </summary>
        /// <param name="folder">Путь к каталогу, в котором производится поиск</param>
        /// <param name="fileExtensions">Массив с расширениями искомых файлов</param>
        /// <param name="option">Искать ли во вложенных каталогах</param>
        /// <returns>Возвращает массив путей файлов с указанными расширениями</returns>
        public static string[] getFolderFilesByExts(String folder, string[] fileExtensions, SearchOption option)
        {
            Dictionary<String, String> files = new Dictionary<string, string>();
            foreach (String ext in fileExtensions)
            {
                String[] sar = Directory.GetFiles(folder, "*" + ext, option);
                foreach (String sa in sar)
                {
                    if (!files.ContainsKey(sa))
                        files.Add(sa, sa);
                }
            }
            //copy to result array
            String[] result = new String[files.Count];
            int i = 0;
            foreach (KeyValuePair<String, String> kvp in files)
            {
                result[i] = kvp.Key;
                i++;
            }

            return result;
        }

        /// <summary>
        /// NT-Разделяет строку расширений файлов на массив расширений файлов
        /// </summary>
        /// <param name="extensions">
        /// Строка с расширениями файлов. 
        /// Расширение начинается с точки и отделяется пробелом
        /// Пример: ".txt .diz" 
        /// </param>
        /// <returns>Возвращает массив строк расширений файлов</returns>
        public static string[] parseFileExtensionsString(string extensions)
        {
            char[] splitter = new char[] { ' ' };
            String[] result = extensions.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            return result;
        }

        /// <summary>
        /// NT-убирает точку и делает следующую букву в верхний регистр.
        /// Пример: .pdf -> Pdf
        /// </summary>
        /// <param name="p">расширение файла</param>
        /// <returns></returns>
        public static string createDocTypeFromFileExtension(string p)
        {
            //добавлена в движок в класс Utility для использования в подобных случаях.
            StringBuilder sb = new StringBuilder();
            char cc = p[0];
            foreach (char c in p)
            {
                if (c != '.')
                    if (cc == '.')
                        sb.Append(Char.ToUpper(c));
                    else sb.Append(Char.ToLower(c));
                //save to next use
                cc = c;
            }
            return sb.ToString();
        }


    }
}
