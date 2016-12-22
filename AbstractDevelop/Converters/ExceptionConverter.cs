using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AbstractDevelop
{
    [ValueConversion(typeof(Exception), typeof(string))]
    public class ExceptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Exception ex)
                return Translate.Key("TestFailed", Properties.Resources.ResourceManager, ex.Message);

            return Translate.Key("TestSucces", Properties.Resources.ResourceManager);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
