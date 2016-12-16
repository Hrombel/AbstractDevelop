using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using AbstractDevelop.Machines;
using AbstractDevelop.Projects;

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
        /// <param name="project">Проект, из которого будут взяты данные для создания машины</param>
        AbstractMachine CreateMachine(AbstractProject project);

        /// <summary>
        /// Создает машину указанного типа (если он поддерживается данной платформой)
        /// </summary>
        /// <param name="machineType">Тип машины для создания</param>
        /// <param name="project">Проект, из которого будут взяты данные для создания машины</param>
        AbstractMachine CreateMachine(Type machineType, AbstractProject project);

    }
}
