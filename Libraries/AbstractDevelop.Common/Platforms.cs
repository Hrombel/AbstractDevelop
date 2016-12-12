using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;

using AbstractDevelop.Machines;

namespace AbstractDevelop
{
    /// <summary>
    /// Сервис, предоставляющий доступ к списку загруженных платформ и управлению ими
    /// </summary>
    public static class PlatformService
    {
        /// <summary>
        /// Вспомогательный класс реализации поставщика расширений
        /// </summary>
        class ExtensibilityProvider :
            IExtensibilityProvider
        {
            CompositionContainer composition;

            public void Export<T>(T value)
                => composition.ComposeExportedValue(value);

            public void Export<T>(T value, string contractName, Type contractType)
                => composition.ComposeExportedValue(contractName ?? contractType?.Name, value);

            public T Import<T>()
                => composition.GetExportedValue<T>();

            public T Import<T>(string contractName, Type contractType)
                => composition.GetExportedValue<T>(contractName ?? contractType?.Name); 

            public ExtensibilityProvider(CompositionContainer container)
            {
                composition = container;
            }
        }

        /// <summary>
        /// Вспомогательный класс для сбора платформ
        /// </summary>
        class PlatformObserver
        {
            [ImportMany]
            List<Lazy<IPlatformProvider>> importedPlatforms;

            internal void Fill(IDictionary<int, IPlatformProvider> platforms, IExtensibilityProvider extesnibilityProvider, Func<IPlatformProvider, bool> initAction = null)
            {
                platforms.Clear();

                foreach (var platformWrapper in importedPlatforms)
                {
                    var platform = platformWrapper.Value;
                    platform.Initialize(extesnibilityProvider);

                    // проверка платформы на пригодность для использования
                    if (initAction?.Invoke(platform) ?? false)
                        platforms.Add(platform.ID, platform);
                    else Debug.Print(Translate.Key("PlatformExcluded", format: platform.Name));
                }
            }
        }

        /// <summary>
        /// Доступные для использования платформы
        /// </summary>
        public static IEnumerable<IPlatformProvider> Available
            => observedPlatforms.Values;

        static CompositionContainer extensibilityContainer;
        static IExtensibilityProvider extesnibilityProvider;

        static PlatformObserver observer;

        static Dictionary<int, IPlatformProvider> observedPlatforms;

        /// <summary>
        /// Находил поставщика платформы по уникальному коду платформы
        /// </summary>
        /// <param name="code">Код платформы для поиска</param>
        /// <returns></returns>
        public static IPlatformProvider GetPlatform(int code)
            => observedPlatforms.TryGetValue(code, out var platform) ? platform :
            throw new KeyNotFoundException(Translate.Key("PlatformNotFoundError", format: code));

        public static void Initialize(CompositionContainer container)
        {
            extensibilityContainer = container ?? throw new ArgumentNullException(nameof(container));
            extesnibilityProvider = new ExtensibilityProvider(extensibilityContainer);

            observer = new PlatformObserver();
            Update();
        }

        // TODO: сделать корректное обновление списка платформ без перезагрузки уже инициализированных
        public static void Update()
        {
            extensibilityContainer.SatisfyImportsOnce(observer);

            // заполнение словаря обнаруженных платформ с их первоначальной инициализацией
            observer.Fill(observedPlatforms, extesnibilityProvider, (platform) =>
                platform.AvailableMachineTypes.All(type => type.BasedOn(typeof(AbstractMachine))));
        }
    }
}
