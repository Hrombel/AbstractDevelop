using AbstractDevelop.Properties;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AbstractDevelop.controls.menus.project.items
{
    /// <summary>
    /// Представляет меню создания проекта для машины Тьюринга.
    /// </summary>
    public partial class TuringMachineProjectItem : UserControl, IProjectMenuItem
    {
        public TuringMachineProjectItem()
        {
            InitializeComponent();
            browser.DocumentText = Resources.TuringInfo;
        }

        public string ProjectName
        {
            get { return "Машина Тьюринга"; }
        }

        public MachineId Machine
        {
            get { return MachineId.Turing; }
        }

        public Dictionary<string, object> Settings
        {
            get { return null; }
        }
    }
}