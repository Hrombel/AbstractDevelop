using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

using AbstractDevelop.Translation;

namespace AbstractDevelop.Machines
{
    public abstract partial class InstructionMachine<InstructionCode, ArgumentType>
    {
        public class InstuctionTranslator :
            ISourceTranslator<Instruction, IDefinitionCollection>
        {
            public class InstructionPattern :
                Regex
            {
                #region [Константы]

                public const string
                    InstructionGroupName = "instruction",
                    Whitespace = @"\s+";


                #endregion

                public string ArgumentSeparators { get; set; }

                public int MaxArgumentCount { get; set; } = -1;

                public IDefinitionCollection Definitions { get; set; }

                // composition
                // $@"\s*({list})\s+({reg1})+\s*({reg2})*\s*({reg3})*";
                // reg3 = $@"r\d+|\[\d+\]",
                // reg1 = $@"{reg3}|[\d-]+",
                // reg2 = $@"{reg1}|[A-Za-z_\d]+",
                // list = "in|out|ror|rol|not|or|and|nor|nand|xor|add|sub|jz|jo",

                public string InstructionGroup
                    => $"";

                public string ArgumentGroup
                    => $"";

                public virtual string RegexString
                    => $"({InstructionGroup})({Whitespace}({ArgumentGroup}))*";
            }

            public virtual InstructionPattern Pattern { get; set; }

            public virtual Encoding SupportedEncoding { get { return Encoding.UTF8; } }

            public TranslationState State
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public virtual IEnumerable<Instruction> Translate(IEnumerable<string> input, IDefinitionCollection definitions)
            {
                var match = default(Match);
                // интерпретация всех ненулевых входных строк в виде инструкций
                foreach (var line in input.WhereNot(string.IsNullOrEmpty))
                {
                    // шаблон инструкции найден в данной строке
                    if ((match = Pattern.Match(line)).Success)
                    { 
                        string instructionName = match.Groups[InstructionPattern.InstructionGroupName].Value;
                        // определение типа инструкции и сопоставление параметров
                        // код операции не должен быть равен зарезервированному значению 'unknown'
                        if (definitions.Aliases.TryGetValue(instructionName, out var code) &&
                            code.CompareTo(default(InstructionCode)) != 0)
                            yield return new Instruction(code, GetArguments(definitions[code], match).ToArray());
                        // несуществующий тип инструкции
                        else throw new InvalidOperationException($"Незарегистрированный тип инструкции: {instructionName}");
                    }
                    else throw new FormatException($"Строка #{input.IndexOf(line)} во входных данных имеет неверный формат");
                }
                yield break;
            }

            public virtual IEnumerable<ArgumentType> GetArguments(IInstructionDefinition definition, Match match)
            {
                ArgumentType value = default(ArgumentType);
                int index = 0;
                bool isValid;

                foreach (var argDef in definition.Arguments)
                {
                    isValid = false; index++;
                    // TODO: реализовать fallback для default парсера
                    try { value = (argDef.Parser ?? null).Invoke(match.Groups[index].Value, State); }
                    catch
                    {
                        // в случае опциональности аргумента возможна замена на значение по умолчанию
                        if (argDef.IsOptional) value = argDef.DefaultValue;
                        // в противном случае дальнейший разбор не представляет смысла
                        else throw new InvalidOperationException($"Недостаточное количество аргументов для инструкции {definition.Alias}.");
                    }
                    // прохождение проверок
                    finally { isValid = (argDef.Validator?.Invoke(value, State) ?? true); }
                    if (isValid) yield return value;
                }
                yield break;
            }

            public IEnumerable Translate(IEnumerable input)
            {
                throw new NotImplementedException();
            }

            public bool Validate(IEnumerable input)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool Validate(string input, out string[] composingParts)
            {
                throw new NotImplementedException();
            }
        }
    }
}
