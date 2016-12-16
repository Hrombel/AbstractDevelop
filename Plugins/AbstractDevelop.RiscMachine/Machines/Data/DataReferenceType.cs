namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Перечисление типов операндов для инструкций RISC
    /// </summary>
    public enum DataReferenceType : int
    {
        /// <summary>
        /// Операнд задан ссылкой на метку
        /// </summary>
        Label,

        /// <summary>
        /// Операнд задан своим значением
        /// </summary>
        Value,

        /// <summary>
        /// Операнд задан содержимым регистра, на который он указывает
        /// </summary>
        Register,

        /// <summary>
        /// Операнд задан значением ячейки памяти, на которую он указывает
        /// </summary>
        Memory
    }
}
