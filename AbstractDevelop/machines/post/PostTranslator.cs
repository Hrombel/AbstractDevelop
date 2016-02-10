using AbstractDevelop.errors.dev;
using AbstractDevelop.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.post
{
    /// <summary>
    /// Представляет средство трансляции исходного текста программы в список команд для для машины Поста.
    /// </summary>
    public sealed class PostTranslator
    {
        public static List<Operation> Translate(string source)
        {
            if (source == null)
                throw new ArgumentNullException("Исходный текст не может быть неопределенным");

            source = SourceTools.RemoveComments(source);

            List<Operation> result = new List<Operation>();
            List<string> errors = new List<string>();

            string[] lines = source.Split('\n');
            int n = lines.Length;

            PostOperationId curId;
            int[] curArgs;

            string word;
            int c = 0;
            for (int i = 0; i < n; i++ )
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                c++;
                word = lines[i].Trim();
                try
                {
                    curId = GetId(word[0].ToString());

                    word = word.Substring(1);
                    curArgs = GetArguments(word, curId);

                    result.Add(new PostOperation(curId, curArgs));
                }
                catch (InvalidOperationTextException ex)
                {
                    errors.Add(string.Format("Строка №{0}: \"{1}\".", c, ex.Message));
                }
            }

            if(errors.Count > 0)
            {
                InvalidOperationTextException ex = new InvalidOperationTextException("Во время трансляции возникли ошибки");
                n = errors.Count;
                for (int i = 0; i < n; i++)
                {
                    ex.Data.Add(i, errors[i]);
                }

                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Получает идентификатор операции машины Поста по её строковому представлению.
        /// </summary>
        /// <param name="cmd">Строковое предтавление операции.</param>
        /// <returns>Идентификатор операции.</returns>
        private static PostOperationId GetId(string cmd)
        {
            switch(cmd.ToLower())
            {
                case "l":
                    return PostOperationId.Left;
                case "r":
                    return PostOperationId.Right;
                case "p":
                    return PostOperationId.Place;
                case "e":
                    return PostOperationId.Erase;
                case "j":
                    return PostOperationId.Decision;
                case "s":
                    return PostOperationId.Stop;
                default:
                    throw new InvalidOperationTextException("Неизвестная операция.");
            }
        }

        /// <summary>
        /// Генерирует исключение о неверном количестве параметров операции.
        /// </summary>
        private static void ThrowInvalidNumParamsException()
        {
            throw new InvalidOperationTextException("Неверное количество параметров операции");
        }

        /// <summary>
        /// Возвращает список аргументов, полученных из их строкового представления.
        /// </summary>
        /// <param name="input">Строковое представление аргументов.</param>
        /// <param name="op">Идентификатор команды, аргументы которой рассматриваются.</param>
        /// <returns>Аргументы команды.</returns>
        private static int[] GetArguments(string input, PostOperationId op)
        {
            int[] result = null;
            List<string> t = input.Trim().Split(' ').ToList();
            t.RemoveAll(x => x == " " || x == "");

            try
            {
                if (op == PostOperationId.Decision)
                {
                    if (t.Count != 2)
                        ThrowInvalidNumParamsException();

                    result = new int[2];
                    result[0] = int.Parse(t[0]);
                    result[1] = int.Parse(t[1]);
                }
                else if (op == PostOperationId.Stop)
                {
                    if (t.Count > 0)
                        ThrowInvalidNumParamsException();

                    result = new int[0];
                }
                else
                {
                    if (t.Count > 1)
                        ThrowInvalidNumParamsException();

                    result = new int[t.Count];
                    if(result.Length == 1)
                    {
                        result[0] = int.Parse(t[0]);
                    }
                }
            }
            catch(InvalidOperationTextException ex)
            {
                throw ex;
            }
            catch
            {
                throw new InvalidOperationTextException("Неверные параметры операции");
            }

            return result;
        }
    }
}
