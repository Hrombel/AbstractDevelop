using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DataType = System.Byte;

namespace AbstractDevelop.Machines
{
    public partial class RiscMachine
    {
        public class RiscTranslator :
            ISourceTranslator<Instruction, object>
        {
            static readonly string
                reg3 = $@"r\d+|\[\d+\]",
                reg1 = $@"{reg3}|[\d-]+",
                reg2 = $@"{reg1}|[A-Za-z_\d]+",
                list = "in|out|ror|rol|not|or|and|nor|nand|xor|add|sub|jz|jo",
                reg = $@"\s*({list})\s+({reg1})+\s*({reg2})*\s*({reg3})*";

            static readonly Regex parsingExpression = new Regex(reg, RegexOptions.Singleline);

            public Encoding SupportedEncoding => Encoding.UTF8;

            public Func<string, DataReference> Convert
            {
                get
                {
                    /*
                     *   for (int i = 2; i < count; i++)
                        {
                            var str = match.Groups[i].Value.ToLower();
                            if (string.IsNullOrEmpty(str)) continue;

                            if (DataType.TryParse(str, out value))
                                list.Add(value);
                            else if (str.StartsWith("r"))
                                list.Add(new DataReference()
                                {
                                    // ������� ����������� ���, ������ ��� �� ����������� ��� ������� �������� �������� Value
                                    Type = DataLinkType.Register,
                                    Reference = DataType.Parse(str.Substring(1, str.Length - 1))
                                });
                            else if (str.StartsWith("[") && str.EndsWith("]") && str.Length > 2)
                                list.Add(new DataReference()
                                {
                                    Type = DataLinkType.Memory,
                                    Reference = DataType.Parse(str.Substring(1, str.Length - 2))
                                });
                            else
                                throw new NotSupportedException($"�������� �������� � ������� {match.Groups[0].Value}: {str}");
                        }
                     */

                    throw new NotImplementedException();
                }
            }

            RiscInstructionType GetOperation(string op)
            {
                switch (op)
                {
                    case "in": return RiscInstructionType.ReadInput;
                    case "out": return RiscInstructionType.WriteOutput;
                    case "ror": return RiscInstructionType.ShiftRight;
                    case "rol": return RiscInstructionType.ShiftLeft;
                    case "not": return RiscInstructionType.Not;
                    case "or": return RiscInstructionType.Or;
                    case "and": return RiscInstructionType.And;
                    case "nor": return RiscInstructionType.NotOr;
                    case "nand": return RiscInstructionType.NotAnd;
                    case "xor": return RiscInstructionType.Xor;
                    case "add": return RiscInstructionType.Addition;
                    case "sub": return RiscInstructionType.Subtraction;
                    case "jz": return RiscInstructionType.JumpIfFalse;
                    case "jo": return RiscInstructionType.JumpIfTrue;
                    default: return RiscInstructionType.Unknown;
                }
            }
         
            public IEnumerable<Instruction> Translate(IEnumerable<string> input, object rules)
            {
                var parts = new string[0];

                // ���������� ������
                foreach (var line in input)
                {
                    if (string.IsNullOrEmpty(line)) continue;

                    // �������� � ������������ ������� ������
                    if (Validate(line, out parts))
                        yield return new Instruction(GetOperation(parts[0]), parts.Skip(1).Select(arg => Convert(arg)));

                    else // TODO: �������� ���������� �� ������� ��������� ����
                        throw new ArgumentException($"����������� ������ ������� � ������: {line}");
                }
            }

            public IEnumerable Translate(IEnumerable input)
                => input is IEnumerable<string> lines ? Translate(lines, null) :
                    throw new InvalidOperationException(/*TODO*/);

            public bool Validate(string input, out string[] composingParts)
            {
                var match = parsingExpression.Match(input);
                // ���� ���� ����������� ���-�� ����������
                if (match.Success)
                {
                    // TODO: ���������� �� �����
                }


                    throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}