using AbstractDevelop.Machines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет элементарную операцию машины Тьюринга.
    /// </summary>
    public class TuringOperation : Operation<TuringOperationId, TuringState>
    {
        /// <summary>
        /// Инициализирует операцию указанными параметрами.
        /// </summary>
        /// <param name="id">Уникальный идентификатор операции машины Тьюринга.</param>
        /// <param name="args">Аргументы операции.</param>
        //public TuringOperation(TuringOperationId id, TuringState args) : base((byte)id, new object[1])
        //{
        //    Args[0] = args as object;
        //}

        /// <summary>
        /// Получает уникальный идентификатор операции.
        /// </summary>
        public TuringOperationId Id
        {
            get { return (TuringOperationId)Id; }
        }

        /// <summary>
        /// Получает состояние, переданное в качестве аргумента операции.
        /// </summary>
        public TuringState State
        {
            get { return Args[0] as TuringState; }
        }
    }
}
