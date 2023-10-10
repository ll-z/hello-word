using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemonForWorldZhu.Matrix.Matrix2D
{
    public partial class Matrix2DForm : Form
    {
        Triangle t;
        public Matrix2DForm()
        {
            InitializeComponent();


            //解决方案一：
            SetStyle(ControlStyles.UserPaint, true);

            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景

            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
        }

        private void Matrix2DForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(300, 300);
            t.Draw(e.Graphics);
        }

        private void Matrix2DForm_Load(object sender, EventArgs e)
        {
            PointF A = new PointF(0, 0);
            PointF B = new PointF(0, 100);
            PointF C = new PointF(100, 100);
            PointF D = new PointF(100, 0);
            PointF O = new PointF(50, 50);
            t = new Triangle(A, B, C, D, O);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate{t.Rotate(1);});
            this.Invalidate();
        }
    }
}
