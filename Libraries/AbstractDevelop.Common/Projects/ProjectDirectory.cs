using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AbstractDevelop.Projects
{
    /// <summary>
    /// Представляет элемент каталога проекта
    /// </summary>
    [Serializable]
    public class ProjectDirectory :
        IProjectEntry
    {

        #region [Свойства и Поля]

        /// <summary>
        /// Содержание элемента (набор прочил элементов)
        /// </summary>
        public object Content
        {
            get { return entries; }
            set
            {
                if (value is IEnumerable<IProjectEntry> collection)
                {
                    entries?.Clear();
                    entries = new List<IProjectEntry>(collection);
                }
            }
        }

        /// <summary>
        /// Имя элемента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип элемента
        /// </summary>
        public EntryType Type => EntryType.Directory;
        [NonSerialized] 
        // содержимое каталога
        internal List<IProjectEntry> entries;

        #endregion

        #region [Методы]

        /// <summary>
        /// Добавляет элемент к содержимому каталога при условии,
        /// что еще не добавлен элемент с таким же именем
        /// </summary>
        /// <param name="entry">Элемент для добавления</param>
        /// <returns>Успешность выполнения операции</returns>
        public bool Add(IProjectEntry entry)
        {
            bool result;
            if (result = !Contains(entry))
                entries.Add(entry);

            return result;
        }

        /// <summary>
        /// Показывает, существует ли данный элемент или элемент
        /// с таким же именем (опционально) в данном каталоге
        /// </summary>
        /// <param name="entry">Элемнт для проверки</param>
        /// <param name="checkName">Следует ли проверять совпадение имен</param>
        /// <returns></returns>
        public bool Contains(IProjectEntry entry, bool checkName = true)
            => (entries?.Any(checkingEntry => checkingEntry == entry ||
               (checkName && checkingEntry?.Name == entry?.Name)) ?? false);

        /// <summary>
        /// Показывает, существует ли элемент определенного типа с заданным именем в данном каталоге
        /// </summary>
        /// <param name="name">Имя элемента для поиска</param>
        /// <param name="type">Тип элемента для поиска</param>
        /// <returns>Наличие элемента с указанными параметрами</returns>
        public bool Contains(string name, EntryType type = EntryType.File, bool shouldCheckType = true)
            => entries.Any(entry => entry?.Name == name && (!shouldCheckType || entry?.Type == type));

        /// <summary>
        /// Освобождает ресурсы объекта и его содержиого
        /// </summary>
        public void Dispose()
        {
            entries?.ForEach(entry => entry.Dispose());
            entries?.Clear();
        }

        public IProjectEntry Find(string path, EntryType type = EntryType.File)
            => type == EntryType.File ? GetFile(path) : GetDirectory(path) as IProjectEntry;

        /// <summary>
        /// Получает каталог по его относительному пути
        /// </summary>
        /// <param name="path">Относительный путь каталога для поиска</param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"/>
        public ProjectDirectory GetDirectory(string path)
        {
            if (path.Contains(Path.DirectorySeparatorChar) || path.Contains(Path.AltDirectorySeparatorChar))
            {
                (EntryType type, string rest) lookup = GetLookup(ref path);

                // поиск через поддиректории
                if (!string.IsNullOrEmpty(lookup.rest))
                    return GetDirectory(path).GetDirectory(lookup.rest);
            }
            else
            {
                var query = entries.Where(entry => entry.Type == EntryType.Directory && entry.Name == path);
                var result = default(IProjectEntry);

                // результат получен
                if ((result = query.FirstOrDefault()) != default(ProjectDirectory))
                    return result as ProjectDirectory;
            }

            throw new DirectoryNotFoundException(Translate.Key("DirectoryNotFoundError", format: path));
        }

        /// <summary>
        /// Получает файл по его относительному пути
        /// </summary>
        /// <param name="path">Относительный путь файла для поиска</param>
        /// <returns>Файл проекта по указанному пути</returns>
        /// <exception cref="FileNotFoundException"/>
        public ProjectFile GetFile(string path)
        {
            (EntryType type, string rest) lookup = GetLookup(ref path);

            if (string.IsNullOrEmpty(lookup.rest) && lookup.type == EntryType.File)
            {
                var query = entries.Where(entry => entry.Type == EntryType.File && entry.Name == path);
                var result = default(IProjectEntry);

                // результат получен
                if ((result = query.FirstOrDefault()) != default(ProjectFile))
                    return result as ProjectFile;
            }
            else if (lookup.type == EntryType.Directory)
                return GetDirectory(path).GetFile(lookup.rest);

            // файл не найден
            throw new FileNotFoundException(string.Format(Translate.Key("FileNotFoundError"), path));
        }

        /// <summary>
        /// Перемещает элемент из одного каталога в другой
        /// </summary>
        /// <param name="entry">Элемент для перемещения</param>
        /// <param name="targetDirectory">Каталог, куда следует поместить перемещаемый элемент</param>
        /// <returns></returns>
        public bool MoveTo(IProjectEntry entry, ProjectDirectory targetDirectory)
            => Remove(entry) && (targetDirectory?.Add(entry) ?? false);

        public bool Replace(IProjectEntry source, IProjectEntry replacement)
            => Remove(source) && Add(replacement);

        /// <summary>
        /// Удаляет элемент из содержимого каталога при условии,
        /// что он существует в данном каталоге
        /// </summary>
        /// <param name="entry">Элемент для удаления</param>
        /// <returns>Успешность выполнения операции</returns>
        public bool Remove(IProjectEntry entry)
        {
            bool result;
            if (result = Contains(entry))
                entries.Remove(entry);

            return result;
        }
        /// <summary>
        /// Производит разбор пути на корень и дополнение,
        /// определеяет тип корневого элемента
        /// </summary>
        /// <param name="path">Путь для разбора</param>
        /// <returns>Кортеж из типа корневого элемента и дополнения (путь, исключающий корень)</returns>
        /// <remarks>Корневой элемент возвращается через параметр <paramref name="path"/></remarks>
        internal (EntryType, string) GetLookup(ref string path)
        {
            if (string.IsNullOrEmpty(path))
                return (EntryType.File, string.Empty);

            var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            path = parts[0];
            return (parts.Length > 1? EntryType.Directory : EntryType.File, 
                string.Join(Path.DirectorySeparatorChar.ToString(), parts, 1, parts.Length - 1));
        }

        #endregion

        #region [Конструкторы и деструкторы]

        /// <summary>
        /// Конструктор по умолчанию лля элемента каталога
        /// </summary>
        /// <param name="name">Имя каталога</param>
        /// <param name="content">Содержимое каталога</param>
        public ProjectDirectory(string name = default(string), params IProjectEntry[] content)
        {
            Name = name;
            Content = content;
        }

        /// <summary>
        /// Деструктор по умолчанию
        /// </summary>
        ~ProjectDirectory() { Dispose(); }

        #endregion

    }
}
