using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.post
{
    /// <summary>
    /// Представляет элементарную операцию машины Поста.
    /// </summary>
    public class PostOperation : Operation
    {
        /// <summary>
        /// Инициализирует операцию указанными параметрами.
        /// </summary>
        /// <param name="id">Уникальный идентификатор операции машины Поста.</param>
        /// <param name="args">Аргументы операции.</param>
        public PostOperation(PostOperationId id, int[] args) : base((byte)id, 
                                                                    (args.ToList().ConvertAll<object>
                                                                                        (
                                                                                              x => (object)x
                                                                                        )
                                                                     ).ToArray()) { }

        /// <summary>
        /// Получает уникальный идентификатор операции.
        /// </summary>
        public PostOperationId Id
        {
            get { return (PostOperationId)id; }
        }

        /// <summary>
        /// Получает аргументы операции.
        /// </summary>
        public int[] Arguments
        {
            get { return Args.ToList().ConvertAll<int>(x => (int)x).ToArray(); }
        }
    }
}
