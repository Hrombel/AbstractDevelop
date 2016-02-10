using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.Properties;

namespace AbstractDevelop.controls.menus.logo
{
    /// <summary>
    /// Представляет компонент для отображения логотипа AbstractDevelop.
    /// </summary>
    public partial class LogoHolder : UserControl
    {
        private Bitmap _bmp;
        private double _ar;
        private int _logoHeight;

        public LogoHolder()
        {
            InitializeComponent();
            _bmp = Resources.Logo.Clone() as Bitmap;
            _ar = (double)_bmp.Height / (double)_bmp.Width;
            _logoHeight = (int)(Width * _ar);

            Paint += LogoHolder_Paint;
        }
        ~LogoHolder()
        {
            Paint -= LogoHolder_Paint;
        }

        /// <summary>
        /// Получает текущую высоту логотипа.
        /// </summary>
        public int LogoHeight { get { return _logoHeight; } }

        private void LogoHolder_Paint(object sender, PaintEventArgs e)
        {
            _logoHeight = (int)(Width * _ar);
            e.Graphics.Clear(BackColor);
            e.Graphics.DrawImage(_bmp, new Rectangle(0, 0, Width, _logoHeight));
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            
            Invalidate();
        }
    }
}
