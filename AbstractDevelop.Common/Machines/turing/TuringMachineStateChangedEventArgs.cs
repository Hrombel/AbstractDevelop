using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет аргументы события, возникающего после смены внутреннего состояния машины Тьюринга.
    /// </summary>
    public class TuringMachineStateChangedEventArgs : EventArgs
    {
        TuringPenDir[] _penDirs;

        /// <summary>
        /// Инициализирует аргументы события указанными направлениями смещения головок соответствующих лент МТ.
        /// </summary>
        /// <param name="dirs">Массив направлений смещения читающих/пишущих головок МТ.</param>
        public TuringMachineStateChangedEventArgs(TuringPenDir[] dirs)
        {
            _penDirs = dirs.Clone() as TuringPenDir[];
        }

        /// <summary>
        /// Получает массив направлений смещения головок МТ.
        /// </summary>
        public TuringPenDir[] Directions
        {
            get { return _penDirs.Clone() as TuringPenDir[]; }
        }
    }
}
