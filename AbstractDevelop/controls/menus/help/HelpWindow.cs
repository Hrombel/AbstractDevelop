using AbstractDevelop.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractDevelop.controls.menus.help
{
    /// <summary>
    /// Представляет окно для отображения помощи по программе.
    /// </summary>
    public partial class HelpWindow : Form
    {
        public HelpWindow()
        {
            InitializeComponent();
            browser.DocumentText = "<h1>Выберите раздел</h1>";
        }

        /// <summary>
        /// Отображает информацию, связанную с указанным узлом.
        /// </summary>
        /// <param name="node">Узел, связанная информация с которым отображается.</param>
        private void ShowInfo(TreeNode node)
        {
            if(node.Parent == null)
            {
                browser.DocumentText = "<h1>Выберите раздел</h1>";
                return;
            }
            if (node.Tag == null)
            {
                browser.DocumentText = "<h1>Нет информации</h1>";
                return;
            }
            
            string res = Resources.ResourceManager.GetString(node.Tag as string);

            browser.DocumentText = res != null ? res : "<h1>Неверная ссылка на ресурс</h1>";
        }

        private void NodeClickHandler(object sender, TreeNodeMouseClickEventArgs e)
        {
            ShowInfo(e.Node);
        }
    }
}
