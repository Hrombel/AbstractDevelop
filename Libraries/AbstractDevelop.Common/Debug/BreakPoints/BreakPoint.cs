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
    }

    public abstract class BreakPoint :
        IBreakPoint
    {
        public event EventHandler TypeChanged;

        public abstract bool IsReached { get; }

        public virtual BreakPointType Type => BreakPointType.All;

        public bool IsActive => Type != BreakPointType.All;
        
        protected virtual void OnTypeChanged(EventArgs args)
            => TypeChanged?.Invoke(this, args);
    }   
}