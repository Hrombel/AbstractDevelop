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
    /// Представляет редактор кода для машины Поста.
    /// </summary>
    public class PostCodeEditor : CodeEditor
    {
        public PostCodeEditor()
        {
            AsmLexerSet();

            ScintillaInstance.Margins[0].Sensitive = true;

            ScintillaInstance.TextChanged += TextChangedHandler;
            ScintillaInstance.MarginClick += ScintillaMarginClickHandler;
        }
        ~PostCodeEditor()
        {
            ScintillaInstance.TextChanged -= TextChangedHandler;
            ScintillaInstance.MarginClick -= ScintillaMarginClickHandler;
        }

        private void ScintillaMarginClickHandler(object sender, MarginClickEventArgs e)
        {
            Scintilla scintilla = sender as Scintilla;
            int line = scintilla.LineFromPosition(e.Position);

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
                if (string.IsNullOrWhiteSpace(scintilla.Lines[i].Text))
                    scintilla.Lines[i].MarginText = "";
                else
                {
                    scintilla.Lines[i].MarginText = c.ToString();
                    c++;
                }
            }
        }

        /// <summary>
        /// Устанавливает лексер ассемблера и настройки к нему в компонент ScintillaNET.
        /// </summary>
        private void AsmLexerSet()
        {
            Scintilla scintilla = ScintillaInstance;

            scintilla.StyleResetDefault();

            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.StyleClearAll();

            scintilla.Margins[0].Type = MarginType.RightText;
            scintilla.Margins[0].Width = 30;

            scintilla.Styles[Style.Asm.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Asm.Comment].ForeColor = Color.Green;
            scintilla.Styles[Style.Asm.CommentBlock].ForeColor = Color.Green;
            scintilla.Styles[Style.Asm.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Asm.CpuInstruction].ForeColor = Color.Blue;
            scintilla.Lexer = Lexer.Asm;

            scintilla.SetKeywords(0, "S s J j P p E e R r L l");
        }

    }
}
