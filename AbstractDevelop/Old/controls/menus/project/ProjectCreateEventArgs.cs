using System;

namespace AbstractDevelop.controls.menus.project
{
    /// <summary>
    /// Представляет аргументы события, возникающего после создания нового проекта.
    /// </summary>
    public class ProjectCreateEventArgs : EventArgs
    {
        private AbstractProject _project;

        /// <summary>
        /// Инициализирует новый экземпляр аргумента указанными параметрами.
        /// </summary>
        /// <param name="project">Созданный проект.</param>
        public ProjectCreateEventArgs(AbstractProject project)
        {
            if (project == null)
                throw new ArgumentNullException("Созданный проект не может быть неопределенным");

            _project = project;
        }

        /// <summary>
        /// Получает ссылку на созданный визуализатор с ассоциированным проектом.
        /// </summary>
        public AbstractProject Project { get { return _project; } }
    }
}