using System.Collections.Generic;
using System.Linq;

namespace bproj
{
    public class Command
    {
        byte cmd = 0;
        public byte SeqNo { get; set; } = 0;
        public List<byte> Data { get; set; } = new List<byte>();


        public Command()
        {
        }

        public Command(List<byte> data)
        {
            cmd = data[0];
            SeqNo = data[1];
            Data.Clear();
            Data.AddRange(data.GetRange(2, data.Count() -2));
        }

        public void SetResponse(ResponseType response)
        {
            this.cmd = (byte)(0x80 | (byte)response);
        }

        public void SetRequest(byte cmd)
        {
            this.cmd = (byte)(0x7F & cmd);
        }

        public List<byte> ToFrame()
        {
            List<byte> raw = new List<byte>();
            raw.Add(cmd);
            raw.Add(SeqNo);
            if(Data != null)
                raw.AddRange(Data);
            return raw;
        }

        public byte GetCommand()
        {
            return (byte)(0x7F & cmd);
        }

        public bool IsResponse()
        {
            return (cmd & 0x80) > 0;
        }

        public ResponseType GetResponse()
        {
            if (IsResponse())
                return (ResponseType)(cmd & 0x7F);
            else
                return ResponseType.NotAResp;
        }

        public enum ResponseType
        {
            Ack = 0,
            Nack = 1,
            Unknown = 2,
            Overflow = 3,
            NotAResp = 255,
        }
        public override string ToString()
        {
            return cmd.ToString();
        }
    }


}
