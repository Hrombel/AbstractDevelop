using AbstractDevelop.Machines.Testing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AbstractDevelop
{
    [ValueConversion(typeof(Exception), typeof(string))]
    public class StateToBrushConverter : IValueConverter
    {
        private ResourceDictionary _resourceDictionary;
        public ResourceDictionary ResourceDictionary
        {
            get { return _resourceDictionary; }
            set
            {
                _resourceDictionary = value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestState state)
                switch (state)
                {
                    case TestState.Passed:
                        return ResourceDictionary["sign_ok"];
                    case TestState.Failed:
                        return ResourceDictionary["sign_error"];
                }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
