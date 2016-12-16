using System;

using AbstractDevelop.Machines;

namespace AbstractDevelop.Debug.BreakPoints
{
    public interface IBreakPoint
    {
        event EventHandler TypeChanged;

        /// <summary>
        /// Показывает, выполняется ли условие данной точки останова
        /// </summary>
        bool IsReached { get; }

        /// <summary>
        /// Определяет тип точки останова
        /// </summary>
        BreakPointType Type { get; }

        /// <summary>
        /// Содержит ссылку на абстрактную машину, которой принадлежит данная точка останова
        /// </summary>
        AbstractMachine Owner { get; }
    }

    public abstract class BreakPoint :
        IBreakPoint
    {
        public event EventHandler TypeChanged;

        public abstract bool IsReached { get; }

        public virtual BreakPointType Type => BreakPointType.Inactive;

        public bool IsActive => Type != BreakPointType.Inactive;

        public AbstractMachine Owner => MasterCollection.Owner;

        protected IBreakPointCollection MasterCollection { get; set; }

        protected BreakPoint(IBreakPointCollection master)
        {
            MasterCollection = master;
        }

        protected virtual void OnTypeChanged(EventArgs args)
            => TypeChanged?.Invoke(this, args);
    }   
}