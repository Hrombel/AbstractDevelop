using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет совокупность модулей лент, поддерживающих более двух состояний, имеющих отдельные ЧГ и
    /// работающих на одном внешнем алфавите.
    /// </summary>
    [Serializable]
    public class TuringTapes :
        ISerializable
    {
        #region [События]

        /// <summary>
        /// Возникает после обновления состояния одной из лент.
        /// </summary>
        public event EventHandler TapeUpdated;

        #endregion

        #region [Свойства]

        /// <summary>
        /// Получает алфавит символов, ипользуемых в лентах.
        /// </summary>
        public char[] Alphabet => _symbols.Alphabet;
       
        /// <summary>
        /// Получает или задает текущее количество ленточных модулей.
        /// Если задано число, большее текущего количества модулей, новые модули добавляются в конец списка, если меньшее -
        /// удаляются с конца списка.
        /// </summary>
        public int Count
        {
            get { return _units.Count; }
            set
            {
                if (value < 0)
                    throw new Exception("Количество модулей не может быть отрицательным");

                if (value > _units.Count)
                {
                    int n = value - Count;
                    TuringUnit unit;
                    for (int i = 0; i < n; i++)
                    {
                        unit = new TuringUnit();
                        unit.Tape.Update += Tape_Update;
                        _units.Add(unit);
                    }
                }
                else if (value < _units.Count)
                {
                    int m = Count - 1;
                    int v = value - 1;
                    while (m != v)
                    {
                        _units[m].Tape.Update -= Tape_Update;
                        _units.RemoveAt(m);
                        m--;
                    }
                }
            }
        }

        /// <summary>
        /// Получает или задает символ, определяющий пустую ячейку ленты.
        /// </summary>
        public char DefaultChar
        {
            get { return _defaultChar; }
            set
            {
                if (char.IsWhiteSpace(value))
                    throw new Exception("Символ-разделитель не может использоваться по умолчанию");

                _defaultChar = value;
            }
        }

        /// <summary>
        /// Получает общее множество символов модулей лент.
        /// </summary>
        public SymbolSet SymbolSet { get { return _symbols; } }

        /// <summary>
        /// Получает совокупность модулей лент.
        /// </summary>
        public List<TuringUnit> Units { get { return _units; } }

        #endregion

        #region [Поля]

        private char _defaultChar = '~';
        private SymbolSet _symbols;
        private List<TuringUnit> _units;

        #endregion

        #region [Методы]

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            int n = _units.Count;
            info.AddValue("n", n);

            for (int i = 0; i < n; i++)
                info.AddValue("u" + i.ToString(), _units[i], typeof(TuringUnit));

            info.AddValue("s", _symbols, typeof(SymbolSet));
            info.AddValue("c", _defaultChar, typeof(char));
        }

        /// <summary>
        /// Получает символы текущих ячеек лент.
        /// </summary>
        /// <returns>Массив символов, находящихся в текущих ячейках.</returns>
        public char[] GetValues()
        {
            int n = Count;
            char[] result = new char[n];

            int code;
            for (int i = 0; i < n; i++)
            {
                code = _units[i].Tape[_units[i].Position];
                if (code != 0)
                    result[i] = _symbols.GetChar(code - 1);
                else
                    result[i] = _defaultChar;
            }

            return result;
        }

        /// <summary>
        /// Передвигает читающие/пишущие головки в указанных направлениях.
        /// </summary>
        /// <param name="directions">Массив направлений головок.</param>
        public void MovePens(TuringPenDir[] directions)
        {
            if (directions == null)
                throw new ArgumentNullException("Массив направлений ячеек не может быть неопределенным");
            if (directions.Length != Count)
                throw new ArgumentException("Количество направлений и количество лент должны быть одинаковыми");

            int n = directions.Length;
            for (int i = 0; i < n; i++)
            {
                switch (directions[i])
                {
                    case TuringPenDir.Left:
                        _units[i].Position--;
                        break;

                    case TuringPenDir.Right:
                        _units[i].Position++;
                        break;
                }
            }
        }

        /// <summary>
        /// Записывает указанные символы в ячейки лент, напротив которых находятся читающие/пишущие головки.
        /// </summary>
        /// <param name="vals">Записываемые значения.</param>
        public void SetValues(char[] vals)
        {
            if (vals == null)
                throw new ArgumentNullException("Массив записываемых значений не может быть неопределенным");
            if (vals.Length != Count)
                throw new ArgumentException("Количество записываемых значений должно быть равным количеству лент");

            int n = vals.Length;
            for (int i = 0; i < n; i++)
            {
                if (vals[i] != _defaultChar)
                {
                    _symbols.AddChar(vals[i]);
                    _units[i].Tape[_units[i].Position] = (char)(_symbols.GetCharCode(vals[i]) + 1);
                }
                else
                    _units[i].Tape[_units[i].Position] = (char)0;
            }
        }

        private void Tape_Update(object sender, EventArgs e)
        {
            if (TapeUpdated != null)
                TapeUpdated(this, EventArgs.Empty);
        }

        #endregion

        #region [Конструкторы]

        /// <summary>
        /// Инициализирует экземпляр указанным количеством модулей лент.
        /// </summary>
        /// <param name="count">Кодичество создаваемых модулей.</param>
        public TuringTapes(int count)
        {
            _units = new List<TuringUnit>();
            _symbols = new SymbolSet();

            Count = count;
        }

        private TuringTapes(SerializationInfo info, StreamingContext context)
        {
            int n = info.GetInt32("n");
            _units = new List<TuringUnit>();

            for (int i = 0; i < n; i++)
                _units.Add(info.GetValue("u" + i.ToString(), typeof(TuringUnit)) as TuringUnit);

            _symbols = info.GetValue("s", typeof(SymbolSet)) as SymbolSet;
            _defaultChar = info.GetChar("c");

            _units.ForEach(x => x.Tape.Update += Tape_Update);
        }

        #endregion
    }
}