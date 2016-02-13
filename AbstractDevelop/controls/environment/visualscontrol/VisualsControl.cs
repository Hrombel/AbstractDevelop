using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AbstractDevelop.controls.visuals;
using AbstractDevelop.machines;

namespace AbstractDevelop.controls.environment.tabcontrol
{
    /// <summary>
    /// Представляет средство отображения визуализаторов абстрактных вычислителей и переключения между ними.
    /// </summary>
    public partial class VisualsControl : UserControl
    {
        /// <summary>
        /// Возникает при смене текущего отображаемого визуализатора.
        /// </summary>
        public event EventHandler<CurrentVisualizerChangeEventArgs> CurrentVisualChanged;

        private List<IMachineVisualizer> _machines;
        private IMachineVisualizer _current;

        public VisualsControl()
        {
            _machines = new List<IMachineVisualizer>();
            InitializeComponent();
            tabControl.Selected += tabControl_TabIndexChanged;
        }
        ~VisualsControl()
        {
            tabControl.Selected -= tabControl_TabIndexChanged;
        }

        /// <summary>
        /// Получает визуализатор по ассоциированному с ним проекту. Если такового нет - null.
        /// </summary>
        /// <param name="dir">Директория, в которой находится проект.</param>
        /// <param name="name">Имя проекта.</param>
        public IMachineVisualizer GetOpenedProject(string dir, string name)
        {
            if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(name))
                throw new ArgumentException("Входная строка имеет неверный формат");

            return _machines.Find(x => x.CurrentProject.ProjectDirectory == dir && x.CurrentProject.Name == name);
        }

        /// <summary>
        /// Определяет, есть ли среди открытых визуализаторов несохраненные.
        /// </summary>
        public bool HasUnsavedData
        {
            get
            {
                foreach(TabPage page in tabControl.TabPages)
                {
                    if (page.Text.Contains("*"))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Устанавливает текущий оторажаемый визуализатор.
        /// </summary>
        /// <param name="current">Устанавливаемый визуализатор.</param>
        private void SetCurrentVisualizer(IMachineVisualizer current)
        {
            if(_current != null)
            {
                _current.DataChanged -= _current_DataChanged;
                _current.DataSaved -= _current_DataSaved;
            }
            IMachineVisualizer old = _current;
            _current = current;
            if(_current != null)
            {
                _current.DataChanged += _current_DataChanged;
                _current.DataSaved += _current_DataSaved;
            }
            if (CurrentVisualChanged != null)
                CurrentVisualChanged(this, new CurrentVisualizerChangeEventArgs(old));
        }

        private void _current_DataSaved(object sender, EventArgs e)
        {
            UpdateTabState(false);
        }

        private void _current_DataChanged(object sender, EventArgs e)
        {
            UpdateTabState(true);
        }

        private void tabControl_TabIndexChanged(object sender, EventArgs e)
        {
            SetCurrentVisualizer(tabControl.SelectedTab != null ? tabControl.SelectedTab.Tag as IMachineVisualizer : null);
        }

        /// <summary>
        /// Устанавливает текущее состояние открытой вкладки.
        /// </summary>
        /// <param name="unsaved">Истина - открытый проект имеет несохраненные изменения, иначе - проект сохранен.</param>
        private void UpdateTabState(bool unsaved)
        {
            IMachineVisualizer vis = tabControl.SelectedTab.Tag as IMachineVisualizer;

            string name = string.Format("{0} - {1}", vis.CurrentProject.Name, GetMachineShortName(vis.CurrentProject.Machine));
            if (unsaved) name += '*';
            if(tabControl.SelectedTab.Text != name)
                tabControl.SelectedTab.Text = name;
        }

        /// <summary>
        /// Добавляет новый визуализатор в список визуализаторов компонента.
        /// </summary>
        /// <param name="visualizer">Пользовательский элемент управления-визуализатор.</param>
        public void AddVisualizer(IMachineVisualizer visualizer)
        {
            if (visualizer == null)
                throw new ArgumentNullException("Добавляемый визуализатор не может быть неопределенным");
            if (!(visualizer is UserControl))
                throw new ArgumentException("Добавляемый визуализатор должен являться пользовательским элементом управления Windows Forms");

            _machines.Add(visualizer);
            AddTab(visualizer);
        }

        /// <summary>
        /// Закрывает активный визуализатор.
        /// </summary>
        public void RemoveCurrentVisualizer()
        {
            if (tabControl.SelectedTab == null)
                throw new InvalidOperationException("В данный момент ни один визуализатор не отображается");

            CloseVisualizer(CurrentVisualizer);
        }

        /// <summary>
        /// Получает текущий отображаемый визуализатор.
        /// </summary>
        public IMachineVisualizer CurrentVisualizer { get { return _current; } }

        /// <summary>
        /// Удаляет визуализатор из списка вкладок.
        /// </summary>
        /// <param name="visualizer">Удаляемый визуализатор.</param>
        public void CloseVisualizer(IMachineVisualizer visualizer)
        {
            int n = tabControl.TabPages.Count;
            for(int i = 0; i < n; i++)
            {
                if(tabControl.TabPages[i].Tag == visualizer)
                {
                    tabControl.TabPages.RemoveAt(i);
                    _machines.Remove(visualizer);

                    SetCurrentVisualizer(tabControl.SelectedTab != null ? tabControl.SelectedTab.Tag as IMachineVisualizer : null);
                    return;
                }
            }
            throw new Exception("Закрываемая вкладка не найдена");
        }

        /// <summary>
        /// Получает краткое название машины по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор машины.</param>
        /// <returns>Название машины.</returns>
        private string GetMachineShortName(MachineId id)
        {
            switch(id)
            {
                case MachineId.Post: return "МП";
                case MachineId.Turing: return "МТ";
                case MachineId.Register: return  "МБР";
            }

            return "???";
        }

        /// <summary>
        /// Добавляет визуализатор в список вкладок.
        /// </summary>
        /// <param name="visualizer">Добавляемый визуализатор.</param>
        private void AddTab(IMachineVisualizer visualizer)
        {
            UserControl ctrl = visualizer as UserControl;
            ctrl.Dock = DockStyle.Fill;

            TabPage page = new TabPage();
            page.Text = string.Format("{0} - {1}", visualizer.CurrentProject.Name, GetMachineShortName(visualizer.CurrentProject.Machine));
            page.Controls.Add(ctrl);
            page.Tag = visualizer;

            tabControl.TabPages.Add(page);
            tabControl.SelectedTab = page;

            SetCurrentVisualizer(tabControl.SelectedTab != null ? tabControl.SelectedTab.Tag as IMachineVisualizer : null);
        }

    }
}
