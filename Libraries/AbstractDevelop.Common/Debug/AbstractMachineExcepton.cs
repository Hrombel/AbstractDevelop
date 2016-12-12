using System;
using System.Runtime.Serialization;

namespace AbstractDevelop.Machines
{
    [Serializable]
    internal class AbstractMachineException : Exception
    {
        public AbstractMachineException()
        {
        }

        public AbstractMachineException(string message) : base(message)
        {
        }

        public AbstractMachineException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AbstractMachineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}