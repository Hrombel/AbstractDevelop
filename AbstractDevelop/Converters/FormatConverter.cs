using AbstractDevelop.Machines;
using AbstractDevelop.Properties;
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
    [ValueConversion(typeof(byte), typeof(string))]
    public class FormatConverter : IValueConverter
    {
        static Dictionary<NumberFormat, string> prefixes = new Dictionary<NumberFormat, string>()
        {
            [NumberFormat.Binary] = "0b",
            [NumberFormat.Octal] = "0o",
            [NumberFormat.Hex] = "0x"
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = Settings.Default.NumberFormat;

            if (value is byte v && targetType == typeof(string))
            {
                return (Settings.Default.ShowFormatPrefixes ? prefixes[format] : string.Empty) +
                      FillZeroes(System.Convert.ToString(v, (int)format), (int)Math.Ceiling(Math.Log(256, (int)format))).ToUpper();
            }

            throw new ArgumentException(nameof(value));

            string FillZeroes(string source, int count)
            {
                 var countToFill = count - source.Length;

                if (countToFill > 0)
                    return new string('0', countToFill) + source;
                else return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(byte) && value is string source)
            {
                if (!prefixes.Values.Any(prefix => source.StartsWith(prefix)))
                    source = prefixes[Settings.Default.NumberFormat] + source;

                try
                {
                    return RiscMachine.GetNumberValue(source);
                }
                catch { return DependencyProperty.UnsetValue; }
            }

            throw new ArgumentException(nameof(value));
        }
    }
}
