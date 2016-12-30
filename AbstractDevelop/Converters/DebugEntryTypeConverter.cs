using Gu.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AbstractDevelop
{
    [ValueConversion(typeof(DebugEntryType), typeof(string))]
    public class DebugEntryTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Translator.CurrentCulture.IetfLanguageTag == "ru" && value is DebugEntryType type)
            {
                switch (type)
                {
                    case DebugEntryType.Error:
                        return Translate.Key("DebugTypeError", Properties.Resources.ResourceManager);
                    case DebugEntryType.Message:
                        return Translate.Key("DebugTypeeMessage", Properties.Resources.ResourceManager);
                    case DebugEntryType.Warning:
                        return Translate.Key("DebugTypeWarning", Properties.Resources.ResourceManager);
                    case DebugEntryType.Output:
                        return Translate.Key("DebugTypeOutput", Properties.Resources.ResourceManager);
                }
            }
               
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
