using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет аргументы события, возникающего перед чтением очередного значения с устройства ввода.
    /// </summary>
    public class RegisterMachineValueInEventArgs
    {
        private BigInteger _value;

        /// <summary>
        /// Создает новый экземпляр класса параметров.
        /// </summary>
        public RegisterMachineValueInEventArgs()
        {
            _value = 0;
        }

        /// <summary>
        /// Получает или задает значение, которое должно быть записано на устройство ввода.
        /// </summary>
        public BigInteger Value
        {
            get { return _value; }
            set
            {
                if (value < 0)
                    throw new Exception("Записываемое значение не может быть отрицательным");
                _value = value;
            }
        }
    }
}
