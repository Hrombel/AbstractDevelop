using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AbstractDevelop.Translation;

using static AbstractDevelop.Machines.RiscMachine.Exceptions;

namespace AbstractDevelop.Machines
{
    public partial class RiscMachine
    {
        #region [Классы и структуры]

        /// <summary>
        /// Состояние транслятора <see cref="RiscTranslator"/>
        /// </summary>
        public class RiscTranslationState :
            TranslationState
        {
            #region [Свойства и Поля]

            /// <summary>
            /// Описание инструкций
            /// </summary>
            public InstructionDefinitions Definitions { get; set; } = instructionsBase;
   
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
                    case SkipLineException skip:
                        return true;
                    case AggregateException aggregation:
                        return aggregation.InnerExceptions.All(ProcessException);
                    default:
                        return base.ProcessException(exception);
                }
            }

            public RiscTranslationState(InstructionDefinitions instructionSet)
            {
                if (instructionSet != default(InstructionDefinitions))
                    Definitions = instructionSet;
            }
        }

        /// <summary>
        /// Реализация транслятора исходного кода для <see cref="RiscMachine"/>
        /// </summary>
        public class RiscTranslator :
            ISourceTranslator<Instruction, InstructionDefinitions>
        {
            #region [Свойства и Поля]

            /// <summary>
            /// Функция преобразования строки в значение для обработки
            /// </summary>
            public Func<string, RiscTranslationState, IArgumentDefinition, DataReference> Convert { get; set; }

            /// <summary>
            /// Состояние трансляции
            /// </summary>
            RiscTranslationState State;

            TranslationState ISourceTranslator.State => State;

            /// <summary>
            /// Кодировка по-умолчанию
            /// </summary>
            public Encoding SupportedEncoding
                => Encoding.UTF8;
            
            #endregion

            #region [Методы]

            /// <summary>
            /// Освобождает ресурсы объекта
            /// </summary>
            public void Dispose()
            {
                State = null;
                Convert = null;
            }

            /// <summary>
            /// Производит трансляцию исходного кода в набор инструкций с применением системы правил
            /// </summary>
            /// <param name="input">Строки исходного кода</param>
            /// <param name="rules">Система правил</param>
            /// <returns></returns>
            public IEnumerable<Instruction> Translate(IEnumerable<string> input, InstructionDefinitions rules = default(InstructionDefinitions))
            {
                State = new RiscTranslationState(rules);
                foreach (var line in input)
                {
                    State.LineNumber++;

                    try { State.Processed.Add(ProcessLine()); }
                    catch (Exception ex)
                    {
                        State.ProcessException(ex);
                        continue;
                    }

                    yield return State.Processed.Last();

                    Instruction ProcessLine()
                    {
                        if (!line.RemoveWhitespaces(out var cleanLine).CheckAny(string.IsNullOrEmpty, IsCommented))
                        {
                            // проверка и декомпозиция текущей строки
                            if (Validate(cleanLine, out var parts))
                            {
                                // кроме метки ничего не нашлось
                                if (!ProcessLabel(out var index))
                                    goto skip;

                                // попытка распознавания инструкции
                                if (State.Definitions.TryGetValue(parts[index], out var def))
                                    return new Instruction(def.Code, GetArguments());
                                else
                                    throw new UnknownInstructionException(parts[index], cleanLine, State.LineNumber);

                                // считывание списка аргументов
                                IEnumerable<DataReference> GetArguments()
                                {
                                    var value = default(DataReference);
                                    // последовательное сопоставление всех определений с полученными данными
                                    foreach (var arg in def.Arguments)
                                    {
                                        // проверка существующих обязательных параметров
                                        if (++index < parts.Length)
                                            value = Parse(parts[index], arg);
                                        // проверка опциональных параметров
                                        else if (arg.IsOptional)
                                            value = arg.DefaultValue;
                                        // параметр не найден
                                        else throw new MissingArgumentException(arg, index, parts.First(part => !State.Labels.Keys.Any(part.Contains)), State.LineNumber);
                                        
                                        if (ValidateValue(value, arg))
                                            yield return value;
                                        // если аргумент не прошел проверку, необходимо добавить его в список ошибок для повторной проверки
                                        else throw new InvalidArgumentException(arg, (index < parts.Length) ? parts[index] : value.ToString(), State.LineNumber);
                                    }

                                    // превышение количества необходимых аргументов
                                    if (parts.Length > ++index)
                                        throw new TooMuchArguentsException(def.Arguments.Length, State.LineNumber);
                                }
                            }
                            else throw new InvalidInstructionSyntaxException(State.LineNumber);

                            // обработка строковых меток
                            bool ProcessLabel(out int index)
                            {
                                bool hasLabel = false;
                                if (hasLabel = parts[index = 0].Replace(":", "", out var label))
                                {
                                    State.Labels.Add(label, State.Labels.ContainsKey(label)? 
                                        throw new LabelRedefinedException(label, State.LineNumber) : 
                                        State.Processed.Count);
                                }

                                return parts.Length > (hasLabel ? 1 : 0);
                            }
                        }

                        skip: throw new SkipLineException();
                    }
                }

                // исключение ошибок, исправленных во время трансляции автоматически
                // (рарешение ссылок на метки)
                State.Exceptions.RemoveAll(ex =>
                    ex is InvalidArgumentException argEx &&
                    ValidateValue(Parse(argEx.Token, argEx.Argument), argEx.Argument));

                #region [Внутренние функции]

                // считывание данных
                DataReference Parse(string data, IArgumentDefinition arg)
                    => (arg.Parser ?? Convert)?.Invoke(data, State, arg) ?? arg.DefaultValue;

                // проверка считанных данных
                bool ValidateValue(DataReference value, IArgumentDefinition arg)
                    => arg.Validator?.Invoke(value, State) ?? true;

                bool IsCommented(string str)
                    => str.StartsWith(";");

                #endregion
            }
               
            /// <summary>
            /// Производит трансляцию исходного кода в набор инструкций
            /// </summary>
            /// <param name="input">Строки исходного кода</param>
            /// <param name="args">Параметры трансляции</param>
            /// <returns></returns>
            public IEnumerable Translate(IEnumerable input, params object[] args)
               => input is IEnumerable<string> lines ? 
                    Translate(lines, args?.FirstOfType<InstructionDefinitions>()).ToArray() :
                    throw new ArgumentException(nameof(input));
   
            /// <summary>
            /// Проверяет входные данные и возворащает их декомпозицию
            /// </summary>
            /// <param name="input">Входные данные (строка кода)</param>
            /// <param name="composingParts">Части для обработки</param>
            /// <returns></returns>
            public bool Validate(string input, out string[] composingParts)
            {
                if (input.Contains(";"))
                    input = input.Substring(0, input.IndexOf(';'));

                composingParts = input.Split(' ');
                bool hasLabel = false;

                // разделение метки и имени команды при совместном написании
                if (composingParts[0].Contains(":") && !composingParts[0].EndsWith(":"))
                {
                    composingParts = composingParts[0].Split(':').Union(composingParts.Skip(1)).ToArray();
                    composingParts[0] = composingParts[0] + ":";
                }

                for (int i = 0; i < composingParts.Length; i++)
                {
                    // поиск меток
                    if (composingParts[i].Contains(":"))
                    {
                        composingParts[i] = composingParts[i].Replace(":", string.Empty);
                        hasLabel = true;

                        // перемещение определителя метки к началу
                        if (!composingParts[0].EndsWith(":"))
                            composingParts[0] = composingParts[0] + ":";
                    }

                    // проверка на содерживание недопустимых символов
                    if (composingParts[i].Length > 0)
                        if (!composingParts[i].All(CheckChar))
                            return false;

                    bool CheckChar(char c)
                        => char.IsLetterOrDigit(c) || c == '[' || c == ']' || (hasLabel &&  i == 0) || c == '_';
                }

                // очистка пустых элементов
                composingParts = composingParts.Where(part => part.Length > 0).ToArray();
                return true;
            }

            #endregion
        }

        #endregion
    }
}