using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.post
{
    /// <summary>
    /// Представляет аргументы события, возникающего при всякой остановке машины Поста.
    /// </summary>
    public class PostMachineStopEventArgs : EventArgs
    {
        private PostMachineStopReason _reason;

        /// <summary>
        /// Инициализирует аргументы события останова причиной останова машины.
        /// </summary>
        /// <param name="reason">Причина останова машины Поста.</param>
        public PostMachineStopEventArgs(PostMachineStopReason reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// Получает причину останова машины Поста.
        /// </summary>
        public PostMachineStopReason Reason
        {
            get
            {
                return _reason;
            }
        }
    }
}
