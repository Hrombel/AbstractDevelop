using System.Collections.Generic;

namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Представляет основу операций абстрактного вычислителя.
    /// Любая операция должна наследовать этот класс.
    /// </summary>
    /// <typeparam name="OperationType">Тип, идентифицирующий операции абстрактного вычислителя</typeparam>
    /// <typeparam name="ArgumentType">Тип аргумента операции</typeparam>
    public abstract class Operation<OperationType, ArgumentType>
    {
        #region [Свойства]

        /// <summary>
        /// Идентификатор типа операции
        /// </summary>
        public virtual OperationType Id { get; protected set; }

        /// <summary>
        /// Аргументы данной операции
        /// </summary>
        public virtual IEnumerable<ArgumentType> Args { get; protected set; }

        #endregion

        /// <summary>
        /// Конструктор по умолчанию для типа операции
        /// </summary>
        /// <param name="id">Идентификатор создаваемого экземпляра операции</param>
        /// <param name="args">Аргументы создаваемого экземпляра операции</param>
        protected Operation(OperationType id, IEnumerable<ArgumentType> args)
        {
            Id = id;
            Args = args;
        }
    }
}