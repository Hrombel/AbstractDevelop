using AbstractDevelop.controls.visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.controls.environment
{
    /// <summary>
    /// Представляет аргументы события смены текущего отображаемого визуализатора.
    /// </summary>
    public class CurrentVisualizerChangeEventArgs
    {
        private IMachineVisualizer _previous;

        /// <summary>
        /// Инициализирует экземпляр аргументов события указанными параметрами.
        /// </summary>
        /// <param name="previous">Предыдущий оторажаемый визуализатор.</param>
        public CurrentVisualizerChangeEventArgs(IMachineVisualizer previous)
        {
            _previous = previous;
        }

        /// <summary>
        /// Получает предыдущий оторажаемый визуализатор.
        /// </summary>
        public IMachineVisualizer Previous { get { return _previous; } }
    }
}
