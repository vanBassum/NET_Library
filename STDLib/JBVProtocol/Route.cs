using STDLib.Ethernet;

namespace STDLib.JBVProtocol
{
    public class Route
    {
		public Framing Connection { get; set; }
		public byte Hops { get; set; }

		public static Route GetBestRoute(Route a, Route b)
		{
			//Prefer non broadcasting 
			if (a.Connection.GetConnectionType() != b.Connection.GetConnectionType())
            {
				if (a.Connection.GetConnectionType() == ConnectionTypes.Direct)
					return a;
				else
					return b;
            }
			else
            {
				if (a.Hops < b.Hops)
					return a;
				else
					return b;
            }
		}
	}

}
