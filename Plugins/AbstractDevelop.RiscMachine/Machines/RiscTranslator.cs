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

            public InstructionDefinitions Definitions { get; set; }
            public List<Exception> Exceptions { get; set; }
            public Dictionary<string, int> Labels { get; set; }
            public int LineNumber { get; set; }

            #endregion
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
            public TranslationState State { get; private set; }

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
            public IEnumerable<Instruction> Translate(IEnumerable<string> input, InstructionDefinitions rules)
            {
                var state = new RiscTranslationState()
                {
                    Definitions = rules ?? instructionsBase,
                    Exceptions = new List<Exception>(),
                    Labels = new Dictionary<string, int>(),
                    LineNumber = 0
                };
                State = state;
                // построчный разбор
                foreach (var line in input)
                {
                    if (!(string.IsNullOrEmpty(line.RemoveWhitespaces(out var cleanLine)) || cleanLine.StartsWith(";")))
                    {
                        // проверка и декомпозиция текущей строки
                        if (Validate(cleanLine, out var parts))
                        {
                            // кроме метки ничего не нашлось
                            if (!ProcessLabel(out var index))
                                continue;

                            // попытка распознавания инструкции
                            if (state.Definitions.TryGetValue(parts[index++], out var def))
                                yield return new Instruction(def.Code, GetArguments());
                            else
                                state.Exceptions.Add(new UnknownInstructionException(parts[index - 1], cleanLine, state.LineNumber));

                            // считывание списка аргументов
                            IEnumerable<DataReference> GetArguments()
                            {
                                var value = default(DataReference);
                                // последовательное сопоставление всех определений с полученными данными
                                foreach (var arg in def.Arguments)
                                {
                                    // проверка существующих обязательных параметров
                                    if (index < parts.Length)
                                        value = Parse(parts[index++], arg);
                                    // проверка опциональных параметров
                                    else if (arg.IsOptional)
                                        value = arg.DefaultValue;
                                    // параметр не найден
                                    else state.Exceptions.Add(new MissingArgumentException(arg, index, parts[0], state.LineNumber));

                                    // если аргумент не прошел проверку, необходимо добавить его в список ошибок для повторной проверки
                                    if (!ValidateValue(value, arg))
                                        state.Exceptions.Add(new InvalidArgumentException(arg, parts[index - 1], state.LineNumber));

                                    yield return value;
                                }

                                // превышение количества необходимых аргументов
                                if (index < parts.Length)
                                    state.Exceptions.Add(new TooMuchArguentsException(def.Arguments.Length, state.LineNumber));
                            }
                        }
                        else state.Exceptions.Add(new InvalidInstructionSyntaxException(state.LineNumber));

                        // обработка строковых меток
                        bool ProcessLabel(out int index)
                        {
                            index = 0;

                            if (parts[index].EndsWith(":"))
                            {
                                var label = parts[index].Replace(":", "");

                                if (state.Labels.ContainsKey(label))
                                    state.Exceptions.Add(new LabelRedefinedException(label, state.LineNumber));
                                else
                                    state.Labels.Add(label, state.LineNumber - 1);

                                if (parts.Length == 1)
                                    return false;

                                index++;
                            }

                            return true;
                        }
                    }

                    state.LineNumber++;
                }

                // исключение ошибок, исправленных во время трансляции автоматически
                // (рарешение ссылок на метки)
                for (int i = 0; i < state.Exceptions.Count; i++)
                    if (state.Exceptions[i] is InvalidArgumentException argEx &&
                       (ValidateValue(Parse(argEx.Token, argEx.Argument), argEx.Argument)))
                    {
                        state.Exceptions.RemoveAt(i);
                        i--;
                    }

                // вывод обобщенного исключения для отображения в списке ошибок
                if (state.Exceptions.Count > 0)
                    throw new AggregateException(state.Exceptions);

                // считывание данных
                DataReference Parse(string data, IArgumentDefinition arg)
                {
                    try { return (arg.Parser ?? Convert)?.Invoke(data, state, arg) ?? arg.DefaultValue; }
                    catch (Exception exception)
                    {
                        state.Exceptions.Add(exception);
                        return arg.DefaultValue;
                    }
                }

                // проверка считанных данных
                bool ValidateValue(DataReference value, IArgumentDefinition arg)
                    => arg.Validator?.Invoke(value, state) ?? true;
            }

            public IEnumerable<Instruction> somekindofmonster(IEnumerable<string> input, InstructionDefinitions rules)
            {
                var state = new RiscTranslationState()
                {
                    Definitions = rules ?? instructionsBase,
                    Exceptions = new List<Exception>(),
                    Labels = new Dictionary<string, int>(),
                    LineNumber = 0
                };
                State = state;
                // построчный разбор
                foreach (var line in input)
                {
                    state.LineNumber++;
                    yield return null;
                }

                // исключение ошибок, исправленных во время трансляции автоматически
                // (рарешение ссылок на метки)
                for (int i = 0; i < state.Exceptions.Count; i++)
                    if (state.Exceptions[i] is InvalidArgumentException argEx &&
                       (ValidateValue(Parse(argEx.Token, argEx.Argument), argEx.Argument)))
                        state.Exceptions.RemoveAt(i);

                // вывод обобщенного исключения для отображения в списке ошибок
                if (state.Exceptions.Count > 0)
                    throw new AggregateException(state.Exceptions);

                // считывание данных
                DataReference Parse(string data, IArgumentDefinition arg)
                {
                    try { return (arg.Parser ?? Convert)?.Invoke(data, state, arg) ?? arg.DefaultValue; }
                    catch (Exception exception)
                    {
                        state.Exceptions.Add(exception);
                        return arg.DefaultValue;
                    }
                }

                // проверка считанных данных
                bool ValidateValue(DataReference value, IArgumentDefinition arg)
                    => arg.Validator?.Invoke(value, state) ?? true;
            }


            /// <summary>
            /// Производит трансляцию исходного кода в набор инструкций
            /// </summary>
            /// <param name="input">Строки исходного кода</param>
            /// <returns></returns>
            public IEnumerable Translate(IEnumerable input)
               => input is IEnumerable<string> lines ? Translate(lines, null) :
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