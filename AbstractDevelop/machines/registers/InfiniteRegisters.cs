using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.registers
{
    /// <summary>
    /// Представляет коллекцию моделей регистров, хранящих неотрицательные целые числа бесконечного размера.
    /// </summary>
    [Serializable]
    public class InfiniteRegisters
    {
        RegistersBlock _currentBlock;
        RegistersBlock _firstBlock;
        RegistersBlock _lastBlock;

        /// <summary>
        /// Инициализирует пустую коллекцию регистров с записанными нулевыми значениями.
        /// </summary>
        public InfiniteRegisters()
        {
            _currentBlock = null;
            _firstBlock = null;
            _lastBlock = null;
        }

        /// <summary>
        /// Увеличивает значение регистра на 1.
        /// </summary>
        /// <param name="index">Индекс регистра.</param>
        public void Increment(BigInteger index)
        {
            SetValue(GetValue(index) + 1, index);
        }

        /// <summary>
        /// Уменьшает значение регистра на 1.
        /// </summary>
        /// <param name="index">Индекс регистра.</param>
        public void Decrement(BigInteger index)
        {
            BigInteger val = GetValue(index);
            if (val == 0) return;
            SetValue(val - 1, index);
        }

        /// <summary>
        /// Записывает указанное значение в регистр.
        /// </summary>
        /// <param name="value">Записываемое значение.</param>
        /// <param name="index">Индекс регистра.</param>
        public void SetValue(BigInteger value, BigInteger index)
        {
            if (index < 0)
                throw new ArgumentException("Индекс должен быть неотрицательным числом");
            if (value < 0)
                throw new ArgumentException("Значение должно быть неотрицательным числом");

            RegistersBlock current = _currentBlock;

            if(current == null)
            {
                current = new RegistersBlock((index / RegistersBlock.BLOCK_SIZE) * RegistersBlock.BLOCK_SIZE);
                _firstBlock = current;
                _lastBlock = current;
            }
            else
            {
                if(index < _firstBlock.Index)
                {
                    current = new RegistersBlock((index / RegistersBlock.BLOCK_SIZE) * RegistersBlock.BLOCK_SIZE);
                    current.Right = _firstBlock;
                    _firstBlock.Left = current;
                    _firstBlock = current;
                }
                else if(index >= _lastBlock.Index + _lastBlock.Length)
                {
                    current = new RegistersBlock((index / RegistersBlock.BLOCK_SIZE) * RegistersBlock.BLOCK_SIZE);
                    current.Left = _lastBlock;
                    _lastBlock.Right = current;
                    _lastBlock = current;
                }
            }
            _currentBlock = current;


            while (current != null)
            {
                if (index < current.Index)
                {
                    current = current.Left;
                    if (current != null)
                    {
                        if (index >= current.Index + current.Length)
                        {
                            _currentBlock = current;
                            current = new RegistersBlock((index / RegistersBlock.BLOCK_SIZE) * RegistersBlock.BLOCK_SIZE);
                            current.Left = _currentBlock;
                            current.Right = _currentBlock.Right;
                            current.Left.Right = current;
                            current.Right.Left = current;
                            _currentBlock = current;
                        }
                    }
                }
                else if (index >= current.Index + current.Length)
                {
                    current = current.Right;
                    if (current != null)
                    {
                        if (index < current.Index)
                        {
                            _currentBlock = current;
                            current = new RegistersBlock((index / RegistersBlock.BLOCK_SIZE) * RegistersBlock.BLOCK_SIZE);
                            current.Left = _currentBlock.Left;
                            current.Right = _currentBlock;
                            current.Left.Right = current;
                            current.Right.Left = current;
                            _currentBlock = current;
                        }
                    }
                }
                else
                {
                    _currentBlock = current;
                    current.SetValue(value, (int)(index - current.Index));
                    if (value == 0)
                    {
                        if (current.IsEmpty) RemoveBlock(current);
                    }
                    current = null;
                }
            }
        }

        /// <summary>
        /// Удаляет указанный блок из спика.
        /// </summary>
        /// <param name="block">Удаляемый блок.</param>
        private void RemoveBlock(RegistersBlock block)
        {
            _currentBlock = null;

            bool left = false;
            bool right = false;
            if (block.Left != null)
            {
                _currentBlock = block.Left;
                block.Left.Right = block.Right;
                left = true;
            }
            if (block.Right != null)
            {
                _currentBlock = block.Right;
                block.Right.Left = block.Left;
                right = true;
            }

            if (left && !right)
                _lastBlock = block.Left;
            else if (right && !left)
                _firstBlock = block.Right;
            else if(!right && !left)
            {
                _firstBlock = null;
                _lastBlock = null;
            }

            block.Dispose();
        }

        /// <summary>
        /// Получает значение указанного регистра.
        /// </summary>
        /// <param name="index">Индекс неоходимого регистра.</param>
        /// <returns>Значение указанного регистра.</returns>
        public BigInteger GetValue(BigInteger index)
        {
            RegistersBlock current = _currentBlock;
            while(current != null)
            {
                if (index < current.Index)
                {
                    current = current.Left;
                    if(current != null)
                        if (index >= current.Index + current.Length)
                            current = null;
                }
                else if (index >= current.Index + current.Length)
                {
                    current = current.Right;
                    if(current != null)
                        if (index < current.Index)
                            current = null;
                }
                else
                {
                    _currentBlock = current;
                    return current.GetValue((int)(index - current.Index));
                }
            }

            return 0;
        }
    }
}
