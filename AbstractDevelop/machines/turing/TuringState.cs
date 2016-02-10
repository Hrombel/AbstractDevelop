using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет состояние машины Тьюринга.
    /// </summary>
    public class TuringState : IComparable<TuringState>, IComparable
    {
        private int _id;
        private TuringConvert[] _converts;

        /// <summary>
        /// Инициализирует состояние машины Тьюринга.
        /// </summary>
        /// <param name="id">Номер состояния машины Тьюринга.</param>
        /// <param name="converts">Массив всех возможных переходов из этого состояния.</param>
        public TuringState(int id, TuringConvert[] converts)
        {
            if (id < 0)
                throw new ArgumentException("Номер состояния должен быть неотрицательным");
            if (converts == null)
                throw new ArgumentNullException("Массив переходов не может быть неопределенным");
            if (converts.Length == 0)
                throw new ArgumentException("Массив переходов не может быть пустым");

            _id = id;
            _converts = converts;
        }

        /// <summary>
        /// Получает номер состояния машины Тьюринга.
        /// </summary>
        public int Id { get { return _id; } }

        /// <summary>
        /// Получает массив переходов состояния.
        /// </summary>
        public TuringConvert[] Converts { get { return _converts; } }

        public int CompareTo(TuringState other)
        {
            return _id.CompareTo(other._id);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as TuringConvert);
        }
    }
}
