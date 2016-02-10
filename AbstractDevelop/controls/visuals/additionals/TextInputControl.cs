using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AbstractDevelop.controls.visuals.additionals
{
    /// <summary>
    /// Представляет контрол для ввода текстовой строки.
    /// </summary>
    public partial class TextInputControl : UserControl
    {
        /// <summary>
        /// Возникает, когда проверка ввода пройдена и пользователь нажал ОК.
        /// </summary>
        public event EventHandler OKPressed;

        public TextInputControl()
        {
            InitializeComponent();
            okBtn.Enabled = false;
            okBtn.Click += okBtn_Click;
            inputBox.TextChanged += inputBox_TextChanged;
        }
        ~TextInputControl()
        {
            okBtn.Click -= okBtn_Click;
            inputBox.TextChanged -= inputBox_TextChanged;
        }

        private void inputBox_TextChanged(object sender, EventArgs e)
        {
            okBtn.Enabled = CheckText(inputBox.Text);
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (OKPressed != null)
                OKPressed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Получает или задает текст строки ввода.
        /// </summary>
        public string InputText 
        { 
            get { return inputBox.Text; }
            set { inputBox.Text = value; }
        }

        /// <summary>
        /// Получает или задает текущее регулярное выражение для проверки правильности ввода.
        /// </summary>
        public string RegExp { get; set; }

        /// <summary>
        /// Проверяет строку на корректность
        /// </summary>
        /// <param name="name">Проверяемая строка.</param>
        /// <returns>Истина, если строка кооректна, иначе - ложь.</returns>
        private bool CheckText(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            if (string.IsNullOrEmpty(RegExp)) return true;

            Regex reg = new Regex(RegExp);
            Match m = reg.Match(name);
            if (!m.Success) return false;

            return name.Replace(m.Value, "").Length == 0;
        }
    }
}
