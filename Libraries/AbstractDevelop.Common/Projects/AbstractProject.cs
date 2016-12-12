using System;
using System.Collections.Generic;
using System.IO;

using AbstractDevelop.Storage.Formats;
using Newtonsoft.Json;

using SettingsContainer = System.Collections.Generic.Dictionary<string, string>;

namespace AbstractDevelop.Projects
{
    [Serializable]
    public class AbstractProject
    {
        #region [Свойства и Поля]

        public string Title { get; set; }

        public int PlatformCode
        {
            get { return Platform.ID; }
            set { Platform = PlatformService.GetPlatform(value); }
        }

        public SettingsContainer Settings
        {
            get { return settings; }
            set { settings = value ?? new SettingsContainer(); }
        }

        public List<IProjectEntry> Content
        {
            get { return rootDirectory.entries; }
            set { rootDirectory.Content = value; }
        }

        [JsonIgnore]
        public IPlatformProvider Platform { get; set; }

        [JsonIgnore]
        public ProjectDirectory Root => rootDirectory;

        public const string ProjectExtension = ".adp";

        // поставщик форматирования
        IDataFormatProvider formatProvider;
        // корневой элемент представления объектов
        ProjectDirectory rootDirectory;
        // хранилище настроек
        SettingsContainer settings = new SettingsContainer();

        #endregion

        #region [Методы]

        /// <summary>
        /// Загружает проект из указанного потока с использованием поставщиа формата
        /// </summary>
        /// <param name="source">Поток, из которого необходимо загрузить проект</param>
        /// <param name="formatProvider">Поставщик формата для данных</param>
        public static AbstractProject Load(Stream source, IDataFormatProvider formatProvider)
        {
            var project = formatProvider?.Deserialize<AbstractProject>(source);

            project.formatProvider = formatProvider;
            return project;
        }

        /// <summary>
        /// Загружает проект из указанного файла и ассоциирует этот файл с проектом.
        /// </summary>
        /// <param name="fileName">Имя файла для загрузки.</param>
        /// <returns>Загруженный проект.</returns>
        public static AbstractProject Load(string filePath, IDataFormatProvider formatProvider)
            => Load(File.OpenRead(filePath), formatProvider);

        /// <summary>
        /// Сохраняет проект в указанный поток при помощи поставщика формата
        /// </summary>
        /// <param name="target"></param>
        public void Save(Stream target, IDataFormatProvider format = default(IDataFormatProvider))
            => (format ?? formatProvider)?.Serialize(this, target);

        /// <summary>
        /// Задает поставщика формата для данного проекта
        /// </summary>
        /// <param name="provider">Поставщик форматов для задания</param>
        public void SetFormatProvider(IDataFormatProvider provider)
            => formatProvider = provider;

        #endregion

        #region [Конструкторы и деструкторы]

        /// <summary>
        /// Конструктор по умолчанию для проекта
        /// </summary>
        /// <param name="title">Заголовок проекта</param>
        /// <param name="platform">Платформа, для которой разработан проект</param>
        /// <param name="format">Поставщик форматирования для хранения данных проекта</param>
        /// <param name="content">Содержимое проекта, доступное изначально</param>
        public AbstractProject(string title = "Untitled Project", IPlatformProvider platform = default(IPlatformProvider), IDataFormatProvider format = default(IDataFormatProvider), params IProjectEntry[] content)
        {
            Title = title;
            Platform = platform;

            rootDirectory = new ProjectDirectory("root", content ?? new IProjectEntry[0]);
            formatProvider = format;
        }

        #endregion

        #region [Индексаторы]

        /// <summary>
        /// Получает\устанавливает элемент проекта по указанному пути
        /// </summary>
        /// <param name="path">Путь элемента</param>
        /// <param name="type">Тип элемента</param>
        /// <param name="generateExceptions">Следует ли выдавать исключения во время работы</param>
        /// <returns></returns>
        public IProjectEntry this[string path, EntryType type = EntryType.File, bool generateExceptions = false]
        {
            get
            {
                try
                {
                    if (type == EntryType.File)
                        return rootDirectory.GetFile(path);
                    else
                        return rootDirectory.GetDirectory(path);
                }
                catch
                {
                    if (generateExceptions)
                        throw;
                    else
                        return default(IProjectEntry);
                }
            }
            set
            {
                (EntryType type, string rest) lookup = (type, path);
                ProjectDirectory current = rootDirectory;
                do
                {
                    path = lookup.rest;
                    lookup = current.GetLookup(ref path);
                    // файл необходимо заменять
                    if (lookup.type == EntryType.File)
                        break;
                    // поиск\создание необходимых папок
                    else if (!current.Contains(path, EntryType.Directory))
                        current.Add(new ProjectDirectory(path));
                    // переход к следующему уровню
                    current = current.GetDirectory(path);
                }
                while (!string.IsNullOrEmpty(lookup.rest));

                // замена существующего файла\папки
                value.Name = path;
                if (current.Contains(path, lookup.type))
                    current.Replace(current.Find(path, lookup.type), value);
                else
                    current.Add(value);
            }
        }

        #endregion
    }
}