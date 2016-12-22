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
        #region [������ � ���������]

        /// <summary>
        /// ��������� ����������� <see cref="RiscTranslator"/>
        /// </summary>
        public class RiscTranslationState :
            TranslationState
        {
            #region [�������� � ����]

            public InstructionDefinitions Definitions { get; set; }
            public List<Exception> Exceptions { get; set; }
            public Dictionary<string, int> Labels { get; set; }
            public int LineNumber { get; set; }

            #endregion
        }

        /// <summary>
        /// ���������� ����������� ��������� ���� ��� <see cref="RiscMachine"/>
        /// </summary>
        public class RiscTranslator :
            ISourceTranslator<Instruction, InstructionDefinitions>
        {
            #region [�������� � ����]

            /// <summary>
            /// ������� �������������� ������ � �������� ��� ���������
            /// </summary>
            public Func<string, RiscTranslationState, IArgumentDefinition, DataReference> Convert { get; set; }

            /// <summary>
            /// ��������� ����������
            /// </summary>
            public TranslationState State { get; private set; }

            /// <summary>
            /// ��������� ��-���������
            /// </summary>
            public Encoding SupportedEncoding
                => Encoding.UTF8;

            #endregion

            #region [������]

            /// <summary>
            /// ����������� ������� �������
            /// </summary>
            public void Dispose()
            {
                State = null;
                Convert = null;
            }

            /// <summary>
            /// ���������� ���������� ��������� ���� � ����� ���������� � ����������� ������� ������
            /// </summary>
            /// <param name="input">������ ��������� ����</param>
            /// <param name="rules">������� ������</param>
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
                // ���������� ������
                foreach (var line in input)
                {
                    if (!(string.IsNullOrEmpty(line.RemoveWhitespaces(out var cleanLine)) || cleanLine.StartsWith(";")))
                    {
                        // �������� � ������������ ������� ������
                        if (Validate(cleanLine, out var parts))
                        {
                            // ����� ����� ������ �� �������
                            if (!ProcessLabel(out var index))
                                continue;

                            // ������� ������������� ����������
                            if (state.Definitions.TryGetValue(parts[index++], out var def))
                                yield return new Instruction(def.Code, GetArguments());
                            else
                                state.Exceptions.Add(new UnknownInstructionException(parts[index - 1], cleanLine, state.LineNumber));

                            // ���������� ������ ����������
                            IEnumerable<DataReference> GetArguments()
                            {
                                var value = default(DataReference);
                                // ���������������� ������������� ���� ����������� � ����������� �������
                                foreach (var arg in def.Arguments)
                                {
                                    // �������� ������������ ������������ ����������
                                    if (index < parts.Length)
                                        value = Parse(parts[index++], arg);
                                    // �������� ������������ ����������
                                    else if (arg.IsOptional)
                                        value = arg.DefaultValue;
                                    // �������� �� ������
                                    else state.Exceptions.Add(new MissingArgumentException(arg, index, parts[0], state.LineNumber));

                                    // ���� �������� �� ������ ��������, ���������� �������� ��� � ������ ������ ��� ��������� ��������
                                    if (!ValidateValue(value, arg))
                                        state.Exceptions.Add(new InvalidArgumentException(arg, parts[index - 1], state.LineNumber));

                                    yield return value;
                                }

                                // ���������� ���������� ����������� ����������
                                if (index < parts.Length)
                                    state.Exceptions.Add(new TooMuchArguentsException(def.Arguments.Length, state.LineNumber));
                            }
                        }
                        else state.Exceptions.Add(new InvalidInstructionSyntaxException(state.LineNumber));

                        // ��������� ��������� �����
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

                // ���������� ������, ������������ �� ����� ���������� �������������
                // (��������� ������ �� �����)
                for (int i = 0; i < state.Exceptions.Count; i++)
                    if (state.Exceptions[i] is InvalidArgumentException argEx &&
                       (ValidateValue(Parse(argEx.Token, argEx.Argument), argEx.Argument)))
                    {
                        state.Exceptions.RemoveAt(i);
                        i--;
                    }

                // ����� ����������� ���������� ��� ����������� � ������ ������
                if (state.Exceptions.Count > 0)
                    throw new AggregateException(state.Exceptions);

                // ���������� ������
                DataReference Parse(string data, IArgumentDefinition arg)
                {
                    try { return (arg.Parser ?? Convert)?.Invoke(data, state, arg) ?? arg.DefaultValue; }
                    catch (Exception exception)
                    {
                        state.Exceptions.Add(exception);
                        return arg.DefaultValue;
                    }
                }

                // �������� ��������� ������
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
                // ���������� ������
                foreach (var line in input)
                {
                    state.LineNumber++;
                    yield return null;
                }

                // ���������� ������, ������������ �� ����� ���������� �������������
                // (��������� ������ �� �����)
                for (int i = 0; i < state.Exceptions.Count; i++)
                    if (state.Exceptions[i] is InvalidArgumentException argEx &&
                       (ValidateValue(Parse(argEx.Token, argEx.Argument), argEx.Argument)))
                        state.Exceptions.RemoveAt(i);

                // ����� ����������� ���������� ��� ����������� � ������ ������
                if (state.Exceptions.Count > 0)
                    throw new AggregateException(state.Exceptions);

                // ���������� ������
                DataReference Parse(string data, IArgumentDefinition arg)
                {
                    try { return (arg.Parser ?? Convert)?.Invoke(data, state, arg) ?? arg.DefaultValue; }
                    catch (Exception exception)
                    {
                        state.Exceptions.Add(exception);
                        return arg.DefaultValue;
                    }
                }

                // �������� ��������� ������
                bool ValidateValue(DataReference value, IArgumentDefinition arg)
                    => arg.Validator?.Invoke(value, state) ?? true;
            }


            /// <summary>
            /// ���������� ���������� ��������� ���� � ����� ����������
            /// </summary>
            /// <param name="input">������ ��������� ����</param>
            /// <returns></returns>
            public IEnumerable Translate(IEnumerable input)
               => input is IEnumerable<string> lines ? Translate(lines, null) :
                   throw new ArgumentException(nameof(input));
            

            /// <summary>
            /// ��������� ������� ������ � ����������� �� ������������
            /// </summary>
            /// <param name="input">������� ������ (������ ����)</param>
            /// <param name="composingParts">����� ��� ���������</param>
            /// <returns></returns>
            public bool Validate(string input, out string[] composingParts)
            {
                if (input.Contains(";"))
                    input = input.Substring(0, input.IndexOf(';'));

                composingParts = input.Split(' ');
                bool hasLabel = false;

                // ���������� ����� � ����� ������� ��� ���������� ���������
                if (composingParts[0].Contains(":") && !composingParts[0].EndsWith(":"))
                {
                    composingParts = composingParts[0].Split(':').Union(composingParts.Skip(1)).ToArray();
                    composingParts[0] = composingParts[0] + ":";
                }

                for (int i = 0; i < composingParts.Length; i++)
                {
                    // ����� �����
                    if (composingParts[i].Contains(":"))
                    {
                        composingParts[i] = composingParts[i].Replace(":", string.Empty);
                        hasLabel = true;

                        // ����������� ������������ ����� � ������
                        if (!composingParts[0].EndsWith(":"))
                            composingParts[0] = composingParts[0] + ":";
                    }

                    // �������� �� ������������ ������������ ��������
                    if (composingParts[i].Length > 0)
                        if (!composingParts[i].All(CheckChar))
                            return false;

                    bool CheckChar(char c)
                        => char.IsLetterOrDigit(c) || c == '[' || c == ']' || (hasLabel &&  i == 0) || c == '_';
                }

                // ������� ������ ���������
                composingParts = composingParts.Where(part => part.Length > 0).ToArray();
                return true;
            }

            #endregion
        }

        #endregion
    }
}