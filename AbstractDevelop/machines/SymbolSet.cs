using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines
{
    /// <summary>
    /// Представляет множество, хранящее символы ограниченного в размерах алфавита.
    /// </summary>
    [Serializable]
    public class SymbolSet : ICloneable
    {
        /// <summary>
        /// Представляет простую ячейку памяти для хранения символа алфавита.
        /// </summary>
        [Serializable]
        private struct Block
        {
            /// <summary>
            /// Хранимый символ.
            /// </summary>
            public char Value;
            /// <summary>
            /// Определяет количество символов в контейнере.
            /// </summary>
            public BigInteger Count;
        }

        private Block[] _chars;

        /// <summary>
        /// Получает максимальное количество символов, которое может содержаться в алфавите множества.
        /// </summary>
        public int AlphabetCapacity { get { return _chars.Length; } }

        /// <summary>
        /// Получает текущее количество символов в алфавите.
        /// </summary>
        public int AlphabetPower { get { return _chars.Count(x => x.Count > 0); } }

        /// <summary>
        /// Вычисляет общее количество символов, находящихся в множестве.
        /// </summary>
        public BigInteger Count
        {
            get
            {
                BigInteger result = 0;
                int n = AlphabetCapacity;
                for(int i = 0; i < n; i++)
                {
                    if (_chars[i].Count > 0)
                        result += _chars[i].Count;
                }
                return result;
            }
        }

        /// <summary>
        /// Возвращает алфавит символов множества.
        /// </summary>
        public char[] Alphabet { get { return Array.ConvertAll<Block, char>(Array.FindAll<Block>(_chars, x => x.Count > 0).ToArray(), x => x.Value); } }

        /// <summary>
        /// Определяет, является ли алфавит полностью заполненным.
        /// </summary>
        public bool IsFull { get { return GetFreeBlock() == -1; } }

        /// <summary>
        /// Создает пустой алфавит.
        /// </summary>
        public SymbolSet()
        {
            _chars = new Block[byte.MaxValue + 1];
        }

        /// <summary>
        /// Удаляет все символы пересечение текущего алфавита и указанного из множества.
        /// </summary>
        /// <param name="alphabet">Алфавит, пересечение с которым рассматривается.</param>
        public void SubtractAlphabet(char[] alphabet)
        {
            if (alphabet == null)
                throw new ArgumentNullException("Алфавит не может быть неопределенным");

            List<char> list = alphabet.ToList();

            int n = _chars.Length;
            int ind;
            for(int i = 0; i < n; i++)
            {
                if (_chars[i].Count == 0) continue;
                ind = list.Find(x => x == _chars[i].Value);
                if (ind == -1) continue;

                _chars[i].Count = 0;
                list.RemoveAt(ind);
            }
        }

        /// <summary>
        /// Инициализирует множество указанными параметрами.
        /// </summary>
        /// <param name="chars">Список неповторяющихся символов алфавита создаваемого множества.</param>
        public SymbolSet(params char[] chars)
        {
            _chars = new Block[byte.MaxValue + 1];

            int n = chars.Length;
            for(int i = 0; i < n; i++)
            {
                AddChar(chars[i]);
            }
        }

        /// <summary>
        /// Стирает все символы из алфавита.
        /// </summary>
        public void Clear()
        {
            Array.ForEach<Block>(_chars, x => x.Count = 0);
        }

        /// <summary>
        /// Добавляет указанный символ в алфавит и возвращает сопоставленный ему код.
        /// </summary>
        /// <param name="value">Добавляемый символ.</param>
        /// <returns>Код, сопоставленный добавленному символу.</returns>
        public int AddChar(char value)
        {
            int i = GetCharCode(value);
            if(i == -1)
            {
                i = GetFreeBlock();
                if (i == -1) throw new Exception("Невозможно добавить символ в множество, поскольку размер алфавита этого множества достиг максимально допустимого значения");
                _chars[i].Value = value;
                _chars[i].Count = 1;
            }
            else
            {
                _chars[i].Count++;
            }

            return i;
        }

        /// <summary>
        /// Производит поиск своодного блока и возвращает его индекс в массиве.
        /// </summary>
        /// <returns>Индекс свободного блока, если он существует, иначе - -1.</returns>
        private int GetFreeBlock()
        {
            return Array.FindIndex<Block>(_chars, x => x.Count == 0);
        }

        /// <summary>
        /// Получает сопоставленный с указанным числом символ.
        /// </summary>
        /// <param name="code">Код символа в алфавите.</param>
        /// <returns>Соответствующий числу символ.</returns>
        public char GetChar(int code)
        {
            if (code < 0 || code >= AlphabetCapacity)
                throw new ArgumentException("Указанный код вышел за пределы возможных значений");

            if (_chars[code].Count == 0)
                throw new ArgumentException("В алфавите не существует символа, закодированного указанным кодом");

            return _chars[code].Value;
        }

        /// <summary>
        /// Удаляет символ по его коду.
        /// </summary>
        /// <param name="code">Код, сопоставленный с символом на множестве.</param>
        public void RemoveCharAt(int code)
        {
            if (code < 0 || code >= AlphabetCapacity)
                throw new ArgumentOutOfRangeException("Указанный код вышел за пределы допустимых значений");

            if (_chars[code].Count == 0)
                throw new ArgumentException("В алфавите не существует символа с указанным кодом");

            _chars[code].Count--;
        }

        /// <summary>
        /// Получает код, сопоставленный указанному символу.
        /// Если символ не существует - -1.
        /// </summary>
        /// <param name="value">Символ.</param>
        public int GetCharCode(char value)
        {
            int n = _chars.Length;
            for (int i = 0; i < n; i++)
            {
                if (_chars[i].Value == value)
                {
                    if (_chars[i].Count > 0)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Удаляет указанный символ из алфавита.
        /// </summary>
        /// <param name="value">Удаляемый символ.</param>
        public void RemoveChar(char value)
        {
            int i = GetCharCode(value);
            if (i == -1)
                throw new ArgumentException("Указанного символа не существует в алфавите");

            _chars[i].Count--;
        }

        /// <summary>
        /// Определяет, существует ли указанный символ в алфавите.
        /// </summary>
        /// <param name="value">Проверяемый символ.</param>
        /// <returns>Истина, если символ существует, иначе - ложь.</returns>
        public bool CharExists(char value)
        {
            return GetCharCode(value) != -1;
        }

        public override string ToString()
        {
            string str = "";

            int n = _chars.Length;
            for (int i = 0; i < n; i++ )
            {
                if(_chars[i].Count > 0)
                    str += " " + _chars[i].Value.ToString() + ",";
            }
            if(str.Length > 0)
                str = str.Remove(str.Length - 1, 1) + " ";

            return string.Format("Alphabet: [{0}]", str);
        }

        public object Clone()
        {
            return new SymbolSet() { _chars = this._chars };
        }
    }
}
