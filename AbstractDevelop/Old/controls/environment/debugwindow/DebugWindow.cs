using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractDevelop.controls.environment.debugwindow
{
    /// <summary>
    /// Представляет средство вывода отладочной информации, поступающей в ходе работы абстрактных вычислителей.
    /// </summary>
    public partial class DebugWindow : UserControl
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавляет новое сообщение в окно отладки.
        /// </summary>
        /// <param name="sender">Отправитель сообщения.</param>
        /// <param name="text">Текст сообщения.</param>
        public void AddMessage(string sender, string text)
        {
            if (sender == null)
                throw new ArgumentNullException("Отправитель сообщения не может быть неопределенным");
            if (text == null)
                throw new ArgumentNullException("Текст сообщения не может быть неопределенным");

            ListViewItem item = new ListViewItem();
            item.Text = listView.Items.Count.ToString();
            item.SubItems.Add(sender);
            item.SubItems.Add(text);

            listView.Items.Add(item);
            int n = listView.Columns.Count;
            for (int i = 0; i < n; i++ )
                listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// Удаляет все сообщения из окна отладки.
        /// </summary>
        public void ClearMessages()
        {
            listView.Items.Clear();
        }

        private void clearItem_Click(object sender, EventArgs e)
        {
            ClearMessages();
        }
    }
}
