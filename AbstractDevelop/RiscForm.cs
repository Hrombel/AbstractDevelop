using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractDevelop
{
    public partial class RiscForm : Form
    {
        AbstractProject currentProject = new AbstractProject("untitled", MachineId.Risc, "default");

        public RiscForm()
        {
            InitializeComponent();
        }

        private void RiscForm_Load(object sender, EventArgs e)
        {
            workEnvironment.OpenProject(currentProject);
        }

        private void saveMenuButton_Click(object sender, EventArgs e)
        {
            workEnvironment.Save();
        }

        private void openMenuButton_Click(object sender, EventArgs e)
        {
            workEnvironment.Load();
            
            //workEnvironment.OpenProject()
        }
    }
}
