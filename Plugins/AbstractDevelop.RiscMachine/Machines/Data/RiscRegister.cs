using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Реализация регистра для <see cref="RiscMachine"/>
    /// </summary>
    public class RiscRegister :
         IRegister
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
        public string ID => $"r{Index}";
   
        /// <summary>
        /// Общий индекс регистра
        /// </summary>
        public int Index { get; }
      
        /// <summary>
        /// Значение, хранимое в регистре
        /// </summary>
        public byte Value
        {
            get => registerValue;
            set
            {
                if (value != registerValue)
                {
                    registerValue = value;
                    OnPropertyChanged();
                }
            }
        }

        byte registerValue;

        #endregion

        #region [Методы]

        /// <summary>
        /// Вызывает событие <see cref="PropertyChanged"/>
        /// </summary>
        /// <param name="propertyName">Имя свойства, которое было изменено</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region [Конструкторы и деструкторы]

        /// <summary>
        /// Стандартный конструктор объекта
        /// </summary>
        /// <param name="index">Общий индекс регистра</param>
        public RiscRegister(int index)
        {
            Index = index;
        }

        #endregion
    }
}
