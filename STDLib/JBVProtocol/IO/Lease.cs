using STDLib.Extentions;
using System;
using System.Linq;

namespace STDLib.JBVProtocol.IO
{

    public class Lease
    {
        public UInt16 ID { get; set; }
        public DateTime Expire { get; set; }
        public Guid Key { get; set; }




        public Lease()
        {

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





        /*
        public static bool TryParse(string raw, out Lease lease)
        {
            lease = new Lease();

            string[] split = raw.Split(',');
            if(split.Length == 3)
            {
                UInt16 id;
                Guid guid;
                DateTime exp;

                bool suc = true;
                suc |= UInt16.TryParse(split[0], out id);
                guid = new Guid(split[1]);
                suc |= Guid.TryParse(, out guid);
                suc |= DateTime.TryParse(split[2], out exp);

                if(suc)
                {
                    lease.ID = id;
                    lease.Key = guid;
                    lease.Expire = exp;
                    return true;
                }
            }
            return false;
        }
        */
        public override string ToString()
        {
            if (Key != null && Expire != null)
                return $"{ID}, {Key.ToString()}, {Expire.ToString("dd-MMM-yyyy HH:mm:ss.ffff K")}";
            else
                return $"{ID}, null, null";
        }
    }
    
}
