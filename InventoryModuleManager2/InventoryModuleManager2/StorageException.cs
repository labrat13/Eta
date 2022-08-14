using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace InventoryModuleManager2
{
    [Serializable]
    public class StorageException : ApplicationException
    {
        /// <summary>
		/// Deserialization constructor 
		/// </summary>
		/// <param name="info"><see cref="System.Runtime.Serialization.SerializationInfo"/> for this constructor</param>
		/// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
		protected StorageException(SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

        /// <summary>
		/// Initializes a new instance of the StorageException class.
		/// </summary>
		public StorageException()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the StorageException class with a specified error message.
		/// </summary>
		/// <param name="message">A message describing the exception.</param>
		public StorageException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the StorageException class with a specified
		/// error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">A message describing the exception.</param>
		/// <param name="innerException">The inner exception</param>
		public StorageException(string message, Exception innerException)
			: base(message, innerException)
		{
		}



    }
}
