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

            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !ExitingProcedure();
        }

        private void ui_ExitRequest(object sender, EventArgs e)
        {
            if (ExitingProcedure())
                Close();
        }

        /// <summary>
        /// Вызывается, при подаче сигнала о завершении программы.
        /// </summary>
        /// <returns>Истина, если после процедуры следует закрыть программу, иначе - ложь.</returns>
        private bool ExitingProcedure()
        {
            bool res = false;

            if (ui.HasUnsavedData)
            {
                DialogResult result = MessageBox.Show("Не все данные сохранены. Вы уверены, что хотите закрыть программу?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                    res = true;
            }
            else
                res = true;

            return res;
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
