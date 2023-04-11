namespace FormsLib.Design
{
    //https://mokole.com/palette.html
    public static class Palettes
    {
        public static IPalette DistinctiveOnBlack => new CustomPalette(new Color[] {
            Colors.Red,
            Colors.Green,
            Colors.Yellow,
            Colors.Blue,
            Colors.Orange,
            Colors.Purple,
            Colors.Cyan,
            Colors.Magenta,
            Colors.Lime,
            Colors.Pink,
            Colors.Teal,
            Colors.Lavender,
            Colors.Brown,
            Colors.Beige,
            Colors.Maroon,
            Colors.Mint,
            Colors.Olive,
            Colors.Apricot,
            Colors.Navy,
            Colors.Grey,
        });


        public static IPalette DistinctiveOnWhite => new CustomPalette(new Color[] {
            Colors.Maroon,
            Colors.Green,
            Colors.Olive,
            Colors.Navy,
            Colors.Brown,
            Colors.Purple,
            Colors.Teal,

            Colors.Pink,
            Colors.Lavender,
            Colors.Brown,
            Colors.Beige,
            Colors.Maroon,
            Colors.Mint,
            Colors.Olive,
            Colors.Apricot,
            Colors.Navy,
            Colors.Grey,

            //Color.FromArgb(unchecked((int)0xFFe6194b)),
            //Color.FromArgb(unchecked((int)0xFF3cb44b)),
            //Color.FromArgb(unchecked((int)0xFFffe119)),
            //Color.FromArgb(unchecked((int)0xFF4363d8)),
            //Color.FromArgb(unchecked((int)0xFFf58231)),
            //Color.FromArgb(unchecked((int)0xFF911eb4)),
            //Color.FromArgb(unchecked((int)0xFF46f0f0)),
            //Color.FromArgb(unchecked((int)0xFFf032e6)),
            //Color.FromArgb(unchecked((int)0xFFbcf60c)),
            //Color.FromArgb(unchecked((int)0xFFfabebe)),
            //Color.FromArgb(unchecked((int)0xFF008080)),
            //Color.FromArgb(unchecked((int)0xFFe6beff)),
            //Color.FromArgb(unchecked((int)0xFF9a6324)),
            //Color.FromArgb(unchecked((int)0xFFfffac8)),
            //Color.FromArgb(unchecked((int)0xFF800000)),
            //Color.FromArgb(unchecked((int)0xFFaaffc3)),
            //Color.FromArgb(unchecked((int)0xFF808000)),
            //Color.FromArgb(unchecked((int)0xFFffd8b1)),
            //Color.FromArgb(unchecked((int)0xFF000075)),
            //Color.FromArgb(unchecked((int)0xFF808080)),
            //Color.FromArgb(unchecked((int)0xFFffffff)),
            //Color.FromArgb(unchecked((int)0xFF000000)),
        });
    }
}
