using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.controls.ui.ceditor.editors
{
    /// <summary>
    /// Представляет редактор кода для машины Тьюринга.
    /// </summary>
    public class TuringCodeEditor : CodeEditor
    {
        public TuringCodeEditor()
        {
            CppLexerSet();
            ScintillaInstance.Margins[0].Sensitive = true;

            ScintillaInstance.TextChanged += TextChangedHandler;
            ScintillaInstance.MarginClick += MarginCLickHandler;
        }
        ~TuringCodeEditor()
        {
            ScintillaInstance.TextChanged -= TextChangedHandler;
            ScintillaInstance.MarginClick -= MarginCLickHandler;
        }

        private void MarginCLickHandler(object sender, MarginClickEventArgs e)
        {
            int line = (sender as Scintilla).LineFromPosition(e.Position);

            ToggleBreakPoint(line);
        }

        private void TextChangedHandler(object sender, EventArgs e)
        {
            MarkupLines();
        }

        /// <summary>
        /// Производит нумерацию всех строк.
        /// </summary>
        private void MarkupLines()
        {
            Scintilla scintilla = ScintillaInstance;

            int c = 1;
            int n = scintilla.Lines.Count;
            for (int i = 0; i < n; i++)
            {
                scintilla.Lines[i].MarginStyle = Style.LineNumber;
                if (scintilla.Lines[i].Text.Contains("->"))
                {
                    scintilla.Lines[i].MarginText = c.ToString();
                    c++;
                }
                else
                    scintilla.Lines[i].MarginText = "";
            }
        }

        /// <summary>
        /// Устанавлтвает лексер Cpp и настройки к нему.
        /// </summary>
        private void CppLexerSet()
        {
            Scintilla scintilla = ScintillaInstance;

            scintilla.StyleResetDefault();

            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.StyleClearAll();

            scintilla.Margins[0].Type = MarginType.RightText;
            scintilla.Margins[0].Width = 30;

            scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.Green;
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.Green;
            scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.Gray;
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            scintilla.Lexer = Lexer.Cpp;

            scintilla.SetKeywords(0, "unit program entry");
        }
    }
}
