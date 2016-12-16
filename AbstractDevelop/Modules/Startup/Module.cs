using AbstractDevelop.Modules.Registers.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.CodeEditor.ViewModels;
using Gemini.Modules.Output;
using Gemini.Modules.StatusBar;
using Gemini.Modules.Toolbox;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AbstractDevelop.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        IStatusBar StatusBar;

        IResourceManager ResourceManager;

        //IToolbox RegisterView;

        [ImportingConstructor]
        public Module(IStatusBar statusBar, IResourceManager manager/*, [Import("RegisterView", typeof(IToolbox))] IToolbox regiseterView*/)
        {
            StatusBar = statusBar;
            ResourceManager = manager;

            //RegisterView = regiseterView;
        }

        public override void Initialize()
        {
            // настройка окна
            MainWindow.Title = "RISC Develop";
            MainWindow.Icon = ResourceManager.GetBitmap("IconLogo.png", Assembly.GetEntryAssembly());

            //MainWindow.MinWidth = 400;
            //MainWindow.MinHeight = 300;

            // настройки StatusBar
            StatusBar.AddItem("Blank space", new GridLength(200));
            StatusBar.AddItem("There is nothig to stare at", new GridLength(2, GridUnitType.Star));
            StatusBar.AddItem("You are sucker", new GridLength(8, GridUnitType.Star));

            Shell.OpenDocument(new CodeEditorViewModel(new Gemini.Modules.CodeEditor.LanguageDefinitionManager()));

            Shell.ShowTool(new SolutionExplorer.ViewModels.SolutionExplorerViewModel());
            Shell.ShowTool(new RegistersViewModel(null, PlatformService.Extensibility));

            Shell.ShowFloatingWindowsInTaskbar = true;
            Shell.ToolBars.Visible = true;

            MainWindow.WindowState = WindowState.Maximized;
        }
    }
}
