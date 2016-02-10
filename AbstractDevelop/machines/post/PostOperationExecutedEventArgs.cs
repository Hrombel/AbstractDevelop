using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.post
{
    /// <summary>
    /// Представляет аргументы события, возникающего при выполнении любой операции машиной Поста.
    /// </summary>
    public class PostOperationExecutedEventArgs : EventArgs
    {
        private PostOperationId _op;

        /// <summary>
        /// Инициализирует аргументы указанной выполненной операцией.
        /// </summary>
        /// <param name="op">Выполненная операция.</param>
        public PostOperationExecutedEventArgs(PostOperationId op)
        {
            _op = op;
        }

        /// <summary>
        /// Получает идентификатор операции, выполненной машиной Поста.
        /// </summary>
        public PostOperationId Operation
        {
            get
            {
                return _op;
            }
        }
    }
}
