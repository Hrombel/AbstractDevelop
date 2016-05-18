using AbstractDevelop.errors.dev;
using AbstractDevelop.tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет средство трансляции исходного текста программы в список команд для машины с бесконечными регистрами.
    /// </summary>
    public sealed class RegisterTranslator
    {
        /// <summary>
        /// Транслирует исходный текст программы для классической машины с бесконечными регистрами в список команд.
        /// </summary>
        /// <param name="src">Исходный текст программы.</param>
        /// <returns>Список команд для классической машины с бесконечными регистрами.</returns>
        public static List<Operation> TranslateClassicProgram(string src)
        {
            if (src == null)
                throw new ArgumentNullException("Исходный текст программы не может быть неопределенным");

            List<RegisterOperation> ops = ReadCommands(src);
            CheckClassicOperations(ops);

            return ops.Cast<Operation>().ToList();
        }

        /// <summary>
        /// Транслирует объединенный исходный текст программы в список команд для параллельной машины с бесконечными регистрами.
        /// </summary>
        /// <param name="src">Исходный текст программы.</param>
        /// <returns>Список программ для параллельной машины с бесконечными регистрами.</returns>
        public static RegisterProgramCollection TranslateParallelProgram(string src)
        {
            if (string.IsNullOrWhiteSpace(src))
                throw new ArgumentNullException("Исходный текст программы не может быть неопределенным");

            Regex programReg = new Regex(@"(program|(entry))\s*(([a-zA-Z\d_]+\.*)+)");
            Regex linksReg = new Regex(@"[a-zA-Z\d_]+(\.[a-zA-Z\d_]+)+");

            RegisterProgramCollection result = new RegisterProgramCollection();
            int n;

            MatchCollection matches = programReg.Matches(src);
            List<string> progs = new List<string>(matches.Count);
            int ind;
            n = matches.Count;
            for (int i = 0; i < n; i++)
            {
                ind = progs.IndexOf(matches[i].Groups[3].Value);
                if (ind < 0)
                    progs.Add(matches[i].Groups[3].Value);
                else
                    throw new Exception("Дублирование названия программы");
            }

            matches = linksReg.Matches(src);
            n = matches.Count;
            for (int i = 0; i < n; i++ )
            {
                ind = progs.IndexOf(matches[i].Value);
                if (ind < 0) throw new Exception("Вызов неизвестной программы");

                src = linksReg.Replace(src, ind.ToString(), 1);
            }

            string[] strings = GetProgramsStrings(src);
            n = progs.Count;
            bool entryFound = false;
            RegisterProgram prog;
            string name;
            for(int i = 0; i < n; i++)
            {
                name = progs[i];
                prog = GetProgram(strings[i], i, name);
                result.Add(prog);

                if(entryFound)
                {
                    if (prog.IsEntry)
                        throw new InvalidOperationTextException("Найдено несколько точек входа");
                }
                else
                {
                    if (prog.IsEntry) entryFound = true;
                }
            }
            if (!entryFound)
                throw new InvalidOperationTextException("Точек входа не найдено");

            return result;

        }

        /// <summary>
        /// Проверяет исходный текст программы для ПМБР, содержащей точку входа, и все модули, на которые она ссылается, а затем
        /// формирует единый исходный текст программы, присваивая каждой программе глобальный путь. Не проверяет логические ошибки.
        /// Удалаяет все корректно введенные комментарии.
        /// </summary>
        /// <param name="entryProgramPath">Путь к файлу с исходным текстом программы для ПМБР, содержащем точку входа.</param>
        /// <returns>Подготовленный исходный текст программы для ПМБР.</returns>
        public static string GetFullSource(string entryProgramPath)
        {
            string src = UniteModules(entryProgramPath);

            Regex entryReg = new Regex(@"entry\s*\S*");
            

            MatchCollection matches = entryReg.Matches(src);
            if (matches.Count != 1)
                throw new InvalidOperationTextException("Неверное количество входных точек. Необходима одна точка входа");

            return src;
        }

        /// <summary>
        /// Проверяет исходный текст программы для ПМБР, содержащей точку входа, и все модули, на которые она ссылается, а затем
        /// формирует единый исходный текст программы.
        /// </summary>
        /// <param name="entryProgramPath">Путь к файлу с исходным текстом программы для ПМБР, содержащем точку входа.</param>
        /// <returns>Подготовленный исходный текст программы для ПМБР.</returns>
        private static string UniteModules(string entryProgramPath)
        {
            if (string.IsNullOrWhiteSpace(entryProgramPath))
                throw new ArgumentException("Неверный формат файла исходного текста программы");
            if (!File.Exists(entryProgramPath))
                throw new ArgumentException("Файл исходного текста не найден");

            List<string> units = new List<string>();

            string result = "";
            string src = null;

            Regex unitsReg = new Regex(@"unit\s*(\S*)");
            Regex programsReg = new Regex(@"(program|entry)\s*(\S*)");
            Regex unitLinksReg = new Regex(@"\((\s*[a-zA-Z\d_]+\s*(\.\s*[a-zA-Z\d_]*)+)");
            Regex thisProgsLinksReg = new Regex(@"s\s*\(([a-zA-Z\d_]+\s*),"); // Ищет отсылки к программам этого юнита.
            MatchCollection matches;

            string unitName;
            int i, n;
            string[] arr;
            using (StreamReader sr = File.OpenText(entryProgramPath))
            {
                src = sr.ReadToEnd();
            }
            src = SourceTools.RemoveComments(src);

            matches = unitsReg.Matches(src);
            if (matches.Count != 1)
                throw new InvalidOperationTextException("В модуле однин раз должно присутствовать его объявление");
            n = matches[0].Value.Length;

            unitName = matches[0].Groups[1].Value;
            if (unitName != Path.GetFileNameWithoutExtension(entryProgramPath))
                throw new InvalidOperationTextException("Название модуля должно совпадать с именем файла, в котором он описан");
            if (units.FindIndex(x => x == unitName) >= 0)
                throw new InvalidOperationTextException("Модуль с таким названием уже существует");
            units.Add(unitName);

            matches = programsReg.Matches(src);
            src = programsReg.Replace(src, @"$1 " + unitName + ".$2");
            
            src = thisProgsLinksReg.Replace(src, string.Format("s({0}.$1,", unitName));

            result += src.Substring(n).TrimStart() + '\n';

            matches = unitLinksReg.Matches(src);
            n = matches.Count;
            for(i = 0; i < n; i++)
            {
                arr = matches[i].Groups[1].Value.Split('.');
                Array.Resize<string>(ref arr, arr.Length - 1);
                if (arr[0] != unitName)
                {
                    entryProgramPath = Path.GetDirectoryName(entryProgramPath);
                    result += UniteModules(entryProgramPath + Path.DirectorySeparatorChar + string.Join(Path.DirectorySeparatorChar.ToString(), arr, 0, arr.Length) + ".rmc");
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращает массив строк, содержащих программы для ПМБР.
        /// </summary>
        /// <param name="src">Преобразованный исходный текст программы.</param>
        private static string[] GetProgramsStrings(string src)
        {
            List<string> res = new List<string>();
            int i = 0;
            int j;
            bool entryFound = false;

            while(i < src.Length)
            {
                i = 0;
                if(entryFound)
                {
                    i = src.IndexOf("program");
                    if (i < 0) break;
                }
                else
                {
                    i = src.IndexOf("entry");
                    if (i < 0) throw new InvalidOperationTextException("Входная точка должна присутствовать ровно один раз");

                    entryFound = true;
                }

                j = src.IndexOf('}', i);
                if (j < 0) throw new InvalidOperationTextException("Неверный формат описания программы");
                j++;

                res.Add(src.Substring(i, j - i));

                src = src.Remove(i, j - i);
            }

            return res.ToArray();
        }

        /// <summary>
        /// Возвращает строку блока операций.
        /// </summary>
        /// <param name="block">Строка, содержащая только блок операций.</param>
        /// <returns>Строка, содержащая только последовательность операций, ограниченной блоком.</returns>
        private static string GetOperationsBlock(string block)
        {
            block = block.Trim();
            if (block.Count(x => x == '{' || x == '}') != 2)
                throw new InvalidOperationTextException("Неверный формат блока операций");

            string[] arr = block.Split(new char[]{ '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 1)
                throw new InvalidOperationTextException("Неверный формат блока операций");

            return arr[0].Trim();
        }

        /// <summary>
        /// Возвращает программу для ПМБР из строки, содержащей ее текстовое представление.
        /// </summary>
        /// <param name="src">Строка, содержащая только объявление программы.</param>
        /// <param name="id">Уникальный идентификатор, выдаваемый программе.</param>
        /// <param name="name">Название программы.</param>
        /// <returns>Программа для ПМБР.</returns>
        private static RegisterProgram GetProgram(string src, int id, string name)
        {
            bool isEntry = src.Contains("entry");

            int i = src.IndexOf('{');
            if (i < 0) throw new InvalidOperationTextException("Неверный формат блока");
            src = src.Substring(i + 1);

            i = src.IndexOf('}');
            if (i < 0) throw new InvalidOperationTextException("Неверный формат блока");
            src = src.Remove(i);

            List<RegisterOperation> ops = ReadCommands(src);

            return new RegisterProgram(ops.ToArray(), id, name, isEntry);
        }

        /// <summary>
        /// Возвращает номер объявления.
        /// </summary>
        /// <param name="defStr">Строка, содержащая обявление.</param>
        /// <param name="def">Ключевое слово объявления.</param>
        /// <returns>Номер объявления.</returns>
        private static string GetProgramIndex(string defStr, string def)
        {
            Regex regex = new Regex(@"([a-z]+)\s+(\d+)");
            MatchCollection matches = regex.Matches(defStr);
            if (matches.Count != 1)
                throw new InvalidOperationTextException("Неверный формат объявления");

            if (matches[0].Groups[1].Value != def)
                throw new InvalidOperationTextException(string.Format("Объявления {0} не найдено", def));

            return matches[0].Groups[2].Value;
        }

        /// <summary>
        /// Определяет, являются ли указанные операции допустимыми для выполнения на классической МБР.
        /// Если команды не являются допустимыми, генерируется исключение.
        /// </summary>
        /// <param name="ops">Список операций.</param>
        private static void CheckClassicOperations(List<RegisterOperation> ops)
        {
            List<string> exceptions = new List<string>();

            int n = ops.Count;
            for(int i = 0; i < n; i++)
            {
                try
                {
                    if (ops[i].Id == RegisterOperationId.Start || ops[i].Id == RegisterOperationId.Lock || ops[i].Id == RegisterOperationId.Unlock ||
                     ops[i].Id == RegisterOperationId.Read || ops[i].Id == RegisterOperationId.Write)
                        throw new InvalidOperationTextException(string.Format("Операция {0} не поддерживается классической МБР", i + 1));
                }
                catch (Exception ex)
                {
                    exceptions.Add(string.Format("Команда {0}: \"{1}\"", i + 1, ex.Message));
                }
            }

            if (exceptions.Count > 0)
            {
                string exStr = "Ошибки трансляции операций: \n";
                exceptions.ForEach(x => exStr += x + '\n');
                throw new InvalidOperationTextException(string.Format(" {0}", exStr));
            }
        }

        /// <summary>
        /// Выполняет парсинг всех операций в контексте и возвращает массив таких операций.
        /// </summary>
        /// <param name="ops">Строка, содержащая совокупность строковых представлений операций.</param>
        /// <returns>Массив операций.</returns>
        private static List<RegisterOperation> ReadCommands(string ops)
        {
            ops = ops.Trim();
            List<string> exceptions = new List<string>();

            string[] strArr = ops.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
            List<RegisterOperation> opsList = new List<RegisterOperation>();
            int n = strArr.Length;
            int c = 1;
            for(int i = 0; i < n; i++)
            {
                if (string.IsNullOrWhiteSpace(strArr[i])) continue;

                try
                {
                    opsList.Add(ReadCommand(strArr[i]));
                }
                catch(Exception ex)
                {
                    exceptions.Add(string.Format("Команда {0}: \"{1}\"", c, ex.Message));
                }
                c++;
            }

            if (exceptions.Count > 0)
            {
                InvalidOperationTextException ex = new InvalidOperationTextException("Ошибка трансляции операций");

                n = exceptions.Count;
                for (int i = 0; i < n; i++)
                    ex.Data.Add(i, exceptions[i]);

                throw ex;
            }

            return opsList;
        }

        /// <summary>
        /// Производит парсинг строки, содержащей команду для машины с бесконечными регистрами и переводит её в операцию для машины с бесконечными регистрами.
        /// </summary>
        /// <param name="cmd">Строка, содержащая описание одной операции для МБР.</param>
        /// <returns>Операция для МБР.</returns>
        private static RegisterOperation ReadCommand(string cmd)
        {
            cmd = cmd.Trim();

            RegisterOperationId id;
            BigInteger[] args;

            try
            {
                id = GetId(cmd[0]);
                args = GetArguments(id, cmd.Substring(1).Trim());
            }
            catch(Exception ex)
            {
                throw new InvalidOperationTextException(ex.Message, ex);
            }

            return new RegisterOperation(id, args);
        }

        /// <summary>
        /// Выполняет парсинг аргументов указанной для указанной операции.
        /// </summary>
        /// <param name="id">Уникальный идентификатор операции, аргументы которой парсятся.</param>
        /// <param name="argsStr">Строка, содержащая аргументы операции.</param>
        /// <returns>Массив аргументов.</returns>
        private static BigInteger[] GetArguments(RegisterOperationId id, string argsStr)
        {
            if(argsStr.Count(x => x == '(' || x == ')') != 2)
                throw new InvalidOperationTextException("Неверный формат записи аргументов");
            string[] args = argsStr.Split(new char[]{'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length != 1)
                throw new InvalidOperationTextException("Неверный формат записи аргументов");
            args = args[0].Split(',');

            BigInteger[] result = new BigInteger[args.Length];
            int n = args.Length;
            for (int i = 0; i < n; i++ )
            {
                try
                {
                    result[i] = BigInteger.Parse(args[i]);
                    if (result[i] < 0) throw new Exception();
                }
                catch
                {
                    throw new InvalidOperationTextException(string.Format("Неверный формат {0} параметра", i + 1));
                }
            }

            switch (id)
            {
                case RegisterOperationId.Decision: // Три аргумента.
                    {
                        if (result.Length != 3)
                            throw new InvalidOperationTextException("Неверное количество параметров операции");

                        return result;
                    }
                case RegisterOperationId.Copy: // По два аргумента.
                case RegisterOperationId.Start:
                    {
                        if (result.Length != 2)
                            throw new InvalidOperationTextException("Неверное количество параметров операции");

                        return result;
                    }
                default: // По одному аргументу.
                    {
                        if (result.Length != 1)
                            throw new InvalidOperationTextException("Неверное количество параметров операции");

                        return result;
                    }
            }
        }

        /// <summary>
        /// Возвращает уникальный идентификатор операции по её символу.
        /// </summary>
        /// <param name="ch">Символ определяемой операции.</param>
        /// <returns>Уникальный идентификатор операции.</returns>
        private static RegisterOperationId GetId(char ch)
        {
            switch(ch)
            {
                case 'Z': return RegisterOperationId.Erase;
                case 'S': return RegisterOperationId.Inc;
                case 'T': return RegisterOperationId.Copy;
                case 'J': return RegisterOperationId.Decision;
                case 's': return RegisterOperationId.Start;
                case 'G': return RegisterOperationId.Lock;
                case 'P': return RegisterOperationId.Unlock;
                case 'I': return RegisterOperationId.Read;
                case 'O': return RegisterOperationId.Write;
                default:
                    throw new ArgumentException("Указанный символ не представляет команду для МБР");
            }
        }

        private class StringValueComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.CompareTo(y);
            }
        }
    }
}
