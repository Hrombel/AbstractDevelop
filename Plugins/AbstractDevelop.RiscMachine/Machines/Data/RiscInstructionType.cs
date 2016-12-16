namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Перечисление типов инструкций RISC
    /// </summary>
    public enum RiscInstructionCode
    {
        /// <summary>
        /// Неизвестная команда
        /// </summary>
        Unknown,

        /// <summary>
        /// Чтение ввода
        /// </summary>
        /// <remarks>Соотвествует команде 'in'</remarks>
        ReadInput,

        /// <summary>
        /// Вывод байта в порт вывода
        /// </summary>
        /// <remarks>Соотвествует команде 'out'</remarks>
        WriteOutput,

        /// <summary>
        /// Побитовый сдвиг вправо
        /// </summary>
        /// <remarks>Соотвествует команде 'shr'</remarks>
        ShiftRight,

        /// <summary>
        /// Побитовый сдвиг влево
        /// </summary>
        /// <remarks>Соотвествует команде 'shl'</remarks>
        ShiftLeft,

        /// <summary>
        /// Побитовое НЕ
        /// </summary>
        /// <remarks>Соотвествует команде 'not'</remarks>
        Not,

        /// <summary>
        /// Побитовое ИЛИ
        /// </summary>
        /// <remarks>Соотвествует команде 'or'</remarks>
        Or,

        /// <summary>
        /// Побитовое И
        /// </summary>
        /// <remarks>Соотвествует команде 'and'</remarks>
        And,

        /// <summary>
        /// Побитовое НЕ-ИЛИ
        /// </summary>
        /// <remarks>Соотвествует команде 'nor'</remarks>
        NotOr,

        /// <summary>
        /// Побитовое НЕ-И
        /// </summary>
        /// <remarks>Соотвествует команде 'nand'</remarks>
        NotAnd,

        /// <summary>
        /// Побитовое ИЛИ-НЕ
        /// </summary>
        /// <remarks>Соотвествует команде 'xor'</remarks>
        Xor,

        /// <summary>
        /// Сложение операндов
        /// </summary>
        /// <remarks>Соотвествует команде 'add'</remarks>
        Addition,

        /// <summary>
        /// Вычитание операндов
        /// </summary>
        /// <remarks>Соотвествует команде 'sub'</remarks>
        Subtraction,

        /// <summary>
        /// Переход по метке, если операнд состоит из 1
        /// </summary>
        /// <remarks>Соотвествует команде 'jo'</remarks>
        JumpIfTrue,

        /// <summary>
        /// Переход по метке, если операнд состоит из 0
        /// </summary>
        /// <remarks>Соотвествует команде 'jz'</remarks>
        JumpIfFalse,

        /// <summary>
        /// Вызов подпроцедуры
        /// </summary>
        /// <remarks>Соотвествует команде 'call'</remarks>
        Call,

        /// <summary>
        /// Возврат управления
        /// </summary>
        /// <remarks>Соотвествует команде 'ret'</remarks>
        Return
    }
}
