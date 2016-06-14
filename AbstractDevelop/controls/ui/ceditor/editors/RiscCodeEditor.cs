using AbstractDevelop.machines.regmachine;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace AbstractDevelop.controls.ui.ceditor.editors
{
    public class RiscCodeEditor :
        CodeEditor
    {
        List<string> keywords = new List<string>()
        {
            "in", "out", "ror", "rol", "not", "or", "and", "nor", "nand", "xor", "add", "sub", "jz", "jo"
        };

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
            int i = ScintillaInstance.Lines.ToList().FindIndex(x => x.MarginText == txt);
            if (i == -1) throw new ArgumentException("Указанной команды не существует");

            return i;
        }
     
        private void MarginClickHandler(object sender, MarginClickEventArgs e)
        {
            int i = ScintillaInstance.LineFromPosition(e.Position);
            ToggleBreakPoint(i);
        }

        /// <summary>
        /// Устанавливает настройки синтаксиса для классической МБР.
        /// </summary>
        private void Initialize()
        {
            Scintilla scintilla = ScintillaInstance;

            scintilla.StyleResetDefault();

            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 12;
            scintilla.StyleClearAll();

            scintilla.Margins[0].Type = MarginType.RightText;
            scintilla.Margins[0].Width = 30;

            scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.Green;
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.Green;
            scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.Gray;
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.Word].Bold = true;
            scintilla.Lexer = Lexer.Cpp;

            scintilla.SetKeywords(0, string.Join(" ", keywords));
        }

        private void TextChangedHandler(object sender, EventArgs e)
        {
            Scintilla scintilla = ScintillaInstance;

            int c = 1;
            int n = scintilla.Lines.Count;
            for (int i = 0; i < n; i++)
            {
                scintilla.Lines[i].MarginStyle = Style.LineNumber;
                if (string.IsNullOrWhiteSpace(scintilla.Lines[i].Text) || scintilla.Lines[i].Text.SkipWhile((ch) => ch == ' ').First() == '/')
                    scintilla.Lines[i].MarginText = "";
                else
                {
                    scintilla.Lines[i].MarginText = c.ToString();
                    c++;
                }
            }
        }

        #endregion

        #region [Конструкторы]

        public RiscCodeEditor()
        {
            ScintillaInstance.Margins[0].Sensitive = true;
            Initialize();

            ScintillaInstance.TextChanged += TextChangedHandler;
            ScintillaInstance.MarginClick += MarginClickHandler;
        }

        ~RiscCodeEditor()
        {
            ScintillaInstance.TextChanged -= TextChangedHandler;
            ScintillaInstance.MarginClick -= MarginClickHandler;
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RiscCodeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "RiscCodeEditor";
            
            this.ResumeLayout(false);

        }
    }
}