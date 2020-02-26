using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Log_temp_verloop_warmwelkom
{

    public abstract class LogItemGateway : LogItemBase
    {
        public override DateTime Timestamp { get; set; } = DateTime.MinValue;
        public byte Code { get; set; } = 0;
        public int ActionCode { get; set; } = 0;
        public string XBEEAddress { get; set; } = "";
        public byte[] Data { get; set; }


        
        public bool TryPopulateBase(string line)
        {

            string[] split = line.Split(';');

            DateTime timestamp;
            byte code;
            int actionCode;
            string xBEEAddress;
            byte[] data;

            if (!DateTime.TryParse(split[0], out timestamp)) return false;

            Match m = Regex.Match(split[1], @"0x([A-Fa-f\d]+)");
            if (!m.Success) return false;

            if (!byte.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code)) return false;
            if (!int.TryParse(split[2], out actionCode)) return false;
            xBEEAddress = split[3];

            string[] dataSplit = split[4].Trim(' ').Split(' ');
            data = new byte[dataSplit.Length];

            for (int i = 0; i < data.Length; i++)
                if (!byte.TryParse(dataSplit[i].Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out data[i]))
                    return false;

            ActionCode = actionCode;
            Code = code;
            Data = data;
            Timestamp = timestamp;
            XBEEAddress = xBEEAddress;

            return true;
        }
    }
}
