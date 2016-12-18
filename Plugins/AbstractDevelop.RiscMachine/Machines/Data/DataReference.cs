using System.Collections.Generic;

using DataType = System.Byte;
using static System.Math;

namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Реализует операнд (указатель на данные) в системе инструкций RISC
    /// </summary>
    public class DataReference
    {
        #region [Свойства и Поля]

        /// <summary>
        /// Владелец данного указателя
        /// </summary>
        public RiscMachine Owner { get; }

        /// <summary>
        /// Значение указателя
        /// </summary>
        public DataReference Reference { get; set; }

        /// <summary>
        /// Тип указателя
        /// </summary>
        public DataReferenceType Type { get; set; }

        /// <summary>
        /// Значение ячейки памяти, на которую ссылается указатель
        /// </summary>
        public virtual DataType Value
        {
            get => GetValue(ref Owner.AccessTimer);
            set => SetValue(value, ref Owner.AccessTimer);
        }

        #endregion

        #region [Методы]

        /// <summary>
        /// Преобразует ссылку в данные 
        /// </summary>
        /// <param name="link"></param>
        public static implicit operator DataType(DataReference link)
             => link.Value;

        /// <summary>
        /// Возвращает значение ячейки памяти, на которую ссылается указатель
        /// </summary>
        /// <param name="accessTime">Счетчик времени доступа</param>
        /// <returns></returns>
        public DataType GetValue(ref int accessTime)
        {
            switch (Type)
            {
                case DataReferenceType.Memory:
                    accessTime += getAccessTime(Owner.Memory);
                    return Owner.Memory[Reference];
                case DataReferenceType.Register:
                    accessTime += 1;
                    return Owner.Registers[Reference].Value;
                default:
                    return Reference.Value;
            }
        }

        /// <summary>
        /// Устанавливает значение в ячейку памяти, на которую ссылается указатель
        /// </summary>
        /// <param name="value">Значение, которое необходимо установить</param>
        /// <param name="accessTime">Счетчик времени доступа</param>
        public void SetValue(DataType value, ref int accessTime)
        {
            switch (Type)
            {
                case DataReferenceType.Memory:
                    accessTime += getAccessTime(Owner.Memory);
                    Owner.Memory[Reference] = value;
                    return;
                case DataReferenceType.Register:
                    accessTime += 1;
                    Owner.Registers[Reference].Value = value;
                    return;
                default:
                    Reference.Value = value;
                    return;
            }
        }

        /// <summary>
        /// Преобразует содержимое объекта в строковое представление
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Value.ToString();

        // функция расчета времени доступа из количества элементов
        int getAccessTime(int length)
            => (int)Ceiling(Log(length, 2));

        // функция расчета времени доступа для коллекции из N элементов
        int getAccessTime<T>(ICollection<T> collection)
            => getAccessTime(collection.Count);

        #endregion

        #region [Конструкторы и деструкторы]

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        /// <param name="owner">Владелец указателя</param>
        /// <param name="reference">Значение указателя</param>
        /// <param name="type">Тип указателя</param>
        public DataReference(RiscMachine owner, DataReference reference, DataReferenceType type = DataReferenceType.Value)
        {
            Owner = owner;
            Reference = reference;
            Type = type;
        }

        #endregion
    }

    /// <summary>
    /// Ссылка на значение
    /// </summary>
    public class ValueReference :
        DataReference
    {
        public override byte Value { get; set; }
      
        public ValueReference(RiscMachine owner, DataType value) :
            base(owner, null, DataReferenceType.Value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Ссылка на метку
    /// </summary>
    public class LabelReference :
        DataReference
    {
        public override byte Value { get; set; }

        public LabelReference(RiscMachine owner, DataType labelIndex) :
            base(owner, null, DataReferenceType.Label)
        {
            Value = labelIndex;
        }
    }
}
