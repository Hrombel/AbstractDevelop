using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.registers
{
    /// <summary>
    /// Представляет блок регистров, хранящий фиксированное количество моделей бесконечных регистров.
    /// </summary>
    [Serializable]
    public class RegistersBlock : IDisposable
    {
        /// <summary>
        /// Количество регистров в одном блоке.
        /// </summary>
        public const int BLOCK_SIZE = 10;

        private BigInteger _index;

        private BigInteger[] _arr;

        /// <summary>
        /// Инициализирует блок с указанным индексом.
        /// </summary>
        /// <param name="index">Индекс первой ячейки блока в коллекции регистров.</param>
        public RegistersBlock(BigInteger index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("Индекс блока должен быть неотрицательным");

            Left = null;
            Right = null;

            _index = index;
            _arr = new BigInteger[BLOCK_SIZE];
        }
        ~RegistersBlock()
        {
            Dispose();
        }

        /// <summary>
        /// Получает индекс первой ячейки блока в коллекции регистров.
        /// </summary>
        public BigInteger Index { get { return _index; } }

        /// <summary>
        /// Получает или задает ссылку на ближаший блок в списке, находящися слева.
        /// </summary>
        public RegistersBlock Left { get; set; }

        /// <summary>
        /// Получает или задает ссылку на ближайший блок в списке, находящийся справа.
        /// </summary>
        public RegistersBlock Right { get; set; }

        /// <summary>
        /// Получает количество регистров в блоке.
        /// </summary>
        public int Length { get { return BLOCK_SIZE; } }

        /// <summary>
        /// Определяет, содержат ли все элементы блока нулевые значения.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                for(int i = 0; i < BLOCK_SIZE; i++)
                    if (_arr[i] != 0) return false;

                return true;
            }
        }

        /// <summary>
        /// Устанавливает значение в указанный регистр.
        /// </summary>
        /// <param name="value">Устанавливаемое значение.</param>
        /// <param name="index">Индекс регистра в блоке.</param>
        public void SetValue(BigInteger value, int index)
        {
            if (value < 0)
                throw new ArgumentException("Значение регистра должно быть положительным");
            if (index < 0 || index >= BLOCK_SIZE)
                throw new ArgumentOutOfRangeException("Индекс регистра вышел за пределы размера блока");

            _arr[index] = value;
        }

        /// <summary>
        /// Возвращает значение указанного регистра.
        /// </summary>
        /// <param name="index">Индекс регистра в блоке.</param>
        /// <returns>Значение указанного регистра.</returns>
        public BigInteger GetValue(int index)
        {
            if (index < 0 || index >= BLOCK_SIZE)
                throw new ArgumentOutOfRangeException("Индекс регистра вышел за пределы размера блока");

            return _arr[index];
        }

        /// <summary>
        /// Освобождает все ресурсы, связанные с блоком.
        /// </summary>
        public void Dispose()
        {
            _arr = null;
            Left = null;
            Right = null;
        }
    }
}
