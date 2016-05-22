using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Machines.Risc
{
    public class DataCell<TargetType>
    {
        public TargetType Value { get; set; }
    }

    public class DataLink<TargetType>
    {

        public static explicit operator DataCell<TargetType>(DataLink<TargetType> source)
        {
            return default(DataCell<TargetType>);
        }
    }
}
