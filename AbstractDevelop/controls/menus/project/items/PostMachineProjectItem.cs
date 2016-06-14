using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.controls.visuals;
using AbstractDevelop.machines;
using AbstractDevelop.machines.post;
using AbstractDevelop.Properties;

namespace AbstractDevelop.controls.menus.project.items
{
    /// <summary>
    /// Представляет меню создания проекта для машины Поста.
    /// </summary>
    public partial class PostMachineProjectItem : UserControl, IProjectMenuItem
    {

        public PostMachineProjectItem()
        {
            InitializeComponent();
            browser.DocumentText = Resources.PostInfo;
        }

        public MachineId Machine
        {
            get { return MachineId.Post; }
        }

        public string ProjectName
        {
            get { return "Машина Поста"; }
        }

        public Dictionary<string, object> Settings
        {
            get { return null; }
        }
    }
}
