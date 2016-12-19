using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Framework;

namespace AbstractDevelop.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
	    public override void Initialize()
		{
		    Shell.ShowFloatingWindowsInTaskbar = true;
            Shell.ToolBars.Visible = true;

            MainWindow.WindowState = WindowState.Maximized;
            MainWindow.Title = "AbstractDevelop RISC R8 Edition";

            Shell.StatusBar.AddItem("Hello world!", new GridLength(1, GridUnitType.Star));
            Shell.StatusBar.AddItem("Ln 44", new GridLength(100));
            Shell.StatusBar.AddItem("Col 79", new GridLength(100));
		}
	}
}