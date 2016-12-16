using Caliburn.Micro;
using Gemini.Framework.Services;
using Gemini.Modules.StatusBar;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AbstractDevelop.Modules.Registers.ViewModels;
using Gemini.Modules.CodeEditor.ViewModels;

namespace AbstractDevelop
{
    class Startup :
        Gemini.AppBootstrapper
    {
        Window MainWindow;

        [Import]
        IResourceManager ResourceManager;

        [Import]
        IStatusBar StatusBar;

        [Import]
        IShell Shell;

        protected override void PreInitialize()
        {
            base.PreInitialize();

            
   

            // собственные проверки
        }

       

        public void ExportObjects(params object[] objectsToExport)
        {
            foreach (var exportingObject in objectsToExport)
                Container.ComposeExportedValue(exportingObject.GetType().Name, exportingObject);
        }
    
        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            var pluginsLoading = Task.Run(() => { }); // TODO

            PlatformService.Initialize(Container);

            // загрузка базовых параметров приложения
            base.OnStartup(sender, e);

            // экспорт экземпляра приложения
            ExportObjects(Application, Application.MainWindow);
            

            //Container.ComposeExportedValue(Application);


            // ожидание завершения загрузки расширений
            await pluginsLoading;
        }
    }
}
