using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop
{
    /// <summary>
    /// Перечисление форматов записи цифр
    /// </summary>
    public enum NumberFormat : int
    {
        /// <summary>
        /// Двоичный формат
        /// </summary>
        Binary = 2,

        /// <summary>
        /// Восьмеричный формат
        /// </summary>
        Octal = 8,

        /// <summary>
        /// Шестнадцатеричный формат
        /// </summary>
        Hex = 16,
    
    }
}
