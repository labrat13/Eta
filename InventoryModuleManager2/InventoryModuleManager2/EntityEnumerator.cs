using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace InventoryModuleManager2
{
    public class EntityEnumerator : IEnumerator
    {
        //TODO: енумератор замудренный, но сейчас пока работает.
        //надо проверять на больших объемах данных
        // и с разным количеством записей от 0 до 16 - специально тест надо проводить такой.

        //TODO: скопировать с DocumentEnumerator, когда он будет проверен и отлажен.
        //Потребуется небольшая переделка, возможно. 
        //Так как тут должны быть уже связанные комплекты для Сущности и файлов, а не просто записи из БД.
        /* Тут используется кэш-список записей размером до 16 элементов. 
 * Это позволяет уменьшить число запросов к БД и функций адаптера БД.
 * И ускоряет работу.
 * Список проверяется и пополняется в MoveNext(). Там вся основная работа.
 * Инициализируется в Reset(), поскольку БД может измениться во время чтения.
 * Вызывающему коду всегда возвращается первый элемент кэш-списка.
 * Если список пустой, он пополняется мелкими шажками по 16 записей.
 * Записи должны быть небольшими по размеру, поэтому такая схема вполне оправдана здесь.
 * */
        private DbAdapter m_adapter;
        private int m_currentId;
        private int m_startId;
        private int m_endId;

        /// <summary>
        /// Список идентификаторов записей для MoveNext()
        /// </summary>
        private List<EntityRecord> m_recordList;



        public EntityEnumerator(DbAdapter adapter)
        {
            m_recordList = new List<EntityRecord>();
            m_adapter = adapter;
            this.Reset();
        }


        #region IEnumerator Members

        public void Reset()
        {
            m_endId = m_adapter.getTableMaxId(DbAdapter.ContentTableName);
            //if (m_endId == -1) - TODO: no records in table
            m_startId = m_adapter.getTableMinId(DbAdapter.ContentTableName);
            //if(m_startId < 0) - no records in table
            m_currentId = 0;
            m_recordList.Clear();
            return;
        }

        public object Current
        {
            get { return m_recordList[0]; }
        }

        public bool MoveNext()
        {
            //TODO: Надо проверить работу для 0 и 1 и 2 записей в таблице, и чтобы не пропускала записи, особенно последнюю.
            //return false if table is empty
            if ((m_endId == -1) || (m_startId == -1)) return false;
            //for first time only
            if (m_currentId == 0)
            {
                m_currentId = m_startId;
                //fill list here for first time
            }
            else
            {
                //not first time
                //delete top item from list
                m_recordList.RemoveAt(0);
                m_currentId++;//next row id?
            }
            //if list is empty, fill list with new items
            if (m_recordList.Count == 0)
            {
                fillListWith16items();
            }

            //if list is empty then no next element. return false.
            if (m_recordList.Count == 0) return false;

            //get id from new top record
            m_currentId = m_recordList[0].TableId;
            return (bool)(m_currentId <= m_endId);//return true if iterator moved to new element

        }



        #endregion

        /// <summary>
        /// NT-
        /// </summary>
        private void fillListWith16items()
        {
            //cur = 1; end = 3;
            for (int i = m_currentId; i <= m_endId; i += 16)
            {
                m_recordList.AddRange(m_adapter.getEntityRecordsBetween( m_currentId, i+16));
                if (m_recordList.Count > 0) break;
            }
            return;
        }
    }
}
