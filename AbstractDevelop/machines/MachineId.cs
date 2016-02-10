using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines
{
    /// <summary>
    /// Кодирует уникальные идентификаторы абстрактных вычислителей.
    /// </summary>
    public enum MachineId : byte
    {
        /// <summary>
        /// Машина Поста.
        /// </summary>
        Post,
        /// <summary>
        /// Машина Тьюринга(включая многоленточную).
        /// </summary>
        Turing,
        /// <summary>
        /// Машина с бесконечными регистрами(включая параллельную).
        /// </summary>
        Register
    }
}
