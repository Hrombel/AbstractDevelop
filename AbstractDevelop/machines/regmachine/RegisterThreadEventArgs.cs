using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет аргументы события, генерируемого потоком параллельной машины с бесконечными регистрами.
    /// </summary>
    public class RegisterThreadEventArgs : EventArgs
    {

        /// <summary>
        /// Инициализирует новые аргументы события.
        /// </summary>
        public RegisterThreadEventArgs()
        {

        }
    }
}
