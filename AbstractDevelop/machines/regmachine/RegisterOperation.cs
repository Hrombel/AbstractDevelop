using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет элементарную операцию паралельной машины с бесконечными регистрами.
    /// </summary>
    public class RegisterOperation : Operation
    {
        public RegisterOperation(RegisterOperationId id, BigInteger[] args) : base((byte)id, 
                                                                    (args.ToList().ConvertAll<object>
                                                                                        (
                                                                                              x => (object)x
                                                                                        )
                                                                     ).ToArray()) { }

        /// <summary>
        /// Получает уникальный идентификатор операции.
        /// </summary>
        public RegisterOperationId Id
        {
            get { return (RegisterOperationId)id; }
        }

        /// <summary>
        /// Получает аргументы операции.
        /// </summary>
        public BigInteger[] Arguments
        {
            get { return Args.ToList().ConvertAll<BigInteger>(x => (BigInteger)x).ToArray(); }
        }
    }
}
