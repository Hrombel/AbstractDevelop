using System.ComponentModel.Composition;
using Caliburn.Micro;

using Gemini.Modules.Settings;

namespace AbstractDevelop.Modules.Settings.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SettingsViewModel : 
        PropertyChangedBase, ISettingsEditor
    {
        // TODO: добавить настроек

        public SettingsViewModel()
        {
           // ConfirmExit = Settings.Default.ConfirmExit;
        }

        //public bool ConfirmExit
        //{
        //    get { return _confirmExit; }
        //    set
        //    {
        //        if (value.Equals(_confirmExit)) return;
        //        _confirmExit = value;
        //        NotifyOfPropertyChange(() => ConfirmExit);
        //    }
        //}


        public string SettingsPageName => "Основные";
        //{
        //    get { return Resources.SettingsPageGeneral; }
        //}

        public string SettingsPagePath => "Окружение";
        //{
        //    get { return Resources.SettingsPathEnvironment; }
        //}

        public void ApplyChanges()
        {
            //if (ConfirmExit == Settings.Default.ConfirmExit)
            //{
            //    return;
            //}

            // Settings.Default.ConfirmExit = ConfirmExit;
            Properties.Settings.Default.Save();
        }
    }
}