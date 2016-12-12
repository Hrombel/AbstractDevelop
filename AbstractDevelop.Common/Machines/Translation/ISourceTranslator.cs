using System.Text;
using System.Collections.Generic;

using static AbstractDevelop.Machines.AbstractMachine;

namespace AbstractDevelop.Machines
{
    public interface ISourceTranslator<OperationType, OperationCode, ArgumentType>
        where OperationType : Operation<OperationCode, ArgumentType>
    {
        Encoding SupportedEncoding { get; }

        //TODO: добавить функцию проверки исходного кода на наличие ошибок

        /// <summary>
        /// Транслирует исходный код в набор инструкций определенного типа
        /// </summary>
        /// <param name="input">Набор строк исходного кода</param>
        /// <returns>Набор инструкций, представленный в указанном формате</returns>
        IEnumerable<OperationType> Translate(IEnumerable<string> input, AbstractMachine<OperationType, OperationCode, ArgumentType>.);
    }
}
