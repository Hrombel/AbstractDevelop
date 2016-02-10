using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.controls.menus.project;
using AbstractDevelop.controls.menus.project.items;
using AbstractDevelop.projects;
using AbstractDevelop.machines;
using System.IO;

namespace AbstractDevelop.controls.menus
{
    public partial class CreateProjectMenu : UserControl
    {
        /// <summary>
        /// Генерируется после создания пользователем нового проекта.
        /// </summary>
        public event EventHandler<ProjectCreateEventArgs> ProjectCreate;

        private List<IProjectMenuItem> _projects;
        private IProjectMenuItem _selected;
        private FolderBrowserDialog _folderDialog;

        public CreateProjectMenu()
        {
            _projects = new List<IProjectMenuItem>();
            _folderDialog = new FolderBrowserDialog();

            InitializeComponent();
            this.SetStyle(ControlStyles.Selectable, false);
            itemsHolder.AutoScroll = true;

            InitializeProjects();

            createBtn.Enabled = false;
        }

        ~CreateProjectMenu()
        {
            _folderDialog.Dispose();
            _folderDialog = null;
            _selected = null;
            while(_projects.Count > 0)
            {
                _projects.RemoveAt(0);
                (itemsHolder.Controls[0] as Button).Click -= btn_Click;
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);

            int h = GetItemHeight();
            int n = itemsHolder.Controls.Count;
            for (int i = 0; i < n; i++)
                itemsHolder.Controls[i].Height = h;
        }

        /// <summary>
        /// Добавляет необходимые проекты в список проектов меню.
        /// </summary>
        private void InitializeProjects()
        {
            AddProject(new PostMachineProjectItem());
            AddProject(new TuringMachineProjectItem());
            AddProject(new RegisterMachineProjectItem());
        }

        /// <summary>
        /// Добавляет проект в список создаваемых проектов.
        /// </summary>
        /// <param name="project">Создаваемый проект.</param>
        private void AddProject(IProjectMenuItem project)
        {
            if (project == null)
                throw new ArgumentNullException("Добавляемый проект не может быть неопределенным");
            if (!(project is UserControl))
                throw new ArgumentException("Проект должен являться пользовательским элементом");

            _projects.Add(project);

            Button btn = new Button();
            btn.Text = project.ProjectName;
            btn.ForeColor = Color.Black;
            btn.BackColor = Color.WhiteSmoke;
            btn.UseVisualStyleBackColor = true;
            btn.Dock = DockStyle.Top;
            btn.Height = GetItemHeight();
            btn.Tag = project;

            btn.Click += btn_Click;
            itemsHolder.Controls.Add(btn);
            btn.TabIndex = createBtn.TabIndex + _projects.Count;
            btn.BringToFront();

            if (_selected == null)
                SetSelected(project);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            SetSelected((sender as Button).Tag as IProjectMenuItem);
        }

        /// <summary>
        /// Устанавливает элемент в качестве выбранного.
        /// </summary>
        /// <param name="item">Устанавливаемый элемент.</param>
        private void SetSelected(IProjectMenuItem item)
        {
            if (item == _selected) return;

            if(_selected != null)
            {
                infoHolder.Controls.RemoveAt(0);
            }
            _selected = item;
            if(_selected != null)
            {
                infoHolder.Controls.Add(item as UserControl);
                (item as UserControl).Dock = DockStyle.Fill;
            }
        }

        /// <summary>
        /// Получает текущий размер элемента меню.
        /// </summary>
        /// <returns></returns>
        private int GetItemHeight()
        {
            return Height / 10;
        }

        /// <summary>
        /// Создает новый проект с указанным названием и типом абстрактной машины.
        /// </summary>
        /// <param name="name">Название создаваемого проекта.</param>
        /// <param name="machine">Тип абстрактной машины, под который создается проект.</param>
        /// <param name="settings">Начальные установки визуализатора.</param>
        /// <returns>Созданный проект.</returns>
        private AbstractProject CreateProject(string name, MachineId machine, Dictionary<string, bool> settings = null)
        {
            string fileName = nameBox.Text;
            string folder = newFolderBox.Checked ? Path.Combine(_folderDialog.SelectedPath, fileName) : _folderDialog.SelectedPath;

            AbstractProject result = new AbstractProject(name, machine, folder);
            result.VisualizerSettings = settings;

            return result;
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            if(_folderDialog.ShowDialog() == DialogResult.OK)
            {
                AbstractProject proj = CreateProject(nameBox.Text, _selected.Machine, (infoHolder.Controls[0] as IProjectMenuItem).Settings);
                proj.Save();

                if (ProjectCreate != null)
                    ProjectCreate(this, new ProjectCreateEventArgs(proj));
            }
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            createBtn.Enabled = AbstractProject.CheckName(nameBox.Text);
        }

    }
}
