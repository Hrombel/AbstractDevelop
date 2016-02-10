using AbstractDevelop.controls.ui.ceditor;
using AbstractDevelop.controls.visuals;
using AbstractDevelop.machines;
using AbstractDevelop.machines.post;
using AbstractDevelop.machines.registers;
using AbstractDevelop.machines.regmachine;
using AbstractDevelop.machines.tape;
using AbstractDevelop.machines.turing;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractDevelop
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;

            Text = string.Format("{0} v{1}.{2}", AssemblyProduct, ver.Major, ver.Minor);
        }

        private void ui_ExitRequest(object sender, EventArgs e)
        {
            Close();
        }

        private string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
    }
}
