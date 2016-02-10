using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.machines;
using AbstractDevelop.Properties;

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

        public Dictionary<string, bool> Settings
        {
            get { return null; }
        }
    }
}
