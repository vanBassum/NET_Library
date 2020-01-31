using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Log_temp_verloop_warmwelkom
{

    public class LogItemDoor : LogItemBase
    {
        public override DateTime Timestamp { get; set; } = DateTime.MinValue;
        public byte Code { get; set; } = 0;



        public override bool TryPopulate(string data)
        {
            string[] split = data.Split(';');

            DateTime timestamp;
            byte code;

            if (!DateTime.TryParse(split[0], out timestamp)) return false;

            Match m = Regex.Match(split[1], @"0x([A-Fa-f\d]+)");
            if (!m.Success) return false;

            if (!byte.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code)) return false;

            this.Code = code;
            this.Timestamp = timestamp;

            return true;
        }



        
    }
}
