using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using AbstractDevelop.Machines;

namespace AbstractDevelop
{
    [ValueConversion(typeof(RiscMachine.InstructionBase), typeof(string))]
    public class InstructionBaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RiscMachine.InstructionBase @base)
                return @base.ToString();

            return Translate.Key("TestInstructionBase", Properties.Resources.ResourceManager);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
