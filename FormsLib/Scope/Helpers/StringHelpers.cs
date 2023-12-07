namespace FormsLib.Scope.Helpers
{
    public static class StringHelpers
    {
        public static string FormatFloatWithUnits(float value, int digits)
        {
            if (float.IsNaN(value))
                return "NaN";
            if (float.IsPositiveInfinity(value))
                return "+Inf";
            if (float.IsNegativeInfinity(value))
                return "-Inf";

            string smallPrefix = "mµnpf";
            string largePrefix = "KMGT";
            bool negative = value < 0;
            if (negative)
                value = -value;

            int thousands = (int)Math.Log(Math.Abs(value), 1000);

            // Adjust the thousands based on the logarithmic calculation
            if (Math.Log(Math.Abs(value), 1000) < 0)
                thousands--;

            if (value == 0)
                thousands = 0;

            float scaledNumber = value * MathF.Pow(1000, -thousands);

            int places = Math.Max(0, digits - (int)Math.Log10(scaledNumber));
            string formattedNumber = scaledNumber.ToString($"F{places}");

            if (thousands > 0 && thousands <= largePrefix.Length)
                formattedNumber += largePrefix[thousands - 1];

            if (thousands < 0 && Math.Abs(thousands) <= smallPrefix.Length)
                formattedNumber += smallPrefix[Math.Abs(thousands) - 1];

            if (negative)
                formattedNumber = $"-{formattedNumber}";

            return formattedNumber;
        }

    }
}
