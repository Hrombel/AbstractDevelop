using AbstractDevelop.Machines;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static ScintillaNET.Style;
using EditColor = System.Drawing.Color;

namespace AbstractDevelop
{
    /// <summary>
    /// Логика взаимодействия для CodeEditor.xaml
    /// </summary>
    public partial class CodeEditor :
        UserControl
    {
        #region [События]

        /// <summary>
        /// Произошла установка точки останова
        /// </summary>
        public event EventHandler BreakPointToggled;

        /// <summary>
        /// Произошло изменение текста
        /// </summary>
        public event EventHandler TextChanged;

        #endregion

        #region [Свойства и Поля]



        public bool IsReadonly
        {
            get
            {
                return (bool)GetValue(IsReadonlyProperty);
            }
            set
            {
                editorComponent.ReadOnly = value;
                SetValue(IsReadonlyProperty, value);
            }
        }
        

        // Using a DependencyProperty as the backing store for IsReadonly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadonlyProperty =
            DependencyProperty.Register("IsReadonly", typeof(bool), typeof(CodeEditor), new PropertyMetadata(false));


        public string Text
        {
            get => editorComponent.Text;
            set => editorComponent.Text = value;
        }

        private List<int> _breakPoints;
        private List<int> _execLines;

        #endregion

        #region [Методы]

        /// <summary>
        /// Выполняет поиск строки, содержащей указанную команду для классической МБР.
        /// </summary>
        /// <param name="command">Номер команды в указанной программе.</param>
        /// <returns>Номер строки. Если строка не найдена - -1.</returns>
        public int FindLine(int command)
        {
            if (command <= 0)
                throw new ArgumentException("Номер команды должен быть положительным");

            string txt = command.ToString();
            int i = editorComponent.Lines.ToList().FindIndex(x => x.MarginText == txt);
            if (i == -1) throw new ArgumentException("Указанной команды не существует");

            return i;
        }


        /// <summary>
        /// Устанавливает символ текущей выполняемой команды. Для этого символа по умолчанию используется 3 маркер Scintilla.
        /// </summary>
        /// <param name="line">Индекс строки.</param>
        public virtual void SetExecutionLine(int line)
        {
            editorComponent.Lines[line].MarkerAdd(3);
            _execLines.Add(line);
        }

        /// <summary>
        /// Добавляет или удаляет символ точки останова. Для этого символа по умолчанию используются 1 и 2 маркеры Scintilla.
        /// </summary>
        /// <param name="line">Индекст строки.</param>
        public void ToggleBreakPoint(int line)
        {
            if (line < 0 || line >= editorComponent.Lines.Count)
                throw new ArgumentOutOfRangeException("Индекс указанной строки вышел за пределы диапазона допустимых значений");

            string text = editorComponent.Lines[line].MarginText;
            int i = text != "" ? int.Parse(text) : -1;
            if (i == -1) return;

            uint mask = editorComponent.Lines[line].MarkerGet();
            if ((mask & 6) == 0)
            {
                _breakPoints.Add(line);

                editorComponent.Lines[line].MarkerAdd(1);
                editorComponent.Lines[line].MarkerAdd(2);
            }
            else
            {
                editorComponent.Lines[line].MarkerDelete(1);
                editorComponent.Lines[line].MarkerDelete(2);
                _breakPoints.RemoveAt(_breakPoints.FindIndex(x => x == line));
            }

            BreakPointToggled?.Invoke(this, EventArgs.Empty);
        }

        private void codeBox_TextChanged(object sender, EventArgs e)
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Выполняет поиск команды для машины Поста среди строк компонента Scintilla.
        /// </summary>
        /// <param name="command">Номер искомой команды.</param>
        /// <returns>Индекс строки, содержащей команду. Если команда не найдена - -1.</returns>
        private int FindCommandLine(int command)
        {
            int n = editorComponent.Lines.Count;
            for (int i = 0; i < n; i++)
            {
                if (command.ToString() == editorComponent.Lines[i].MarginText)
                    return i;
            }

            return -1;
        }

        void Initialize()
        {
            editorComponent.StyleResetDefault();

            editorComponent.Styles[Default].Font = "Consolas";
            editorComponent.Styles[Default].Size = 12;
            editorComponent.StyleClearAll();

            editorComponent.Margins[0].Type = MarginType.RightText;
            editorComponent.Margins[0].Width = 30;

            editorComponent.Styles[Cpp.Default].ForeColor = EditColor.Silver;
            editorComponent.Styles[Cpp.Comment].ForeColor = EditColor.Green;
            editorComponent.Styles[Cpp.CommentLineDoc].ForeColor = EditColor.Gray;
            editorComponent.Styles[Cpp.Number].ForeColor = EditColor.Olive;
            editorComponent.Styles[Cpp.Word].ForeColor = EditColor.Blue;
            editorComponent.Styles[Cpp.Word].Bold = true;
            editorComponent.Lexer = Lexer.Cpp;

            editorComponent.SetKeywords(0, string.Join(" ", new[]
            {
                "in",
                "out",
                "ror",
                "rol",
                "not",
                "or",
                "and",
                "nor",
                "nand",
                "xor",
                "add",
                "sub",
                "jz",
                "jo"
            }));
        }


        private void LinesAmountChanged(object sender, ModificationEventArgs e)
        {
            int line = editorComponent.LineFromPosition(e.Position);
            int n = _breakPoints.Count;
            for (int i = 0; i < n; i++)
            {
                if (_breakPoints[i] >= line)
                    _breakPoints[i] += e.LinesAdded;
            }
            n = _execLines.Count;
            for (int i = 0; i < n; i++)
            {
                if (_execLines[i] >= line)
                    _execLines[i] += e.LinesAdded;
            }

        }

        private void MarginClickHandler(object sender, MarginClickEventArgs e)
        {
            int i = editorComponent.LineFromPosition(e.Position);
            ToggleBreakPoint(i);
        }

        /// <summary>
        /// Выполняет настройку маркеров Scintilla.
        /// </summary>
        void MarkersSetup()
        {
            editorComponent.Markers[1].Symbol = MarkerSymbol.Background;
            editorComponent.Markers[1].SetBackColor(EditColor.Red);

            editorComponent.Markers[2].Symbol = MarkerSymbol.Circle;
            editorComponent.Markers[2].SetForeColor(EditColor.Red);
            editorComponent.Markers[2].SetBackColor(EditColor.Red);

            editorComponent.Markers[3].Symbol = MarkerSymbol.ShortArrow;
            editorComponent.Markers[3].SetForeColor(EditColor.Orange);
            editorComponent.Markers[3].SetBackColor(EditColor.Orange);
        }

        private void TextChangedHandler(object sender, EventArgs e)
        {
            int c = 1;
            int n = editorComponent.Lines.Count;
            for (int i = 0; i < n; i++)
            {
                editorComponent.Lines[i].MarginStyle = LineNumber;
                if (string.IsNullOrWhiteSpace(editorComponent.Lines[i].Text) || editorComponent.Lines[i].Text.SkipWhile((ch) => ch == ' ').First() == '/')
                    editorComponent.Lines[i].MarginText = "";
                else
                {
                    editorComponent.Lines[i].MarginText = c.ToString();
                    c++;
                }
            }
        }

        #endregion

        #region [Конструкторы и деструкторы]

        public CodeEditor()
        {
            InitializeComponent();

            _execLines = new List<int>();
            _breakPoints = new List<int>();
            editorComponent.Margins[0].Sensitive = true;

            MarkersSetup();
            Initialize();

            editorComponent.TextChanged += codeBox_TextChanged;
            editorComponent.Insert += LinesAmountChanged;
            editorComponent.Delete += LinesAmountChanged;
            editorComponent.KeyDown += (o, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.F9)
                    ToggleBreakPoint(editorComponent.CurrentLine);
            };

            editorComponent.TextChanged += TextChangedHandler;
            editorComponent.MarginClick += MarginClickHandler;
        }

        #endregion
    }
}
