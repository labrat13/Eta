using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryModuleManager2
{
    internal class ArchiveTransaction: Object
    {
        /// <summary>
        /// Контроллер архива, для которого выполняются транзакции
        /// </summary>
        internal ArchiveController m_controller;

        /// <summary>
        /// Конструктор
        /// </summary>
        internal ArchiveTransaction()
        {


        }
        
        ///// <summary>
        ///// Initiates a nested database transaction.
        ///// </summary>
        ///// <returns>A nested database transaction.</returns>
        ///// <exception cref="System.InvalidOperationException">Nested transactions are not supported.</exception>
        //internal ArchiveTransaction Begin()
        //{
        //}

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        /// <exception cref="System.Exception">An error occurred while trying to commit the transaction.</exception>
        /// <exception cref="System.InvalidOperationException">The transaction has already been committed or rolled back.</exception>
        internal void Commit()
        {

        }

 
        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        /// <exception cref="System.Exception">An error occurred while trying to commit the transaction.</exception>
        /// <exception cref="System.InvalidOperationException">The transaction has already been committed or rolled back.</exception>
        internal void Rollback()
        {


        }


    }
}
