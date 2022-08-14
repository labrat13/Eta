using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryModuleManager2.ClassificationEntity
{
    /// <summary>
    /// Представляет классы в системе Хранилищ - для Сущности и для Хранилища 
    /// </summary>
    public class ClassItem
    {

        /// <summary>
        /// Разделитель классов в записи классов
        /// </summary>
        internal const string ClassDelimiter = "::";
        
        /// <summary>
        /// Строка названия класса
        /// </summary>
        private String m_ClassPathString;


        /// <summary>
        /// NT-Конструктор
        /// </summary>
        public ClassItem()
        {
            this.m_ClassPathString = String.Empty;
        }

        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="classPathString">Строка названия класса</param>
        public ClassItem(string classPathString)
        {
            this.m_ClassPathString = String.Copy(classPathString);
        }

        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="baseClassPathString">Строка названия базового класса</param>
        /// <param name="classPathString">Строка названия класса</param>
        public ClassItem(string baseClassPathString, string classPathString)
        {
            //throw new System.NotImplementedException();
            this.m_ClassPathString = MergeClassPath(baseClassPathString, classPathString);
        }

        /// <summary>
        /// NT-Конструктор оптимизированный для групповых операций Хранилища
        /// </summary>
        /// <param name="baseClasses">Массив названий классов базового класса Хранилища</param>
        /// <param name="classPathString">Строка названия класса</param>
        public ClassItem(string[] baseClasses, string classPathString)
        {
            this.m_ClassPathString = MergeClassPath(baseClasses, classPathString);
        }



        /// <summary>
        /// Строка полного пути класса
        /// </summary>
        public String ClassPath
        {
            get { return m_ClassPathString; }
        }
        /// <summary>
        /// Строка названия класса
        /// </summary>
        public String Title
        {
            get { return getLastClass(this.m_ClassPathString); }
        }

        /// <summary>
        /// NT-Получить родительский класс как самостоятельный объект
        /// </summary>
        /// <returns>Возвращает объект родительского класса или null, если родительский класс не удалось найти.</returns>
        public ClassItem getParentClass()
        {
            int position = m_ClassPathString.LastIndexOf(ClassDelimiter);
            //если это единственный класс в строке названия, то вернуть null.
            if (position == -1) return null;
            //иначе разделить строку на части по последнему разделителю и вернуть объект, созданный из первой части.
            string baseclass = m_ClassPathString.Remove(position);//todo: check this!!!
            return new ClassItem(baseclass.Trim());
        }
        /// <summary>
        /// NT-Получить укороченную строку пути класса для таблицы БД.
        /// </summary>
        /// <param name="storageBaseClassPath">Строка пути БазовыйКлассХранилища</param>
        /// <returns></returns>
        public string GetShortClassPath(string storageBaseClassPath)
        {
            return SubtractClassPath(this.m_ClassPathString, storageBaseClassPath);
        }

        /// <summary>
        /// get string representation of object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_ClassPathString;
        }

        #region *** Статические функции работ с цепочками классов ***

        /// <summary>
        /// NT-Объединить имена классов в путь классов 
        /// </summary>
        /// <param name="classes">Массив имен классов</param>
        /// <returns>
        /// Возвращает путь классов вида: Предмет::Радиодеталь::Транзистор 
        /// или: Предмет
        /// </returns>
        public static string JoinClasses(string[] classes)
        {
            return String.Join(ClassDelimiter, classes);
        }

        /// <summary>
        /// NT-Получить название последнего класса цепочки классов
        /// </summary>
        /// <param name="classPath"></param>
        /// <returns></returns>
        private static string getLastClass(string classPath)
        {
            //разделить строку по последнему разделителю
            int position = classPath.LastIndexOf(ClassDelimiter);
            //если это единственный класс в строке названия, то вернуть его.
            if (position == -1) return classPath;
            //иначе вернуть вторую часть строки
            string result = classPath.Substring(position + ClassDelimiter.Length);
            return result.Trim();
        }

        /// <summary>
        /// NT-Разделить цепочку классов на отдельные классы
        /// </summary>
        /// <param name="s">Цепочка классов вида Предмет::Радиодеталь::ххх</param>
        /// <returns>Возвращает массив строк имен классов в исходном порядке следования.</returns>
        public static string[] SplitClassPath(string s)
        {
            String[] splitter = new String[] { ClassDelimiter };
            string[] classes =  s.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            //trim class names
            for (int i = 0; i < classes.Length; i++)
                classes[i] = classes[i].Trim();

            return classes;
        }

        /// <summary>
        /// NT-соединить две строки путей классов вместе.
        /// Объединение происходит по правилам для классов Хранилища
        /// </summary>
        /// <param name="baseClassPathString">Строка названия базового класса</param>
        /// <param name="classPathString">Строка названия класса</param>
        /// <returns>Возвращает объединенную строку путей классов</returns>
        public static string MergeClassPath(string baseClassPathString, string classPathString)
        {
            if (String.IsNullOrEmpty(baseClassPathString))
                throw new ArgumentException("Class path cannot be empty!", "baseClassPathString");
            if (String.IsNullOrEmpty(classPathString))
                throw new ArgumentException("Class path cannot be empty!", "classPathString");
            
            String[] baseclasses = ClassItem.SplitClassPath(baseClassPathString);

            //String[] classes = ClassItem.SplitClassPath(classPathString);
            ////ищем первый класс в списке базовых классов, чтобы собрать иерархию правильно
            //String first = classes[0];
            //String result = null;
            //for (int i = 0; i < baseclasses.Length; i++)
            //{
            //    if (String.Equals(first, baseclasses[i], StringComparison.OrdinalIgnoreCase))
            //    {
            //        //нашли, теперь собираем полное имя класса без повтора этого имени.
            //        result = String.Join(ClassDelimiter, baseclasses, 0, i+1);
            //        if(classes.Length > 1)//для случая классов Собака и Собака
            //            result = result + ClassDelimiter + String.Join(ClassDelimiter, classes, 1, classes.Length - 1);
            //        return result;
            //    }
            //}
            ////если ничего не нашли, просто соединяем все вместе
            //result = baseClassPathString + ClassDelimiter + classPathString;

            //перенес код в одну функцию - так проще ее отлаживать и содержать.
            string result = MergeClassPath(baseclasses, classPathString);


            return result;
        }

        /// <summary>
        /// NT-соединить две строки путей классов вместе.
        /// Объединение происходит по правилам для классов Хранилища
        /// Оптимизированная версия для массовых операций Хранилища
        /// </summary>
        /// <param name="baseClasses">Массив названий классов базового класса Хранилища</param>
        /// <param name="classPathString">Строка пути класса</param>
        /// <returns>Возвращает объединенную строку путей классов</returns>
        public static string MergeClassPath(string[] baseClasses, string classPathString)
        {
            if (String.IsNullOrEmpty(classPathString))
                throw new ArgumentException("Class path cannot be empty!", "classPathString");
            if (baseClasses.Length == 0)
                throw new ArgumentException("Class array cannot be empty!", "baseClasses");

            String[] classes = ClassItem.SplitClassPath(classPathString);
            //ищем первый класс из классов сущности в списке базовых классов, чтобы собрать иерархию правильно
            String first = classes[0];
            String result = null;
            for (int i = 0; i < baseClasses.Length; i++)
            {
                if (String.Equals(first, baseClasses[i], StringComparison.OrdinalIgnoreCase))
                {
                    //нашли, теперь собираем полное имя класса без этого члена.
                    result = String.Join(ClassDelimiter, baseClasses, 0, i);
                    //result = result + ClassDelimiter + String.Join(ClassDelimiter, classes, 1, classes.Length - 1); - wrong!
                    if (String.IsNullOrEmpty(result)) //случай Радиодеталь и Радиодеталь
                        return classPathString;
                    else 
                        result = result + ClassDelimiter + classPathString;
                    return result;
                }
            }
            //если ничего не нашли, просто соединяем все вместе
            result = String.Join(ClassDelimiter, baseClasses) + ClassDelimiter + classPathString;
            return result;
        }

        /// <summary>
        /// NT-Удалить БазовыйКлассХранилища из строки пути класса
        /// </summary>
        /// <param name="baseClassPathString">БазовыйКлассХранилища</param>
        /// <param name="classPathString">Полная строка пути класса</param>
        /// <returns>Возвращает разностную строку путей классов</returns>
        public static string SubtractClassPath(string classPathString, string baseClassPathString )
        {
            if (String.IsNullOrEmpty(baseClassPathString))
                throw new ArgumentException("Class path cannot be empty!", "baseClassPathString");
            if (String.IsNullOrEmpty(classPathString))
                throw new ArgumentException("Class path cannot be empty!", "classPathString");
            //если обе строки одинаковы, вернуть последний класс базового класса
            //классы должны быть из одной ветви
            //если классы разные, выбросить исключение о неправильном пути классов.
            //примеры:
            //Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный - Предмет::Радиодеталь = Транзистор::ТранзисторБиполярный
            //Предмет::Радиодеталь - Предмет::Радиодеталь = Радиодеталь //по правилам для Хранилищ
            //Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный - Радиодеталь = Транзистор::ТранзисторБиполярный
            //Предмет::Радиодеталь::Транзистор::ТранзисторБиполярный - Предмет::Собака = исключение.

            //разложить на массивы имен классов, искать совпадение в именах классов от конца к началу. Если совпадения нет, выдать исключение.
            //Если совпадение есть, то все последующие классы собрать в выходную строку.

            String[] baseclasses = ClassItem.SplitClassPath(baseClassPathString);
            String[] classes = ClassItem.SplitClassPath(classPathString);
            //ищем верхний базовый класс в пути классов сущности, чтобы разделить иерархию правильно
            String first = baseclasses[baseclasses.Length - 1];
            String result = null;
            for (int i = 0; i < classes.Length; i++)
            {
                if (String.Equals(first, classes[i], StringComparison.OrdinalIgnoreCase))
                {
                    //нашли класс, после которого идут уже только требуемые классы
                    //нашли, теперь собираем полное имя класса без повтора этого имени.
                    if (classes.Length == i + 1)//если это последний класс в списке классов сущности, то возвращаем этот последний класс
                        result = first;
                    else
                        result = String.Join(ClassDelimiter, classes, i + 1, classes.Length - i -1);
                    return result;
                }
            }
            //если ничего не нашли, выбрасываем исключение
            throw new Exception("Classes not compatible");
        }
        
        //TODO: Надо решить, какая из этих двух функций лучше работает и будет использоваться в коде
        
        /// <summary>
        /// NT-Удалить БазовыйКлассХранилища из строки пути класса
        /// </summary>
        /// <param name="baseClassPathString">БазовыйКлассХранилища</param>
        /// <param name="classPathString">Полная строка пути класса</param>
        /// <returns>Возвращает разностную строку путей классов</returns>
        public static string SubtractClassPath2(string classPathString, string baseClassPathString)
        {
            if (String.IsNullOrEmpty(baseClassPathString))
                throw new ArgumentException("Class path cannot be empty!", "baseClassPathString");
            if (String.IsNullOrEmpty(classPathString))
                throw new ArgumentException("Class path cannot be empty!", "classPathString");

            String result = null;
            int position = classPathString.IndexOf(baseClassPathString, StringComparison.OrdinalIgnoreCase);
            if(position == -1)
                throw new Exception(String.Format("Incompatible entity classes: \"{0}\" and \"{1}\"", classPathString, baseClassPathString ));
            //else
            position = position + baseClassPathString.Length;
            if (position < classPathString.Length)
                result = classPathString.Substring(position).TrimStart(':'); //вернуть остаток
            else
                result = getLastClass(baseClassPathString);//вернуть последний базовый класс по правилам Хранилища

            return result.Trim();
        }



        /// <summary>
        /// NT-Отделить от выражения классов базовый класс хранилища и распарсить его на классы
        /// </summary>
        /// <param name="exp">Выражение классов</param>
        /// <returns>Возвращает массив классов, образующих БазовыйКлассХранилища</returns>
        public static string[] ParseClassPathFromClassExpression(string exp)
        {
            //варианты выражений:
            //[0] Мои места:: Коллекция музыки<Файл::ФайлМузыки>
            //[1] Файловая система ::Папка < Файловая система::Папка,Файл>
            //[2] ФайлМузыки
            //TODO: недоработка: предполагается, что только конечный класс содержит сведения о агрегации (<> и классы)
            //- это не универсальный случай, вообще-то каждый класс может содержать такие сведения о агрегации.
            //Но здесь это будет вызывать ошибку.

            //1 отделим путь класса от записи агрегации по < и >
            String[] sar = exp.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

            //2 обрабатываем разделы
            //[0] суперкласс и класс - Мои места:: Коллекция музыки / Файловая система ::Папка / ФайлМузыки
            //[1] агрегированные подклассы -  Файл::ФайлМузыки / Файловая система::Папка,Файл / нет 
            //2.1 обрабатываем название класса и суперклассов. Элемент 0 всегда должен существовать.
            String[] result = SplitClassPath(sar[0]);

            return result;
        }
        /// <summary>
        /// NT-Отделить от выражения классов базовый класс хранилища, распарсить его на классы и собрать снова.
        /// </summary>
        /// <param name="exp">Выражение классов</param>
        /// <returns>Возвращает БазовыйКлассХранилища</returns>
        public static string GetClassPathFromClassExpression(string exp)
        {
            string[] classes = ParseClassPathFromClassExpression(exp);
            string result = ClassItem.JoinClasses(classes);
            return result;
        }


        #endregion

    }
}
