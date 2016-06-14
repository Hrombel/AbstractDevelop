using AbstractDevelop.controls.environment.debugwindow;
using System;

namespace AbstractDevelop.controls.visuals
{
    /// <summary>
    /// Описывает методы визуализатора абстрактного вычислителя.
    /// </summary>
    public interface IMachineVisualizer
    {
        /// <summary>
        /// Возникает, когда пользователь вносит любые изменения в визуализаторе.
        /// </summary>
        event EventHandler DataChanged;

        /// <summary>
        /// Возникает после сохранения всех изменений, совершенных пользователем.
        /// </summary>
        event EventHandler DataSaved;

        /// <summary>
        /// Возникает после смены состояния визуализатора.
        /// </summary>
        event EventHandler<MachineVisualizerStateChangedEventArgs> OnStateChanged;

        /// <summary>
        /// Получает текущее состояние визуализатора абстрактного вычислителя.
        /// </summary>
        VisualizerState State { get; }

        /// <summary>
        /// Получает или задает проект, созданный для визуализируемого абстрактного вычислителя.
        /// </summary>
        AbstractProject CurrentProject { get; set; }

        /// <summary>
        /// Получает или задает текущее ассоциированное окно отладки.
        /// </summary>
        DebugWindow Debug { get; set; }

        /// <summary>
        /// Сохраняет последние изменения, внесенные пользователем.
        /// </summary>
        void SaveState();

        /// <summary>
        /// Инициирует работу абстрактного вычислителя.
        /// </summary>
        void Run();

        /// <summary>
        /// Инициирует полную остановку абстрактного вычислителя.
        /// </summary>
        void Stop();

        /// <summary>
        /// Инициирует временную остановку работы абстрактного вычислителя.
        /// </summary>
        void Pause();

        /// <summary>
        /// Инициирует возоновление работы абстрактного вычислителя.
        /// </summary>
        void Continue();

        /// <summary>
        /// Инициирует выполнение следующей операции абстрактным вычислителем.
        /// </summary>
        void Step();
    }
}