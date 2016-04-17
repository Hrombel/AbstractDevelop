using AbstractDevelop.machines.regmachine;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace AbstractDevelop.controls.ui.ceditor.editors
{
    /// <summary>
    /// Представляет редактор кода для машины с бесконечными регистрами.
    /// </summary>
    public class RegisterCodeEditor : CodeEditor
    {
        private bool _isParallel;

        public RegisterCodeEditor()
        {
            _isParallel = false;
            SetClassic();

            ScintillaInstance.Margins[0].Sensitive = true;

            ScintillaInstance.TextChanged += TextChangedHandler;
            ScintillaInstance.MarginClick += MarginClickHandler;
        }
        ~RegisterCodeEditor()
        {
            ScintillaInstance.TextChanged -= TextChangedHandler;
            ScintillaInstance.MarginClick -= MarginClickHandler;
        }

        private void MarginClickHandler(object sender, MarginClickEventArgs e)
        {
            int i = ScintillaInstance.LineFromPosition(e.Position);

            ToggleBreakPoint(i);
        }

        private void TextChangedHandler(object sender, EventArgs e)
        {
            MarkupLines(_isParallel);
        }

        /// <summary>
        /// Возвращает список указанных пользователем точек останова ПМБР.
        /// </summary>
        /// <returns>Массив точек останова.</returns>
        public List<RegisterBreakPoint> GetParallelBreakPoints()
        {
            if (!_isParallel) throw new InvalidOperationException("Редактор не находится в режиме ПМБР");

            Scintilla scintilla = ScintillaInstance;

            Regex regex = new Regex(@"unit\s+([a-zA-Z\d]+)");
            Match match = regex.Match(scintilla.Text);
            if (!match.Success) throw new Exception("Определение модуля не найдено");

            int[] lines = base.GetBreakPoints();
            int n = lines.Length;
            List<RegisterBreakPoint> result = new List<RegisterBreakPoint>(n);

            string unit = match.Groups[1].Value;
            string name;
            int cmd;
            for(int i = 0; i < n; i++)
            {
                name = GetProgramNameFromLine(lines[i]);
                cmd = int.Parse(scintilla.Lines[lines[i]].MarginText);

                result.Add(new RegisterBreakPoint() { Program = unit + '.' + name, Command = cmd });
            }
            
            return result;
        }

        /// <summary>
        /// Возвращает массив указанных пользователем точек останова классической МБР.
        /// </summary>
        /// <returns>Массив номеров команд, в которых установлены точки останова.</returns>
        public int[] GetClassicBreakPoints()
        {
            if (_isParallel) throw new InvalidOperationException("Редактор находится не в режиме классической МБР");

            int[] result = base.GetBreakPoints();

            Scintilla scintilla = ScintillaInstance;

            return Array.ConvertAll<int, int>(result, x => int.Parse(scintilla.Lines[x].MarginText));
        }

        /// <summary>
        /// Возвращает название программы, тело которой содержит указанную строку.
        /// </summary>
        /// <param name="line">Индекс строки.</param>
        /// <returns>Строка, представляющая название программы.</returns>
        private string GetProgramNameFromLine(int line)
        {
            Regex regex = new Regex(@"(program|entry)\s+([a-zA-Z\d]+)");

            Scintilla scintilla = ScintillaInstance;
            Match m;
            while(line > 0)
            {
                line--;

                m = regex.Match(scintilla.Lines[line].Text);
                if(m.Success)
                    return m.Groups[2].Value;
            }

            throw new Exception("Указанная строка не принадлежит ни одной программе");
        }

        /// <summary>
        /// Выполняет поиск строки, содержащей указанную команду для ПМБР.
        /// </summary>
        /// <param name="program">Название программы, в которой находится команда.</param>
        /// <param name="command">Номер команды в указанной программе.</param>
        /// <returns>Номер строки. Если строка не найдена - -1.</returns>
        public int FindLine(string program, int command)
        {
            if (string.IsNullOrWhiteSpace(program))
                throw new ArgumentException("Название программы имеет неверный формат");
            if (command <= 0)
                throw new ArgumentException("Номер команды должен быть положительным");
            if (!_isParallel)
                throw new InvalidOperationException("Редактор должен находиться в режиме кода для ПМБР");

            Scintilla scintilla = ScintillaInstance;

            Regex programReg = new Regex(@"(program|(entry))\s*(" + program + ")");
            Match match = programReg.Match(scintilla.Text);
            if (!match.Success) throw new ArgumentException("Программы с указанным названием не существует");

            int s = scintilla.LineFromPosition(match.Index);
            int e = scintilla.Text.IndexOf('}', match.Index);
            if (e == -1) throw new Exception("Конец описания программы не найден");
            e = scintilla.LineFromPosition(e);

            string str = command.ToString();
            for(int i = s; i <= e; i++)
            {
                if (scintilla.Lines[i].MarginText == str)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Выполняет поиск строки, содержащей указанную команду для классической МБР.
        /// </summary>
        /// <param name="command">Номер команды в указанной программе.</param>
        /// <returns>Номер строки. Если строка не найдена - -1.</returns>
        public int FindLine(int command)
        {
            if (command <= 0)
                throw new ArgumentException("Номер команды должен быть положительным");
            if (_isParallel)
                throw new InvalidOperationException("Редактор должен находиться в режиме кода для классической МБР");

            string txt = command.ToString();
            int i = ScintillaInstance.Lines.ToList().FindIndex(x => x.MarginText == txt);
            if(i == -1) throw new ArgumentException("Указанной команды не существует");

            return i;
        }

        /// <summary>
        /// Производит нумерацию всех строк.
        /// </summary>
        /// <param name="parallel">Определяет, для какой модификации машины выполнять разметку.</param>
        private void MarkupLines(bool parallel)
        {
            Scintilla scintilla = ScintillaInstance;

            if(parallel)
            {
                int c = 1;
                int n = scintilla.Lines.Count;
                for(int i = 0; i < n; i++)
                {
                    scintilla.Lines[i].MarginStyle = Style.LineNumber;
                    if (scintilla.Lines[i].Text.Contains(")"))
                    {
                        scintilla.Lines[i].MarginText = c.ToString();
                        c++;
                    }
                    else
                    {
                        scintilla.Lines[i].MarginText = "";
                        if (scintilla.Lines[i].Text.Contains("program")) c = 1;
                        else if (scintilla.Lines[i].Text.Contains("entry")) c = 1;
                    }
                }
            }
            else
            {
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
        }

        /// <summary>
        /// Устанавливает настройки синтаксиса для классической МБР.
        /// </summary>
        private void SetClassic()
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

            scintilla.SetKeywords(0, "Z S T J");
        }

        /// <summary>
        /// Устанавливает настройки синтаксиса для параллельной МБР.
        /// </summary>
        private void SetParallel()
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
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            scintilla.Lexer = Lexer.Cpp;

            scintilla.SetKeywords(0, "unit entry program");
            scintilla.SetKeywords(1, "Z S T J s G P I O");
        }

        /// <summary>
        /// Определяет, предназначен ли редактируемый код для параллельной МБР.
        /// </summary>
        public bool IsParallel
        {
            get { return _isParallel; }
            set
            {
                if (_isParallel == value) return;

                _isParallel = value;
                if (_isParallel)
                    SetParallel();
                else
                    SetClassic();
            }
        }
    }
}
