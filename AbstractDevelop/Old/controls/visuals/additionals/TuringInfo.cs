using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractDevelop.controls.visuals.additionals
{
    
    /// <summary>
    /// Представляет контрол для отображения информации о внешнем алфавите машины Тьюринга.
    /// </summary>
    public partial class TuringInfo : UserControl
    {
        /// <summary>
        /// Возникает после изменения свойства DefaultChar.
        /// </summary>
        public event EventHandler DefaultCharChanged;

        private const char DEFAULT_CHAR = '~';

        private char[] _alphabet;

        public TuringInfo()
        {
            InitializeComponent();
            defBox.Text = DEFAULT_CHAR.ToString();
            alphBox.Text = "[ ]";

            defBox.KeyPress += defBox_KeyPress;
            defBox.TextChanged += defBox_TextChanged;
        }
        ~TuringInfo()
        {
            defBox.KeyPress -= defBox_KeyPress;
            defBox.TextChanged -= defBox_TextChanged;
        }

        /// <summary>
        /// Получает или задает текущий установленный символ пустой ячейки ленты.
        /// </summary>
        public char DefaultChar
        {
            get { return defBox.Text[0]; }
            set
            {
                if (!IsCorrect(value))
                    throw new Exception("Указанный символ недопустим");

                defBox.Text = value.ToString();
                defBox.SelectionStart = 1;

                if (DefaultCharChanged != null)
                    DefaultCharChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Получает или задает отображаемый внешний алфавит.
        /// </summary>
        public char[] Alphabet
        {
            get { return _alphabet; }
            set
            {
                if (_alphabet != null)
                    alphBox.Text = "[ ]";

                _alphabet = value;

                UpdateInfo();
            }
        }

        /// <summary>
        /// Перерисовывает информацию о внешнем алфавите.
        /// </summary>
        public void UpdateInfo()
        {
            if (_alphabet != null)
            {
                string str = "[ ";
                Array.ForEach(_alphabet, x => str += x + ", ");
                if (_alphabet.Length > 0) str.Remove(str.Length - 2);
                str += ']';
                alphBox.Text = str;
            }
            else
            {
                alphBox.Text = "[ ]";
            }
        }

        /// <summary>
        /// Проверяет указанный символ на допустимость.
        /// </summary>
        /// <param name="symbol">Проверяемый символ.</param>
        /// <returns>Истина - символ допустим, иначе - недопустим.</returns>
        private bool IsCorrect(char symbol)
        {
            return !(char.IsWhiteSpace(symbol) || char.IsControl(symbol));
        }

        private void defBox_TextChanged(object sender, EventArgs e)
        {
            if (defBox.Text == "") DefaultChar = DEFAULT_CHAR;
        }

        private void defBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (IsCorrect(e.KeyChar))
                DefaultChar = e.KeyChar;
            else
                DefaultChar = DEFAULT_CHAR;
        }
    }
}
