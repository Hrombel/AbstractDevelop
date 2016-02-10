using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines
{
    /// <summary>
    /// Представляет основу операций абстрактного вычислителя.
    /// Любая операция должна наследовать этот класс.
    /// </summary>
    public abstract class Operation
    {
        private byte _id;
        private object[] _args;

        /// <summary>
        /// Инициализирует операцию указанными параметрами.
        /// </summary>
        /// <param name="id">Уникальный идентификатор операции.</param>
        /// <param name="args">Аргументы операции.</param>
        public Operation(byte id, object[] args)
        {
            _id = id;
            _args = args;
        }

        /// <summary>
        /// Получает уникальный идентификатор операции среди операций конкретной машины.
        /// </summary>
        protected byte id
        {
            get { return _id; }
        }

        /// <summary>
        /// Получает аргументы операции.
        /// </summary>
        protected object[] Args
        {
            get { return _args; }
        }

    }
}
