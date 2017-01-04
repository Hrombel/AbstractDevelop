using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Translation
{
    public static class TranslationHelper
    {
        public static bool Translate(this ISourceTranslator translator, IEnumerable input, out IEnumerable output, params object[] args)
        {
            output = translator.Translate(input, args);
            return translator.State.Succeded;
        }

        public static IEnumerable TranslateFile(this ISourceTranslator translator, string file, params object[] args)
            => translator.Translate(file.Try(File.ReadAllLines), args);

        public static bool TranslateFile(this ISourceTranslator translator, string file, out IEnumerable output, params object[] args)
        {
            output = translator.TranslateFile(file, args);
            return translator.State.Succeded;
        }
    }
}
