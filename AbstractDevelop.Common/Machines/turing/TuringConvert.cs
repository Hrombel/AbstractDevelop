using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет описание перехода машины Тьюринга к новому состоянию.
    /// </summary>
    public class TuringConvert
    {
        private char[] _input;
        private char[] _output;
        private TuringPenDir[] _directions;
        private int _outId;

        /// <summary>
        /// Инициализирует переход машины Тьюринга.
        /// </summary>
        /// <param name="input">Символы, находящиеся в текущих ячейках лент.</param>
        /// <param name="output">Заменяющие символы.</param>
        /// <param name="outId">Номер состояния, к которому перейдет машина Тьюринга.</param>
        public TuringConvert(char[] input, char[] output, TuringPenDir[] directions, int outId)
        {
            if (input == null || output == null)
                throw new ArgumentNullException("Символы не могут быть неопределенными");
            if (directions == null)
                throw new ArgumentNullException("Направления не могут быть неопределенными");
            if(input.Length == 0 || output.Length == 0)
                throw new ArgumentException("Массивы символов не могут быть пустыми");
            if (input.Length != output.Length)
                throw new ArgumentException("Массивы входных и выходных символов должны иметь одинаковый размер");
            if (directions.Length != output.Length)
                throw new ArgumentNullException("Массив направлений по размеру должен совпадать с количеством символов");
            if (outId < 0)
                throw new ArgumentException("Номер выходного состояния должен быть неотрицательным");

            _input = input.Clone() as char[];
            _output = output.Clone() as char[];
            _directions = directions.Clone() as TuringPenDir[];
            _outId = outId;
        }

        /// <summary>
        /// Получает копию массива входных символов перехода.
        /// </summary>
        public char[] Input { get { return _input.Clone() as char[]; } }

        /// <summary>
        /// Получает копию массива выходных символов перехода.
        /// </summary>
        public char[] Output { get { return _output.Clone() as char[]; } }

        /// <summary>
        /// Получает копию массива направлений движения читающих/пишущих головок.
        /// </summary>
        public TuringPenDir[] Directions { get { return _directions.Clone() as TuringPenDir[]; } }

        /// <summary>
        /// Получает номер выходного состояния.
        /// </summary>
        public int OutId { get { return _outId; } }
    }
}
