using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractDevelop.controls.ui.statecontrol
{
    /// <summary>
    /// Представляет компонент для отображения элементов управления с возможностью их быстрой смены.
    /// </summary>
    public partial class StateControl : UserControl
    {
        private UserControl _currentControl;

        public StateControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Получает или задает текущий отображаемый элемент управления.
        /// </summary>
        public UserControl CurrentControl
        {
            get { return _currentControl; }
            set
            {
                if (value == _currentControl) return;

                if(_currentControl != null)
                {
                    Controls.RemoveAt(0);
                }
                _currentControl = value;
                if(_currentControl != null)
                {
                    _currentControl.Dock = DockStyle.Fill;
                    _currentControl.Margin = Padding.Empty;
                    Controls.Add(value);

                }
            }
        }


    }
}
