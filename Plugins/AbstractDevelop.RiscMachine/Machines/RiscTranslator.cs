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

            /// <summary>
            /// �������� ����������
            /// </summary>
            public InstructionDefinitions Definitions { get; set; } = instructionsBase;
   
            /// <summary>
            /// ����� ��������� (������� ����������)
            /// </summary>
            public Dictionary<string, int> Labels { get; set; } = new Dictionary<string, int>();
   
            /// <summary>
            /// ������������ ����������
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
            RiscTranslationState State;

            TranslationState ISourceTranslator.State => State;

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
                            // �������� � ������������ ������� ������
                            if (Validate(cleanLine, out var parts))
                            {
                                // ����� ����� ������ �� �������
                                if (!ProcessLabel(out var index))
                                    goto skip;

                                // ������� ������������� ����������
                                if (State.Definitions.TryGetValue(parts[index], out var def))
                                    return new Instruction(def.Code, GetArguments());
                                else
                                    throw new UnknownInstructionException(parts[index], cleanLine, State.LineNumber);

                                // ���������� ������ ����������
                                IEnumerable<DataReference> GetArguments()
                                {
                                    var value = default(DataReference);
                                    // ���������������� ������������� ���� ����������� � ����������� �������
                                    foreach (var arg in def.Arguments)
                                    {
                                        // �������� ������������ ������������ ����������
                                        if (++index < parts.Length)
                                            value = Parse(parts[index], arg);
                                        // �������� ������������ ����������
                                        else if (arg.IsOptional)
                                            value = arg.DefaultValue;
                                        // �������� �� ������
                                        else throw new MissingArgumentException(arg, index, parts.First(part => !State.Labels.Keys.Any(part.Contains)), State.LineNumber);
                                        
                                        if (ValidateValue(value, arg))
                                            yield return value;
                                        // ���� �������� �� ������ ��������, ���������� �������� ��� � ������ ������ ��� ��������� ��������
                                        else throw new InvalidArgumentException(arg, (index < parts.Length) ? parts[index] : value.ToString(), State.LineNumber);
                                    }

                                    // ���������� ���������� ����������� ����������
                                    if (parts.Length > ++index)
                                        throw new TooMuchArguentsException(def.Arguments.Length, State.LineNumber);
                                }
                            }
                            else throw new InvalidInstructionSyntaxException(State.LineNumber);

                            // ��������� ��������� �����
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

                // ���������� ������, ������������ �� ����� ���������� �������������
                // (��������� ������ �� �����)
                State.Exceptions.RemoveAll(ex =>
                    ex is InvalidArgumentException argEx &&
                    ValidateValue(Parse(argEx.Token, argEx.Argument), argEx.Argument));

                #region [���������� �������]

                // ���������� ������
                DataReference Parse(string data, IArgumentDefinition arg)
                    => (arg.Parser ?? Convert)?.Invoke(data, State, arg) ?? arg.DefaultValue;

                // �������� ��������� ������
                bool ValidateValue(DataReference value, IArgumentDefinition arg)
                    => arg.Validator?.Invoke(value, State) ?? true;

                bool IsCommented(string str)
                    => str.StartsWith(";");

                #endregion
            }
               
            /// <summary>
            /// ���������� ���������� ��������� ���� � ����� ����������
            /// </summary>
            /// <param name="input">������ ��������� ����</param>
            /// <param name="args">��������� ����������</param>
            /// <returns></returns>
            public IEnumerable Translate(IEnumerable input, params object[] args)
               => input is IEnumerable<string> lines ? 
                    Translate(lines, args?.FirstOfType<InstructionDefinitions>()).ToArray() :
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