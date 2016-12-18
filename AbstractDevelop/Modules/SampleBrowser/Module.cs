using System.ComponentModel.Composition;
using Caliburn.Micro;
using AbstractDevelop.Modules.SampleBrowser.ViewModels;
using Gemini.Framework;

namespace AbstractDevelop.Modules.SampleBrowser
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void PostInitialize()
        {
            Shell.OpenDocument(IoC.Get<SampleBrowserViewModel>());
        }
    }
}