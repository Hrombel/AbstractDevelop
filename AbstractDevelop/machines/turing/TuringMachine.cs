using AbstractDevelop.machines.tape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    [Serializable]
    /// <summary>
    /// Представляет модель машины Тьюринга.
    /// </summary>
    public class TuringMachine : IAbstractMachine, ISerializable
    {
        /// <summary>
        /// Возникает после изменения состояния одной из лент машины Тьюринга.
        /// </summary>
        public event EventHandler TapeUpdated;
        /// <summary>
        /// Возникает после остановки машины Тьюринга.
        /// </summary>
        public event EventHandler<TuringMachineStopEventArgs> OnMachineStopped;
        /// <summary>
        /// Возникает после смены состояния машины Тьюринга.
        /// </summary>
        public event EventHandler<TuringMachineStateChangedEventArgs> OnMachineStateChanged;
        
        private TuringTapes _tapes;
        [NonSerialized]
        private TuringState[] _states;
        [NonSerialized]
        private int _stateIndex; // Индекс состояния в массиве состояний.

        private SymbolSet _symbols;

        /// <summary>
        /// Инициализирует машину Тьюринга единственной пустой лентой.
        /// </summary>
        public TuringMachine()
        {
            _tapes = new TuringTapes(1);
            _stateIndex = -1;
            _states = null;
            _symbols = new SymbolSet();

            _tapes.TapeUpdated += _tapes_TapeUpdated;
        }
        ~TuringMachine()
        {
            _tapes.TapeUpdated -= _tapes_TapeUpdated;
        }

        private TuringMachine(SerializationInfo info, StreamingContext context)
        {
            _tapes = info.GetValue("t", typeof(TuringTapes)) as TuringTapes;
            _symbols = info.GetValue("s", typeof(SymbolSet)) as SymbolSet;

            _tapes.TapeUpdated += _tapes_TapeUpdated;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("t", _tapes, typeof(TuringTapes));
            info.AddValue("s", _symbols, typeof(SymbolSet));
        }

        private void _tapes_TapeUpdated(object sender, EventArgs e)
        {
            if (TapeUpdated != null)
                TapeUpdated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Вызывается сразу после десериализации.
        /// </summary>
        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            _stateIndex = -1;
            _states = null;
            _symbols = new SymbolSet();
        }

        /// <summary>
        /// Получает или задает символ, определяющий пустую ячейку ленты. 
        /// </summary>
        public char DefaultChar
        {
            get { return _tapes.DefaultChar; }
            set { _tapes.DefaultChar = value; }
        }

        /// <summary>
        /// Получает индекс текущей комбинации перехода в списке всех комбинаций.
        /// </summary>
        /// <returns>Индекс комбинации перехода. Если не найдено - -1.</returns>
        public int GetConversionIndex()
        {
            if (_stateIndex == -1)
                throw new InvalidOperationException("Машина не запущена");

            int res = FindConvertIndex() + 1;
            if (res == -1) return -1;

            for(int i = 0; i < _stateIndex; i++)
                res += _states[i].Converts.Length;

            return res;
        }

        /// <summary>
        /// Инициализирует машину Тьюринга заданным количеством лент.
        /// </summary>
        /// <param name="tapesCount">Количество создаваемых лент.</param>
        public TuringMachine(int tapesCount)
        {
            if (tapesCount < 1)
                throw new ArgumentException("Количество лент должно быть положительным");

            _tapes = new TuringTapes(tapesCount);
            _stateIndex = -1;
            _states = null;
            _symbols = new SymbolSet();
        }

        /// <summary>
        /// Получает совокупность лент МТ.
        /// </summary>
        public TuringTapes Tapes { get { return _tapes; } }

        /// <summary>
        /// Получает или задает количество лент машины Тьюринга.
        /// </summary>
        public int TapesCount 
        {
            get { return _tapes.Count; }
            set 
            {
                _tapes.Count = value;
            }
        }

        /// <summary>
        /// Проверяет, совпадают ли текущие символы напротив читающих/пишущих головок с входными символами перехода.
        /// </summary>
        /// <param name="convert">Проверяемый переход машины Тьюринга.</param>
        /// <returns>Истина, если символы совпадают, иначе - ложь.</returns>
        private bool CheckTapeConfig(TuringConvert convert)
        {
            char[] current = _tapes.GetValues();
            int n = current.Length;
            for(int i = 0; i < n; i++)
            {
                if(convert.Input[i] != current[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Выполняет поиск индекса подходящего перехода для текущей конфигурации машины и возвращает его.
        /// Если переход не найден - возвращается -1.
        /// </summary>
        private int FindConvertIndex()
        {
            int n = _states[_stateIndex].Converts.Length;
            for (int i = 0; i < n; i++)
            {
                if (CheckTapeConfig(_states[_stateIndex].Converts[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Выполняет поиск подходящего перехода для текущей конфигурации машины и возвращает его.
        /// Если переход не найден - возвращается null.
        /// </summary>
        private TuringConvert FindConvert()
        {
            int i = FindConvertIndex();

            return i == -1 ? null : _states[_stateIndex].Converts[i];
        }

        /// <summary>
        /// Выполняет переход к новой конфигурации машины Тьюринга.
        /// </summary>
        /// <param name="convert">Переход.</param>
        private void MakeConversion(TuringConvert convert)
        {
            _tapes.SetValues(convert.Output);
            _tapes.MovePens(convert.Directions);
            _stateIndex = GetStateIndex(convert.OutId);

            if (OnMachineStateChanged != null)
                OnMachineStateChanged(this, new TuringMachineStateChangedEventArgs(convert.Directions));
        }

        /// <summary>
        /// Получает индекс состояния в массиве по его номеру.
        /// </summary>
        /// <param name="id">Номер искомого состояния.</param>
        /// <returns>Индекс состояния в массиве. Если состояния не существует - -1.</returns>
        private int GetStateIndex(int id)
        {
            return Array.FindIndex<TuringState>(_states, x => x.Id == id);
        }

        /// <summary>
        /// Выполняет преобразование операций для машины Тьюринга в состояния.
        /// </summary>
        /// <param name="ops">Список неповторяющихся операций для машины Тьюринга.</param>
        /// <returns>Список неповторяющихся состояний.</returns>
        private List<TuringState> GetStates(List<TuringOperation> ops)
        {
            List<TuringState> result = new List<TuringState>();

            int n = ops.Count;
            for (int i = 0; i < n; i++ )
            {
                result.Add(ops[i].State);
            }

            return result;
        }

        /// <summary>
        /// Запускает выполнение операций без задержек между ними.
        /// </summary>
        /// <param name="ops">Список операций для машины Тьюринга.</param>
        public void Start(List<Operation> ops)
        {
            if (ops == null)
                throw new ArgumentNullException("Список операций не может быть неопределенным");
            if (ops.Count == 0)
                throw new ArgumentException("Количество операций для машины Тьюринга не может быть равным нулю");

            _symbols.Clear();

            _states = GetStates(ops.Cast<TuringOperation>().ToList<TuringOperation>()).ToArray();

            _stateIndex = 0;
            while (_stateIndex != -1) Forward();
        }

        /// <summary>
        /// Проверяет состояния на корректность. Если состояния не прошли проверку, генерируется исключение.
        /// </summary>
        /// <param name="states">Список проверяемых состояний.</param>
        private void StatesOK(TuringState[] states)
        {
            if (states[0].Converts[0].Input.Length != _tapes.Count)
                throw new Exception("Описанные переходы для состояний должны соответствовать текущему количеству лент машины Тьюринга");
        }

        /// <summary>
        /// Переводит машину Тьюринга в режим пошагового выполнения операций.
        /// </summary>
        /// <param name="ops">Список операций для машины Тьюринга.</param>
        public void StartManual(List<Operation> ops)
        {
            if (ops == null)
                throw new ArgumentNullException("Список операций не может быть неопределенным");
            if (ops.Count == 0)
                throw new ArgumentException("Количество операций для машины Тьюринга не может быть равным нулю");

            _states = GetStates(ops.Cast<TuringOperation>().ToList<TuringOperation>()).ToArray();

            try
            {
                StatesOK(_states);
            }
            catch (Exception e)
            {
                _states = null;
                throw new Exception(string.Format("Ошибка запуска работы машины Тьюринга: \"{0}\"", e.Message), e);
            }

            _stateIndex = 0;
        }

        /// <summary>
        /// Выполняет следующую операцию из текущего списка операций.
        /// </summary>
        /// <returns>Истина, если операция выполнена успешно и не вызвала остановку машины, 
        /// иначе - был произведен переход в несуществующее состояние или не найден соответствующий переход МТ при текущей конфигурации.</returns>
        public bool Forward()
        {
            if(_stateIndex == -1) return false;

            TuringConvert convert = FindConvert();
            if (convert != null)
            {
                MakeConversion(convert);
                if (_stateIndex == -1)
                {
                    Stop(TuringMachineStopReason.UndefinedState);
                    return false;
                }
                return true;
            }
            else
            {
                Stop(TuringMachineStopReason.UndefinedConversion);
                return false;
            }
        }

        /// <summary>
        /// Останавливает работу машины Тьюринга.
        /// </summary>
        public void Stop()
        {
            Stop(TuringMachineStopReason.UserInterrupt);
        }

        /// <summary>
        /// Останавливает работу машины Тьюринга с указанием причины останова.
        /// </summary>
        /// <param name="reason">Причина останова машины Тьюринга.</param>
        private void Stop(TuringMachineStopReason reason)
        {
            _states = null;
            _stateIndex = -1;
            if (OnMachineStopped != null)
                OnMachineStopped(this, new TuringMachineStopEventArgs(reason));
        }
    }
}
