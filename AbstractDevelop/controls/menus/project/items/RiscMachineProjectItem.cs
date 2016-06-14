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

namespace AbstractDevelop.controls.menus.project.items
{
    public partial class RiscMachineProjectItem : UserControl, IProjectMenuItem
    {
        public RiscMachineProjectItem() { InitializeComponent(); }
        

        public MachineId Machine => MachineId.Risc;


        public string ProjectName => "Машина RISC";


        public Dictionary<string, object> Settings => null;
     
    }
}
