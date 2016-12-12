using AbstractDevelop.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AbstractDevelop.controls.menus.project.items
{
    /// <summary>
    /// Представляет инструмент краткой справки и настройки проекта для МБР.
    /// </summary>
    public partial class RegisterMachineProjectItem : UserControl, IProjectMenuItem
    {
        private Dictionary<string, object> _settings;

        public RegisterMachineProjectItem()
        {
            _settings = new Dictionary<string, object>();
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

        public Dictionary<string, object> Settings
        {
            get { return _settings; }
        }
    }
}