using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryModuleManager2
{
    public enum SessionState
    {
        /// <summary>
        /// Неизвестное (0)
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Сеанс начат (1)
        /// </summary>
        Opened = 1,
        /// <summary>
        /// Сеанс завершен (2)
        /// </summary>
        Closed = 2,
    }
}
