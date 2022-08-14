using System;
using System.Collections.Generic;
using System.Text;

namespace ClassificationEntity.oldversion
{
    /// <summary>
    /// Представляет несколько классов сущностей.
    /// Представляет несколько классов разных классификаций для одной сущности.
    /// </summary>
    public class EntityTypesCollection
    {
        #region *** Fields ***
        /// <summary>
        /// Словарь типов сущностей первого уровня
        /// </summary>
        private Dictionary<String, EntityType> m_EntityTypes;
        #endregion


        /// <summary>
        /// Default constructor
        /// </summary>
        public EntityTypesCollection()
        {
            this.m_EntityTypes = new Dictionary<string, EntityType>();
        }
        #region *** Properties ***
        /// <summary>
        /// Словарь типов сущностей первого уровня
        /// </summary>
        public Dictionary<String, EntityType> EntityTypes
        {
            get { return m_EntityTypes; }
            set { m_EntityTypes = value; }
        }
        #endregion

        /// <summary>
        /// NT-Ищет запись типа по его названию. Если тип не упомянут, возвращается null
        /// </summary>
        /// <param name="nameOfType">Название типа</param>
        /// <returns>Возвращает первый найденный объект записи типа или null если объект не найден</returns>
        public EntityType ContainsType(String nameOfType)
        {
            //ищем в словаре объект по названию класса. 
            //Предполагается, что название класса уникальное?
            if (m_EntityTypes.ContainsKey(nameOfType))
                return m_EntityTypes[nameOfType];
            else
            {
                //а если не нашли, то перебором всех элементов словаря ищем в них класс по имени класса. 
                foreach (KeyValuePair<String, EntityType> kvp in m_EntityTypes)
                {
                    EntityType res = kvp.Value.ContainsType(nameOfType);
                    if (res != null) return res;
                }
                return null;
            }
        }

        /// <summary>
        /// NT-Распарсить строку описания классов сущности в дерево классов
        /// </summary>
        /// <param name="expression">Выражение описания классов сущности </param>
        public void ParseExpression(String expression)
        {
            //Это образец формата для разработки парсинга.
            //выражение представляет собой список выражений типов через ;
            //Мои места:: Коллекция музыки<Файл::ФайлМузыки>;Файловая система ::Папка < Файловая система::Папка,Файл>; 

            //тут надо бы исключение перехватывать и выводить boolean результат парсинга.

            //парсим
            //1) удаляем пробелы с начала и конца выражения
            String exp = expression.Trim();
            //2) разделяем на элементы по ;
            String[] sar = exp.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //тут мы получим несколько элементов:
            //[0] Мои места:: Коллекция музыки<Файл::ФайлМузыки>
            //[1] Файловая система ::Папка < Файловая система::Папка,Файл>
            //[2] элемент после последней ; должен уже быть удален как пустой элемент
            //3) передаем каждое выражение для парсинга в объект типа 
            foreach (String s in sar)
            {
                EntityType t = new EntityType(); //создаем новый объект
                t.ParseExpression(s); //отправляем строку на парсинг
                this.m_EntityTypes.Add(t.Title, t); //добавляем объект в коллекцию.
            }

            return;
        }


    }//end class
}
