using System;
using RegisterOperation = AbstractDevelop.Machines.Operation<AbstractDevelop.machines.regmachine.RegisterOperationId, System.Numerics.BigInteger>;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет программный модуль для параллельной машины с бесконечными регистрами, выполняемый в отдельно
    /// выделенном потоке.
    /// </summary>
    public class RegisterProgram : IComparable<RegisterProgram>, IComparable
    {
        private RegisterOperation[] _ops;
        private int _id;
        private string _name;
        private bool _isEntry;

        /// <summary>
        /// Инициализирует программу указанными параметрами.
        /// </summary>
        /// <param name="ops">Массив операций для выполнения.</param>
        /// <param name="id">Уникальный идентификатор программы.</param>
        /// <param name="name">Название программы.</param>
        /// <param name="isEntry">Определяет, является ли программа входной точкой.</param>
        public RegisterProgram(RegisterOperation[] ops, int id, string name, bool isEntry = false)
        {
            if (ops == null)
                throw new ArgumentNullException("Список операций программы не может быть неопределенным");
            if (id < 0)
                throw new ArgumentException("Уникальный идентификатор программы не может быть отрицательным");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название программы имеет неверный формат");

            _ops = ops;
            _id = id;
            _name = name;
            _isEntry = isEntry;
        }

        /// <summary>
        /// Определяет, является ли программа входной точкой.
        /// </summary>
        public bool IsEntry { get { return _isEntry; } }

        /// <summary>
        /// Получает массив операций программы.
        /// </summary>
        public RegisterOperation[] Operations { get { return _ops; } }

        /// <summary>
        /// Получает уникальный идентификатор программы.
        /// </summary>
        public int Id { get { return _id; } }

        /// <summary>
        /// Получает название программы.
        /// </summary>
        public string Name { get { return _name; } }

        public int CompareTo(RegisterProgram other)
        {
            return _id.CompareTo(other._id);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as RegisterProgram);
        }
    }
}