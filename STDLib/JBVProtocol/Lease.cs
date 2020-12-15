using STDLib.Extentions;
using System;
using System.Linq;

namespace STDLib.JBVProtocol
{
    public class Lease
    {
        public UInt16 ID { get; set; }
        public DateTime Expire { get; set; }
        public Guid Key { get; set; }
        public TimeSpan ExpiresIn { get { return Expire - DateTime.Now; } }

        public bool IsValid
        {
            get
            {
                if (Expire == null) return false;
                if (Expire < DateTime.Now) return false;
                if (Key == null) return false;
                return true;
            }
        }


        public Lease()
        {
            Key = Guid.NewGuid();
        }

        public Lease(byte[] data)
        {
            ID = BitConverter.ToUInt16(data, 0);
            Key = new Guid(data.SubArray(2, 16));
            long unixDateTime = BitConverter.ToInt64(data, 18);
            Expire = DateTimeOffset.FromUnixTimeSeconds(unixDateTime).DateTime.ToLocalTime();
        }

        public byte[] ToByteArray()
        {
            byte[] data = new byte[0];
            DateTimeOffset dateTimeOffset = new DateTimeOffset(Expire);
            long unixDateTime = dateTimeOffset.ToUnixTimeSeconds();
            data = data.Concat(BitConverter.GetBytes(ID)).ToArray();
            data = data.Concat(Key.ToByteArray()).ToArray();
            data = data.Concat(BitConverter.GetBytes(unixDateTime)).ToArray();
            return data;
        }

        public override string ToString()
        {
            if (Key != null && Expire != null)
                return $"{ID}, {Key.ToString()}, {Expire.ToString("dd-MMM-yyyy HH:mm:ss.ffff K")}";
            else
                return $"{ID}, null, null";
        }
    }

}