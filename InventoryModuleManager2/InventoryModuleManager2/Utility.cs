using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;



namespace InventoryModuleManager2
{
    /// <summary>
    /// Содержит функции для использования и в движке и в клиентах менеджера Хранилищ.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Массив запрещенных для веб-имен символов - здесь для оптимизации функции проверки веб-имен 
        /// </summary>
        private static Char[] RestrictedWebLinkSymbols = { ' ', '\\', '/', '?', ';', ':', '@', '&',
                                                             '=', '+', '$', ',', '<', '>', '"', '#',
                                                             '{', '}', '|', '^', '[', ']', '‘', '%',
                                                             '\n', '\t', '\r', '\b' };
        /// <summary>
        /// Массив запрещенных имен файлов - для коррекции имен файлов
        /// </summary>
        private static String[] RestrictedFileNames = { "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM1", "LPT1", "LPT2", "LPT3", "COM2", "COM3", "COM4" };

        /// <summary>
        /// NFT-Нормализовать имя файла
        /// </summary>
        /// <param name="title">имя файла без расширения</param>
        public static string RemoveWrongSymbols(string title)
        {
            //TODO: Optimize - переработать для ускорения работы насколько это возможно
            //надо удалить все запрещенные символы
            //если пробелы, то символ после них перевести в верхний регистр
            //если прочие символы, заменить на подчеркивание
            //если имя длиннее 16, то обрезать до 16.
            Char[] chars = title.ToCharArray();
            //create string builder
            StringBuilder sb = new StringBuilder(chars.Length);
            //если символ в строке является недопустимым, заменить его на подчеркивание.
            Char c;
            bool toUp = false;//для перевода в верхний регистр
            foreach (char ch in chars)
            {
                c = ch;
                if (ch == ' ')
                {
                    toUp = true;
                    //ничего не записываем в выходной накопитель - так пропускаем все пробелы.
                }
                else
                {
                    //foreach (char ct in RestrictedWebLinkSymbols)
                    //{
                    //    if (ch.Equals(ct))
                    //        c = '_';//замена недопустимого символа на подчеркивание
                    //}
                    //Unicode chars throw exceptions

                    //тут надо пропускать только -_A-Za-zА-Яа-я и все равно будут проблемы с именами файлов архива

                    if (!Char.IsLetterOrDigit(ch))
                        c = '_';//замена недопустимого символа на подчеркивание
                    //перевод в верхний регистр после пробела
                    if (toUp == true)
                    {                       
                        c = Char.ToUpper(c);
                        toUp = false;           //fix 16102017
                    }
                    //if c == мю then c = u
                    if (c == 'µ') c = 'u';
                    //добавить в выходной накопитель
                    sb.Append(c);
                }

            }


            //если имя длиннее максимума, обрезать по максимуму
            if (sb.Length > 16) sb.Length = 16;
            //если имя короче минимума, добавить псевдослучайную последовательность.
            //и проверить, что имя не запрещенное
            if ((sb.Length < 3) || isRestrictedFileName(sb.ToString()))
            {
                sb.Append('_');
                sb.Append(new Random().Next(10, 100).ToString(CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }
        /// <summary>
        /// NT-Проверить, что имя файла является неправильным
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool isRestrictedFileName(string p)
        {
            //fast check = check length
            if ((p.Length > 6) || (p.Length < 3))
                return false;
            //slow check - check content
            foreach (String s in RestrictedFileNames)
                if (String.Equals(s, p, StringComparison.OrdinalIgnoreCase))
                    return true;
            //no restrictions
            return false;
        }

        /// <summary>
        /// NT- Вычислить CRC32 для файла
        /// </summary>
        /// <param name="filepath">Путь к файлу</param>
        /// <returns>Возвращает CRC32</returns>
        public static UInt32 getFileCrc(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read, 16384);
            CRC32s crc = new CRC32s();
            uint val = crc.GetCrc32(fs);
            fs.Close();//08102017 - bugfix

            return val;
        }

        /// <summary>
        /// NT-Создать свободное имя файла в том же каталоге
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string createFreeFileName(string filePath)
        {
            //fast return
            if (!File.Exists(filePath)) return filePath;
            //get parts of name
            String folder = Path.GetDirectoryName(filePath);
            String name = Path.GetFileNameWithoutExtension(filePath);
            if (name.Length > 5)
                name = name.Substring(0, 5);
            String ext = Path.GetExtension(filePath);
            //assembly new filename
            int cnt = 0;
            String result = String.Empty;
            while (true)
            {
                result = Path.Combine(folder, name + cnt.ToString() + ext);
                if (!File.Exists(result))
                    break;
                else
                    cnt++;
            }
            return result;
        }

        /// <summary>
        /// NT-Проверить что содержимое потоков одинаковое
        /// </summary>
        /// <param name="zs">Поток 1</param>
        /// <param name="fs">Поток 2</param>
        /// <returns></returns>
        public static bool CompareStreams(Stream zs, Stream fs)
        {
            Byte[] buf = new byte[4096];
            Byte[] buz = new byte[4096];
            int rdf = 0;
            int rdz = 0;
            while (true)
            {
                rdf = fs.Read(buf, 0, 4096);
                rdz = zs.Read(buz, 0, 4096);
                //если длины не равны, то и файлы не равны
                if (rdf != rdz) return false;
                //content
                for (int i = 0; i < rdf; i++)
                    if (buf[i] != buz[i])
                        return false;

                //конец файла
                if (rdf != 4096) break;
            }
            // вернуть результат
            return true;
        }


        public static string ToHexString(uint val)
        {
            return val.ToString("X8");
        }

        public static uint FromHexString(string val)
        {
            return uint.Parse(val, NumberStyles.HexNumber);
        }
        /// <summary>
        /// NT- compare two files
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool CompareFiles(string filePath, string file2)
        {
            FileStream s1 = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream s2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read);
            bool result = Utility.CompareStreams(s1, s2);
            s1.Close();
            s2.Close();
            return result;
        }


        /// <summary>
        /// NT-Удалить из строки символы, запрещенные в веб-ссылках, 
        /// обрезать по установленному пределу длины,
        /// если текст короче 3 символов, то дополнить его случайными цифрами,
        /// исключить зарезервированные служебные имена файлов и устройств.
        /// </summary>
        /// <param name="title">Исходный текст</param>
        /// <param name="maxlength">Максимальная длина текста</param>
        /// <returns>Возвращает обработанную строку текста</returns>
        public static string makeSafeLinkTitle(string title, int maxlength)
        {
            //TODO: Optimize - переработать для ускорения работы насколько это возможно
            //надо удалить все запрещенные символы
            //мю заменить на u
            //если пробелы, то заменить на подчеркивание
            //прочие символы заменить на подчеркивание
            //сократить подчеркивания до 1
            //если имя длиннее maxlength, то обрезать до maxlength.

            Char[] chars = title.ToCharArray();
            //create string builder
            StringBuilder sb = new StringBuilder(chars.Length);
            Char oldChar = '!';
            foreach (Char ch in chars)
            {
                Char c_out = ch;//так как это выделенная для перечисления переменная, то ее нельзя изменять.
                //если пробелы или запрещенные символы, то заменить на подчеркивание
                foreach (char ct in RestrictedWebLinkSymbols)
                {
                    if (ch == ct)
                    {
                        c_out = '_';//замена недопустимого символа на подчеркивание
                        break;
                    }
                }
                //если мю то заменить на u
                if (ch == 'µ') c_out = 'u';
                //сократить подчеркивания до 1
                if ((c_out != '_') || (oldChar != '_'))
                    sb.Append(c_out);//добавить в выходной накопитель.
                oldChar = c_out;
            }
            //если имя длиннее максимума, обрезать по максимуму
            if (sb.Length > maxlength) sb.Length = maxlength;
            //если имя короче минимума, добавить псевдослучайную последовательность.
            //и проверить, что имя не запрещенное
            if ((sb.Length < 3) || isRestrictedFileName(sb.ToString()))
            {
                sb.Append('_');
                sb.Append(new Random().Next(10, 100).ToString(CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        /// <summary>
        /// NT-если не удалось создать и открыть файл для записи, то вернуть null.
        /// </summary>
        /// <remarks>Функция используется вне сборки</remarks>
        /// <param name="filename">Путь к файлу</param>
        /// <returns></returns>
        public static FileStream tryCreateFile(String filename)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (Exception)
            {
                fs = null;
            }
            return fs;
        }

        #region *** Функции, используемые в клиентах менеджера хранилищ ***
        /// <summary>
        /// NT-убирает точку и делает следующую букву в верхний регистр.
        /// Пример: .pdf -> Pdf
        /// </summary>
        /// <param name="p">расширение файла</param>
        /// <returns></returns>
        public static string createDocTypeFromFileExtension(string p)
        {
            //TODO: Эту функцию можно перенести в движок Хранилищ - она часто используется.

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


        #endregion

        /// <summary>
        /// NT-Проверить наличие в словаре ключа  без учета регистра символов
        /// </summary>
        /// <param name="dict">Словарь</param>
        /// <param name="key">Ключ</param>
        /// <returns>Функция возвращает True если словарь содержит данный ключ без учета регистра символов. Иначе возвращается False</returns>
        internal static bool DictionaryContainsKeyIgnoreCase(Dictionary<string, int> dict, string key)
        {
            foreach (KeyValuePair<string, Int32> kvp in dict)
            {
                if (String.Equals(key, kvp.Key, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
