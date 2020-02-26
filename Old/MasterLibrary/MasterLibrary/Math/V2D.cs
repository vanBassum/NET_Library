using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterLibrary.PropertySensitive;

namespace MasterLibrary.MathX
{

    public class V2D : PropertySensitiveExternal
    {
        public static V2D Null { get { return new V2D(0, 0); } }
        public double X { get { return GetPar<double>(); } set { SetPar(value); } }
        public double Y { get { return GetPar<double>(); } set { SetPar(value); } }
        public double Rad { get { return Math.Atan2(Y, X); } set { double l = L; X = l * Math.Cos(value); Y = l * Math.Sin(value); } }
        public double Deg { get { return Rad * 180 / Math.PI; } set { Rad = value * Math.PI / 180; } }
        public double L { get { return Math.Sqrt(Y * Y + X * X); } set { double a = Rad; X = value * Math.Cos(a); Y = value * Math.Sin(a); } }


        public V2D()
        {
            X = 0;
            Y = 0;
        }

        public V2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public V2D Conjugate() => new V2D(X, -Y);

        public static V2D operator /(double c, V2D v) => v * c;
        public static V2D operator *(double c, V2D v) => v * c;
        public static V2D operator /(V2D v, double c) => new V2D(v.X / c, v.Y / c);
        public static V2D operator *(V2D v, double c) => new V2D(v.X * c, v.Y * c);
        public static V2D operator +(V2D v, V2D c) => new V2D(v.X + c.X, v.Y + c.Y);
        public static V2D operator -(V2D v, V2D c) => new V2D(v.X - c.X, v.Y - c.Y);

        public static implicit operator PointF(V2D d) => new PointF((float)d.X, (float)d.Y);
        public static implicit operator V2D(PointF d) => new V2D(d.X, d.Y);
        public static implicit operator Point(V2D d) => new Point((int)d.X, (int)d.Y);
        public static implicit operator V2D(Point d) => new V2D(d.X, d.Y);
        public static implicit operator SizeF(V2D d) => new SizeF((float)d.X, (float)d.Y);
        public static implicit operator V2D(SizeF d) => new V2D(d.Width, d.Height);
        public static implicit operator Size(V2D d) => new Size((int)d.X, (int)d.Y);
        public static implicit operator V2D(Size d) => new V2D(d.Width, d.Height);


        public override string ToString()
        {
            return X.ToString("0.00") + (Y<0?"":"+") + Y.ToString("0.00") + "i";
        }

    }
}
