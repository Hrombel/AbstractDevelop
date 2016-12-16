namespace AbstractDevelop.Debug.BreakPoints
{
    public enum BreakPointType
    {
        /// <summary>
        /// Неактивная точка останова
        /// </summary>
        Inactive,
        /// <summary>
        /// Точка останова, активирующаяся перед выполнением действия
        /// </summary>
        PreAction,
        /// <summary>
        /// Точка останова, активирущаяся после выполнения действия
        /// </summary>
        PostAction
    }
}