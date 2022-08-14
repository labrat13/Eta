using System;
using System.Collections.Generic;
using System.Text;
using InventoryModuleManager2;
using System.IO;
using System.Data.OleDb;
using InventoryModuleManager2.ClassificationEntity;


namespace Test
{
    
    /// <summary>
    /// Различные тесты и функции-конвертеры для Хранилищ
    /// Тут большая куча мусора, которую надо бы разгрести
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            
            testReading("C:\\Temp\\RadioBase Storages");

            //AddGifToРадиодетали("C:\\Temp\\RadioBase Storages\\Копия Радиодеталь", "C:\\Temp\\В ХРАНИЛИЩЕ МИКРОСХЕМ КАК ДОКУМЕНТЫ\\MicrosGifPart1");
            //AddGifToРадиодетали("C:\\Temp\\RadioBase Storages\\Копия Радиодеталь", "C:\\Temp\\В ХРАНИЛИЩЕ МИКРОСХЕМ КАК ДОКУМЕНТЫ\\MicrosGifPart2");
            //AddGifToРадиодетали("C:\\Temp\\RadioBase Storages\\Копия Копия Радиодеталь", "C:\\Temp\\GIF - добавить в хранилище микросхем как документы");
            
            //AddToStorageРадиодетали("C:\\Temp\\RadioBase Storages\\Копия Микросхема", "", "C:\\Temp\\Chip\\listing.csv");
            //AddToStorageРадиодетали("C:\\Temp\\RadioBase Storages\\Копия Радиодеталь", "F:\\Radiodata\\Справочные\\АлфавитныйДаташиты", "C:\\Temp\\Подготовить данные для радиобазы\\alphavit.csv");
            //УдалитьФайлы("C:\\Temp\\Chip\\listing.csv");
            //УдалитьФайлы("C:\\Temp\\Подготовить данные для радиобазы\\alphavit.csv");
            //TestLinkBuilder();
            
            //TestClasses();
            
            //StorageElectronics();

            //RunTests();
            Console.WriteLine("Press enter to exit application");
            Console.ReadLine();
        }

        private static void AddGifToРадиодетали(string storage, string srcFolder)
        {
            //get files
            String[] files = Directory.GetFiles(srcFolder, "*.gif");
            String[] files2 = Directory.GetFiles(srcFolder, "*.jpg");
            List<string> filesList = new List<string>();
            filesList.AddRange(files);
            filesList.AddRange(files2);
            //open manager
            Manager man = new Manager();
            man.Open(storage);
            //process files
            foreach (String file in filesList)
            {
                string title = Path.GetFileNameWithoutExtension(file);
                //clear partname from any trash
                string[] sar = title.Split(' ', '(', '=');
                string title2 = sar[0];
                //load description text file if exists
                string textfilepath = Path.ChangeExtension(file, ".txt");
                String description = "";
                if (File.Exists(textfilepath))
                {
                    StreamReader sr = new StreamReader(textfilepath, Encoding.GetEncoding(1251));
                    description = sr.ReadToEnd();
                    sr.Close();
                }
                //add to storage as document
                tryAddEntity(man, title2, "Микросхема", description, file, null);
            }
            //4 закрыть хранилище

            man.Close();//это может быть долго - менеджер добавляет в хранилище остаток буфера файлов.
            return;
        }
        /// <summary>
        /// Удалить файлы, перечисленные в ксв-файле, теперь они не нужны, так как помещены в хранилище
        /// </summary>
        /// <param name="csvFilePath"></param>
        private static void УдалитьФайлы(string csvFilePath)
        {
            //1 открыть и распарсить ксв-файл
            StreamReader sr = new StreamReader(csvFilePath, Encoding.GetEncoding(1251));
            sr.ReadLine(); //read column names line
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                String[] sar = line.Split(';');
                if (sar.Length != 7)
                    continue;
                //read values
                Console.WriteLine(sar[0]); //show number
                String file = sar[6].Trim();
                File.Delete(file);
            }


            // закрыть ксв-файл
            sr.Close();
            return;
        }

        private static void AddToStorageРадиодетали(string storagePath, string filesDir, string csvFilePath)
        {
            //open manager
            Manager man = new Manager();
            man.Open(storagePath);
            
            //1 открыть и распарсить ксв-файл
            StreamReader sr = new StreamReader(csvFilePath, Encoding.GetEncoding(1251));
            sr.ReadLine(); //read column names line
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                String[] sar = line.Split(';');
                if (sar.Length != 7)
                    continue;
                //read values
                Console.WriteLine(sar[0]); //show number
                String file = sar[6].Trim();
                String category = sar[2].Trim();
                String partname = sar[1].Trim();
                String description = String.Format("{0}; {1};", sar[3].Replace("\"", "").Trim(), sar[4]);

                if (category == "ApplicationNote")
                    continue;

                ////parse partnames string
                //String[] names = partnames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //if (names.Length < 1)
                //    continue;
                //write to storage here
                //foreach (string name in names)
                tryAddEntity(man, partname, category, description, file, null);

            }

            //2 открыть хранилище
            //3 добавить сущности в хранилище пакетно
            //4 закрыть хранилище и ксв-файл
            sr.Close();
            man.Close();//это может быть долго - менеджер добавляет в хранилище остаток буфера файлов.
            return;
        }

        private static void TestLinkBuilder()
        {
            LinkBuilder lb1 = new LinkBuilder("");
            lb1.Prefix = "tree";
            lb1.StorageQName = "Предмет.Радиодеталь.Радиодеталь1";
            lb1.TableId = 117;
            lb1.Title = "Квартет: резиновый {гандон} на гусеничном ходу";
            lb1.Filepath = "docs1/file.ext";
            //сделаем массив строк для теста парсера
            List<string> li = new List<string>();
            li.Add(lb1.getStorageLink());
            li.Add(lb1.getFileLink());
            li.Add(lb1.getEntityRecordLink());
            li.Add(lb1.getDocumentRecordLink());
            li.Add(lb1.getPictureRecordLink());
            //проверим парсер
            foreach (String s in li)
            {
                Console.WriteLine(s);
                LinkBuilder lb = new LinkBuilder(s);
                Console.WriteLine("Link type = {0}", lb.Linktype.ToString());
            }
        }

        private static void TestClasses()
        {
            String[] baseClasses = new string[] {
            "Предмет",
            "Предмет::Радиодеталь",
            "Предмет::Радиодеталь::Транзистор",
            "Собака",
            "Собака:::Лайка" };//это типа неправильный ввод класса оператором. Парсер просто делит это на Собака и :Лайка, и так и обрабатывает потом.

            String[] entityClasses = new string[] {
            "Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный",
            "Радиодеталь::Транзистор::ТранзисторБиполярный",
            "Транзистор::ТранзисторБиполярный",
            "ТранзисторБиполярный",
            "Собака" };

            //test single class
            ClassItem c1 = new ClassItem("Предмет");
            ClassItem c11 = c1.getParentClass();

            ClassItem c2 = new ClassItem("Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный");
            ClassItem c21 = c2.getParentClass();

            ClassItem c3 = new ClassItem("Собака", "Собака");//специальный случай, он корректно обрабатывается сейчас.

            //foreach(string s in baseClasses)
            //    foreach (string ss in entityClasses)
            //    {
            //        ClassItem ci = new ClassItem(s, ss);
            //        Console.WriteLine(ci.ToString());
            //    }

            //test subtraction
            string result = ClassItem.SubtractClassPath(c2.ClassPath, c1.ClassPath);
            result = ClassItem.SubtractClassPath2(c2.ClassPath, c1.ClassPath);

            result = ClassItem.SubtractClassPath("Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный", "Радиодеталь");
            result = ClassItem.SubtractClassPath2("Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный", "Радиодеталь");

            result = ClassItem.SubtractClassPath("Собака", "Собака");
            result = ClassItem.SubtractClassPath2("Собака", "Собака");

            //result = ClassItem.SubtractClassPath("Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный", "Собака");
            result = ClassItem.SubtractClassPath2("Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный", "Собака");

            return;
        }

        private static void StorageElectronics()
        {
            //createStorageElectronics();
            //fillStorageElectronics("КнигаЭлектроникаСборникЗарубежнаяЭлектроннаяТехника", "F:\\Books\\Электроника\\Сборники\\Зарубежная электронная техника Сборники");

            testReading(null); //read all files in all storages
        }

        /// <summary>
        /// Add content to Electronics storage
        /// </summary>
        /// <param name="srcCategory"></param>
        /// <param name="srcFolder"></param>
        private static void fillStorageElectronics(String baseCategory, String srcDir)
        {
            Console.WriteLine();
            Console.WriteLine("Import {0}: {1}", baseCategory, srcDir);

            //3 добавляем журналы из каталога F:\Magazins\radio_konstruktor
            //get files
            String[] pdfs = Directory.GetFiles(srcDir, "*.pdf", SearchOption.AllDirectories);
            String[] djvs = Directory.GetFiles(srcDir, "*.djv*", SearchOption.AllDirectories);
            //String[] jpg = Directory.GetFiles(srcDir, "*.jp*", SearchOption.AllDirectories);
            //open manager
            Manager man = new Manager();
            man.Open("C:\\Work\\myVirtualDisk\\Storages\\КнигаЭлектроника");
            //process files
            List<string> files = new List<string>(pdfs);
            files.AddRange(djvs);
            int counter = 0;
            foreach (string doc in files)
            {
                counter++;
                //get picture and description files
                String pic = tryGetPicture(doc);
                string description = tryGetDescription(doc);

                String cat = baseCategory;
                String title = Path.GetFileNameWithoutExtension(doc);
                //make entity description
                description = title
                    + Environment.NewLine
                    + description;

                tryAddEntity(man, title, cat, description, doc, pic);
                Console.CursorLeft = 0;
                Console.Write("Добавлено {0} из {1}", counter, files.Count);
            }

            man.Close();

            return;
        }

        private static void createStorageElectronics()
        {
            StorageInfo si = new StorageInfo();
            //si.Creator = "Я";//это не обязательное поле, можно прописать значение в конструкторе.
            si.Title = "КнигаЭлектроника"; //Название Хранилища,
            si.QualifiedName = "Книга.КнигаЭлектроника";//Квалифицированное имя хранилища
            si.Description = "Различные книги по электронике";
            si.StorageType = "Книга::КнигаЭлектроника<Документ, Изображение>";//Тип Хранилища или его схема в нотации типов Оператора
            si.StoragePath = "C:\\Work\\myVirtualDisk\\Storages\\КнигаЭлектроника";//Это путь к КаталогХранилища.

            //Создаем Хранилище
            Manager man = Manager.CreateStorage(si);
            //далее работаем с хранилищем
            man.Close();

            return;
        }




        private static void RunTests()
        {
            try
            {

                string storagePath = "C:\\Temp\\ЖурналМурзилка"; //тестовое хранилище
                //NOT TEST! try import prop modules
                //ImportPropModule();


                //ImportMagazins("Журнал",
                //    "Домашняя лаборатория",
                //    "Научно-прикладной некоммерческий интернет-журнал самоделок и полезных сведений.",
                //    "ДомашняяЛаборатория",
                //    "F:\\Magazins\\Домашняя лаборатория"
                //    );

                //ImportMagazins("Сборник",
                //    "Юный моделист конструктор",
                //    "",
                //    "ЮныйМоделистКонструктор",
                //    "F:\\Books\\Сборники\\имя\\Юный моделист конструктор"
                //    );

                //ImportMagazins("Сборник",
                //    "В помощь радиолюбителю",
                //    "Описания любительской радиоаппаратуры, справочные и расчетные материалы.",
                //    "ВПомощьРадиолюбителю",
                //    "F:\\Books\\Электроника\\Сборники\\В помощь радиолюбителю"
                //    );

                testReading(null); //read all files in all storages

                //-------------------------------------------------
                //testStorageInfo_LoadStore();
                //testSession();
                ////1 - create storage
                //testCreateStorage();

                ////2 - add entity
                //testAddEntity(storagePath);

                ////3 get entity
                //testGetEntity(storagePath);
                //TestGetFile(storagePath);

                ////4
                //testImportPropModule(storagePath, "C:\\Temp\\Радиодетали16");// - импортирует данные из модуля свойств, создавая большое хранилище для тестов

                //testStorageInfo(storagePath);

                ////5
                //testEnumerateEntity(storagePath);

                ////6
                //testEnumerateDocuments(storagePath);

                ////7
                //testLikeCmd(storagePath);//успешно

                ////test change entity

                ////test find document dublicates

                ////test find picture dublicates

                ////test Optimize Storage

                ////8
                //testClearStorage(storagePath);

                ////9
                //testStorageInfo(storagePath);

                ////10
                //testDeleteStorage(storagePath);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return;
        }

        /// <summary>
        /// Создать хранилище для журнала.
        /// </summary>
        /// <param name="magType">Тип сборника: Журнал Сборник итд</param>
        /// <param name="MagazinTitle">Название журнала исходное, с дефисами и пробелами итп.</param>
        /// <param name="MagazinDescription">Описание журнала  исходное, с дефисами и пробелами итп.</param>
        /// <param name="baseName">Название журнала без спецсимволов и пробелов</param>
        /// <param name="srcDir">Каталог файлов журнала</param>
        private static void ImportMagazins(String magType, String MagazinTitle, String MagazinDescription, String baseName, String srcDir)
        {
            //1 - Задаем описание и настройки
            //String MagazinTitle = "Радиоаматор";//Название журнала исходное, с дефисами и пробелами итп.
            //String MagazinDescription = "";
            //String baseName = "Радиоаматор";//Название журнала без спецсимволов и пробелов
            String baseCategory = magType + baseName;
            //String srcDir = "F:\\Magazins\\radioamator";
            Console.WriteLine();
            Console.WriteLine("Import {0}: {1}", magType, MagazinTitle); 
            //Изображения журналов должны иметь то же имя файла, что и файлы журналов!
            //Название файла журнала должно быть в формате: Название_год_номер или Название-год-номер  или Название год номер
            
            //2 создаем описание хранилища
            StorageInfo si = new StorageInfo();
            //si.Creator = "Я";//это не обязательное поле, можно прописать значение в конструкторе.
            si.Title = baseCategory; //Название Хранилища,
            si.QualifiedName = magType + "." + baseCategory;//Квалифицированное имя хранилища
            si.Description = "Название=" + magType + MagazinTitle + Environment.NewLine + "Описание=" + MagazinDescription + Environment.NewLine;
            si.StorageType = String.Format("{0}::{1}<Документ, Изображение>", magType, baseCategory);//Тип Хранилища или его схема в нотации типов Оператора
            si.StoragePath = "C:\\Work\\myVirtualDisk\\Storages\\" + baseCategory;//Это путь к КаталогХранилища.

            //Создаем Хранилище
            Manager man = Manager.CreateStorage(si);
            //далее работаем с хранилищем
            man.Close();

            //3 добавляем журналы из каталога F:\Magazins\radio_konstruktor
            
            //установим базовую категорию для формирования подкатегорий по годам

            //get files
            String[] pdfs = Directory.GetFiles(srcDir, "*.pdf", SearchOption.AllDirectories);
            String[] djvs = Directory.GetFiles(srcDir, "*.djv*", SearchOption.AllDirectories);
            //String[] jpg = Directory.GetFiles(srcDir, "*.jp*", SearchOption.AllDirectories);
            //open manager
            man = new Manager();
            man.Open(si.StoragePath);
            //process files
            List<string> files = new List<string>(pdfs);
            files.AddRange(djvs);
            int counter = 0;
            foreach (string doc in files)
            {
                counter++;
                //get picture
                String pic = tryGetPicture(doc);
                string description = tryGetDescription(doc);
                //extract name, year, number of magazine
                String year;
                String name;
                String no;
                //TODO: тут выбрать правильный способ разбора имени файла
                //tryParseNameNameYearNoPlus(doc, out name, out year, out no);
                //create entity with files
                //name = baseName; //заменить имя на культурное
                //String cat = baseCategory + "::" + baseCategory + year.ToString() + "г";
                //String title = String.Format("{0}-{1}-{2}", baseName, year, no);
                //make entity description
                //description = String.Format("{0} {1} №{2} за {3} год.", magType, MagazinTitle, no, year)
                //    + Environment.NewLine
                //    + description;

                String cat = baseCategory;
                String title = Path.GetFileNameWithoutExtension(doc);
                //make entity description
                description = String.Format("{0} {1} : {2}", magType, MagazinTitle, title)
                    + Environment.NewLine
                    + description;

                tryAddEntity(man, title, cat, description, doc, pic);
                Console.CursorLeft = 0;
                Console.Write("Добавлено {0} из {1}", counter, files.Count);
            }

            man.Close();

            return;
        }

        private static string tryGetDescription(string doc)
        {
            //если там нет файла с тем же именем и расширением txt, то возвращаем пустую строку
            String filename = Path.ChangeExtension(doc, null);
            String s = filename + ".txt";
            String result = String.Empty;
            if (File.Exists(s))
            {
                StreamReader sr = new StreamReader(s, Encoding.Default, true);
                result = sr.ReadToEnd();
                sr.Close();
                return result;
            }
            s = filename + "_inf.txt";
            if (File.Exists(s))
            {
                StreamReader sr = new StreamReader(s, Encoding.Default, true);
                result = sr.ReadToEnd();
                sr.Close();
                return result;
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="man"></param>
        /// <param name="name"></param>
        /// <param name="year"></param>
        /// <param name="no"></param>
        /// <param name="doc"></param>
        /// <param name="pic"></param>
        private static void tryAddEntity(Manager man, string title, string cat, string descr, string docpath, string picpath)
        {
            //4 создать запись сущности
            //создаем полную запись с файлами документа и картинки
            EntityRecord e = new EntityRecord();
            e.Title = title;
            e.EntityType = new ClassItem(man.StorageBaseClass, cat);
            e.Description = descr;
            if (!String.IsNullOrEmpty(docpath))
            {
                if (File.Exists(docpath))
                {
                    FileRecord frd = new FileRecord(docpath);
                    frd.Description = descr;
                    frd.DocumentType = "Document" + makeTypeByExt(Path.GetExtension(docpath));//указывать только конечный тип сущности
                    e.Document = frd;
                }
                else
                    Console.WriteLine("Error: "+docpath); //log file access error
            }
            if (!String.IsNullOrEmpty(picpath))
            {
                if (File.Exists(picpath))
                {
                    FileRecord frp = new FileRecord(picpath);
                    frp.Description = title;
                    frp.DocumentType = "Picture" + makeTypeByExt(Path.GetExtension(picpath));//указывать только конечный тип сущности
                    e.Picture = frp;
                }
                else
                    Console.WriteLine("Error: " + picpath); //log file access error
            }
            //5 добавить сущность в менеджер хранилища
            man.AddEntityBuffered(e);
            //6 закончить цикл
            return;
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <param name="year"></param>
        /// <param name="no"></param>
        private static void tryParseNameNameYearNo(string doc, out string name, out string year, out string no)
        {
            String filename = Path.GetFileNameWithoutExtension(doc);
            Char[] delimiters = new char[] { '_', ' ', '-' };
            String[] parts = filename.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //тут надо разделить на имя, год и выпуск, если они есть.
            //имя потом заменить на правильное
            //rk_2000_01
            if (parts.Length != 3)
                throw new Exception("Invalid name format: " + doc);
            name = parts[0];
            year = parts[1];
            no = parts[2];

            return;
        }

        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <param name="year"></param>
        /// <param name="no"></param>
        private static void tryParseNameNameYearNoPlus(string doc, out string name, out string year, out string no)
        {
            String filename = Path.GetFileNameWithoutExtension(doc);
            Char[] delimiters = new char[] { '_', ' ', '-' };
            String[] parts = filename.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //тут надо разделить на имя, год и выпуск, если они есть.
            //имя потом заменить на правильное
            //rk_2000_01
            if (parts.Length < 3)
                throw new Exception("Invalid name format: " + doc);
            name = parts[0];
            year = parts[1];
            no = "";
            for (int i = 2; i < parts.Length; i++ )
                no = no + "-" + parts[i];

            return;
        }

        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static string tryGetPicture(string doc)
        {
            //картинка если есть, то в той же папке что и сам журнал
            String s = Path.ChangeExtension(doc, ".jpg");
            if (File.Exists(s))
                return s;
            s = Path.ChangeExtension(doc, ".jpeg");
            if (File.Exists(s))
                return s;
            s = Path.ChangeExtension(doc, ".png");
            if (File.Exists(s))
                return s;
            s = Path.ChangeExtension(doc, ".gif");
            if (File.Exists(s))
                return s;
            s = Path.ChangeExtension(doc, ".bmp");
            if (File.Exists(s))
                return s;
            //not found
            return null;
        }

        private static void ImportPropModule()
        {
            //1 создаем описание хранилища
            StorageInfo si = new StorageInfo();
            //si.Creator = "Я";//это не обязательное поле, можно прописать значение в конструкторе.
            si.Title = "AppNote"; //Название Хранилища,
            si.QualifiedName = "Документ.ДокументAppNote";//Квалифицированное имя хранилища
            si.Description = "Импорт из модуля свойств";
            si.StorageType = "Документ::ДокументAppNote<Документ, Изображение>";//Тип Хранилища или его схема в нотации типов Оператора
            si.StoragePath = "C:\\Work\\myVirtualDisk\\Радиодата\\ДокументAppNote";//Это путь к КаталогХранилища.

            //Создаем Хранилище
            Manager man = Manager.CreateStorage(si);
            //далее работаем с хранилищем
            man.Close();
            

            //2 импортируем данные из модуля свойств
            testImportPropModule(si.StoragePath, "C:\\Temp\\Радиодата\\Документ");

            Console.WriteLine("Import prop module success");
            Console.WriteLine("Press Enter to get basic test's");
            Console.ReadLine();
            return;
        }

        private static void TestGetFile(string path)
        {
            Console.WriteLine("Test get file");
            Manager man = new Manager();
            man.Open(path);
            //далее работаем с хранилищем
            EntityRecord[] recs = man.GetEntity("КТ315А");
            recs[0].Document.GetFile("C:\\Temp\\doc_t.pdf");
            man.GetFile(recs[0].Picture.QualifiedName, "C:\\Temp\\pic_t.jpeg");

            man.Close();
            return;
        }
        /// <summary>
        /// Тест получения всех файлов всех хранилищ, находящихся в указанной папке
        /// </summary>
        /// <param name="StoragesFolder">Папка, содержащая папки проверяемых хранилищ</param>
        private static void testReading(String StoragesFolder)
        {
            
            Console.WriteLine("Тест чтения файлов и записей");
            String[] storageFolders = Directory.GetDirectories(StoragesFolder);
            //String outputFile = "C:\\Temp\\tmpfile.pdf";
            //Byte[] buffer = new Byte[32768];
            String tempDocPath = "C:\\Temp\\doc.pdf";
            String tempPicPath = "C:\\Temp\\pic.jpg";
            String archfilename = String.Empty;
            Manager man;
            foreach (String s in storageFolders)
            {
                Console.WriteLine("Тест " + s);
                //check manager
                try
                {
                    man = new Manager();
                    man.Open(s);
                    //далее работаем с хранилищем
                    Console.WriteLine("Read documents");
                    int doccnt = 0;
                    foreach (FileRecord fr in man.Documents)
                    {
                        doccnt++;
                        archfilename = fr.StoragePath;
                        //просто проверим что что-то доступно из файла
                        fr.GetFile(tempDocPath);
                        if (File.Exists(tempDocPath))
                            File.Delete(tempDocPath);
                        else throw new Exception(String.Format("File {0} not exists", archfilename));
                        //print 
                        Console.CursorLeft = 0;
                        Console.Write(doccnt.ToString() + "  " + archfilename + "    ");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Read pictures");
                    int piccnt = 0;
                    foreach (FileRecord fr in man.Pictures)
                    {
                        piccnt++;
                        archfilename = fr.StoragePath;
                        //просто проверим что что-то доступно из файла
                        fr.GetFile(tempPicPath);
                        if (File.Exists(tempPicPath))
                            File.Delete(tempPicPath);
                        else throw new Exception(String.Format("File {0} not exists", archfilename));
                        //print 
                        Console.CursorLeft = 0;
                        Console.Write(piccnt.ToString() + "  " + archfilename + "     ");
                    }
                    man.Close();
                    Console.WriteLine();
                    Console.WriteLine("Storage files read success!");
                    Console.WriteLine("Doc files: " + doccnt.ToString());
                    Console.WriteLine("Pic files: " + piccnt.ToString());
                    //Console.WriteLine("Press enter to process next storage");
                    //Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("File: " + archfilename);
                    Console.WriteLine(ex.ToString());

                }

            }

            return;
        }

        private static void testClearStorage(string path)
        {
            Console.WriteLine("Test clear storage");
            Manager man = new Manager();
            man.Open(path);
            //далее работаем с хранилищем
            man.ClearStorage();

            man.Close();
            return;
        }

        private static void testDeleteStorage(string p)
        {
            Console.WriteLine("Test delete storage");
            Manager.DeleteStorage(p);
        }

        private static void testLikeCmd(string path)
        {
            Console.WriteLine("Test get entitles LIKE pattern");
            Manager man = new Manager();
            man.Open(path);
            //далее работаем с хранилищем
            EntityRecord[] rs = man.FindEntity("КД*", true, false, false, 100);
            foreach (EntityRecord er in rs)
                Console.WriteLine(er.ToString());
            man.Close();
            return;
        }

        private static void deleteSourceFiles(string p)
        {
            String PropertiesModuleFolder = "C:\\Temp\\Радиодетали16";
            //1 открыть БД модуля
            String dbpath = Path.Combine(PropertiesModuleFolder, "Радиодетали16.mdb");
            String constring = createConnectionString(dbpath);
            //create connection
            OleDbConnection con = new OleDbConnection(constring);

            //3 считать каждую запись из БД модуля 
            //open new connection and set as primary
            con.Open();
            String query = "SELECT * FROM info;";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.CommandTimeout = 1000;
            OleDbDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    String cat = getDbString(rdr, 1);
                    String title = getDbString(rdr, 2);
                    String descr = getDbString(rdr, 3);
                    String doc = getDbString(rdr, 4);
                    String pic = getDbString(rdr, 5);

                    Console.WriteLine("id={0}", id);

                    //4 
                    if (!String.IsNullOrEmpty(doc))
                    {
                        String docpath = Path.Combine(PropertiesModuleFolder, doc);
                        if (File.Exists(docpath))
                        {
                            File.Delete(docpath);
                        }
                    }
                    if (!String.IsNullOrEmpty(pic))
                    {
                        String picpath = Path.Combine(PropertiesModuleFolder, pic);
                        if (File.Exists(picpath))
                        {
                            File.Delete(picpath);
                        }

                    }
                    //6 закончить цикл
                }
            rdr.Close();
            //7 закрыть БД
            con.Close();


            return;
        }

        private static void testStorageInfo(string path)
        {
            Console.WriteLine("Test storage info");
            Manager man = new Manager();
            man.Open(path);
            //далее работаем с хранилищем
            StorageInfo si = man.GetStorageInfo();
            Console.WriteLine(si.ToString());

            man.Close();
            return;
        }


        private static void testImportPropModule(String storagePath, String PropertiesModuleFolder)
        {
            Console.WriteLine("Test import properties module");
            //1 открыть БД модуля
            String dbname = Path.GetFileName(PropertiesModuleFolder) + ".mdb";
            String dbpath = Path.Combine(PropertiesModuleFolder, dbname);
            String constring = createConnectionString(dbpath);
            //create connection
            OleDbConnection con = new OleDbConnection(constring);


            ////2 создать новое хранилище для конверсии модуля
            ////создаем описание хранилища
            //StorageInfo si = new StorageInfo();
            ////si.Creator = "Я";//это не обязательное поле, можно прописать значение в конструкторе.
            //si.Title = "Модуль16"; //Название Хранилища,
            //si.QualifiedName = "Радиодетали.Модуль16";//Квалифицированное имя хранилища
            //si.Description = "Модуль свойств Радиодетали16";
            //si.StorageType = "МодульСвойствИнвентарь<Радиодеталь>";//Тип Хранилища или его схема в нотации типов Оператора
            //si.StoragePath = "C:\\Temp\\Модуль16";//Это путь к КаталогХранилища.

            //Открываем Хранилище
            Manager man = new Manager();
            man.Open(storagePath);
            //далее работаем с хранилищем
            int docCounter = 0;//счетчики обработанных файлов
            int picCounter = 0;
            StreamWriter sw = new StreamWriter("C:\\Temp\\missing.txt", false, Encoding.Unicode);
            //3 считать каждую запись из БД модуля 
            //open new connection and set as primary
            con.Open();
            String query = "SELECT * FROM info;";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.CommandTimeout = 1000;
            OleDbDataReader rdr = cmd.ExecuteReader();
            if(rdr.HasRows)
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    String cat = getDbString(rdr, 1);
                    String title = getDbString(rdr, 2);
                    String descr = getDbString(rdr, 3);
                    String doc = getDbString(rdr, 4);
                    String pic = getDbString(rdr, 5);

                    Console.CursorLeft = 0;
                    Console.Write("id={0}", id);
                    
                    //4 создать запись сущности
                    //создаем полную запись с файлами документа и картинки
                    EntityRecord e = new EntityRecord();
                    e.Title = title;
                    e.EntityType = new ClassItem(man.StorageBaseClass, cat);
                    e.Description = descr;
                    if (!String.IsNullOrEmpty(doc))
                    {
                        String docpath = Path.Combine(PropertiesModuleFolder, doc);
                        if (File.Exists(docpath))
                        {
                            FileRecord frd = new FileRecord(docpath);
                            frd.Description = "";
                            frd.DocumentType = "Document" + makeTypeByExt(Path.GetExtension(docpath));//указывать только конечный тип сущности
                            e.Document = frd;

                            docCounter++;
                        }
                        else
                            sw.WriteLine(docpath); //log file access error
                    }
                    if (!String.IsNullOrEmpty(pic))
                    {
                        String picpath = Path.Combine(PropertiesModuleFolder, pic);
                        if (File.Exists(picpath))
                        {
                            FileRecord frp = new FileRecord(picpath);
                            frp.Description = "";
                            frp.DocumentType = "Picture" + makeTypeByExt(Path.GetExtension(picpath));//указывать только конечный тип сущности
                            e.Picture = frp;

                            picCounter++;
                        }
                        else
                            sw.WriteLine(picpath); //log file access error
                    }
                        //5 добавить сущность в менеджер хранилища
                    man.AddEntityBuffered(e);
                    //6 закончить цикл
                }
            rdr.Close();
            //7 закрыть БД
            con.Close();
            //8 закрыть менеджер
            man.Close();
            //9 конец
            sw.Close();
            Console.WriteLine();
            Console.WriteLine("Doc files: {0}", docCounter);
            Console.WriteLine("Pic files: {0}", picCounter);
            //Console.ReadKey();
            return;
        }

        /// <summary>
        /// NT-убирает точку и делает следующую букву в верхний регистр.
        /// Пример: .pdf -> Pdf
        /// </summary>
        /// <param name="p">расширение файла</param>
        /// <returns></returns>
        private static string makeTypeByExt(string p)
        {
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

        private static string getDbString(OleDbDataReader rdr, int p)
        {
            if (rdr.IsDBNull(p))
                return String.Empty;
            else return rdr.GetString(p);
        }



        private static void testEnumerateDocuments( String storagePath)
        {
            //TODO: Надо переделывать енумератор замудренный, и дает больше записей чем есть в БД, но сейчас пока работает.

            Console.WriteLine("Test enumerate 256 documents");
            Manager man = new Manager();
            man.Open(storagePath);
            //далее работаем с хранилищем
            int cnt = 0;
            foreach (FileRecord er in man.Documents)
            {
                Console.WriteLine("[" + cnt + "] = " + er.ToString());
                //выводим только первые 256 записей - уж очень долго их все ждать.
                cnt++;
                if (cnt == 256) break;
            }

            man.Close();
            return;
        }

        private static void testEnumerateEntity(String storagePath)
        {
            //TODO: Надо переделывать енумератор замудренный, и дает больше записей чем есть в БД, но сейчас пока работает.

            Console.WriteLine("Test enumerate 256 entities");
            Manager man = new Manager();
            man.Open(storagePath);
            //далее работаем с хранилищем
            int cnt = 0;
            foreach (EntityRecord er in man.Entities)
            {
                Console.WriteLine("["+ cnt + "] = " + er.ToString());
                //выводим только первые 256 записей - уж очень долго их все ждать.
                cnt++;
                if (cnt == 256) break;
            }

            man.Close();
            return;
        }

        private static void testGetEntity(String storagePath)
        {
            Console.WriteLine("Test get entity");
            Manager man = new Manager();
            man.Open(storagePath);
            //далее работаем с хранилищем
            EntityRecord[] recs = man.GetEntity("КТ315А");

            man.Close();
            return;
        }

        private static void testAddEntity(string storagePath)
        {
            Console.WriteLine("Test add entity");
            Manager man = new Manager();
            man.Open(storagePath);
            //далее работаем с хранилищем
            //создаем полную запись с файлами документа и картинки
            EntityRecord e = new EntityRecord();
            e.Title = "КТ315А";
            e.EntityType = new ClassItem(man.StorageBaseClass, "Транзистор");
            e.Description = "NPN;Uke=20v;Ikmax=100mA;h21e=60;";

            FileRecord doc = new FileRecord("C:\\Temp\\sample\\doc.pdf");
            doc.Description = "User description text";
            doc.DocumentType = "DocumentPdf";//указывать только конечный тип сущности
            e.Document = doc;
            FileRecord pic = new FileRecord("C:\\Temp\\sample\\pic.jpeg");
            pic.Description = "User description text for picture";
            pic.DocumentType = "PictureJpeg";//указывать только конечный тип сущности
            e.Picture = pic; 
            //или, чтобы без конструкторов
            //e.SetDocument("C:\\Temp\\entity\\Kasapenko.pdf");//это скрывает реализацию
            //e.SetPicture("C:\\Temp\\entity\\схема.jpg");
            ////а чтобы отцепить файлы
            //e.RemoveDocument();
            //e.RemovePicture();

            man.AddEntity(e);


            man.Close();
            return;
        }

        private static void testSession()
        {
            Session s = new Session();
            s.Open();
            s.Close();
        }

        private static void testStorageInfo_LoadStore()
        {
            //create and fill test values
            StorageInfo si = new StorageInfo();
            si.fillTestValues();
            //store to file
            si.StoreToFile("C:\\Temp\\test1.xml");
            // load file
            StorageInfo si2 = new StorageInfo();
            si2.LoadFromFile("C:\\Temp\\test1.xml");
            //compare manually

            return;
        }

        static void testCreateStorage()
        {
            Console.WriteLine("Test create storage...");
            //создаем описание хранилища
            StorageInfo si = new StorageInfo();
            //si.Creator = "Я";//это не обязательное поле, можно прописать значение в конструкторе.
            si.Title = "ЖурналМурзилка"; //Название Хранилища,
            si.QualifiedName = "Журнал.ЖурналМурзилка";//Квалифицированное имя хранилища
            si.Description = "Все имеющиеся журналы Мурзилка.";
            si.StorageType = "Журнал::ЖурналМурзилка<Статья>";//Тип Хранилища или его схема в нотации типов Оператора
            si.StoragePath = "C:\\Temp\\ЖурналМурзилка";//Это путь к КаталогХранилища.

            //Создаем Хранилище
            Manager man = Manager.CreateStorage(si);
            //далее работаем с хранилищем

            man.Close();
            return;
        }

        static void testOpenStorage(String storagePath)
        {
            Console.WriteLine("Test open storage");
            Manager man = new Manager();
            man.Open(storagePath);
            //далее работаем с хранилищем
            man.Close();
            return;
        }


        /// <summary>
        /// NT-Создать строку соединения с БД
        /// </summary>
        /// <param name="dbFile">Путь к файлу БД</param>
        public static string createConnectionString(string dbFile)
        {
            //Provider=Microsoft.Jet.OLEDB.4.0;Data Source="C:\Documents and Settings\salomatin\Мои документы\Visual Studio 2008\Projects\RadioBase\радиодетали.mdb"
            OleDbConnectionStringBuilder b = new OleDbConnectionStringBuilder();
            b.Provider = "Microsoft.Jet.OLEDB.4.0";
            b.DataSource = dbFile;
            //user id and password can specify here
            return b.ConnectionString;
        }

    }
}
