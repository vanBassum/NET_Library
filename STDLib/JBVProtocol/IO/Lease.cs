using System;

namespace STDLib.JBVProtocol.IO
{

    public class Lease
    {
        public UInt16 ID { get; set; }
        public DateTime? Expire { get; set; }
        public Guid? Key { get; set; }

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
                suc |= Guid.TryParse(split[1], out guid);
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

        public override string ToString()
        {
            if (Key != null && Expire != null)
                return $"{ID}, {Key.Value.ToString()}, {Expire.Value.ToString("dd-MMM-yyyy HH:mm:ss.ffff K")}";
            else
                return $"{ID}, null, null";
        }
    }
    
}
