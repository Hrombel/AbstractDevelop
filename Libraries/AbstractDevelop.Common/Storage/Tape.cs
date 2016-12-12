using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using IndexType = System.Int64/*System.Numerics.BigInteger*/;

namespace AbstractDevelop.Machines
{
    [Serializable]
    public class Tape<DataType> :
        IEnumerable<DataType>
    {
        /// <summary>
        /// Возникает после обновления состояния ленты.
        /// </summary>
        public event EventHandler Update;

        #region [Вложенные типы]

        [Serializable]
        public class TapeBlock :
            IEnumerable<DataType>, IComparable<TapeBlock>
        {
            #region [Поля]

            public const int DefaultBlockSize = 128;

            #endregion

            #region [Свойства]

            /// <summary>
            /// Получает индекс последней ячейки последовательности относительно ленты.
            /// </summary>
            public IndexType End => (Start + Length - 1);

            /// <summary>
            /// Получает текущее количество ячеек в блоке.
            /// </summary>
            public int Length => container.Length;

            /// <summary>
            /// Получает или задает индекс первой ячейки последовательности относительно ленты.
            /// </summary>
            public IndexType Start { get; set; }
            #endregion

            #region [Методы]

            #endregion

            #region [Конструкторы]

            #endregion

            #region [Индексаторы]

            #endregion

            private DataType[] container;

            #region [Методы]

            public int CompareTo(TapeBlock other) => Start.CompareTo(other.Start);

            public IEnumerator<DataType> GetEnumerator() => (container as IEnumerable<DataType>)?.GetEnumerator();

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion

            public TapeBlock(IndexType start, int length = DefaultBlockSize, DataType value = default(DataType))
            {
                Start = start;
                container = Enumerable.Repeat(value, length).ToArray();
            }

            public DataType this[int index]
            {
                get { return container[index.CheckIndex(max: Length)]; }
                set { container[index.CheckIndex(max: Length)] = value; }
            }
        }

        public class TapeDebugInfo
        {
            #region [События]

            public event Action<DataType, DataType, TapeDebugInfo> SituationHandler;

            #endregion

            #region [Свойства]

            public bool Enabled { get; set; }

            public StringWriter Output { get; set; }

            #endregion
        }

        #endregion

        #region [Свойства]

        public LinkedList<TapeBlock> Blocks { get; } = new LinkedList<TapeBlock>();

        public DataType Current
        {
            get { return this[Position]; }
            set { this[Position] = value; }
        }

        public TapeDebugInfo DebugInfo { get; } = new TapeDebugInfo();

        /// <summary>
        /// Показывает, является ли лента бесконечной (автоматически дополняемой в нужных секторах)
        /// </summary>
        public bool IsInfinite { get; set; }
        public IndexType Position { get; set; }

        #endregion

        private int blockLocalIndex;

        #region [Методы]

        /// <summary>
        /// Добавляет блок в список блоков на соответствующее ему место
        /// </summary>
        /// <param name="block">Блок для добавления</param>
        /// <returns></returns>
        public TapeBlock AddBlock(TapeBlock block)
        {
            if (block == default(TapeBlock))
                throw new ArgumentNullException(nameof(block));

            var nextBlock = Blocks.FirstOrDefault(b => b.Start > block.End);
            if (nextBlock == default(TapeBlock))
                Blocks.AddFirst(block);
            else
                Blocks.AddBefore(Blocks.Find(nextBlock), block);

            return block;
        }

        public void Clear(IndexType length = default(IndexType), IndexType start = default(IndexType), DataType fillingValue = default(DataType))
        {
            if (length == default(IndexType))
                length = Blocks.Sum(b => Math.Min((long)(b.Start - start), b.Length));

            int index = 0;
            var beenCreated = false;
            var block = default(TapeBlock);
            for (IndexType pos = 0; pos < length; pos++)
            {
                block = CreateEmptyBlock(index, out index, out beenCreated, fillingValue, true);
                // пустые блоки заполнять не требуется
                if (beenCreated)
                    pos += block.Length;
                else block[index] = fillingValue;
            }
        }

        /// <summary>
        /// Создает пустой блок вокруг элемента с указанным индексом
        /// </summary>
        /// <param name="index">Индекс элемента</param>
        /// <param name="localIndex">Относительный индекс элемента в блоке</param>
        /// <param name="fillingValue">Значение, которым необходимо заполнить создаваемый блок</param>
        /// <returns></returns>
        public TapeBlock CreateEmptyBlock(IndexType index, out int localIndex, out bool hasBeenCreated, DataType fillingValue = default(DataType), bool shouldCheckExistance = true)
        {
            var resultBlock = default(TapeBlock);
            // проверка на существование данного блока
            if (shouldCheckExistance && (resultBlock = FindBlock(index, out localIndex)) != default(TapeBlock))
            {
                hasBeenCreated = false;
                return resultBlock;
            }
            else
            {
                // поиск границ создаваемого блока
                IndexType left, right;
                
                if (Blocks.First == default(LinkedListNode<TapeBlock>))
                {
                    left = index - TapeBlock.DefaultBlockSize;
                    right = index + TapeBlock.DefaultBlockSize;
                }
                else
                {
                    left = Blocks.Min(b => b.End);
                    right = Blocks.Max(b => b.Start);
                }

               
                foreach (var block in Blocks)
                {
                    if (index > block.End && block.End > left) left = block.End;
                    if (index < block.Start && block.Start < right) right = block.Start;
                }
                // вычисление размера создаваемого блока
                var length = Math.Abs(Math.Min((int)(right - left), TapeBlock.DefaultBlockSize));
                var freeSpaceLeft = (index - left);
                var freeSpaceRight = (right - index);
                var halfSize = length / 2;
                // TODO: проверить вычисления на корректность2
                if (halfSize <= freeSpaceLeft)
                {
                    resultBlock = new TapeBlock(index - halfSize, length, fillingValue);
                    localIndex = halfSize;
                }
                // места слева недостаточно для расположения половины размера блока
                else
                {
                    resultBlock = new TapeBlock(left, length, fillingValue);
                    localIndex = (int)freeSpaceLeft;
                }
                hasBeenCreated = true;
                // добавление созданного блока в список
                return AddBlock(resultBlock);
            }
        }

        /// <summary>
        /// Находит существующий блок по абсолютному индексу элемента
        /// </summary>
        /// <param name="index">Абсолютный индекс элемента</param>
        /// <param name="localIndex">Индекс элемента относительно блока</param>
        public TapeBlock FindBlock(IndexType index, out int localIndex)
        {
            var local = default(int);
            var value = Blocks.FirstOrDefault(block =>
            {
                local = (int)(index - block.Start);
                return local.IsInRange(end: block.Length);
            });

            if (value != default(TapeBlock))
            {
                localIndex = local;
                return value;
            }
            else
            {
                localIndex = 0;
                return default(TapeBlock);
            }
        }
        public IEnumerator<DataType> GetEnumerator()
        {
            foreach (var block in Blocks)
                foreach (var element in block)
                    yield return element;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        internal TapeBlock AccessBlock(IndexType index, out int localIndex, DataType fillingValue = default(DataType))
        {
            var block = FindBlock(index, out localIndex);
            var beenCreated = false;
            if (block != default(TapeBlock))
                return block;
            else if (IsInfinite)
                // выделение нового блока памяти под указанный индекс
                return CreateEmptyBlock(index, out localIndex, out beenCreated, fillingValue, false);
            else throw new IndexOutOfRangeException($"Не удалось найти блок, содержащий индекс {index}");
        }

        #endregion

        #region [Конструкторы]

        //TODO: переделать конструктор
        public Tape() { }

        #endregion

        #region [Индексаторы]

        public DataType this[IndexType index]
        {
            get { return AccessBlock(index, out blockLocalIndex)[blockLocalIndex]; }
            set
            {
                AccessBlock(index, out blockLocalIndex)[blockLocalIndex] = value;
                Update?.Invoke(this, new EventArgs());
            }
        }

        #endregion
    }
}