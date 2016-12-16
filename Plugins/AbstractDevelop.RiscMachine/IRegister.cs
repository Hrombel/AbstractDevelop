using System.ComponentModel;

namespace AbstractDevelop
{
    /// <summary>
    /// Описывает регистр архитектуры RISC
    /// </summary>
    public interface IRegister :
        INotifyPropertyChanged
    {
        /// <summary>
        /// Индекс регистра в системе учета
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Идентификатор регистра (название)
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Значение, хранящееся в регистре
        /// </summary>
        byte Value { get; set; }
    }
}
