using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.controls.environment.toolpanel
{
    /// <summary>
    /// Кодирует кнопку панели инструментов.
    /// </summary>
    public enum ToolPanelButton : byte
    {
        /// <summary>
        /// Кнопка начала работы абстрактной машины.
        /// </summary>
        Play,
        /// <summary>
        /// Кнопка полной остановки абстрактной машины.
        /// </summary>
        Stop,
        /// <summary>
        /// Кнопка приостановки работы абстрактной машины.
        /// </summary>
        Pause,
        /// <summary>
        /// Кнопка следующего шага выполнения алгоритма.
        /// </summary>
        Step
    }
}
