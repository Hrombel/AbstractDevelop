using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.controls.visuals
{
    /// <summary>
    /// Представляет аргументы события, происходящего после смены состояния визуализатора абстрактного вычислителя.
    /// </summary>
    public class MachineVisualizerStateChangedEventArgs : EventArgs
    {
        VisualizerState _prev;

        /// <summary>
        /// Инициализирует аргументы предыдущим состоянием визуализатора.
        /// </summary>
        /// <param name="prev">Состояние визуализатора до смены состояния.</param>
        public MachineVisualizerStateChangedEventArgs(VisualizerState prev)
        {
            _prev = prev;
        }

        /// <summary>
        /// Предыдущее состояние визуализатора.
        /// </summary>
        public VisualizerState PrevState
        {
            get
            {
                return _prev;
            }
        }
    }
}
