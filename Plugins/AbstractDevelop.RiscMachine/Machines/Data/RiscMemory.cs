using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Реализация памяти для <see cref="RiscMachine"/>
    /// </summary>
    public class RiscMemory :
         IDataCell
    {
        #region [События]

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region [Свойства и Поля]

        /// <summary>
        /// Строковый иденитфикатор регистра
        /// </summary>
        public string ID => $"[{Index}]";
   
        /// <summary>
        /// Общий индекс памяти
        /// </summary>
        public int Index { get; }
      
        /// <summary>
        /// Значение, хранимое в памяти
        /// </summary>
        public byte Value
        {
            get => memoryValue;
            set
            {
                if (value != memoryValue)
                {
                    memoryValue = value;
                    OnPropertyChanged();
                }
            }
        }

        byte memoryValue;

        #endregion

        #region [Методы]

        /// <summary>
        /// Вызывает событие <see cref="PropertyChanged"/>
        /// </summary>
        /// <param name="propertyName">Имя свойства, которое было изменено</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region [Конструкторы и деструкторы]

        /// <summary>
        /// Стандартный конструктор объекта
        /// </summary>
        /// <param name="index">Общий индекс регистра</param>
        public RiscMemory(int index)
        {
            Index = index;
        }

        #endregion
    }
}
