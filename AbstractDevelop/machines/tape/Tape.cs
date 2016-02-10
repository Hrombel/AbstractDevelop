using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.tape
{
    [Serializable]
    /// <summary>
    /// Представляет модель ленты, имеющей бесконечное количество ячеек.
    /// </summary>
    public class Tape : ISerializable
    {
        /// <summary>
        /// Возникает после обновления состояния ленты.
        /// </summary>
        public event EventHandler Update;

        public const int BLOCK_SIZE = 100;

        private TapeType _type;

        TapeBlock _firstBlock;
        TapeBlock _lastBlock;
        TapeBlock _currentBlock;

        /// <summary>
        /// Инициализирует ленту с заданным типом.
        /// </summary>
        /// <param name="type">Тип создаваемой ленты.</param>
        public Tape(TapeType type)
        {
            _type = type;
        }

        /// <summary>
        /// Десериализирует ленту из бинарного представления.
        /// </summary>
        private Tape(SerializationInfo info, StreamingContext context)
        {
            try
            {
                _type = (TapeType)info.GetValue("type", typeof(TapeType));

                int n = info.GetInt32("amount");
                for(int i = 0; i < n; i++)
                {
                    AddBlock(info.GetValue("block" + i.ToString(), typeof(TapeBlock)) as TapeBlock);
                }
            }
            catch(Exception ex)
            {
                throw new ArgumentException(string.Format("Неверные параметры десериализации: \"{0}\"", ex.Message), ex);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", _type, typeof(TapeType));

            List<TapeBlock> blocks = new List<TapeBlock>();
            TapeBlock current = _firstBlock;
            int n = 0;
            while (current != null)
            {
                n++;
                blocks.Add(current);
                current = current.Next;
            }
            info.AddValue("amount", n);
            for(int i = 0; i < n; i++)
            {
                info.AddValue("block" + i.ToString(), blocks[i], typeof(TapeBlock));
            }
        }

        /// <summary>
        /// Получает тип ленты.
        /// </summary>
        public TapeType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Стирает все символы, присутствующие на ленте.
        /// </summary>
        public void Clear()
        {
            _firstBlock = null;
            _lastBlock = null;
            _currentBlock = null;
        }

        /// <summary>
        /// Добавляет блок ячеек на свое место в списке.
        /// </summary>
        /// <param name="block">Добавляемый блок.</param>
        private void AddBlock(TapeBlock block)
        {
            if(_currentBlock == null)
            {
                _currentBlock = block;
                _firstBlock = block;
                _lastBlock = block;
                return;
            }
            TapeBlock current = _firstBlock;

            while(block.CompareTo(current) == 1)
            {
                current = current.Next;
                if (current == null) break;
            }
            if(current == null)
            {
                _lastBlock.Next = block;
                block.Prev = _lastBlock;
                _lastBlock = block;
            }
            else
            {
                if (current.Prev != null)
                {
                    current.Prev.Next = block;
                    block.Prev = current.Prev;
                }
                else
                    _firstBlock = block;
                current.Prev = block;
                block.Next = current;
            }
            _currentBlock = block;
        }

        /// <summary>
        /// Удаляет блок из ленты.
        /// </summary>
        /// <param name="block">Удаляемый блок.</param>
        private void RemoveBlock(TapeBlock block)
        {
            if (block == null)
                throw new ArgumentNullException("Удаляемый блок не может быть неопределенным");

            if(block.Prev == null) // блок стоит первым в списке либо он единственный.
            {
                _firstBlock = block.Next;
                if(_firstBlock != null)
                    _firstBlock.Prev = null;
                else
                    _lastBlock = null;
                _currentBlock = _firstBlock;
            }
            else if(block.Next == null) // блок стоит последним в списке.
            {
                _lastBlock = block.Prev;
                _lastBlock.Next = null;
                _currentBlock = _lastBlock;
            }
            else // блок стоит в центре списка.
            {
                block.Prev.Next = block.Next;
                block.Next.Prev = block.Prev;
                _currentBlock = block.Prev;
            }
        }

        /// <summary>
        /// Получает индекс первой ячейки блока, которому принадлежит указанная ячейка.
        /// </summary>
        /// <param name="cell">Ячейка.</param>
        private BigInteger GetBlockIndex(BigInteger cell)
        {
            if (cell.Sign >= 0)
                return (cell / BLOCK_SIZE) * BLOCK_SIZE;
            else
                return (((cell + 1) / BLOCK_SIZE) - 1) * BLOCK_SIZE;
        }

        /// <summary>
        /// Возвращает значение указанной ячейки.
        /// </summary>
        /// <param name="cell">Индекс указанной ячейки.</param>
        /// <returns>Значение указанной ячейки.</returns>
        public int GetValue(BigInteger cell)
        {
            if (_currentBlock == null) return 0;
            if (cell < _firstBlock.FirstCell) return 0;
            if (cell > _lastBlock.LastCell) return 0;

            if (cell < _currentBlock.FirstCell)
            {
                TapeBlock initialValue = _currentBlock;
                _currentBlock = _currentBlock.Prev;
                BigInteger blockCell;
                while (_currentBlock != null)
                {
                    if (cell >= _currentBlock.FirstCell)
                    {
                        blockCell = cell - _currentBlock.FirstCell;
                        if (blockCell >= 0)
                        {
                            if (blockCell < BLOCK_SIZE)
                                return _currentBlock.GetValue((int)blockCell);
                        }
                    }
                    _currentBlock = _currentBlock.Prev;
                }
                _currentBlock = initialValue;
                return 0;
            }
            else if (cell > _currentBlock.LastCell)
            {
                TapeBlock initialValue = _currentBlock;
                _currentBlock = _currentBlock.Next;
                BigInteger blockCell;
                while (_currentBlock != null)
                {
                    if (cell <= _currentBlock.LastCell)
                    {
                        blockCell = cell - _currentBlock.FirstCell;
                        if (blockCell >= 0)
                        {
                            if (blockCell < BLOCK_SIZE)
                                return _currentBlock.GetValue((int)blockCell);
                        }
                    }

                    _currentBlock = _currentBlock.Next;
                }
                _currentBlock = initialValue;
                return 0;
            }
            else
            {
                return _currentBlock.GetValue((int)(cell - _currentBlock.FirstCell));
            }
        }

        /// <summary>
        /// Устанавливает значение в указанную ячейку.
        /// </summary>
        /// <param name="cell">Индекс ячейки.</param>
        /// <param name="value">Устанавливаемое значение.</param>
        public void SetValue(BigInteger cell, byte value)
        {
            bool valueSet = false;
            TapeBlock current = _currentBlock;

            if (_currentBlock != null && cell >= _firstBlock.FirstCell && cell <= _lastBlock.LastCell)
            {
                BigInteger blockCell;
                if (cell < current.FirstCell)
                {
                    current = current.Prev;
                    while (current != null)
                    {
                        if (cell >= current.FirstCell)
                        {
                            blockCell = cell - current.FirstCell;
                            if (blockCell >= 0)
                            {
                                current.SetValue((int)blockCell, value);
                                valueSet = true;
                                break;
                            }
                        }
                        current = current.Prev;
                    }
                }
                else if (cell > current.LastCell)
                {
                    current = current.Next;
                    while (current != null)
                    {
                        if (cell <= current.LastCell)
                        {
                            blockCell = cell - current.FirstCell;
                            if(blockCell >= 0)
                            {
                                current.SetValue((int)blockCell, value);
                                valueSet = true;
                                break;
                            }
                        }
                        current = current.Next;
                    }
                }
                else
                {
                    current.SetValue((int)(cell - current.FirstCell), value);
                    valueSet = true;
                }
            }

            if(!valueSet)
            {
                BigInteger index = GetBlockIndex(cell);
                AddBlock(new TapeBlock(index, BLOCK_SIZE, _type));
                current = _currentBlock;
                current.SetValue((int)(cell - index), value);
            }

            if(value == 0)
            {
                if (current.Empty)
                    RemoveBlock(current);
            }

            if(Update != null)
                Update(this, new EventArgs());
        }
    }
}
