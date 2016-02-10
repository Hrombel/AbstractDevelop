using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;

namespace AbstractDevelop.controls.ui.ceditor
{
    /// <summary>
    /// Представляет редактор кода под абстрактные вычислители.
    /// </summary>
    public partial class CodeEditor : UserControl
    {
        /// <summary>
        /// Возникает после изменения свойства Text.
        /// </summary>
        public event EventHandler TextChanged;
        /// <summary>
        /// Возникает после каждого добавления или снятия точки останова.
        /// </summary>
        public event EventHandler BreakPointToggled;

        private List<int> _execLines;
        private List<int> _breakPoints;

        public CodeEditor()
        {
            _execLines = new List<int>();
            _breakPoints = new List<int>();
            InitializeComponent();
            SetMarkers();

            scintilla.TextChanged += codeBox_TextChanged;
            scintilla.Insert += LinesAmountChanged;
            scintilla.Delete += LinesAmountChanged;
            scintilla.KeyDown += scintilla_KeyDown;
        }
        ~CodeEditor()
        {
            scintilla.TextChanged -= codeBox_TextChanged;
            scintilla.Insert -= LinesAmountChanged;
            scintilla.Delete -= LinesAmountChanged;
            scintilla.KeyDown -= scintilla_KeyDown;
        }

        /// <summary>
        /// Получает или задает текст, введенный пользователем.
        /// </summary>
        override public string Text
        {
            get { return scintilla.Text; }
            set { scintilla.Text = value; }
        }

        /// <summary>
        /// Получает или задает флаг, определяющий, является ли содержимое редактора доступным только для чтения.
        /// </summary>
        public bool ReadOnly
        {
            get { return scintilla.ReadOnly; }
            set { scintilla.ReadOnly = value; }
        }

        /// <summary>
        /// Получает список номеров строк, в которых установлены точки останова.
        /// </summary>
        /// <returns></returns>
        protected virtual int[] GetBreakPoints()
        {
            return _breakPoints.ToArray();
        }

        /// <summary>
        /// Получает список номеров команд, на которых установлены точки останова.
        /// </summary>
        /// <returns>Массив номеров команд.</returns>
        public virtual int[] GetCommandBreakPoints()
        {
            Scintilla scintilla = ScintillaInstance;
            return _breakPoints.ConvertAll<int>(x => int.Parse(scintilla.Lines[x].MarginText)).ToArray();
        }

        private void LinesAmountChanged(object sender, ModificationEventArgs e)
        {
            int line = scintilla.LineFromPosition(e.Position);
            int n = _breakPoints.Count;
            for(int i = 0; i < n; i++)
            {
                if (_breakPoints[i] >= line)
                    _breakPoints[i] += e.LinesAdded;
            }
            n = _execLines.Count;
            for(int i = 0; i < n; i++)
            {
                if (_execLines[i] >= line)
                    _execLines[i] += e.LinesAdded;
            }

        }

        /// <summary>
        /// Выполняет настройку маркеров Scintilla.
        /// </summary>
        private void SetMarkers()
        {
            scintilla.Markers[1].Symbol = MarkerSymbol.Background;
            scintilla.Markers[1].SetBackColor(Color.Red);

            scintilla.Markers[2].Symbol = MarkerSymbol.Circle;
            scintilla.Markers[2].SetForeColor(Color.Red);
            scintilla.Markers[2].SetBackColor(Color.Red);

            scintilla.Markers[3].Symbol = MarkerSymbol.ShortArrow;
            scintilla.Markers[3].SetForeColor(Color.Orange);
            scintilla.Markers[3].SetBackColor(Color.Orange);
        }

        /// <summary>
        /// Устанавливает символ текущей выполняемой команды. Для этого символа по умолчанию используется 3 маркер Scintilla.
        /// </summary>
        /// <param name="line">Индекс строки.</param>
        public virtual void SetExecutionLine(int line)
        {
            scintilla.Lines[line].MarkerAdd(3);
            _execLines.Add(line);
        }

        /// <summary>
        /// Удаляет все символы текущих выполняемых команд.
        /// </summary>
        public void RemoveAllExecLines()
        {
            while (_execLines.Count > 0)
            {
                scintilla.Lines[_execLines[0]].MarkerDelete(3);
                _execLines.RemoveAt(0);
            }
        }

        /// <summary>
        /// Добавляет или удаляет символ точки останова. Для этого символа по умолчанию используются 1 и 2 маркеры Scintilla.
        /// </summary>
        /// <param name="line">Индекст строки.</param>
        public void ToggleBreakPoint(int line)
        {
            if (line < 0 || line >= ScintillaInstance.Lines.Count)
                throw new ArgumentOutOfRangeException("Индекс указанной строки вышел за пределы диапазона допустимых значений");

            string text = ScintillaInstance.Lines[line].MarginText;
            int i = text != "" ? int.Parse(text) : -1;
            if (i == -1) return;

            uint mask = scintilla.Lines[line].MarkerGet();
            if ((mask & 6) == 0)
            {
                _breakPoints.Add(line);

                scintilla.Lines[line].MarkerAdd(1);
                scintilla.Lines[line].MarkerAdd(2);
            }
            else
            {
                scintilla.Lines[line].MarkerDelete(1);
                scintilla.Lines[line].MarkerDelete(2);
                _breakPoints.RemoveAt(_breakPoints.FindIndex(x => x == line));
            }

            if (BreakPointToggled != null)
                BreakPointToggled(this, EventArgs.Empty);
        }

        /// <summary>
        /// Выполняет поиск команды для машины Поста среди строк компонента Scintilla.
        /// </summary>
        /// <param name="command">Номер искомой команды.</param>
        /// <returns>Индекс строки, содержащей команду. Если команда не найдена - -1.</returns>
        private int FindCommandLine(int command)
        {
            int n = scintilla.Lines.Count;
            for (int i = 0; i < n; i++)
            {
                if (command.ToString() == scintilla.Lines[i].MarginText)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Устанавливает символ выполняемой команды на указанную команду машины Поста.
        /// </summary>
        /// <param name="command">Номер команды.</param>
        public void SetExecutionCommand(int command)
        {
            if (command < 0)
                throw new ArgumentException("Команда не может быть отрицательной");

            int i = FindCommandLine(command);
            if (i == -1) RemoveAllExecLines();
            else SetExecutionLine(i);
        }

        /// <summary>
        /// Получает ссылку на компонент ScintillaNET.
        /// </summary>
        protected Scintilla ScintillaInstance { get { return scintilla; } }

        /// <summary>
        /// Устанавливает текущий шрифт для текста по умолчанию.
        /// </summary>
        /// <param name="font">Устанавливаемый шрифт.</param>
        public void SetDefaultFont(Font font)
        {
            if (font == null)
                throw new ArgumentNullException("Шрифт не может быть неопределенным");

            scintilla.Font = font;
        }

        private void codeBox_TextChanged(object sender, EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, EventArgs.Empty);
        }

        private void scintilla_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F9)
            {
                ToggleBreakPoint(scintilla.CurrentLine);
            }
        }
    }
}
