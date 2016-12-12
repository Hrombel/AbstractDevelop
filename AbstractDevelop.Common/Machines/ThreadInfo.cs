using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines
{
    /// <summary>
    /// Представляет базовую информацию о потоке, выполняемом в абстрактном вычислителе.
    /// </summary>
    public class ThreadInfo : IComparable<ThreadInfo>, IComparable
    {
        private int _id;
        private string _program;
        private int _command;

        /// <summary>
        /// Инициализирует экземпляр указанными параметрами.
        /// </summary>
        /// <param name="id">Уникальный идентификатор потока.</param>
        /// <param name="program">Название программы, выполняемой потоком.</param>
        /// <param name="command">Номер выполняемой команды.</param>
        public ThreadInfo(int id, string program, int command)
        {
            if (id < 0) throw new ArgumentOutOfRangeException("Уникальный идентификатор не может быть отрицательным");
            if (string.IsNullOrWhiteSpace(program)) throw new ArgumentException("Неверный формат названия выполняемой программы");

            _id = id;
            _program = program;
            Command = command;
        }
        /// <summary>
        /// Получает уникальный идентификатор потока.
        /// </summary>
        public int Id { get { return _id; } }
        /// <summary>
        /// Получает номер программы, выполняемой потоком.
        /// </summary>
        public string Program { get { return _program; } }
        /// <summary>
        /// Получает или задает номер команды, выполняемой потоком.
        /// </summary>
        public int Command
        {
            get{ return _command; }
            set
            {
                if (value < 0)
                    throw new Exception("Значение выполняемой программы вышло за пределы допустимых значений");
                _command = value;
            }
        }

        public int CompareTo(ThreadInfo other)
        {
            return _id.CompareTo(other._id);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as ThreadInfo);
        }
    }
}
