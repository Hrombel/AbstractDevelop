using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Machines
{
    public interface ISourceTranslator<OperationType, OperationCode, ArgumentType>
        where OperationType : Operation<OperationCode, ArgumentType>
    {
        IEnumerable<OperationType> Translate(IEnumerable<string> input);

    }
}
