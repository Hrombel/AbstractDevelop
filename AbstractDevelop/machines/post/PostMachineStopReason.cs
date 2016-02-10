using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.post
{
    /// <summary>
    /// Кодирует причину останова работы машины Поста.
    /// </summary>
    public enum PostMachineStopReason : byte
    {
        /// <summary>
        /// Остановка инициирована пользователем.
        /// </summary>
        USER_INTERRUPT,
        /// <summary>
        /// Выполнена операция останова машины Поста.
        /// </summary>
        STOP_OPERATION,
        /// <summary>
        /// Переход к несуществующей операции.
        /// </summary>
        OUT_OF_OPERATION_NUMBER,
        /// <summary>
        /// Попытка установки метки в ячейку, которая уже хранит метку.
        /// </summary>
        SET_EXISTING_LABEL,
        /// <summary>
        /// Попытка удаления метки из ячейки, которая не хранит метку.
        /// </summary>
        REMOVE_NULL_LABEL
    }
}
