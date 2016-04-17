using AbstractDevelop.Properties;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace AbstractDevelop
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("ScintillaNET"))
                return Assembly.Load(Resources.ScintillaNET);
            else
                return null;
        }
    }
}
