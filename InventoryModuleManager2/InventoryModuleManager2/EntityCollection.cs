using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryModuleManager2
{
    public class EntityCollection: System.Collections.IEnumerable
    {
        //TODO: скопировать с DocumentCollection, когда он будет проверен и отлажен.
        private DbAdapter m_adapter;

        public EntityCollection( DbAdapter adapter)
        {
            m_adapter = adapter;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new EntityEnumerator( m_adapter);
        }

        #endregion
    }
}
