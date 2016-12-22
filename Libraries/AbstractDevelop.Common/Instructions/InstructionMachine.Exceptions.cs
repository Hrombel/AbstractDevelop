using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Machines
{
    public abstract partial class InstructionMachine<InstructionCode, ArgumentType> : AbstractMachine
        where InstructionCode : struct, IComparable
    {
        /// <summary>
        /// Набор исключений при работе машины, основанной на выполнении инструкций
        /// </summary>
        public static class Exceptions
        {
            public interface ILineException
            {
                int Line { get; }
            }

            /// <summary>
            /// Ошибка отсутствия аргумента 
            /// </summary>
            public class MissingArgumentException :
                Exception, ILineException
            {
                public IArgumentDefinition Argument { get; }

                public int Index { get; }

                public string InstrctionName { get; }

                public int Line { get; }

                public MissingArgumentException(IArgumentDefinition argument, int index, string instructionName, int line) :
                    base(Translate.Key(nameof(MissingArgumentException), format: Translate.Format(index, instructionName)))
                {
                    Argument = argument;
                    Index = index;
                    InstrctionName = instructionName;
                    Line = line;
                }
            }

            /// <summary>
            /// Ошибка недопустимого значения аргумента
            /// </summary>
            public class InvalidArgumentException :
                Exception, ILineException
            {
                public IArgumentDefinition Argument { get; }

                public string Token { get; }

                public int Line { get; }

                public InvalidArgumentException(IArgumentDefinition argument, string token, int line) :
                    base(Translate.Key(nameof(InvalidArgumentException), format: token))
                {
                    Argument = argument;
                    Token = token;
                    Line = line;
                }

            }

            /// <summary>
            /// Ошибка превышения числа аргументов инструкции
            /// </summary>
            public class TooMuchArguentsException :
                Exception, ILineException
            {
                public int TargetCount { get; }

                public int Line { get; }

                public TooMuchArguentsException(int targetCount, int line) :
                    base(Translate.Key(nameof(TooMuchArguentsException), format: targetCount))
                {
                    TargetCount = targetCount;
                    Line = line;
                }
            }

            /// <summary>
            /// Ошибка неизвестной инструкции
            /// </summary>
            public class UnknownInstructionException :
                Exception, ILineException
            {
                public string InstructionName { get; }

                public string Text { get; }

                public int Line { get; }

                public UnknownInstructionException(string instructionName, string text, int line) :
                    base(Translate.Key(nameof(UnknownInstructionException), format: instructionName))
                {
                    InstructionName = instructionName;
                    Text = text;
                    Line = line;
                }
            }

            /// <summary>
            /// Ошибка неверного формата записи инструкции
            /// </summary>
            public class InvalidInstructionSyntaxException :
                Exception, ILineException
            {
                public int Line { get; }

                public InvalidInstructionSyntaxException(int line) :
                    base(Translate.Key(nameof(InvalidInstructionSyntaxException), format: line))
                {
                    Line = line;
                }
            }

            /// <summary>
            /// Ошибка повторого определения существующей метки
            /// </summary>
            public class LabelRedefinedException :
               Exception, ILineException
            {
                public string Label { get; }

                public int Line { get; }

                public LabelRedefinedException(string label, int line) :
                    base(Translate.Key(nameof(LabelRedefinedException), format: label))
                {
                    Label = label;
                    Line = line;
                }
            }
        }
    }
}
