using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.errors.runtime
{
    /// <summary>
    /// Представляет исключение, генерируемое во время выполнения операции абстрактным вычислителем.
    /// </summary>
    public class ExecutionAbstractException : AbstractException
    {
        public ExecutionAbstractException() : base() { }
        public ExecutionAbstractException(string message) : base(message) { }
        public ExecutionAbstractException(string message, Exception inner) : base(message, inner) { }

        protected ExecutionAbstractException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
