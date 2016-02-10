using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Кодирует элементарные команды для параллельной машины с бесконечными регистрами.
    /// </summary>
    public enum RegisterOperationId : byte
    {
        /// <summary>
        /// Обнуление регистра.
        /// </summary>
        Erase,
        /// <summary>
        /// Инкремент значения регистра.
        /// </summary>
        Inc,
        /// <summary>
        /// Копирование значения из одного регистра в другой.
        /// </summary>
        Copy,
        /// <summary>
        /// Условный переход.
        /// </summary>
        Decision,
        /// <summary>
        /// Начало выполнения программы в параллельном потоке.
        /// </summary>
        Start,
        /// <summary>
        /// Блокирование регистра с доступом к нему только программой-блокиратором.
        /// </summary>
        Lock,
        /// <summary>
        /// Разблокировывание регистра и разрешение доступа из других программ.
        /// </summary>
        Unlock,
        /// <summary>
        /// Чтение из проассоциированного устройства ввода.
        /// </summary>
        Read,
        /// <summary>
        /// Запись в проассоциированное устройство вывода.
        /// </summary>
        Write
    }
}
