using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет аргументы события, генерируемого менеджером потоков ПМБР.
    /// </summary>
    public class RegisterThreadManagerEventArgs
    {
        private bool _registersChanged;

        /// <summary>
        /// Инициализирует экземпляр аргументов события менеджера потоков ПМБР.
        /// </summary>
        /// <param name="registerChanged">Определяет, было ли изменено состояние хотя бы одного регистра в ходе работы потоков.</param>
        public RegisterThreadManagerEventArgs(bool registerChanged)
        {
            _registersChanged = registerChanged;
        }

        /// <summary>
        /// Получает флаг, определяющий, было ли изменено состояние хотя бы одного регистра в ходе работы потоков.
        /// </summary>
        public bool RegisterChanged { get { return _registersChanged; } }
    }
}
