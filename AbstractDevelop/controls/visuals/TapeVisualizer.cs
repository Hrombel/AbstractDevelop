using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using AbstractDevelop.Properties;
using AbstractDevelop.machines.tape;
using AbstractDevelop.machines;

namespace AbstractDevelop.controls.visuals
{
    /// <summary>
    /// Представляет средство визуализации абстрактной ленты.
    /// </summary>
    public partial class TapeVisualizer : UserControl
    {
        /// <summary>
        /// Возникает после сдвига каждой ячейки через левый край компонента.
        /// </summary>
        public event EventHandler PositionChanged;
        /// <summary>
        /// Возникает после изменения состояния ленты.
        /// </summary>
        public event EventHandler TapeUpdated;
        /// <summary>
        /// Возникает после остановки анимации навигации к ячейке.
        /// </summary>
        public event EventHandler OnNavigationEnd;

        private const int FRAME_RATE = 40;

        private Bitmap _cell;
        private Bitmap _cellChecked;
        private Bitmap _cellFocused;
        private int _numHeight;

        private Bitmap _canvas;
        private Timer _timer;

        private double _velocity;
        private BigInteger _pos;
        private BigInteger _lastIndex;
        private int _mouseX;
        private bool _dragging;
        private bool _mouseMoved;

        private bool _navigating;
        private BigInteger _target;

        private Tape _tape;

        private BigInteger _focusCell;
        private bool _focusedExists;

        private SymbolSet _chars;
        private SymbolSet _externalSymbolSet;

        public TapeVisualizer()
        {
            CreateCanvas();
            ClearCanvas();
            CreateCellBitmap();
            InitializeComponent();
            _velocity = 0;
            _pos = 0;
            _dragging = false;
            _mouseMoved = false;
            _timer = new Timer();
            _timer.Interval = (int)Math.Round(1000.0 / (double)FRAME_RATE);
            _timer.Tick += _timer_Tick;
            _timer.Enabled = true;
            Navigate(0);

            InputMode = true;

            _focusCell = 0;
            _focusedExists = false;
            _lastIndex = GetCellIndex(_pos);

            _chars = new SymbolSet();
        }
        ~TapeVisualizer()
        {
            _timer.Tick -= _timer_Tick;
            _timer.Enabled = false;
            _timer.Dispose();
            _timer = null;
            DisposeCanvas();
            DisposeCellBitmap();
        }

        /// <summary>
        /// Получает рекомендуемую скорость прокрутки ленты на одну ячейку.
        /// </summary>
        public int RecommendedNavigationSpeed { get{return CellWidth >> 3;} }

        /// <summary>
        /// Устанавливает или задает внешнее множество символов. Если оно установлено, то символы, вводимые на ленту, кодируются в 
        /// соответствии с указанным множеством. При любых изменениях значения этого поля состояние ленты обнуляется.
        /// Изменения, внесенные во множество данной лентой, не отменяются.
        /// </summary>
        public SymbolSet ExternalSymbolSet
        {
            get { return _externalSymbolSet; }
            set
            {
                if(_tape != null) _tape.Clear();
                _chars.Clear();
                _externalSymbolSet = value;
            }
        }


        /// <summary>
        /// Получает или задает текущую визуализируемую ленту.
        /// </summary>
        public Tape CurrentTape
        {
            get
            {
                return _tape;
            }
            set
            {
                if(_tape != null)
                {
                    _tape.Update -= _tape_Update;
                    this.MouseDown -= TapeVisualizer_MouseDown;
                    this.MouseUp -= TapeVisualizer_MouseUp;
                    this.KeyDown -= TapeVisualizer_KeyDown;
                    this.PreviewKeyDown -= TapeVisualizer_PreviewKeyDown;
                    this.KeyPress -= TapeVisualizer_KeyPress;
                }

                _tape = value;
                DisposeCellBitmap();
                CreateCellBitmap();

                if(_tape != null)
                {
                    this.MouseDown += TapeVisualizer_MouseDown;
                    this.MouseUp += TapeVisualizer_MouseUp;
                    this.KeyDown += TapeVisualizer_KeyDown;
                    this.PreviewKeyDown += TapeVisualizer_PreviewKeyDown;
                    this.KeyPress += TapeVisualizer_KeyPress;
                    _tape.Update += _tape_Update;
                }
            }
        }

        private void _tape_Update(object sender, EventArgs e)
        {
            Redraw();
            if (TapeUpdated != null)
                TapeUpdated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Устанавливает указанное значение в необходимую ячейку и производит контроль над
        /// внутренним алфавитом ленты.
        /// </summary>
        /// <param name="cell">Индекс ячейки.</param>
        /// <param name="value">Устанавливаемое значение.</param>
        private void SetChar(BigInteger cell, char value)
        {
            if (_tape.Type == TapeType.TwoStated)
                throw new Exception("Попытка установки символа на ленту, ячейки которой могут иметь только два состояния");

            RemoveChar(cell);

            int i;
            if(_externalSymbolSet == null)
                i = _chars.AddChar(value);
            else
                i = _externalSymbolSet.AddChar(value);

            _tape.SetValue(cell, (byte)(i + 1));

            Console.WriteLine(_externalSymbolSet);
        }

        /// <summary>
        /// Удаляет символ из указанной ячейки и производит контроль над
        /// внутренним алфавитом ленты.
        /// </summary>
        /// <param name="cell">Индекс ячейки, символ из которой удаляется.</param>
        private void RemoveChar(BigInteger cell)
        {
            if (_tape.Type == TapeType.TwoStated)
                throw new Exception("Попытка удаления символа с ленты, ячейки которой могут иметь только два состояния");

            int val = _tape.GetValue(cell);
            if (val == 0) return;
            val--;

            if (_externalSymbolSet == null)
                _chars.RemoveCharAt(val);
            else
                _externalSymbolSet.RemoveCharAt(val);

            _tape.SetValue(cell, 0);
        }

        /// <summary>
        /// Получает или задает свойство, определяющее, находится ли лента в состоянии ввода данных.
        /// Истина - разрешен ввод данных пользователем, иначе - не разрешен.
        /// </summary>
        public bool InputMode { get; set; }

        /// <summary>
        /// Получает ширину одной ячейки.
        /// </summary>
        public int CellWidth { get { return _cell.Width; } }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            BigInteger index = GetCellIndex(_pos - GetCursorOffset());
            BigInteger targetIndex = 0;

            if (_navigating)
            {
                targetIndex = GetCellIndex(_target - GetCursorOffset());
            }

            base.SetBoundsCore(x, y, width, height, specified);

            DisposeCanvas();
            CreateCanvas();
            CreateCellBitmap();

            Redraw();

            Navigate(index);
            if (_navigating)
                Navigate(targetIndex, (int)Math.Abs(_velocity));
        }

        /// <summary>
        /// Перематывает ленту к указанной ячейке так, чтобы эта ячейка расположилась точно
        /// по середине компонента.
        /// </summary>
        /// <param name="index">Индекс рассматриваемой ячейки.</param>
        public void Navigate(BigInteger index)
        {
            if (InputMode)
                _velocity = 0;

            _pos = -GetCellPos(index) + GetCursorOffset() - (_cell.Width >> 1);
            Redraw();
        }

        /// <summary>
        /// Перематывает ленту к указанной ячейке так, чтобы эта ячейка расположилась точно
        /// по середине компонента.
        /// </summary>
        /// <param name="index">Индекс рассматриваемой ячейки.</param>
        /// <param name="speed">Скорость перематывания.</param>
        public void Navigate(BigInteger index, int speed)
        {
            if (speed <= 0)
                throw new ArgumentException("Скорость перемещения должна быть положительной");

            _target = -GetCellPos(index) + GetCursorOffset() - (_cell.Width >> 1);
            _velocity = _target >= _pos ? speed : -speed;
            _navigating = true;
        }

        /// <summary>
        /// Получает индекс ячейки, в данный момент находящейся в центре компонента.
        /// </summary>
        public BigInteger GetCurrentCell()
        {
            BigInteger res = GetCellIndex(_pos - GetCursorOffset());

            return res;
        }

        /// <summary>
        /// Возвращает индекс ячейки, находящейся по указанной координате, выраженной в пикселях.
        /// </summary>
        /// <param name="pos">Позиция ячейки.</param>
        private BigInteger GetCellIndex(BigInteger pos)
        {
            BigInteger result;
            if (pos >= 0)
            {
                BigInteger rem;
                result = -BigInteger.DivRem(pos, _cell.Width, out rem);
                if (rem != 0) result--;
            }
            else
            {
                result = -pos / _cell.Width;
            }

            return result;
        }

        /// <summary>
        /// Получает расположение ячейки в СК компонента.
        /// </summary>
        /// <param name="index">Индекс ячейки.</param>
        private BigInteger GetCellPos(BigInteger index)
        {
            return index * _cell.Width;
        }

        /// <summary>
        /// Возвращает смещение позиции курсора, на которое он отстоит от левого края компонента.
        /// </summary>
        /// <returns>Смещение курсора по оси абсцисс.</returns>
        private int GetCursorOffset()
        {
            return this.Width >> 1;
        }

        private void TapeVisualizer_KeyDown(object sender, KeyEventArgs e)
        {
            if(_tape.Type == TapeType.MultiStated)
            {
                if (_focusedExists)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Escape:
                            HideFocus();
                            break;
                        case Keys.Back:
                            if(_tape.GetValue(_focusCell) != 0)
                            {
                                RemoveChar(_focusCell);
                                if (_tape.GetValue(_focusCell - 1) != 0)
                                {
                                    MoveFocusLeft();
                                }
                            }
                            break;
                        case Keys.Delete:
                            if(_tape.GetValue(_focusCell) != 0)
                            {
                                RemoveChar(_focusCell);
                                if (_tape.GetValue(_focusCell + 1) != 0)
                                {
                                    MoveFocusRight();
                                }
                            }
                            break;
                        case Keys.V:
                            if (e.Control)
                            {
                                string data = Clipboard.GetText();
                                if(data.Length == 1)
                                    SetChar(_focusCell, data.ToCharArray()[0]);
                            }

                            break;
                    }
                }
            }
        }

        private void TapeVisualizer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(_tape.Type == TapeType.MultiStated)
            {
                if (_focusedExists)
                {
                    if (!IsInputKey(e.KeyChar))
                        SetChar(_focusCell, e.KeyChar);
                }
            }
        }

        /// <summary>
        /// Определяет, используется ли указанная клавиша для управления лентой.
        /// </summary>
        /// <param name="key">Проверяемая клавиша.</param>
        /// <returns>Истина, если клавиша ипользуется для управления, иначе - ложь.</returns>
        private bool IsInputKey(char key)
        {
            int code = (int)key;

            return code == 8 || // Backspace
                   code == 13 || // Enter
                   code == 27 || // Esc
                   code == 32 || // Space
                   code == 22; // Ctrl + V
        }

        private void TapeVisualizer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    MoveFocusLeft();
                    break;
                case Keys.Right:
                    MoveFocusRight();
                    break;
            }
        }

        /// <summary>
        /// Перемещает курсор выделенной ячейки влево.
        /// </summary>
        private void MoveFocusLeft()
        {
            if (!_focusedExists) return;

            _focusCell--;
            Redraw();
            if (_focusCell < GetCellIndex(_pos))
            {
                Navigate(GetCurrentCell() - 1);
            }
        }

        /// <summary>
        /// Перемещает курсор выделенной ячейки вправо.
        /// </summary>
        private void MoveFocusRight()
        {
            if (!_focusedExists) return;

            _focusCell++;
            Redraw();
            if (_focusCell > GetCellIndex(_pos - Width))
            {
                Navigate(GetCurrentCell() + 1);
            }
        }

        /// <summary>
        /// Снимает курсор фокуса с текущей выделенной ячейки.
        /// </summary>
        private void HideFocus()
        {
            _focusedExists = false;
            Redraw();
        }

        private void TapeVisualizer_MouseUp(object sender, MouseEventArgs e)
        {
            if (!InputMode) return;

            _navigating = false;
            _dragging = false;

            if(!_mouseMoved) // Событие клика...
            {
                if(e.Button == MouseButtons.Left) //...по ячейке.
                {
                    CellClick(GetCellIndex(_pos - PointToClient(MousePosition).X));

                    Redraw();
                }
            }
        }

        private void TapeVisualizer_MouseDown(object sender, MouseEventArgs e)
        {
            if (!InputMode) return;

            _navigating = false;
            _focusedExists = false;

            if (e.Button == MouseButtons.Left)
            {
                _mouseX = MousePosition.X;
                _dragging = true;
                _velocity = 0;
            }
        }

        /// <summary>
        /// Вызывается по клику на ячейку ленты.
        /// </summary>
        /// <param name="index">Индекс ячейки, по которой был произведен клик.</param>
        private void CellClick(BigInteger index)
        {
            if (_tape != null)
            {
                if (_tape.Type == TapeType.TwoStated)
                {
                    if (_tape.GetValue(index) != 0)
                        _tape.SetValue(index, 0);
                    else
                        _tape.SetValue(index, 1);
                }
                else if(_tape.Type == TapeType.MultiStated)
                {
                    _focusedExists = true;
                    _focusCell = index;
                }
            }
            else
                MessageBox.Show("Невозможно установить метку на ленту, поскольку она не определена");
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if(_dragging)
            {
                _velocity = MousePosition.X - _mouseX;
                _mouseX = MousePosition.X;

                if(!_mouseMoved)
                {
                    if (Math.Abs(_velocity) > 0)
                        _mouseMoved = true;
                }
            }
            else
            {
                if(_velocity != 0)
                {
                    if(_navigating)
                    {
                        _pos += (int)_velocity;
                        if(_velocity > 0)
                        {
                            if(_pos >= _target)
                            {
                                _pos = _target;
                                _velocity = 0;
                                _navigating = false;
                                Redraw();
                                if (OnNavigationEnd != null)
                                    OnNavigationEnd(this, new EventArgs());
                            }
                        }
                        else
                        {
                            if (_pos < _target)
                            {
                                _pos = _target;
                                _velocity = 0;
                                _navigating = false;
                                Redraw();
                                if (OnNavigationEnd != null)
                                    OnNavigationEnd(this, new EventArgs());
                            }
                        }
                    }
                    else
                    {
                        if (Math.Abs(_velocity) < 1.0)
                            _velocity = 0;
                        else
                            _velocity *= 0.9;
                    }
                }

                if (_mouseMoved)
                    _mouseMoved = false;
            }

            if (_velocity != 0)
                Redraw();
        }

        /// <summary>
        /// Вызывает процедуру перерисовки текущего состояния.
        /// </summary>
        private void Redraw()
        {
            ClearCanvas();
            DrawCells((int)Math.Ceiling(_velocity));
            UpdateGraphics();
        }

        /// <summary>
        /// Создает холст для компонента с его размерами.
        /// </summary>
        private void CreateCanvas()
        {
            _canvas = new Bitmap(Size.Width, Size.Height);
        }

        /// <summary>
        /// Удаляет холст компонента.
        /// </summary>
        private void DisposeCanvas()
        {
            _canvas.Dispose();
            _canvas = null;
        }

        /// <summary>
        /// Отображает графику, находящуюся на холсте.
        /// </summary>
        private void UpdateGraphics()
        {
            Invalidate();
        }

        /// <summary>
        /// Стирает отрисованную на холсте графику.
        /// </summary>
        private void ClearCanvas()
        {
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                g.Clear(Color.White);
            }
        }

        /// <summary>
        /// Освобождает ресурсы, связанные с изображением ячейки.
        /// </summary>
        private void DisposeCellBitmap()
        {
            if (_cell == null)
                throw new Exception("Попытка освобождения ресурсов несуществующего изображения ячейки.");

            _cell.Dispose();
            _cell = null;
        }

        /// <summary>
        /// Производит масштабирование изображения по высоте и возвращает результат.
        /// </summary>
        /// <param name="src">Исходное изображение.</param>
        /// <param name="height">Новая высота изображения.</param>
        private Bitmap ScaleByHeight(Bitmap src, int height)
        {
            Bitmap result;

            double ar = (double)src.Width / (double)src.Height;
            int h = height;
            int w = (int)Math.Ceiling((double)h * ar);
            result = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(result);
            g.DrawImage(src, 0, 0, w, h);
            g.Dispose();
            src.Dispose();

            return result;
        }

        /// <summary>
        /// Обновляет текущее изображение ячейки, исходя из размеров компонента.
        /// </summary>
        private void CreateCellBitmap()
        {
            int height = (int)((double)Size.Height * 0.8);
            if (height == 0) height = 1;

            _cell = ScaleByHeight(Resources.TapeCell, height);
            _numHeight = Size.Height - height;
            if(_tape != null)
            {
                if(_tape.Type == TapeType.TwoStated)
                {
                    _cellChecked = ScaleByHeight(Resources.TapeCellChecked, height);
                }
                else if(_tape.Type == TapeType.MultiStated)
                {
                    _cellFocused = ScaleByHeight(Resources.TapeCellFocused, height);
                }
            }
        }

        /// <summary>
        /// Отрисовывает ячейки в компоненте.
        /// </summary>
        /// <param name="offset">Смещение ячеек относительно текущего положения ленты (в пикселях).</param>
        private void DrawCells(int offset)
        {
            _pos += offset;

            BigInteger i = GetCellIndex(_pos);
            if(i != _lastIndex)
            {
                if (PositionChanged != null)
                    PositionChanged(this, EventArgs.Empty);
                _lastIndex = i;
            }
            
            int p = (int)(_pos % _cell.Width);
            if (p > 0)
                p -= _cell.Width;

            Font indexFont = new Font(FontFamily.GenericSerif, _numHeight > 1 ? _numHeight >> 1 : 1);
            SolidBrush indexBrush = new SolidBrush(Color.Black);
            RectangleF indexTextRect = new RectangleF((float)p, _cell.Height, _cell.Width, _numHeight);

            Font valueFont = new Font(FontFamily.GenericSerif, _cell.Height > 1 ? _cell.Height >> 1 : 1);
            SolidBrush valueBrush = new SolidBrush(Color.White);
            RectangleF valueTextRect = new RectangleF((float)p, 0, _cell.Width, _cell.Height);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            SymbolSet currentSet = ExternalSymbolSet != null ? ExternalSymbolSet : _chars;
            using(Graphics g = Graphics.FromImage(_canvas))
            {
                while(p < Size.Width)
                {
                    if(_tape != null)
                    {
                        if(_tape.Type == TapeType.TwoStated)
                        {
                            if (_tape.GetValue(i) == 0)
                                g.DrawImageUnscaled(_cell, p, 0);
                            else
                                g.DrawImageUnscaled(_cellChecked, p, 0);
                        }
                        else if(_tape.Type == TapeType.MultiStated)
                        {
                            if(_focusedExists)
                            {
                                if (i == _focusCell)
                                    g.DrawImageUnscaled(_cellFocused, p, 0);
                                else
                                    g.DrawImageUnscaled(_cell, p, 0);
                            }
                            else
                                g.DrawImageUnscaled(_cell, p, 0);

                            if(_tape.GetValue(i) != 0)
                            {
                                g.DrawString(currentSet.GetChar(_tape.GetValue(i) - 1).ToString(), valueFont, valueBrush, valueTextRect, format);
                            }
                        }
                    }
                    else
                        g.DrawImageUnscaled(_cell, p, 0);

                    g.DrawString(i.ToString(), indexFont, indexBrush, indexTextRect, format);

                    indexTextRect.X += _cell.Width;
                    valueTextRect.X += _cell.Width;
                    p += _cell.Width;
                    i++;
                }
                p = GetCursorOffset();
                g.DrawLine(new Pen(Color.Red), p, 0, p, _cell.Height);
            }
        }

        private void TapeVisualizer_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_canvas, Point.Empty);
            e.Dispose();
        }
    }
}
