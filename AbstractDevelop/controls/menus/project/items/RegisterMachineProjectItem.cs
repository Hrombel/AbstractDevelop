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
    /// Представляет инструмент краткой справки и настройки проекта для МБР.
    /// </summary>
    public partial class RegisterMachineProjectItem : UserControl, IProjectMenuItem
    {
        private Dictionary<string, bool> _settings;

        public RegisterMachineProjectItem()
        {
            _settings = new Dictionary<string, bool>();
            InitializeComponent();
            browser.DocumentText = Resources.RegisterInfo;

            _settings["parallel"] = parallelBtn.Checked;
            classicBtn.CheckedChanged += CheckedHandler;
            parallelBtn.CheckedChanged += CheckedHandler;
        }
        ~RegisterMachineProjectItem()
        {
            classicBtn.CheckedChanged -= CheckedHandler;
            parallelBtn.CheckedChanged -= CheckedHandler;
        }

        private void CheckedHandler(object sender, EventArgs e)
        {
            _settings["parallel"] = parallelBtn.Checked;
        }

        public string ProjectName
        {
            get { return "Машина с бесконечными регистрами"; }
        }

        public MachineId Machine
        {
            get { return MachineId.Register; }
        }

        public Dictionary<string, bool> Settings
        {
            get { return _settings; }
        }
    }
}
