using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет аргументы события, возникающего после записи значения на устройство вывода.
    /// </summary>
    public class RegisterMachineValueOutEventArgs
    {
        private BigInteger[] _buffer;

        /// <summary>
        /// Инициализирует экземпляр указанными параметрами.
        /// </summary>
        /// <param name="buffer">Буфер последних выведенных значений.</param>
        public RegisterMachineValueOutEventArgs(BigInteger[] buffer)
        {
            _buffer = buffer;
        }

        /// <summary>
        /// Получает буфер последних выведенных машиной значений.
        /// </summary>
        public BigInteger[] Buffer { get { return _buffer; } }
    }
}
