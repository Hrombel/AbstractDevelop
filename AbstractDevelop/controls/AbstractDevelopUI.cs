using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.controls.menus;
using AbstractDevelop.controls.environment;
using AbstractDevelop.controls.menus.project;
using AbstractDevelop.controls.menus.about;
using AbstractDevelop.controls.menus.help;

namespace AbstractDevelop.controls
{
    /// <summary>
    /// Представляет весь пользовательский интерфейс AbstractDevelop.
    /// </summary>
    public partial class AbstractDevelopUI : UserControl
    {
        /// <summary>
        /// Генерируется, когда пользователь подает сигнал выхода из приложения.
        /// </summary>
        public event EventHandler ExitRequest;

        private CreateProjectMenu _createProjMenu;
        private WorkEnvironment _environment;

        private OpenFileDialog _openDialog;

        public AbstractDevelopUI()
        {
            _openDialog = new OpenFileDialog();
            _openDialog.Filter = "AbstractDevelop project file(*.adp)|*.adp";

            InitializeComponent();
            SetStyle(ControlStyles.Selectable, false);

            InitializeStates();
            ItemsToCreationState();
            backBtn.Visible = false;

            stateCtrl.CurrentControl = _createProjMenu;
        }
        ~AbstractDevelopUI()
        {
            DisposeStates();
        }

        /// <summary>
        /// Определяет, сохранено ли состояние программы.
        /// </summary>
        public bool HasUnsavedData
        {
            get { return _environment.HasUnsavedProject; }
        }

        private void _createProjMenu_ProjectCreate(object sender, ProjectCreateEventArgs e)
        {
            OpenProject(e.Project);
        }

        private void openItem_Click(object sender, EventArgs e)
        {
            if(_openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OpenProject(AbstractProject.Load(_openDialog.FileName));
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format("Произошла ошибка во время открытия проекта: \"{0}\"", ex.Message), ex);
                }
            }
        }

        /// <summary>
        /// Открывает проект в новой вкладке рабочей среды.
        /// </summary>
        /// <param name="project">Открываемый проект.</param>
        private void OpenProject(AbstractProject project)
        {
            _environment.OpenProject(project);
            stateCtrl.CurrentControl = _environment;
            ItemsToEnvState();
            backBtn.Visible = false;
        }

        /// <summary>
        /// Создает все состояния пользовательского интерфейса.
        /// </summary>
        private void InitializeStates()
        {
            _createProjMenu = new CreateProjectMenu();
            _createProjMenu.ProjectCreate += _createProjMenu_ProjectCreate;

            _environment = new WorkEnvironment();
        }

        /// <summary>
        /// Освобождает все ресурсы, связанные с созданными состояниями.
        /// </summary>
        private void DisposeStates()
        {
            _createProjMenu.ProjectCreate -= _createProjMenu_ProjectCreate;
            _createProjMenu.Dispose();
            _createProjMenu = null;

            _environment.Dispose();
            _environment = null;
        }

        private void createItem_Click(object sender, EventArgs e)
        {
            stateCtrl.CurrentControl = _createProjMenu;

            ItemsToCreationState();
            if (_environment.CurrentProjectExists)
                backBtn.Visible = true;
        }

        private void saveItem_Click(object sender, EventArgs e)
        {
            _environment.Save();
        }

        private void closeItem_Click(object sender, EventArgs e)
        {
            _environment.CloseCurrentProject();
            if (!_environment.CurrentProjectExists)
            {
                ItemsToCreationState();
                stateCtrl.CurrentControl = _createProjMenu;
            }
        }

        private void exitItem_Click(object sender, EventArgs e)
        {
            if (ExitRequest != null)
                ExitRequest(this, EventArgs.Empty);
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            stateCtrl.CurrentControl = _environment;
            ItemsToEnvState();
            backBtn.Visible = false;
        }

        /// <summary>
        /// Переводит пункты меню в состояние создания проекта.
        /// </summary>
        private void ItemsToCreationState()
        {
            createItem.Visible = false;
            closeItem.Visible = false;
            saveItem.Visible = false;
        }

        /// <summary>
        /// Переводит пункты меню в состояние среды разработки.
        /// </summary>
        private void ItemsToEnvState()
        {
            createItem.Visible = true;
            closeItem.Visible = true;
            saveItem.Visible = true;
        }

        private void aboutItem_Click(object sender, EventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.ShowDialog();
        }

        private void helpItem_Click(object sender, EventArgs e)
        {
            HelpWindow help = new HelpWindow();
            help.Show();
        }
    }
}
