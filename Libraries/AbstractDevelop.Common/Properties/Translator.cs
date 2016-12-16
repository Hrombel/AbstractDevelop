namespace AbstractDevelop
{
    using Gu.Localization;
    using AbstractDevelop.Properties;
    using System.Resources;
    using System.Linq;

    public static class Translate
    {
        public static string[] Format(params object[] objects)
            => objects.Select(o => o.ToString()).ToArray();

        /// <summary>Call like this: Translate.Key(nameof(Resources.Saved_file__0_)).</summary>
        /// <param name="key">A key in Properties.Resources</param>
        /// <param name="errorHandling">How to handle translation errors like missing key or culture.</param>
        /// <returns>A translation for the key.</returns>
        public static string Key(string key, ResourceManager source = default(ResourceManager), params object[] format)
            => string.Format(TranslationFor(key, ErrorHandling.ReturnErrorInfoPreserveNeutral, source).Translated, format);

        /// <summary>Call like this: Translate.Key(nameof(Resources.Saved_file__0_)).</summary>
        /// <param name="key">A key in Properties.Resources</param>
        /// <param name="errorHandling">How to handle translation errors like missing key or culture.</param>
        /// <returns>A translation for the key.</returns>
        public static ITranslation TranslationFor(string key, ErrorHandling errorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral, ResourceManager source = default(ResourceManager))
            => Gu.Localization.Translation.GetOrCreate(source ?? Resources.ResourceManager, key, errorHandling);
    }
}
