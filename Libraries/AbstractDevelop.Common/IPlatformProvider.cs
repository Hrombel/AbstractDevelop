using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using AbstractDevelop.Machines;

namespace AbstractDevelop
{
    /// <summary>
    /// Описывает поставщика платформы
    /// </summary>
    [InheritedExport]
    public interface IPlatformProvider
    {
        /// <summary>
        /// Название платформы
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Численный идетификатор платформы
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Доступные типы машин для создания
        /// </summary>
        IEnumerable<Type> AvailableMachineTypes { get; }

        /// <summary>
        /// Последняя запущенная на данной платформе машина
        /// </summary>
        AbstractMachine CurrentMachine { get; }

        /// <summary>
        /// Инициализирует платформу с указанным поставщиком расширений
        /// </summary>
        /// <param name="extensibilityProvider">Поставщик расширений</param>
        void Initialize(IExtensibilityProvider extensibilityProvider);

        /// <summary>
        /// Создает машину указанного типа (если он поддерживается данной платформой)
        /// </summary>
        /// <param name="machineType">Тип машины для создания</param>
        AbstractMachine CreateMachine(Type machineType, Dictionary<string, string> settings);

    }
}
