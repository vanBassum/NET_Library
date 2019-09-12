using System;

namespace MasterLibrary.Extentions
{
    public static class TimeSpan_Ext
    { 

        public static string ToReadableString(this TimeSpan ts)
        {

            if (ts.Days > 0)
                return string.Format("{0} days",  ts.Days);
                                       
            if (ts.Hours > 0)          
                return string.Format("{0} hours",  ts.Hours);
                                       
            if (ts.Minutes > 0)        
                return string.Format("{0} minutes",  ts.Minutes);
                                       
            if (ts.Seconds > 0)        
                return string.Format("{0} seconds",  ts.Seconds);


            return string.Format("0 seconds");
        }
    }
}
