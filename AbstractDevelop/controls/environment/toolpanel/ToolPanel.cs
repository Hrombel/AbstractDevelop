using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractDevelop.controls.environment.toolpanel
{
    /// <summary>
    /// Представляет панель управления работой абстрактного вычислителя.
    /// </summary>
    public partial class ToolPanel : UserControl
    {
        /// <summary>
        /// Возникает после нажатия пользователем на одну из кнопок панели.
        /// </summary>
        public event EventHandler<ToolPanelEventArgs> OnButtonPressed;


        public ToolPanel()
        {
            InitializeComponent();
            runBtn.Enabled = true;
            stopBtn.Enabled = false;
            pauseBtn.Enabled = false;
            stepBtn.Enabled = false;
        }

        /// <summary>
        /// Выполняет операцию нажатия на указанную кнопку без диспетчеризации события.
        /// </summary>
        /// <param name="btn">Код нажимаемой кнопки.</param>
        public void PerformClick(ToolPanelButton btn)
        {
            if(!Enum.IsDefined(typeof(ToolPanelButton), btn))
                throw new ArgumentException("Указанной кнопки нет в перечислении");

            switch (btn)
            {
                case ToolPanelButton.Play:
                    {
                        stopBtn.Enabled = true;
                        pauseBtn.Enabled = true;
                        stepBtn.Enabled = false;
                        runBtn.Enabled = false;
                        break;
                    }
                case ToolPanelButton.Stop:
                    {
                        runBtn.Enabled = true;
                        stepBtn.Enabled = false;
                        stopBtn.Enabled = false;
                        pauseBtn.Enabled = false;
                        break;
                    }
                case ToolPanelButton.Pause:
                    {
                        runBtn.Enabled = true;
                        stepBtn.Enabled = true;
                        stopBtn.Enabled = true;
                        pauseBtn.Enabled = false;
                        break;
                    }
                case ToolPanelButton.Step:
                    {

                        break;
                    }
                default:
                    throw new Exception("Неизвестная кнопка");
            }

            startItem.Enabled = runBtn.Enabled;
            stopItem.Enabled = stopBtn.Enabled;
            pauseItem.Enabled = pauseBtn.Enabled;
            stepItem.Enabled = stepBtn.Enabled;
        }

        /// <summary>
        /// Генерирует соытие нажатой кнопки.
        /// </summary>
        /// <param name="btn">Кнопка.</param>
        private void DispatchClick(ToolPanelButton btn)
        {
            if (OnButtonPressed != null)
            {
                ToolPanelEventArgs args = new ToolPanelEventArgs(btn);
                OnButtonPressed(this, args);
            }
        }

        private void ShortPressed(object sender, EventArgs e)
        {
            ToolPanelButton btn;

            if (sender == startItem)
                btn = ToolPanelButton.Play;
            else if (sender == stopItem)
                btn = ToolPanelButton.Stop;
            else if (sender == pauseItem)
                btn = ToolPanelButton.Pause;
            else if (sender == stepItem)
                btn = ToolPanelButton.Step;
            else
                throw new Exception("Неизвестная кнопка");

            DispatchClick(btn);
        }

        private void BtnClick(object sender, EventArgs e)
        {
            ToolPanelButton btn;

            if (sender == runBtn)
                btn = ToolPanelButton.Play;
            else if (sender == stopBtn)
                btn = ToolPanelButton.Stop;
            else if (sender == pauseBtn)
                btn = ToolPanelButton.Pause;
            else if (sender == stepBtn)
                btn = ToolPanelButton.Step;
            else
                throw new Exception("Неизвестная кнопка");

            DispatchClick(btn);
        }
    }
}
