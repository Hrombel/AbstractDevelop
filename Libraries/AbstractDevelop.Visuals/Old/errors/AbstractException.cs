using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.errors
{
    [Serializable()]
    /// <summary>
    /// Представляет ошибки, генерируемые средой AbstractDevelop.
    /// </summary>
    public class AbstractException : Exception
    {
        public AbstractException() : base() { }
        public AbstractException(string message) : base(message) { }
        public AbstractException(string message, Exception inner) : base(message, inner) { }

        protected AbstractException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
