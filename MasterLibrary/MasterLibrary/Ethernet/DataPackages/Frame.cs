using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Ethernet.DataPackages
{
    public interface Frame
    {
        Command CMD { get; }

    }

    public class SendID : Frame
    {
        public Command CMD => Command.SendID;

        public int ID { get; set; }
    }


    public enum Command
    {
        SendID,
        Decline,
        Accept
    }
}
