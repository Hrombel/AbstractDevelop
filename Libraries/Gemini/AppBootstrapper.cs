using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;

using Gu.Localization;
using Caliburn.Micro;

namespace AbstractDevelop
{
    public class Bootstrapper :
        BootstrapperBase
    {
        protected CompositionContainer Container { get; set; }

        public IList<Assembly> PriorityAssemblies { get; protected set; }

        public Bootstrapper()
        {
            PreInitialize();
            Initialize();
        }

        protected virtual void PreInitialize()
        {
            var code = Properties.Settings.Default.LanguageCode;

            // выбор языка программы из настроек
            if (!string.IsNullOrWhiteSpace(code))
            {
                var culture = CultureInfo.GetCultureInfo(code);
                try
                {
                    Translator.Culture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                    Thread.CurrentThread.CurrentCulture = culture;
                }
                catch { }
            }
        }

        protected override void BuildUp(object instance)
        {
            // заполнение всех запросов импорта
            Container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            // показ главного окна
            DisplayRootViewFor<IMainWindow>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
            => new[] { Assembly.GetEntryAssembly() };

        protected override void Configure()
        {
            // настройка MEF
            var directoryCatalog = new DirectoryCatalog(@"./");
            AssemblySource.Instance.AddRange(
                directoryCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));

            // отдельно обрабатываются приоритетные сборки
            PriorityAssemblies = SelectAssemblies().ToList();
            var priorityCatalog = new AggregateCatalog(PriorityAssemblies.Select(x => new AssemblyCatalog(x)));
            var priorityProvider = new CatalogExportProvider(priorityCatalog);

            // обработка прочих сборок
            var mainCatalog = new AggregateCatalog(
                AssemblySource.Instance
                    .Where(assembly => !PriorityAssemblies.Contains(assembly))
                    .Select(x => new AssemblyCatalog(x)));
            var mainProvider = new CatalogExportProvider(mainCatalog);

            // композиция контейнера
            Container = new CompositionContainer(priorityProvider, mainProvider);
            priorityProvider.SourceProvider = Container;
            mainProvider.SourceProvider = Container;

            var batch = new CompositionBatch();

            BindServices(batch);
            batch.AddExportedValue(mainCatalog);

            Container.Compose(batch);
        }

        protected virtual void BindServices(CompositionBatch batch)
        {
            // экспорт сервисов
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(Container);
            batch.AddExportedValue(this);
        }

        // реализация для MEF
        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = Container.GetExports<object>(contract);

            if (exports.Any())
                return exports.First().Value;

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }
    }
}