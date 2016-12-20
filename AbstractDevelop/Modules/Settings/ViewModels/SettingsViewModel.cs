using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Linq;

using Gemini.Modules.Settings;
using System.Collections.Generic;
using System.Collections.Specialized;

using AbstractDevelop.Properties;
using Gemini.Modules.MainMenu.ViewModels;
using Gemini.Framework.Themes;

namespace AbstractDevelop.Modules.Settings.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SettingsViewModel :
        PropertyChangedBase, ISettingsEditor
    {
        Dictionary<string, string> languageDefinitions = new Dictionary<string, string>()
        {
            ["ru"] = Translate.Key("RussianLang", Resources.ResourceManager),
            ["en"] = Translate.Key("EnglishLang", Resources.ResourceManager),
            // язык по умолчаню - системный
            [""] = Translate.Key("SystemLang", Resources.ResourceManager)
        };

        // TODO: добавить настроек
        Properties.Settings Settings =>
            Properties.Settings.Default;
        public SettingsViewModel()
        {
            

           // ConfirmExit = Settings.Default.ConfirmExit;
        }

        public string LanguageTitle => Translate.Key("Language", Resources.ResourceManager);

        public IEnumerable<string> LanguageList
            => languageDefinitions.Values as IEnumerable<string>;

        public string Language { get; set; }
            
        public string SettingsPageName => "Основные";
        //{
        //    get { return Resources.SettingsPageGeneral; }
        //}

        public string SettingsPagePath => "Окружение";
        //{
        //    get { return Resources.SettingsPathEnvironment; }
        //}

        public override void Refresh()
        {
            Language = languageDefinitions[Settings.LanguageCode];
            base.Refresh();
        }

        public void ApplyChanges()
        {
            Settings.LanguageCode = languageDefinitions.First(pair => pair.Value == Language).Key;
            Settings.Reset();
        }
    }
}