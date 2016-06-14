using AbstractDevelop.machines.registers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;

namespace AbstractDevelop.controls.visuals
{
    /// <summary>
    /// Представляет средство визуализации модели бесконечного набора регистров, хранящих бесконечно большие целые неотрицательные числа.
    /// </summary>
    public partial class InfiniteRegistersVisualizer : UserControl
    {
        #region [События]

        /// <summary>
        /// Возникает, когда изменяется состояние регистров.
        /// </summary>
        public event EventHandler StateChanged;

        #endregion

        #region [Свойства]

        /// <summary>
        /// Получает или задает количество столбцов визуализатора. Значения могут находиться в диапазоне от 1 до MAX_COLUMNS.
        /// </summary>
        public int Columns
        {
            get { return _columns; }
            set
            {
                if (value < 1)
                    _columns = 1;
                else if (value > MAX_COLUMNS)
                    _columns = 15;

                _columns = value;
            }
        }

        /// <summary>
        /// Получает или задает текущие визуализируемые регистры.
        /// </summary>
        public InfiniteRegisters Registers
        {
            get { return _registers; }
            set
            {
                if (_registers != null)
                {
                    DisposeControls();
                }
                _registers = value;
                if (_registers != null)
                {
                    InitControls();
                }
            }
        }

        #endregion

        #region [Поля]

        public const int MAX_COLUMNS = 15;

        private const int FRAME_RATE = 40;

        private int _blockHeight;
        private int _blockWidth;
        private Bitmap _canvas;
        private int _columns = 5;
        private BigInteger _currentRegister;
        private bool _currentRegisterBig;
        private BigInteger _currentRegisterWidth;
        private int _lastPos;
        private int _margin;
        private bool _mouseDown;
        private bool _mouseMoved;
        private BigInteger _pos;
        private InfiniteRegisters _registers;
        private Timer _timer;
        private Font _valueFont;
        private float _velocity;

        #endregion

        #region [Методы]

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);

            DestroyCanvas();
            CreateCanvas();

            ClearView();
            DrawCells(_pos);
            UpdateView();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_mouseDown)
            {
                _velocity = _lastPos - MousePosition.Y;
                _lastPos = MousePosition.Y;

                if (!_mouseMoved)
                {
                    if (_velocity != 0)
                        _mouseMoved = true;
                }
            }
            else
            {
                if (Math.Abs(_velocity) < 1.0)
                    _velocity = 0;
                else
                    _velocity *= 0.95f;
            }

            _pos += (int)Math.Round(_velocity);
            if (_pos < 0)
            {
                _pos = 0;
                _velocity = 0;
            }

            ClearView();
            DrawCells(_pos);
            UpdateView();
        }

        /// <summary>
        /// Очищает отрисованную на полотне графику.
        /// </summary>
        private void ClearView()
        {
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                g.Clear(Color.White);
            }
        }

        /// <summary>
        /// Создает полотно с размерами компонента.
        /// </summary>
        private void CreateCanvas()
        {
            _canvas = new Bitmap(Width > 0 ? Width : 1, Height > 0 ? Height : 1);
        }

        /// <summary>
        /// Освобождает ресурсы полотна.
        /// </summary>
        private void DestroyCanvas()
        {
            _canvas.Dispose();
            _canvas = null;
        }

        /// <summary>
        /// Генерирует событие изменения состояния регистров.
        /// </summary>
        private void DispatchStateChanged()
        {
            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Уничтожает средства взаимодействия пользователя с компонентом.
        /// </summary>
        private void DisposeControls()
        {
            MouseDown -= InfiniteRegistersVisualizer_MouseDown;
            MouseUp -= InfiniteRegistersVisualizer_MouseUp;
            MouseWheel -= InfiniteRegistersVisualizer_MouseWheel;
            KeyDown -= InfiniteRegistersVisualizer_KeyDown;
            PreviewKeyDown -= InfiniteRegistersVisualizer_PreviewKeyDown;
            KeyPress -= InfiniteRegistersVisualizer_KeyPress;

            _timer.Tick -= _timer_Tick;
            _timer.Enabled = false;
            _timer.Dispose();
            _timer = null;
        }

        /// <summary>
        /// Заполняет пространство компонента ячейками регистров.
        /// </summary>
        /// <param name="pos">Позиция верхнего левого угла по вертикальной оси.</param>
        private void DrawCells(BigInteger pos)
        {
            int margin = Width >> 6;
            int minWidth = (Width - (margin * (_columns - 1))) / _columns;
            int cellHeight = minWidth >> 1;
            int indexHeight = cellHeight >> 2;

            if (minWidth == 0) minWidth = 1;
            if (cellHeight == 0) cellHeight = 1;
            if (indexHeight == 0) indexHeight = 1;

            _margin = margin;
            _blockWidth = minWidth;
            _blockHeight = cellHeight + indexHeight;

            int f;
            f = cellHeight >> 2;
            Font valueFont = new Font(SystemFonts.DefaultFont.FontFamily, f > 0 ? f : 1);
            f = indexHeight >> 1;
            Font indexFont = new Font(SystemFonts.DefaultFont.FontFamily, f > 0 ? f : 1);
            StringFormat valFormat = new StringFormat();
            valFormat.Alignment = StringAlignment.Near;
            valFormat.LineAlignment = StringAlignment.Center;

            _valueFont = valueFont;

            bool selected;
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                SolidBrush cellBrush = new SolidBrush(Color.Aqua);
                SolidBrush selectedBrush = new SolidBrush(Color.Blue);
                SolidBrush textBrush = new SolidBrush(Color.Black);

                int w, t;
                string val, ind;

                int x = 0;
                int y = -(int)(pos % (_blockHeight + _margin));
                BigInteger c;
                c = (pos / (_blockHeight + _margin)) * _columns;
                if (_currentRegister != -1)
                {
                    BigInteger pw = (pos / (_blockHeight + _margin)) * Width;
                    BigInteger crw = (_currentRegister / _columns) * Width;
                    if (pw > crw)
                    {
                        if (pw < crw + _currentRegisterWidth)
                        {
                            c = _currentRegister / _columns * _columns;
                            x = (int)((pw - crw) % Width);
                            y -= (int)(((pw - crw) / Width) * (_blockHeight + _margin));
                        }
                    }
                }

                while (y < Height)
                {
                    ind = c.ToString();
                    val = _registers != null ? _registers.GetValue(c).ToString() : "0";
                    w = (int)Math.Ceiling(g.MeasureString(val, valueFont).Width);
                    if (w <= minWidth || c != _currentRegister)
                        w = minWidth;

                    if (c == _currentRegister)
                    {
                        selected = true;
                        _currentRegisterBig = w != minWidth;
                        _currentRegisterWidth = w;
                    }
                    else
                        selected = false;

                    if (x + w <= Width)
                    {
                        g.FillRectangle(selected ? selectedBrush : cellBrush, new Rectangle(x, y + indexHeight, w, cellHeight));
                        g.DrawString(ind, indexFont, textBrush, x, y);
                        g.DrawString(val, valueFont, textBrush, new RectangleF(x, y + indexHeight, w, cellHeight), valFormat);
                        x += w + margin;
                    }
                    else
                    {
                        if (w == minWidth)
                        {
                            x = 0;
                            y += _blockHeight + margin;
                            g.FillRectangle(selected ? selectedBrush : cellBrush, new Rectangle(x, y + indexHeight, minWidth, cellHeight));
                            g.DrawString(ind, indexFont, textBrush, x, y);
                            g.DrawString(val, valueFont, textBrush, new RectangleF(x, y + indexHeight, minWidth, cellHeight), valFormat);
                            x += minWidth + margin;
                        }
                        else
                        {
                            bool indPlaced = false;
                            t = 0;
                            if (x < Width)
                            {
                                indPlaced = true;
                                t = Width - x;
                                g.FillRectangle(selected ? selectedBrush : cellBrush, new Rectangle(x, y + indexHeight, t, cellHeight));
                                g.DrawString(ind, indexFont, textBrush, x, y);
                                DrawTextTranslated(g, val, valueFont, textBrush, x, y + indexHeight + (cellHeight >> 1), 0, valFormat);
                                w -= t;
                            }

                            x = 0;
                            y += _blockHeight + margin;
                            if (!indPlaced) g.DrawString(ind, indexFont, textBrush, x, y);

                            while (w > Width)
                            {
                                g.FillRectangle(selected ? selectedBrush : cellBrush, new Rectangle(x, y + indexHeight, Width, cellHeight));
                                DrawTextTranslated(g, val, valueFont, textBrush, x, y + indexHeight + (cellHeight >> 1), -t, valFormat);

                                w -= Width;
                                x = 0;
                                y += _blockHeight + margin;
                                t += Width;
                            }
                            if (w > 0)
                            {
                                g.FillRectangle(selected ? selectedBrush : cellBrush, new Rectangle(x, y + indexHeight, w, cellHeight));
                                DrawTextTranslated(g, val, valueFont, textBrush, x, y + indexHeight + (cellHeight >> 1), -t, valFormat);
                                x += w + margin;
                            }
                        }
                    }
                    c++;
                }
            }
        }

        /// <summary>
        /// Отрисовывает текст, выполнив указанный сдвиг пикселей по горизонтали.
        /// </summary>
        /// <param name="g">Графика, в которую должен отрисоваться текст.</param>
        /// <param name="text">Текст.</param>
        /// <param name="font">Шрифт.</param>
        /// <param name="brush">Кисть.</param>
        /// <param name="x">Горизонтальная координата текста.</param>
        /// <param name="y">Вертикальная координата текста.</param>
        /// <param name="translationX">Сдвиг конечного изображения по горизонтальной оси.</param>
        /// <returns>Растеризованный текст.</returns>
        private void DrawTextTranslated(Graphics g, string text, Font font, Brush brush, int x, int y, int translationX = 0, StringFormat format = null)
        {
            if (translationX != 0)
            {
                Matrix m = new Matrix();
                m.Translate(translationX, 0);
                g.Transform = m;
            }
            g.DrawString(text, font, brush, x, y, format);
            g.Transform = new Matrix();
        }

        /// <summary>
        /// Получает индекс ячейки по ее координатам.
        /// </summary>
        /// <param name="x">Точка на горизонтальной оси в СК экрана.</param>
        /// <param name="y">Точка на вертикальной оси в СК экрана.</param>
        /// <returns>Индекс указанной ячейки.</returns>
        private BigInteger GetRegisterIndex(int x, BigInteger y)
        {
            return (y / (_blockHeight + _margin)) * _columns + (x / (_blockWidth + _margin));
        }

        private void InfiniteRegistersVisualizer_KeyDown(object sender, KeyEventArgs e)
        {
            bool changed = false;
            switch (e.KeyCode)
            {
                case Keys.Add:
                    if (e.Control)
                    {
                        if (_columns < MAX_COLUMNS)
                            _columns++;
                    }
                    else if (_currentRegister != -1)
                    {
                        _registers.Increment(_currentRegister);
                        changed = true;
                    }
                    break;

                case Keys.Subtract:

                    if (e.Control)
                    {
                        if (_columns != 1)
                            _columns--;
                    }
                    else if (_currentRegister != -1)
                    {
                        _registers.Decrement(_currentRegister);
                        changed = true;
                    }
                    break;

                case Keys.Back:
                    if (_currentRegister == -1) break;

                    string str = _registers.GetValue(_currentRegister).ToString();
                    str = str.Remove(str.Length - 1);
                    if (str == "") str = "0";
                    _registers.SetValue(BigInteger.Parse(str), _currentRegister);
                    changed = true;
                    break;

                case Keys.Delete:
                    if (_currentRegister == -1) break;

                    _registers.SetValue(0, _currentRegister);
                    changed = true;
                    break;

                case Keys.PageDown:
                    _velocity = 0;
                    _pos += Height;
                    break;

                case Keys.PageUp:
                    _velocity = 0;
                    _pos -= Height;
                    break;
            }

            if (changed) DispatchStateChanged();
        }

        private void InfiniteRegistersVisualizer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_currentRegister != -1)
            {
                if (char.IsDigit(e.KeyChar))
                {
                    string str = _registers.GetValue(_currentRegister).ToString();
                    str += e.KeyChar;
                    _registers.SetValue(BigInteger.Parse(str), _currentRegister);
                    DispatchStateChanged();
                }
            }
        }

        private void InfiniteRegistersVisualizer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lastPos = MousePosition.Y;
                _velocity = 0;
                _mouseDown = true;
                _mouseMoved = false;
            }
        }

        private void InfiniteRegistersVisualizer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mouseDown = false;
                _velocity = _lastPos - MousePosition.Y;
                BigInteger index = GetRegisterIndex(e.X, _pos + e.Y);
                if (!_mouseMoved)
                {
                    if (_currentRegister != -1 && _currentRegisterBig)
                    {
                        if (index != _currentRegister)
                        {
                            _currentRegister = -1;
                            _currentRegisterWidth = -1;
                        }
                    }
                    else
                        RegisterClick(index);
                }
            }
        }

        private void InfiniteRegistersVisualizer_MouseWheel(object sender, MouseEventArgs e)
        {
            _velocity -= e.Delta >> 5;
        }

        private void InfiniteRegistersVisualizer_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_canvas, 0, 0);
        }

        private void InfiniteRegistersVisualizer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (_currentRegister != -1)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (_currentRegister > 0)
                            _currentRegister--;
                        break;

                    case Keys.Right:
                        _currentRegister++;
                        break;

                    case Keys.Up:
                        if (_currentRegister > _columns)
                            _currentRegister -= _columns;
                        break;

                    case Keys.Down:
                        _currentRegister += _columns;
                        break;
                }

                if (_currentRegister < (_pos / (_blockHeight + _margin) * _columns))
                    _pos -= _blockHeight + _margin;
                else if (_currentRegister - _columns > ((_pos + Height) / (_blockHeight + _margin) * _columns))
                    _pos += _blockHeight + _margin;
            }
        }

        /// <summary>
        /// Инициализирует средства взаимодействия пользователя с компонентом.
        /// </summary>
        private void InitControls()
        {
            _timer = new Timer();
            _timer.Interval = (int)Math.Round(1000.0 / FRAME_RATE);
            _timer.Tick += _timer_Tick;
            _timer.Enabled = true;

            MouseDown += InfiniteRegistersVisualizer_MouseDown;
            MouseUp += InfiniteRegistersVisualizer_MouseUp;
            MouseWheel += InfiniteRegistersVisualizer_MouseWheel;
            KeyDown += InfiniteRegistersVisualizer_KeyDown;
            PreviewKeyDown += InfiniteRegistersVisualizer_PreviewKeyDown;
            KeyPress += InfiniteRegistersVisualizer_KeyPress;
        }

        /// <summary>
        /// Преобразует линейную координату в экранные координаты, основываясь на высоте
        /// блока регистра и отступе между такими регистрами.
        /// </summary>
        /// <param name="w">Преобразуемая линейная координата.</param>
        /// <returns>Структура, представляющая пару координат.</returns>
        private Point LinearToScreen(int w)
        {
            return new Point(w % Width,
                            (w / Width) * (_blockHeight + _margin));
        }

        /// <summary>
        /// Вызывается после клика по ячейке.
        /// </summary>
        /// <param name="index">Индекс ячейки, по которой был совершен клик.</param>
        private void RegisterClick(BigInteger index)
        {
            Console.WriteLine(index);
            _currentRegister = index;
        }

        /// <summary>
        /// Вызывает процедуру перерисовки графики.
        /// </summary>
        private void UpdateView()
        {
            Invalidate();
        }

        #endregion

        #region [Конструкторы]

        public InfiniteRegistersVisualizer()
        {
            _velocity = 0;
            _pos = 0;
            _currentRegister = -1;
            _currentRegisterBig = false;
            _currentRegisterWidth = -1;
            _mouseMoved = false;
            _mouseDown = false;
            CreateCanvas();
            InitializeComponent();
        }

        ~InfiniteRegistersVisualizer()
        {
            Registers = null;

            DestroyCanvas();
        }

        #endregion
    }
}