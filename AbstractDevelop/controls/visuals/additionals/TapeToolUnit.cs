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
    /// Контрол, представляющий модуль управления лентой.
    /// </summary>
    public partial class TapeToolUnit : UserControl
    {
        /// <summary>
        /// Обертка для кнопки, которая не может иметь фокуса.
        /// </summary>
        private class UnfocusableBtn : Button
        {
            public UnfocusableBtn()
            {
                this.SetStyle(ControlStyles.Selectable, false);
            }
        }

        /// <summary>
        /// Возникает, когда пользователь нажимает на кнопку удаления ленты.
        /// </summary>
        public event EventHandler OnTapeRemovePressed;

        private UnfocusableBtn _removeBtn;

        /// <summary>
        /// Получает ленту, отображаемую в модуле.
        /// </summary>
        public TapeVisualizer TapeVisualizer { get { return tapeVis; } }

        public TapeToolUnit()
        {
            InitializeComponent();
            _removeBtn = new UnfocusableBtn();
            _removeBtn.Width = this.Width - tapeVis.Width;
            _removeBtn.Height = this.Height;
            _removeBtn.Location = new Point(this.Width - _removeBtn.Width, 0);
            _removeBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _removeBtn.Text = "X";
            Controls.Add(_removeBtn);

            _removeBtn.Click += removeBtn_Click;
        }
        ~TapeToolUnit()
        {
            _removeBtn.Click -= removeBtn_Click;
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {
            if (OnTapeRemovePressed != null)
                OnTapeRemovePressed(this, e);
        }
    }
}
