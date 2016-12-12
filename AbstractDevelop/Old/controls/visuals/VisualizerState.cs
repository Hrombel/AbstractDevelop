using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.controls.visuals
{
    /// <summary>
    /// Кодирует состояние визуализитора абстрактного вычислителя.
    /// </summary>
    public enum VisualizerState : byte
    {
        /// <summary>
        /// Вычислитель не выполняет никаких операций.
        /// </summary>
        Stopped,
        /// <summary>
        /// Вычислитель выполняет программу.
        /// </summary>
        Executing,
        /// <summary>
        /// Вычислитель находится в состоянии паузы.
        /// </summary>
        Paused
    }
}
