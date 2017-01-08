using System;

namespace AbstractDevelop.Debug.BreakPoints
{
    [Flags]
    public enum BreakPointType
    {
        /// <summary>
        /// Точка останова, активирующаяся перед выполнением действия
        /// </summary>
        PreAction = 1,

        /// <summary>
        /// Точка останова, активирущаяся после выполнения действия
        /// </summary>
        PostAction = 2,

        /// <summary>
        /// Включает в себя все тип точек останова
        /// </summary>
        All = PreAction | PostAction
    }
}