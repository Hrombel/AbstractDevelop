using AbstractDevelop.controls.visuals;
using AbstractDevelop.machines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.controls.menus.project
{
    /// <summary>
    /// Представляет описание элемента меню создания проекта.
    /// </summary>
    public interface IProjectMenuItem
    {
        /// <summary>
        /// Получает название проекта.
        /// </summary>
        string ProjectName { get; }

        /// <summary>
        /// Получает тип машины, для которой предназначен проект.
        /// </summary>
        MachineId Machine { get; }

        /// <summary>
        /// Получает начальные настройки проекта.
        /// </summary>
        Dictionary<string, object> Settings { get; }
    }
}
