using System;
using System.Runtime.Serialization;

namespace AbstractDevelop.Machines
{
    [Serializable]
    internal class AbstractMachineExcepton : Exception
    {
        public AbstractMachineExcepton()
        {
        }

        public AbstractMachineExcepton(string message) : base(message)
        {
        }

        public AbstractMachineExcepton(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AbstractMachineExcepton(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}