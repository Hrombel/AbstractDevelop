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
using WKeys = System.Windows.Forms.Keys;

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

        public List<int> BreakPoints
        {
            get => breakpoints;
            set
            {
                if (value != null)
                {
                    // TODO: сделать очистку текстового поля
                    breakpoints.Clear();

                    foreach (var bp in value)
                        ToggleBreakPoint(FindLine(bp + 1)?.Index ?? -1);
                }
            }
        }

        public Scintilla Component => editorComponent;

        public ScintillaNET.Line CurrentLine =>
            editorComponent.Lines[editorComponent.CurrentLine];

        public int ExecutionLine
        {
            get { return executionLine; }
            set
            {
                // удаление предыдущего маркера
                if (executionLine != -1)
                    FindLine(executionLine + 1).MarkerDelete(3);

                var line = FindLine(value + 1);

                if (line != null)
                {
                    line.MarkerAdd(3);
                    executionLine = value;
                }
                else executionLine = -1;
            }
        }

        public int InstructionCount { get; protected set; }

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

        public string[] Lines => Component.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        public string Text
        {
            get => editorComponent.Text;
            set => editorComponent.Text = value;
        }

        // Using a DependencyProperty as the backing store for IsReadonly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadonlyProperty =
            DependencyProperty.Register("IsReadonly", typeof(bool), typeof(CodeEditor), new PropertyMetadata(false));

        List<int> breakpoints = new List<int>();
        int executionLine = -1;
        char[] whitespace = new char[] { ' ' };

        #endregion

        #region [Методы]

        /// <summary>
        /// Выполняет поиск строки, содержащей указанную команду
        /// </summary>
        /// <param name="command">Номер команды в указанной программе.</param>
        /// <returns>Номер строки. Если строка не найдена - -1.</returns>
        public ScintillaNET.Line FindLine(int command)
        {
            if (command <= 0)
                return null;

            string txt = command.ToString();
            return Component.Lines.FirstOrDefault(x => x.MarginText == txt);
         }


        /// <summary>
        /// Добавляет или удаляет символ точки останова. Для этого символа по умолчанию используются 1 и 2 маркеры Scintilla.
        /// </summary>
        /// <param name="line">Индекст строки.</param>
        public void ToggleBreakPoint(int line)
        {
            if (line < 0 || line >= editorComponent.Lines.Count)
                return;

            ToggleBreakPoint(editorComponent.Lines[line]);
        }

        public void ToggleBreakPoint(ScintillaNET.Line line)
        {
            string text = line.MarginText;
            int i = text != "" ? int.Parse(text) : -1;
            if (i == -1) return;

            uint mask = line.MarkerGet();
            if ((mask & 6) == 0)
            {
                BreakPoints.Add(int.Parse(text) - 1);

                line.MarkerAdd(1);
                line.MarkerAdd(2);
            }
            else
            {
                line.MarkerDelete(1);
                line.MarkerDelete(2);
                BreakPoints.Remove(int.Parse(text) - 1);
            }

            BreakPointToggled?.Invoke(this, EventArgs.Empty);
        }


        internal void Refresh(string input)
        {
            editorComponent.Refresh();
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
                            
            editorComponent.Styles[Asm.Default].ForeColor = EditColor.Silver;
            editorComponent.Styles[Asm.Comment].ForeColor = EditColor.Gray;
            editorComponent.Styles[Asm.CommentBlock].ForeColor = EditColor.Gray;
            editorComponent.Styles[Asm.Number].ForeColor = EditColor.Olive;
            editorComponent.Styles[Asm.CpuInstruction].ForeColor = EditColor.Blue;
            editorComponent.Styles[Asm.CpuInstruction].Bold = true;
           
            editorComponent.Lexer = Lexer.Asm;

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
                    "jo",
                    "call",
                    "ret"
                }));
        }


        private void LinesAmountChanged(object sender, ModificationEventArgs e)
        {
            int line = editorComponent.LineFromPosition(e.Position);
            int n = BreakPoints.Count;
            for (int i = 0; i < n; i++)
            {
                if (BreakPoints[i] >= line)
                    BreakPoints[i] += e.LinesAdded;
            }
        }
     
        private void MarginClickHandler(object sender, MarginClickEventArgs e)
        {
            CustomCommands.DebugBreakpoint.Execute(editorComponent.LineFromPosition(e.Position), MainWindow.Instance);
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

                if (string.IsNullOrWhiteSpace(editorComponent.Lines[i].Text)  || editorComponent.Lines[i].Text.SkipWhile((ch) => ch == ' ').First() == ';' || editorComponent.Lines[i].Text.Replace("\r\n", "").Split(whitespace, StringSplitOptions.RemoveEmptyEntries).Last().Contains(":"))
                    editorComponent.Lines[i].MarginText = "";
                else
                {
                    editorComponent.Lines[i].MarginText = c.ToString();
                    InstructionCount = c++;
                }
            }
        }

        internal void Clear()
        {
            editorComponent.ClearAll();
        }

        #endregion

        #region [Конструкторы и деструкторы]

        public CodeEditor()
        {
            InitializeComponent();
            editorComponent.Margins[0].Sensitive = true;

            MarkersSetup();
            Initialize();

            editorComponent.TextChanged += codeBox_TextChanged;
            editorComponent.Insert += LinesAmountChanged;
            editorComponent.Delete += LinesAmountChanged;
            editorComponent.KeyDown += (o, e) =>
            {
                e.SuppressKeyPress = true;
                if (e.Modifiers == WKeys.Control)
                {
                   
                    switch (e.KeyCode)
                    {
                        case WKeys.S: (e.Shift? ApplicationCommands.SaveAs : ApplicationCommands.Save).Execute(null, MainWindow.Instance); break;
                        case WKeys.O: ApplicationCommands.Open.Execute(null, MainWindow.Instance); break;
                        case WKeys.Z: ApplicationCommands.Undo.Execute(null, MainWindow.Instance); break;
                        case WKeys.Y: ApplicationCommands.Redo.Execute(null, MainWindow.Instance); break;

                        default: e.SuppressKeyPress = false; break;
                    }
                }
                else
                {
                    switch (e.KeyCode)
                    {
                        case WKeys.F1: ApplicationCommands.Help.Execute(null, MainWindow.Instance); break;

                        case WKeys.F5: CustomCommands.DebugStartStop.Execute(null, MainWindow.Instance); break;
                        case WKeys.F6: CustomCommands.DebugStep.Execute(null, MainWindow.Instance); break;
                        case WKeys.F7: CustomCommands.DebugPause.Execute(null, MainWindow.Instance); break;
                        case WKeys.F9: CustomCommands.DebugBreakpoint.Execute(null, MainWindow.Instance); break;

                        default: e.SuppressKeyPress = false; break;
                    }
                }

            };

            editorComponent.TextChanged += TextChangedHandler;
            editorComponent.MarginClick += MarginClickHandler;
        }

        #endregion

    }
}
