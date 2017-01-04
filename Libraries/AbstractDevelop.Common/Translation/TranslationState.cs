using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Translation
{
    /// <summary>
    /// Представляет состояние трансляции исходного кода
    /// </summary>
    public abstract class TranslationState
    {
        /// <summary>
        /// Номер обрабатываемой в данный момент строки
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Показывает, была ли успешно завершена процедура трансляции
        /// </summary>
        public virtual bool Succeded => Exceptions.Count == 0;

        /// <summary>
        /// Ошибки, озникшие в процессе трансляции
        /// </summary>
        public List<Exception> Exceptions { get; } = new List<Exception>();

        /// <summary>
        /// Производит попытку выполнения указанного действия 
        /// </summary>
        /// <param name="executingAction">Действие для выполнения</param>
        /// <returns></returns>
        public virtual bool TryExecute(Action executingAction)
            => executingAction.Try(ProcessException);

        /// <summary>
        /// Обрабатывает результат возникновения ошибки выполнения
        /// </summary>
        /// <param name="exception">Ошибка для обработки</param>
        /// <returns>Значение, указывающее возможность продолжения работы</returns>
        public virtual bool ProcessException(Exception exception)
            => !exception.Try(Exceptions.Add);
    }
}
