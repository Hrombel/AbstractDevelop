using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AbstractDevelop.Translation
{
    /// <summary>
    /// Представляет транслятор исходных данных для абстрактной машины
    /// </summary>
    public interface ISourceTranslator : 
        IDisposable
    {
        /// <summary>
        /// Поддерживаемая кодировка входного потока
        /// </summary>
        Encoding SupportedEncoding { get; }

        /// <summary>
        /// Состояние трансляции исходного кода
        /// </summary>
        TranslationState State { get; }

        /// <summary>
        /// Транслирует исходные данные в набор команд
        /// </summary>
        /// <param name="input">Набор входных данных для трансляции</param>
        /// <returns>Набор команд для исполнения с использованием абстрактнойй машины неустановленного типа</returns>
        IEnumerable Translate(IEnumerable input);

        /// <summary>
        /// Проверяет входную строку на соответствие шаблону исходного текста
        /// и разделяет ее на части для дальнейшей обработки
        /// </summary>
        /// <param name="input">Входная строка для проверки</param>
        /// <param name="composingParts">Части для дальнейшей обработки</param>
        /// <returns></returns>
        bool Validate(string input, out string[] composingParts);
    }

    /// <summary>
    /// Представляет транслятор исходных данных определнного типа для абстрактной машины
    /// </summary>
    /// <typeparam name="OutputType">Тип данных результата трансляции</typeparam>
    /// <typeparam name="RuleSystem">Сисема правил, применяемая при трансляции</typeparam>
    public interface ISourceTranslator<OutputType, RuleSystem> :
        ISourceTranslator
    {
        /// <summary>
        /// Транслирует исходный код в набор инструкций определенного типа
        /// </summary>
        /// <param name="input">Набор строк исходного кода</param>
        /// <returns>Набор инструкций, представленный в указанном формате</returns>
        IEnumerable<OutputType> Translate(IEnumerable<string> input, RuleSystem rules);
    }
}