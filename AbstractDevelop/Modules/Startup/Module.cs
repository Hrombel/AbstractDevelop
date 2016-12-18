using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Framework;

using Gemini.Modules.Output;
using Gemini.Modules.ErrorList;

namespace AbstractDevelop.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		private readonly IOutput _output;


        [ImportingConstructor]
	    public Module(IOutput output, IErrorList errorList)
        {
            _output = output;
        }

	    public override void Initialize()
		{
		    Shell.ShowFloatingWindowsInTaskbar = true;
            Shell.ToolBars.Visible = true;

            //MainWindow.WindowState = WindowState.Maximized;
            MainWindow.Title = "AbstractDevelop RISC Edition";

            Shell.StatusBar.AddItem("Hello world!", new GridLength(1, GridUnitType.Star));
            Shell.StatusBar.AddItem("Ln 44", new GridLength(100));
            Shell.StatusBar.AddItem("Col 79", new GridLength(100));

            Shell.ShowTool(_output);

			_output.AppendLine("Started up");
		}
	}
}