using Gu.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AbstractDevelop
{
    class Bootstrapper :
        Gemini.AppBootstrapper
    {
        protected override void Configure()
        {
            var code = Properties.Settings.Default.LanguageCode;

            if (!string.IsNullOrWhiteSpace(code))
            {
                var culture = CultureInfo.GetCultureInfo(code);
                Translator.Culture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
            else { Translator.Culture = Thread.CurrentThread.CurrentUICulture; }

            base.Configure();
        }
    }
}
