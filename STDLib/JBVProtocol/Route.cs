using STDLib.Ethernet;

namespace STDLib.JBVProtocol
{
    public class Route
    {
		public Framing Connection { get; set; }
		public byte Hops { get; set; }
	}

}
