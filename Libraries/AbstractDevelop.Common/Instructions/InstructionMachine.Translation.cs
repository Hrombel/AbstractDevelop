using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

using AbstractDevelop.Translation;
using AbstractDevelop.Symbols;

using System.Globalization;

namespace AbstractDevelop.Machines
{
    public abstract partial class InstructionMachine<InstructionCode, ArgumentType>
    {
        public class InstructionTranslationState :
                TranslationState
        {
            #region [Свойства и Поля]

            /// <summary>
            /// Описание инструкций
            /// </summary>
            public InstructionDefinitions Definitions { get; set; }

            /// <summary>
            /// Метки переходов (индексы инструкций)
            /// </summary>
            public Dictionary<string, int> Labels { get; set; } = new Dictionary<string, int>();

            /// <summary>
            /// Обработанная инструкция
            /// </summary>
            public List<Instruction> Processed { get; set; } = new List<Instruction>();

            #endregion

            public override bool ProcessException(Exception exception)
            {
                switch (exception)
                {
                    case Exceptions.SkipLineException skip:
                        return true;
                    case AggregateException aggregation:
                        return aggregation.InnerExceptions.All(ProcessException);
                    default:
                        return base.ProcessException(exception);
                }
            }

            public InstructionTranslationState(InstructionDefinitions instructionSet)
            {
                if (instructionSet != default(InstructionDefinitions))
                    Definitions = instructionSet;
            }
        }

        public class InstuctionTranslator :
            ISourceTranslator<Instruction, InstructionDefinitions>
        {
            public class ValidationRules
            {
                public const char
                    DefaultLabelSign = ':',
                    DefaultWhiteSpace = ' ',
                    DefaultTabSign = '\t';

                public virtual char Whitespace { get; set; } = DefaultWhiteSpace;

                public virtual char LabelSign { get; set; } = DefaultLabelSign;

                public virtual string[] CommentSigns { get; set; }

                public virtual char[] Separators => 
                    new char[] { Whitespace, LabelSign, DefaultTabSign };

                public virtual ISymbolSet AllowedSymbols { get; } 
                    = new ISymbolSetElement[]
                    {
                        // буквенно-цифровые символы
                        new SymbolGroup(UnicodeCategory.LowercaseLetter),
                        new SymbolGroup(UnicodeCategory.UppercaseLetter),
                        new SymbolGroup(UnicodeCategory.DecimalDigitNumber),

                        // символы пунктуации
                        new SymbolGroup(UnicodeCategory.OpenPunctuation),
                        new SymbolGroup(UnicodeCategory.ClosePunctuation),
                        new SymbolGroup(UnicodeCategory.DashPunctuation),
                        new SymbolGroup(UnicodeCategory.ConnectorPunctuation),
                        new SymbolGroup(UnicodeCategory.OtherPunctuation)
                    }.ToSymbolSet();

                public bool IsValid { get; protected set; }

                public virtual bool IsCommented(string token)
                    => CommentSigns.Any(token.StartsWith);

                public virtual bool HasLabel(string token, out string label)
                    => token.RemoveChars(out label, LabelSign);

                public virtual IEnumerable<string> GetParts(IEnumerable<char> input)
                {
                    var word = new StringBuilder();
                    IsValid = true;

                    var separators = Separators;
                    bool isSeparated = false, wasSeparated = false;

                    foreach (var @char in input)
                    {
                        // пропуск пробелов
                        if (isSeparated = separators.Contains(@char))
                        {
                            if (wasSeparated) continue;
                            // все символы, кроме пробелов, должны быть сохранены
                            else yield return (char.IsWhiteSpace(@char) ? word.GetAndClear() :
                                    word.GetAndClear() + @char);
                        }
                        else
                        {
                            // проверка на допустимость символа
                            if (!AllowedSymbols?.Contains(@char) ?? false)
                            {
                                IsValid = false;
                                yield break;
                            }
                            // добавление символа в слово
                            word.Append(@char);
                        }
                        wasSeparated = isSeparated;
                    }

                    if (word.Length > 0)
                        yield return word.ToString();
                }

                public virtual bool Decompose(string input, out string label, out string instruction, out IEnumerable<string> args)
                {
                    label = instruction = null;

                    var parts = args = GetParts(input);
                    var first = parts?.FirstOrDefault();

                    if (IsValid && !string.IsNullOrEmpty(first))
                    {
                        var hasLabel = HasLabel(first, out label);
                        // определение метки и инструкции
                        label = hasLabel ? label : string.Empty;
                        instruction = hasLabel ? parts.TryGet(1, out args) : first;
                        // переход на аргументы
                        args = args.Skip(1);
                        return true;
                    }
                    return false;
                }
            }
           
            public Encoding SupportedEncoding
                => Encoding.UTF8;

            /// <summary>
            /// Функция преобразования строки в значение для обработки
            /// </summary>
            public Func<string, InstructionTranslationState, IArgumentDefinition, ArgumentType> Convert { get; set; }

            public ValidationRules Validation { get; set; } = new ValidationRules();

            public InstructionDefinitions DefaultDefinitions { get; set; }

            public InstructionTranslationState State { get; set; }
  
            TranslationState ISourceTranslator.State => State;

            public void Dispose()
            {
                State = null;
                Convert = null;
            }

            public IEnumerable Translate(IEnumerable input, params object[] args)
                =>  input is IEnumerable<string> lines ?
                    Translate(lines, args?.FirstOfType<InstructionDefinitions>() ?? DefaultDefinitions).ToArray() :
                    throw new ArgumentException(nameof(input));

            public IEnumerable<Instruction> Translate(IEnumerable<string> input, InstructionDefinitions rules)
            {
                var current = default(Instruction);
                State = new InstructionTranslationState(rules);
                
                foreach (var inputLine in input)
                {
                    (State.LineNumber++).ToVariable(out var line);

                    try { State.Processed.Add(current = ProcessLine()); }
                    catch (Exception ex)
                    {
                        State.ProcessException(ex);
                        continue;
                    }

                    yield return current;

                    Instruction ProcessLine()
                    {
                        if (!inputLine.RemoveWhitespaces(out var cleanLine).CheckAny(string.IsNullOrEmpty, Validation.IsCommented) &&
                            // декомпозиция текущей строки и сохранене метки
                            Validation.Decompose(cleanLine, out var label, out var instruction, out var args) && ProcessLabel(label) && !instruction.Check(string.IsNullOrEmpty))
                        {
                            // попытка распознавания инструкции
                            if (State.Definitions.TryGetValue(instruction, out var def))
                                return new Instruction(def.Code, GetArguments());
                            else
                                throw new Exceptions.UnknownInstructionException(instruction, cleanLine, line);

                            // считывание списка аргументов
                            IEnumerable<ArgumentType> GetArguments()
                            {
                                var value = default(ArgumentType);
                                var argumentEnum = args.GetEnumerator();

                                // последовательное сопоставление всех определений с полученными данными
                                foreach (var argPrototype in def.Arguments)
                                {
                                    // проверка существующих обязательных параметров
                                    if (argumentEnum.MoveNext(out var argument).ToVariable(out var foundArgument))
                                        value = Parse(argument, argPrototype);
                                    // проверка опциональных параметров
                                    else if (argPrototype.IsOptional)
                                        value = argPrototype.DefaultValue;
                                    // параметр не найден
                                    else throw new Exceptions.MissingArgumentException(argPrototype, Array.IndexOf(def.Arguments, argPrototype) + 1, instruction, line);

                                    if (ValidateValue(value, argPrototype)) yield return value;
                                    // если аргумент не прошел проверку, необходимо добавить его в список ошибок для повторной проверки
                                    else throw new Exceptions.InvalidArgumentException(argPrototype, foundArgument? argument : value.ToString(), line);
                                }

                                // превышение количества необходимых аргументов
                                if (argumentEnum.MoveNext())
                                    throw new Exceptions.TooMuchArguentsException(def.Arguments.Length, line);
                            }
                        }

                        throw new Exceptions.SkipLineException();
                    }

                    // обработка строковых меток
                    bool ProcessLabel(string value)
                        => value.Check(string.IsNullOrEmpty) || !State.Execute(s => s.Labels.Add(value, s.Labels.ContainsKey(value)? 
                        throw new Exceptions.LabelRedefinedException(value, s.LineNumber) : s.Processed.Count));
                }

                // исключение ошибок, исправленных во время трансляции автоматически
                // (разрешение ссылок на метки)
                State.Exceptions.RemoveAll(ex =>
                    ex is Exceptions.InvalidArgumentException argEx &&
                    ValidateValue(Parse(argEx.Token, argEx.Argument), argEx.Argument));

                #region [Внутренние функции]

                // считывание данных
                ArgumentType Parse(string data, IArgumentDefinition arg)
                {
                    (arg.Parser ?? Convert).Invoke(data, State, arg).ToVariable(out var value);
                    return value.Equals(default(ArgumentType)) ? arg.DefaultValue : value;
                }

                // проверка считанных данных
                bool ValidateValue(ArgumentType value, IArgumentDefinition arg)
                    => arg.Validator?.Invoke(value, State) ?? true;

                #endregion
            }

            public bool Validate(string input, out string[] composingParts)
                => Validation.Decompose(input, out var label, out var instruction, out var args)
                .Select(new List<string>() { label, instruction }.WhereNot(string.IsNullOrEmpty).Union(args).ToArray(),
                        new  string[0], out composingParts);
        }
    }
}
