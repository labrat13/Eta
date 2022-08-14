using System;

namespace InventoryModuleManager2
{
    public class DocumentCollection: System.Collections.IEnumerable
    {
        private DbAdapter m_adapter;
        private String m_tablename;
        public DocumentCollection(String tablename, DbAdapter adapter)
        {
            m_tablename = tablename;
            m_adapter = adapter;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new DocumentEnumerator(m_tablename, m_adapter);
        }

        #endregion
    }
}
