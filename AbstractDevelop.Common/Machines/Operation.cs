namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Представляет основу операций абстрактного вычислителя.
    /// Любая операция должна наследовать этот класс.
    /// </summary>
    /// <typeparam name="OperationCode">Тип, идентифицирующий операции абстрактного вычислителя</typeparam>
    /// <typeparam name="ArgumentType">Тип аргумента операции</typeparam>
    public abstract class Operation<OperationCode, ArgumentType>
    {
        #region [Фиктивная реализация класса операции (auto-implementation)]

        /// <summary>
        /// Фиктивный класс операции, используемый для создания объектов типа 
        /// <see cref="T:Operation<OperationType, ArgumentType>"/> напрямую, без использования
        /// сторонних оболочек
        /// </summary>
        sealed class FakeOperation : Operation<OperationCode, ArgumentType> { }
                    
        /// <summary>
        /// Создает экземпляр операции с указанным идетификатором и набором аргументов
        /// </summary>
        /// <param name="id">Идентификатор операции</param>
        /// <param name="arg">Набор аргументов операции</param>
        /// <returns></returns>
        public static Operation<OperationCode, ArgumentType> Create(OperationCode id = default(OperationCode), params ArgumentType[] arg) 
            => new FakeOperation() { Id = id, Args = arg };

        #endregion

        #region [Свойства]

        /// <summary>
        /// Идентификатор типа операции
        /// </summary>
        public virtual OperationCode Id { get; protected set; }

        /// <summary>
        /// Аргументы данной операции
        /// </summary>
        public virtual ArgumentType[] Args { get; protected set; }

        #endregion

        #region [Методы]

        // TODO: переработать список методов класса <Operation>

        public override string ToString()
            => $"Operation of {typeof(OperationCode).Name} with args of {typeof(ArgumentType).Name} (count = {Args.Length})";

        #endregion

    }
}