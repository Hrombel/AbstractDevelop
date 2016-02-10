using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет аргументы события, возникающего при всякой остановке машины Тьюринга.
    /// </summary>
    public class TuringMachineStopEventArgs : EventArgs
    {
        TuringMachineStopReason _reason;

        /// <summary>
        /// Инициализирует аргументы причиной останова машины Тьюринга.
        /// </summary>
        /// <param name="reason">Причина останова машины Тьюринга.</param>
        public TuringMachineStopEventArgs(TuringMachineStopReason reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// Получает причину останова машины Тьюринга.
        /// </summary>
        public TuringMachineStopReason Reason
        {
            get { return _reason; }
        }
    }
}
