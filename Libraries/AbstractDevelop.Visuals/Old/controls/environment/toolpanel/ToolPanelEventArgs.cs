using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.controls.environment.toolpanel
{
    /// <summary>
    /// Представляет аргументы события, возникающего в результате взаимодействия пользователя с панелью инструментов.
    /// </summary>
    public class ToolPanelEventArgs : EventArgs
    {
        private ToolPanelButton _btn;

        public ToolPanelEventArgs(ToolPanelButton button)
        {
            _btn = button;
            Success = true;
        }

        /// <summary>
        /// Получает или задает успешность выполнения операции, полседовавшей после нажатия кнопки.
        /// Истина, если операция выполнена успешно, иначе - ложь. Значение по умолчанию - истина.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Получает код нажатой клавиши панели инструментов.
        /// </summary>
        public ToolPanelButton Button { get { return _btn; } }
    }
}
