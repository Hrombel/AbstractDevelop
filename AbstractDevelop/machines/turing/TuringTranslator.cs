using AbstractDevelop.errors.dev;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет средство трансляции исходного текста программы в список команд для машины Тьюринга.
    /// </summary>
    public sealed class TuringTranslator
    {
        /// <summary>
        /// Возвращает алфавит, использованный в списке операций для машины Тьюринга.
        /// </summary>
        /// <param name="operations">Список операций для машины Тьюринга.</param>
        /// <returns>Алфавит.</returns>
        public static SymbolSet GetAlphabet(List<TuringOperation> operations)
        {
            List<TuringOperation> ops = operations.Cast<TuringOperation>().ToList();

            SymbolSet result = new SymbolSet();

            int i, j, n, m;
            n = ops.Count;
            for (i = 0; i < n; i++)
            {
                m = ops[i].State.Converts.Length;
                for (j = 0; j < m; j++)
                {
                    try
                    {
                        AddChars(result, ops[i].State.Converts[j].Input);
                        AddChars(result, ops[i].State.Converts[j].Output);
                    }
                    catch
                    {
                        throw new Exception("Размер алфавита операций превысил допустимое значение");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Проверяет, есть ли указанные символы в алфавите. Если нет, то добавляет их.
        /// </summary>
        /// <param name="al">Расширяемый алфавит.</param>
        /// <param name="chars">Массив добавляемых символов.</param>
        private static void AddChars(SymbolSet al, char[] chars)
        {
            int n = chars.Length;
            for (int i = 0; i < n; i++)
            {
                if (!al.CharExists(chars[i]))
                {
                    al.AddChar(chars[i]);
                }
            }
        }

        /// <summary>
        /// Транслирует исходный текст программы в список команд для машины Тьюринга.
        /// </summary>
        /// <param name="source">Исходный текст программы.</param>
        /// <returns>Список сформированных команд для машины Тьюринга.</returns>
        public static List<TuringOperation> Translate(string source)
        {
            if (source == null)
                throw new ArgumentException("Исходный текст не может быть неопределенным");

            source = SourceTools.RemoveComments(source);

            List<TuringOperation> result = GetOperations(source);

            return result.Cast<TuringOperation>().ToList();
        }

        /// <summary>
        /// Возвращает список состояний, описанных в исходном тексте.
        /// </summary>
        /// <param name="src">Исходный текст программы.</param>
        private static List<TuringOperation> GetOperations(string src)
        {
            return GetStates(src).ConvertAll(x => TuringOperation.Create(TuringOperationId.StateDefinition, x) as TuringOperation);
        }

        /// <summary>
        /// Получает список состояний машины Тьюринга из указанного исходного текста программы.
        /// </summary>
        /// <param name="src">Иходный текст программы.</param>
        private static List<TuringState> GetStates(string src)
        {
            List<TuringState> res = new List<TuringState>();

            int tapesCount = -1;
            int ptr = 0;
            TuringState st;
            while (NextSymbol(src, ref ptr))
            {
                st = GetState(src, ref ptr);
                if (tapesCount != -1)
                {
                    if (st.Converts[0].Input.Length != tapesCount)
                        throw new InvalidOperationTextException("Все переходы машины Тьюринга должны соответствовать одинаковому количеству лент");
                }
                else
                    tapesCount = st.Converts[0].Input.Length;

                res.Add(st);
                ptr++;
            }

            return res;
        }

        /// <summary>
        /// Получает очередное состояние машины Тьюринга и передвигает указатель на следующий за блоком состояния символ.
        /// </summary>
        /// <param name="src">Исходный текст программы.</param>
        /// <param name="ptr">Ссылка на текущую позицию указателя.</param>
        private static TuringState GetState(string src, ref int ptr)
        {
            int id;
            List<TuringConvert> converts = new List<TuringConvert>();

            NextSymbol(src, ref ptr);
            try
            {
                id = ReadInt(src, ref ptr);
            }
            catch
            {
                throw new InvalidOperationTextException("Неверный формат номера");
            }
            ptr++;
            NextSymbol(src, ref ptr);
            if (src[ptr] != ':')
                throw new InvalidOperationTextException(string.Format("\":\" ожидалось, но найдено \"{0}\"", src[ptr]));

            int t;

            t = ptr + 1;
            NextSymbol(src, ref t);
            bool stateLeftBracket = src[t] == '{';
            if (stateLeftBracket) ptr = t;

            while (true)
            {
                ptr++;
                if (!NextSymbol(src, ref ptr)) break;

                if (src[ptr] == '}') break;

                converts.Add(GetConvert(src, ref ptr));
                t = ptr + 1;
                if (!NextSymbol(src, ref t))
                {
                    if (t == src.Length)
                    {
                        if (stateLeftBracket)
                        {
                            throw new InvalidOperationTextException("\"}\" ожидалось");
                        }
                        break;
                    }
                }

                if (src[t] == '}')
                {
                    if (!stateLeftBracket)
                        throw new InvalidOperationTextException("\"}\" не ожидается");
                    ptr = t;
                    break;
                }
                else
                {
                    t++;
                    if (!NextSymbol(src, ref t))
                    {
                        if (stateLeftBracket)
                        {
                            throw new InvalidOperationTextException("\"}\" ожидалось");
                        }
                        break;
                    }
                    if (src[t] == ':')
                    {
                        if (stateLeftBracket)
                            throw new InvalidOperationTextException("\"}\" ожидалось");
                        break;
                    }
                }
            }

            if (converts.Count == 0)
                throw new InvalidOperationTextException("Множество переходов состояния не может быть пустым");

            return new TuringState(id, converts.ToArray());
        }

        /// <summary>
        /// Получает переход машины Тьюринга из указанного исходного текста.
        /// </summary>
        /// <param name="src">Исходный текст программы.</param>
        /// <param name="ptr">Указатель текущего символа.</param>
        private static TuringConvert GetConvert(string src, ref int ptr)
        {
            char[] inputs;
            char[] outputs;
            int nextState;
            TuringPenDir[] directions;

            inputs = GetSymbols(src, ref ptr);
            ptr++;
            NextSymbol(src, ref ptr);
            if (src[ptr] != '=')
                throw new InvalidOperationTextException(string.Format("\"=\" ожидалось, но найдено \"{0}\"", src[ptr]));
            ptr++;

            outputs = GetSymbols(src, ref ptr);
            ptr++;
            NextSymbol(src, ref ptr);
            if (src[ptr] != '-' || src[ptr + 1] != '>')
                throw new InvalidOperationTextException(string.Format("\"->\" ожидалось, но найдено \"{0}{1}\"", src[ptr], src[ptr + 1]));
            ptr += 2;

            NextSymbol(src, ref ptr);
            try
            {
                nextState = ReadInt(src, ref ptr);
            }
            catch
            {
                throw new InvalidOperationTextException("Неверный формат номера");
            }

            ptr++;
            NextSymbol(src, ref ptr);
            if (src[ptr] != ',')
                throw new InvalidOperationTextException(string.Format("\",\" ожидалось, но найдено \"{0}\"", src[ptr]));

            ptr++;

            char[] dirs = GetSymbols(src, ref ptr);
            directions = new TuringPenDir[dirs.Length];
            int n = dirs.Length;
            for (int i = 0; i < n; i++)
            {
                switch (char.ToUpper((char)dirs[i]))
                {
                    case 'L':
                        directions[i] = TuringPenDir.Left;
                        break;

                    case 'R':
                        directions[i] = TuringPenDir.Right;
                        break;

                    case 'S':
                        directions[i] = TuringPenDir.Stay;
                        break;

                    default:
                        throw new InvalidOperationTextException("Неверная команда для читающей/пишущей головки");
                }
            }

            if (inputs.Length != outputs.Length || inputs.Length != directions.Length)
                throw new InvalidOperationTextException("Все переходы машины Тьюринга должны соответствовать одинаковому количеству лент");

            return new TuringConvert(inputs, outputs, directions, nextState);
        }

        /// <summary>
        /// Получает символы, идущие через запятую из исходного текста. Вся последовательность может быть заключена в скобки.
        /// Указатель будет находиться на последнем символе блока.
        /// </summary>
        /// <param name="src">Исходный текст программы.</param>
        /// <param name="ptr">Указатель текущего символа.</param>
        private static char[] GetSymbols(string src, ref int ptr)
        {
            List<char> result = new List<char>();

            NextSymbol(src, ref ptr);
            bool symbolsLeftBracket = src[ptr] == '(';
            if (symbolsLeftBracket) ptr++;

            int t;
            NextSymbol(src, ref ptr);
            while (true)
            {
                result.Add(src[ptr]);
                t = ptr;
                ptr++;
                if (!NextSymbol(src, ref ptr))
                {
                    if (ptr == src.Length)
                    {
                        ptr = t;
                        break;
                    }
                }
                if (src[ptr] == ',')
                {
                    ptr++;
                    NextSymbol(src, ref ptr);
                }
                else
                {
                    if (symbolsLeftBracket)
                    {
                        if (src[ptr] != ')')
                            throw new InvalidOperationTextException(string.Format("\")\" ожидалось, но найдено \"{0}\"", src[ptr]));
                    }
                    else
                    {
                        if (src[ptr] == ')')
                            throw new InvalidOperationTextException("\")\" не ожидалось.");
                        ptr = t;
                    }
                    break;
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Передвигает указатель на следующий значащий символ строки.
        /// </summary>
        /// <param name="src">Строка, в которой ведется поиск.</param>
        /// <param name="ptr">Перемещаемый указатель.</param>
        /// <returns>Истина, если конец строки не достигнут, иначе - ложь.</returns>
        private static bool NextSymbol(string src, ref int ptr)
        {
            if (ptr >= src.Length - 1) return false;

            while (char.IsWhiteSpace(src, ptr))
            {
                ptr++;
                if (ptr == src.Length) return false;
            }

            return true;
        }

        /// <summary>
        /// Читает целое четырехбайтовое число, записанное в десятичной СС до знака пробела.
        /// </summary>
        /// <param name="src">Строка.</param>
        /// <param name="ptr">Индекс старшего разряда в строке.</param>
        private static int ReadInt(string src, ref int ptr)
        {
            int end = ptr;
            while (char.IsDigit(src, end)) end++;

            string t = src.Substring(ptr, end - ptr);
            ptr = end - 1;
            return int.Parse(t);
        }
    }
}