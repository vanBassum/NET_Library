using System;

namespace FormsLib.Scope
{
    public static class Ext
    {

        public static int LowestDiv(this int num)
        {
            int i = 0;
            for (i = 2; i < num; i++)
                if (num % i == 0)
                    break;
            return i;
        }

        public static string ToHumanReadable(this double number, int digits = 3)
        {
            if (double.IsNaN(number))
                return "NaN";
            if (double.IsPositiveInfinity(number))
                return "+Inf";
            if (double.IsNegativeInfinity(number))
                return "-Inf";

            string smallPrefix = "mµnpf";
            string largePrefix = "kMGT";
            bool negative = number < 0;
            if (negative)
                number = -number;

            int thousands = (int)System.Math.Log(System.Math.Abs(number), 1000);

            if (System.Math.Log(System.Math.Abs(number), 1000) < 0)
                thousands--;

            if (number == 0)
                thousands = 0;

            double scaledNumber = number * System.Math.Pow(1000, -thousands);

            int places = System.Math.Max(0, digits - (int)System.Math.Log10(scaledNumber));
            string s = scaledNumber.ToString("F" + places.ToString());



            if (thousands > 0)
                if (thousands < largePrefix.Length)
                    s += largePrefix[thousands - 1];

            if (thousands < 0)
                if (System.Math.Abs(thousands) < largePrefix.Length)
                    s += smallPrefix[System.Math.Abs(thousands) - 1];
            if (negative)
                s = $"-{s}";
            return s;
        }
        /*
        public static void Add(this ThreadedBindingList<PointD> list, double x, double y)
        {
            PointD pt = new PointD(x, y);
            list.Add(pt);
        }
        */
    }

}
