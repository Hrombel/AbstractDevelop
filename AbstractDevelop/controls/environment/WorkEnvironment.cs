using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.controls.environment.toolpanel;
using AbstractDevelop.machines.post;
using AbstractDevelop.controls.visuals;
using AbstractDevelop.machines.turing;
using AbstractDevelop.machines.regmachine;
using AbstractDevelop.projects;
using AbstractDevelop.machines;

namespace AbstractDevelop.controls.environment
{
    /// <summary>
    /// Представляет рабочую среду пользователя для взаимодействия с абстрактными вычислителями.
    /// </summary>
    public partial class WorkEnvironment : UserControl
    {

        public WorkEnvironment()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable, false);

            toolPanel.OnButtonPressed += toolPanel_OnButtonPressed;
            visualsCtrl.CurrentVisualChanged += visualsCtrl_CurrentVisualChanged;
        }
        ~WorkEnvironment()
        {
            visualsCtrl.CurrentVisualChanged -= visualsCtrl_CurrentVisualChanged;
            toolPanel.OnButtonPressed -= toolPanel_OnButtonPressed;
        }

        /// <summary>
        /// Сохраняет текущее состояние среды.
        /// </summary>
        public void Save()
        {
            if(visualsCtrl.CurrentVisualizer != null)
            {
                visualsCtrl.CurrentVisualizer.SaveState();
                visualsCtrl.CurrentVisualizer.CurrentProject.Save();
            }
        }

        /// <summary>
        /// Определяет, есть ли среди открытых проектов несохраненные.
        /// </summary>
        public bool HasUnsavedProject
        {
            get
            {
                return visualsCtrl.HasUnsavedData;
            }
        }

        private void visualsCtrl_CurrentVisualChanged(object sender, CurrentVisualizerChangeEventArgs e)
        {
            if (e.Previous != null)
                e.Previous.OnStateChanged -= CurrentVisualizer_OnStateChanged;

            UpdateToolState();

            if(visualsCtrl.CurrentVisualizer != null)
                visualsCtrl.CurrentVisualizer.OnStateChanged += CurrentVisualizer_OnStateChanged;
        }

        /// <summary>
        /// Выполняет обновление состояния панели инструментов в соответствии с состоянием текущей отображаемой машины.
        /// </summary>
        private void UpdateToolState()
        {
            if(visualsCtrl.CurrentVisualizer != null)
            {
                toolPanel.Enabled = true;
                switch (visualsCtrl.CurrentVisualizer.State)
                {
                    case VisualizerState.Paused:
                        toolPanel.PerformClick(ToolPanelButton.Pause);
                        break;
                    case VisualizerState.Executing:
                        toolPanel.PerformClick(ToolPanelButton.Play);
                        break;
                    case VisualizerState.Stopped:
                        toolPanel.PerformClick(ToolPanelButton.Stop);
                        break;
                }
            }
            else
            {
                toolPanel.PerformClick(ToolPanelButton.Stop);
                toolPanel.Enabled = false;
            }
        }

        private void CurrentVisualizer_OnStateChanged(object sender, MachineVisualizerStateChangedEventArgs e)
        {
            UpdateToolState();
        }

        /// <summary>
        /// Определяет, открыт ли в данный момент какой-либо проект.
        /// </summary>
        public bool CurrentProjectExists { get { return visualsCtrl.CurrentVisualizer != null; } }

        /// <summary>
        /// Закрывает текущий открытый проект.
        /// </summary>
        public void CloseCurrentProject()
        {
            if (visualsCtrl.CurrentVisualizer == null) return;

            visualsCtrl.CloseVisualizer(visualsCtrl.CurrentVisualizer);
        }

        /// <summary>
        /// Открывает указанный проект в новой вкладке.
        /// </summary>
        /// <param name="project">Открываемый проект.</param>
        public void OpenProject(AbstractProject project)
        {
            if (project == null)
                throw new ArgumentNullException("Открываемый проект не может быть неопределенным");

            IMachineVisualizer vis = visualsCtrl.GetOpenedProject(project.ProjectDirectory, project.Name);
            if (vis != null)
                visualsCtrl.CloseVisualizer(vis);
            
            switch(project.Machine)
            {
                case MachineId.Post:
                    {
                        visualsCtrl.AddVisualizer(new PostVisualizer() { CurrentProject = project, Debug = debug });
                        break;
                    }
                case MachineId.Turing:
                    {
                        visualsCtrl.AddVisualizer(new TuringVisualizer() { CurrentProject = project, Debug = debug });
                        break;
                    }
                case MachineId.Register:
                    {
                        visualsCtrl.AddVisualizer(new RegisterMachineVisualizer() { CurrentProject = project, Debug = debug });
                        break;
                    }
                default:
                    throw new ArgumentException("Невозможно открыть проект, поскольку он имеет неизвестный тип абстрактной машины");
            }
        }

        private void toolPanel_OnButtonPressed(object sender, ToolPanelEventArgs e)
        {
            try
            {
                switch (e.Button)
                {
                    case ToolPanelButton.Play:
                        {
                            if (visualsCtrl.CurrentVisualizer.State == VisualizerState.Stopped)
                                visualsCtrl.CurrentVisualizer.Run();
                            else if (visualsCtrl.CurrentVisualizer.State == VisualizerState.Paused)
                                visualsCtrl.CurrentVisualizer.Continue();
                            else
                                throw new InvalidOperationException("Неизветный контекст нажатия кнопки Play");
                            break;
                        }
                    case ToolPanelButton.Pause:
                        {
                            visualsCtrl.CurrentVisualizer.Pause();
                            break;
                        }
                    case ToolPanelButton.Step:
                        {
                            visualsCtrl.CurrentVisualizer.Step();
                            break;
                        }
                    case ToolPanelButton.Stop:
                        {
                            visualsCtrl.CurrentVisualizer.Stop();
                            break;
                        }
                }
            }
            catch(Exception ex)
            {
                e.Success = false;
            }
        }
    }
}
