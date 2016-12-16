namespace AbstractDevelop.Modules.Registers
{
    /// <summary>
    /// Описывает регистр архитектуры RISC
    /// </summary>
    public interface IRegister
    {
        /// <summary>
        /// Индекс регистра в системе учета
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Идентификатор регистра (название)
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Значение, хранящаеся в регистре
        /// </summary>
        int Value { get; set; }
    }
}
