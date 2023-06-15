using System;
using System.Globalization;
using System.Text;

namespace FormsLib.Extentions
{
    public static class Numers_Ext
    {
        public static string ToHumanReadable(this decimal num, int size = 8)
        {
            const string prefix = "fpnum\0kMGT";
            decimal abs = System.Math.Abs(num);
            bool negative = num < 0;
            int digits = size - 4;  //terminator + dot + sign + prefix = 4 characters.

            if (digits <= 0)
                throw new Exception("Size to small, use at least 5");

            if (num == 0)
            {
                return new string(' ', size - 2) + "0 ";
            }

            abs *= (decimal)System.Math.Pow(10, 15);

            int i = 0;

            for (i = 0; i < prefix.Length - 2 && abs >= 1000; i++)
                abs /= 1000;

            int ditigsBeforeDot = (int)System.Math.Log10((double)abs) + 1;
            int ditigsAfterDot = digits - ditigsBeforeDot;

            StringBuilder result = new StringBuilder();
            result.Append(negative ? '-' : ' ');

            if (ditigsAfterDot == 0)                //Dont have to show the dot, so add space
                result.Append(' ');

            result.AppendFormat(string.Format(new NumberFormatInfo() { NumberDecimalDigits = ditigsAfterDot }, "{0:F}", abs));
            if (prefix[i] != '\0')
                result.Append(prefix[i]);
            return result.ToString();
        }


    }



}
