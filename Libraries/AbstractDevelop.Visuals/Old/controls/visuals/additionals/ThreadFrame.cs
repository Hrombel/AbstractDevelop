using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.machines;
using System.Collections.ObjectModel;

namespace AbstractDevelop.controls.visuals.additionals
{
    /// <summary>
    /// Представляет контрол для отображения и выбора работающих потоков.
    /// </summary>
    public partial class ThreadFrame : UserControl
    {
        /// <summary>
        /// Возникает, когда пользователь изменяет выделение потоков.
        /// </summary>
        public event EventHandler SelectionChanged;

        public ThreadFrame()
        {
            InitializeComponent();
            listView.ItemChecked += listView_ItemChecked;
        }
        ~ThreadFrame()
        {
            listView.ItemChecked -= listView_ItemChecked;
        }

        private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Возвращает отмеченные пользователем потоки.
        /// </summary>
        /// <returns>Список отмеченных потоков.</returns>
        public List<ThreadInfo> GetSelected()
        {
            List<ThreadInfo> result = new List<ThreadInfo>();

            int n = listView.Items.Count;
            for (int i = 0; i < n; i++)
            {
                if (listView.Items[i].Checked)
                    result.Add(listView.Items[i].Tag as ThreadInfo);
            }

            return result;
        }

        /// <summary>
        /// Обновляет отображаемые данные.
        /// </summary>
        public void UpdateInfo()
        {
            int n = listView.Items.Count;
            int i = 0;
            while (i < n)
            {
                if ((listView.Items[i].Tag as ThreadInfo).Command == 0)
                {
                    listView.Items.RemoveAt(i);
                    n--;
                }
                else
                {
                    listView.Items[i].SubItems[2].Text = (listView.Items[i].Tag as ThreadInfo).Command.ToString();
                    i++;
                }
            }
        }

        /// <summary>
        /// Определяет, содержится ли указанный экземпляр в списке отображения.
        /// </summary>
        /// <param name="info">Проверяемый экземпляр.</param>
        /// <returns>Истина - содержится, иначе - ложь.</returns>
        public bool Contains(ThreadInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("Проверяемая информация не может быть неопределенной");

            int i = listView.Items.OfType<ListViewItem>().ToList().FindIndex(x => (x.Tag as ThreadInfo) == info);

            return i >= 0;
        }

        /// <summary>
        /// Добавляет информацию о потоке в список.
        /// </summary>
        /// <param name="info">Отображаемая базовая информация.</param>
        /// <param name="selected">Определяет, является ли добавляемая информация выделенной.</param>
        public void AddThreadInfo(ThreadInfo info, bool selected)
        {
            if (info == null)
                throw new ArgumentNullException("Отображаемая информация не может быть неопределенной");

            ListViewItem item = new ListViewItem();
            item.Text = info.Id.ToString();
            item.SubItems.Add(info.Program);
            item.SubItems.Add(info.Command.ToString());
            item.Tag = info;
            listView.Items.Add(item);
            item.Checked = selected;

            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// Удаляет информацию о потоке с указанным уникальным идентификатором.
        /// </summary>
        /// <param name="id">Уникальный идентификатор потока, информация о котором удаляется.</param>
        public void RemoveThreadInfo(int id)
        {
            if (id < 0) throw new ArgumentException("Уникальный идентификатор потока не может быть отрицательным");

            int i = listView.Items.OfType<ListViewItem>().ToList().FindIndex(x => (x.Tag as ThreadInfo).Id == id);
            if (i < 0) throw new ArgumentException("Информации о потоке с указанным id не существует в списке");

            listView.Items.RemoveAt(i);
        }

        /// <summary>
        /// Удаляет все оторажаемые потоки.
        /// </summary>
        public void Clear()
        {
            listView.Items.Clear();
        }
    }
}
