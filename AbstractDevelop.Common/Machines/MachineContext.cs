using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace AbstractDevelop.Machines
{
    public class AbstractMachineContext
    {
        /// <summary>
        /// Основной объект сериалзиции, связанный с машиной
        /// </summary>
        /// <remarks>Необходимость в будущем для реализации проектов</remarks>
        IFormatter Formatter { get; set; }

    }
}
