using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.errors.dev
{
    [Serializable()]
    /// <summary>
    /// Представляет исключение, генерируемое при ошибке создания операции из текста.
    /// </summary>
    public class InvalidOperationTextException : AbstractException
    {
        public InvalidOperationTextException() : base() { }
        public InvalidOperationTextException(string message) : base(message) { }
        public InvalidOperationTextException(string message, Exception inner) : base(message, inner) { }

        protected InvalidOperationTextException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
