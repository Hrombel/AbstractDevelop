using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using AbstractDevelop.Machines;

namespace AbstractDevelop.Debug.BreakPoints
{
    /// <summary>
    /// Представляет коллекцию точек остаанова
    /// </summary>
    public interface IBreakPointCollection :
        ICollection<IBreakPoint>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region [Свойства и Поля]

        /// <summary>
        /// Владелец коллекции точек остнова
        /// </summary>
        AbstractMachine Owner { get; }

        #endregion
    }

    /// <summary>
    /// Реализует коллекцию точек останова
    /// </summary>
    /// <remarks>Реализуется через <see cref="ObservableCollection{BreakPoint}"/>/remarks>
    public class BreakPointCollection :
       ObservableCollection<IBreakPoint>, IBreakPointCollection, IDisposable
    {
        #region [Свойства и Поля]

        /// <summary>
        /// Владелец коллекции точек остнова
        /// </summary>
        public AbstractMachine Owner { get; }

        /// <summary>
        /// Показывает, были ри ресурсы данной коллекции освобождены ранее
        /// </summary>
        protected bool IsDisposed { get; set; }

        /// <summary>
        /// Показвает, должна ли данная коллекция подавлять отправку уведомлений о событиях
        /// </summary>
        protected bool SuppressNotifications { get; set; }

        #endregion

        #region [Методы]

        /// <summary>
        /// Освобождает все ресурсы, связанные с данной коллекцией
        /// </summary>
        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                // перед очисткой необходимо включить
                // режим подавления рассылки уведомлений
                SuppressNotifications = true;

                // очистка коллекции
                while (Count > 0)
                    RemoveAt(0);

                // пометка о выполненной очистке
                IsDisposed = true;
            }
            else throw new InvalidOperationException($"Невозможно повторно освободить ресурсы объекта {nameof(BreakPointCollection)}");
        }

        /// <summary>
        /// Обрабатывает создание событий изменения коллекции
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!SuppressNotifications)
                base.OnCollectionChanged(e);

            // в случае подавления уведомления не рассылаются
        }

        #endregion
    }
}