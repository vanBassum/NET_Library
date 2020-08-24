using System;

namespace STDLib.JBVProtocol.IO
{

    public class Lease
    {
        public UInt16 ID { get; set; }
        public DateTime? Expire { get; set; }
        public Guid? Key { get; set; }

        public static Lease FromString(string raw)
        {
            Lease newLease = new Lease();

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
                    newLease.ID = id;
                    newLease.Key = guid;
                    newLease.Expire = exp;
                    return newLease;
                }
            }
            throw new Exception("Couln't parse lease");
        }

        public override string ToString()
        {
            return $"{ID},{Key.Value.ToString()},{Expire.Value.ToString("dd-MMM-yyyy HH:mm:ss.ffff K")}";
        }
    }
    
}
