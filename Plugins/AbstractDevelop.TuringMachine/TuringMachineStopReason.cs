using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Кодирует причину останова работы машины Тьюринга.
    /// </summary>
    public enum TuringMachineStopReason : byte
    {
        /// <summary>
        /// Вмешательство пользователя.
        /// </summary>
        UserInterrupt,
        /// <summary>
        /// Произведен переход в несуществующее состояние.
        /// </summary>
        UndefinedState,
        /// <summary>
        /// Машина перешла в известное состояния, но не нашла переход к новому состоянию при текущей конфигурации.
        /// </summary>
        UndefinedConversion
    }
}
