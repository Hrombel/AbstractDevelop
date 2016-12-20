using System.ComponentModel;

namespace AbstractDevelop
{
    /// <summary>
    /// Описывает ячейку памяти архитектуры RISC
    /// </summary>
    public interface IDataCell :
        INotifyPropertyChanged
    {
        /// <summary>
        /// Индекс ячейки в системе учета
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Идентификатор ячейки (название)
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Значение, хранящееся в ячейке
        /// </summary>
        byte Value { get; set; }

        /// <summary>
        /// Обновляет свойство с указанным именем
        /// </summary>
        /// <param name="propertyName">Имя свойства для обновления</param>
        void OnPropertyChanged(string propertyName);
    }
}
