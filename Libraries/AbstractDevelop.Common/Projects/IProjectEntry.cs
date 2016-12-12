namespace AbstractDevelop.Projects
{
    /// <summary>
    /// Описывает тип элемента проекта
    /// </summary>
    public enum EntryType
    {
        /// <summary>
        /// Каталог
        /// </summary>
        Directory,
        /// <summary>
        /// Файл
        /// </summary>
        File
    }

    /// <summary>
    /// Описывает элемент проекта
    /// </summary>
    public interface IProjectEntry :
        System.IDisposable
    {
        /// <summary>
        /// Имя элемента (без полного пути)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Тип элемента (каталог или файл)
        /// </summary>
        EntryType Type { get; }

        /// <summary>
        /// Содержимое элемента
        /// </summary>
        object Content { get; set; }
    }
}
