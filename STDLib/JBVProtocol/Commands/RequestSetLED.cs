namespace STDLib.JBVProtocol
{
    public class RequestSetLED : CMD
    {
        public bool LedStatus { get; set; } = false;

        protected override byte[] Data { get => new byte[] { (byte)(LedStatus ? 1 : 0) }; set => LedStatus = value[0] >= (byte)1; }
    }

}
