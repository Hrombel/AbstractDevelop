using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.tape
{
    [SerializableAttribute]
    /// <summary>
    /// Представляет последовательность ячеек.
    /// </summary>
    public class TapeBlock : IComparable<TapeBlock>, IComparable, ISerializable
    {
        private TapeType _type;
        private ICollection _cells;

        /// <summary>
        /// Инициализирует пустой блок ячеек с индексом первой ячейки.
        /// </summary>
        /// <param name="firstCell">Индекс первой ячейки.</param>
        /// <param name="length">Длина блока.</param>
        public TapeBlock(BigInteger firstCell, int length, TapeType tapeType)
        {
            _type = tapeType;
            if(tapeType == TapeType.TwoStated)
                _cells = new BitArray(length);
            else if(tapeType == TapeType.MultiStated)
                _cells = new byte[length];
            else
                throw new ArgumentException("Неизвестный тип ленты");

            FirstCell = firstCell;
        }

        private TapeBlock(SerializationInfo info, StreamingContext context)
        {
            _type = (TapeType)info.GetValue("type", typeof(TapeType));
            FirstCell = (BigInteger)info.GetValue("first", typeof(BigInteger));
            if (_type == TapeType.TwoStated)
                _cells = info.GetValue("tape", typeof(BitArray)) as BitArray;
            else
                _cells = info.GetValue("tape", typeof(byte[])) as byte[];
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", _type, typeof(TapeType));
            info.AddValue("first", FirstCell, typeof(BigInteger));

            if (_type == TapeType.TwoStated)
                info.AddValue("tape", _cells as BitArray, typeof(BitArray));
            else if (_type == TapeType.MultiStated)
                info.AddValue("tape", _cells as byte[], typeof(byte[]));
            else
                throw new SerializationException("Неизвестный тип ленты");
        }

        /// <summary>
        /// Получает или задает индекс первой ячейки последовательности относительно ленты.
        /// </summary>
        public BigInteger FirstCell { get; set; }

        /// <summary>
        /// Получает индекс последней ячейки последовательности относительно ленты.
        /// </summary>
        public BigInteger LastCell
        {
            get
            {
                return FirstCell + Length - 1;
            }
        }

        /// <summary>
        /// Получает текущее количество ячеек в блоке.
        /// </summary>
        public int Length
        {
            get 
            {
                return _type == TapeType.TwoStated ?
                           (_cells as BitArray).Length :
                           (_cells as byte[]).Length; 
            }
        }

        /// <summary>
        /// Получает или задает ссылку на следующий блок.
        /// </summary>
        public TapeBlock Next { get; set; }

        /// <summary>
        /// Получает или задает ссылку на предыдущий блок.
        /// </summary>
        public TapeBlock Prev { get; set; }

        /// <summary>
        /// Определяет, являются ли все ячейки блока пустыми.
        /// </summary>
        public bool Empty
        {
            get
            {
                int n = Length;
                if(_type == TapeType.TwoStated)
                {
                    BitArray cells = _cells as BitArray;
                    for (int i = 0; i < n; i++)
                    {
                        if (cells[i])
                            return false;
                    }
                }
                else if(_type == TapeType.MultiStated)
                {
                    byte[] cells = _cells as byte[];
                    for (int i = 0; i < n; i++)
                    {
                        if (cells[i] != 0)
                            return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Получает значение ячейки блока с указанным индексом относительно блока.
        /// </summary>
        /// <param name="index">Индекс проверяемой ячейки.<param>
        public int GetValue(int index)
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException("Указанный индекс находится за пределами диапазона.");

            return _type == TapeType.TwoStated ? ((_cells as BitArray)[index] ? 1 : 0) : (_cells as byte[])[index];
        }

        /// <summary>
        /// Устанавливает указанное значение в ячейку с заданным индексом.
        /// </summary>
        /// <param name="index">Индекс ячейки, в котороую устанавливается значение.</param>
        /// <param name="value">Устанавливаемое значение.</param>
        public void SetValue(int index, byte value)
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException("Указанный индекс ячейки находится за пределами диапазона.");
            if (_type == TapeType.TwoStated)
                (_cells as BitArray).Set(index, value > 0);
            else if (_type == TapeType.MultiStated)
                (_cells as byte[])[index] = value;
        }

        /// <summary>
        /// Создает последовательность, состоящую из ложных булевых значений.
        /// </summary>
        /// <param name="length">Длина последовательности.</param>
        /// <returns>Созданная последовательность.</returns>
        private List<bool> CreateFalseSequence(int length)
        {
            return new List<bool>(new bool[length]);
        }

        public int CompareTo(TapeBlock other)
        {
            return FirstCell.CompareTo(other.FirstCell);
        }

        public int CompareTo(object obj)
        {
            return FirstCell.CompareTo((obj as TapeBlock).FirstCell);
        }
    }
}
