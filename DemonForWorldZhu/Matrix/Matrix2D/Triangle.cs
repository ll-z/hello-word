using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonForWorldZhu.Matrix.Matrix2D
{
    public class Triangle
    {
        PointF A, B, C, D, O;

        public Triangle(PointF A, PointF B, PointF C, PointF D, PointF O)
        {
            this.A = A;
            this.B = B;
            this.C = C;
            this.D = D;
            this.O = O;
        }

        public void Draw(Graphics g)
        {
            Pen pen = new Pen(Color.Red, 2);
            g.DrawLine(pen, A, B);
            g.DrawLine(pen, B, C);
            g.DrawLine(pen, C, D);
            g.DrawLine(pen, D, A);
            g.DrawLine(pen, O, A);
            g.DrawLine(pen, O, B);
            g.DrawLine(pen, O, C);
            g.DrawLine(pen, O, D);
        }

        public void Rotate(double degrees)
        {
            float angle = (float)(degrees / 360.0f * Math.PI);
            float newX = (float)(A.X * Math.Cos(angle) - A.Y * Math.Sin(angle));
            float newY = (float)(A.X * Math.Sin(angle) + A.Y * Math.Cos(angle));
            A.X = newX;
            A.Y = newY;
            newX = (float)(B.X * Math.Cos(angle) - B.Y * Math.Sin(angle));
            newY = (float)(B.X * Math.Sin(angle) + B.Y * Math.Cos(angle));
            B.X = newX;
            B.Y = newY;

            newX = (float)(C.X * Math.Cos(angle) - C.Y * Math.Sin(angle));
            newY = (float)(C.X * Math.Sin(angle) + C.Y * Math.Cos(angle));
            C.X = newX;
            C.Y = newY;

            newX = (float)(D.X * Math.Cos(angle) - D.Y * Math.Sin(angle));
            newY = (float)(D.X * Math.Sin(angle) + D.Y * Math.Cos(angle));
            D.X = newX;
            D.Y = newY;
        }
    }
}
