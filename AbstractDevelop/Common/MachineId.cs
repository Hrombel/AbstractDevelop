namespace AbstractDevelop
{
    /// <summary>
    /// Кодирует уникальные идентификаторы абстрактных вычислителей.
    /// </summary>
    public enum MachineId : byte
    {
        /// <summary>
        /// Машина Поста.
        /// </summary>
        Post,

        /// <summary>
        /// Машина Тьюринга(включая многоленточную).
        /// </summary>
        Turing,

        /// <summary>
        /// Машина с бесконечными регистрами(включая параллельную).
        /// </summary>
        Register,

        /// <summary>
        /// Машина RISC
        /// </summary>
        Risc
    }
}