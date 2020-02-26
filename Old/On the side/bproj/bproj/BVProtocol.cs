using System;
using System.Collections.Generic;

namespace bproj
{
    public class BVProtocol
    {
        ByteStuffing bs;
        Dictionary<byte, Action<Command>> pendingRequests = new Dictionary<byte, Action<Command>>();

        public BVProtocol()
        {
            bs = new ByteStuffing();
            bs.OnFrameCollected += Bs_OnFrameCollected;
        }

        private void Bs_OnFrameCollected(List<byte> data)
        {
            Command cmd = new Command(data);

            if (cmd.IsResponse())
            {
                //We received a response to an earlier send command.

                pendingRequests[cmd.SeqNo](cmd);
                pendingRequests.Remove(cmd.SeqNo);
            }
            else
            {
                //Execute the command.
                OnCommandRecieved(cmd);
            }
        }

        public void RawDataIn(List<byte> data)
        {
            bs.Unstuff(data);
        }

        public void SendRequest(byte cmdNo, List<byte> data, Action<Command> callback)
        {
            Command cmd = new Command();
            cmd.SetRequest(cmdNo);
            cmd.Data = data;

            for (cmd.SeqNo = 0; cmd.SeqNo < 255; cmd.SeqNo++)
                if (!pendingRequests.ContainsKey(cmd.SeqNo))
                    break;

            if (cmd.SeqNo < 255)
            {
                pendingRequests[cmd.SeqNo] = callback;
                OnRawDataOut(bs.Stuff(cmd.ToFrame()));
            }
            else
                throw new Exception("pending requests full.");
        }

        
        public void SendResponse(Command.ResponseType response, List<byte> data)
        {
            Command cmd = new Command();
            cmd.SetResponse(response);
            cmd.Data = data;
            OnRawDataOut(bs.Stuff(cmd.ToFrame()));
        }
        

        public Action<Command> OnCommandRecieved;
        public Action<List<byte>> OnRawDataOut;

    }


}
