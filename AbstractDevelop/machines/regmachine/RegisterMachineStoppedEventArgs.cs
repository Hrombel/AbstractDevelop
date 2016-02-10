using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет аргументы события остановки МБР.
    /// </summary>
    public class RegisterMachineStoppedEventArgs
    {
        private bool _registersChanged;

        /// <summary>
        /// Инициализиурет аргументы события.
        /// </summary>
        /// <param name="registersChanged">Определяет, изменилось ли состояние регистров во время работы МБР.</param>
        public RegisterMachineStoppedEventArgs(bool registersChanged)
        {
            _registersChanged = registersChanged;
        }

        /// <summary>
        /// Определяет, изменилось ли состояние регистров во время работы МБР.
        /// </summary>
        public bool RegistersChanged { get { return _registersChanged; } }
    }
}
