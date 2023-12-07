namespace FormsLib.Scope.Enums
{
    [Flags]
    public enum DrawOptions
    {
        None = 0,
        ShowCrosses = 0x01,
        ExtendBegin = 0x02,
        ExtendEnd = 0x04,
        ShowScale = 0x08,
    }
}
