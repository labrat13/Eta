using System;
using System.Collections.Generic;
using System.Text;

namespace StorageSearch
{
    /// <summary>
    /// Флаги состояния чекбоксов поиска
    /// </summary>
    [FlagsAttribute]
    internal enum SearchCheckboxStates
    {
        /// <summary>
        /// All checkboxes are cleared
        /// </summary>
        None = 0,
        /// <summary>
        /// Use title field
        /// </summary>
        Title = 1,
        /// <summary>
        /// Use description field
        /// </summary>
        Descr = 2,
        /// <summary>
        /// Use docfile description field
        /// </summary>
        DocDescr = 4,
        /// <summary>
        /// Use picfile description field
        /// </summary>
        PicDescr = 8,
        /// <summary>
        /// Search in previous results
        /// </summary>
        Previous = 16,
    }
}
