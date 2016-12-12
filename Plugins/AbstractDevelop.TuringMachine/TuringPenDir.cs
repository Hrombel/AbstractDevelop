using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Кодирует направления движения читающей/пишущей головки мишины Тьюринга.
    /// </summary>
    public enum TuringPenDir : byte
    {
        /// <summary>
        /// Остаться на месте.
        /// </summary>
        Stay,
        /// <summary>
        /// Сдвиг вправо.
        /// </summary>
        Right,
        /// <summary>
        /// Сдвиг влево.
        /// </summary>
        Left
    }
}
