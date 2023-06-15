using System;

namespace FormsLib.Extentions
{
    public static class Enum_Ext
    {
        public static int ToInt(this Enum val)
        {
            return (int)Enum.ToObject(val.GetType(), val);
        }

        public static Enum ToEnum(Type etype, int i)
        {
            return (Enum)Enum.ToObject(etype, i);
        }

        public static Enum SetFlags(this Enum val, Enum flags)
        {
            int iFlags = flags.ToInt();
            int iVal = val.ToInt();
            int iResult = iFlags | iVal;
            return ToEnum(val.GetType(), iResult);
        }

        public static Enum ClearFlags(this Enum val, Enum flags)
        {
            int iFlags = flags.ToInt();
            int iVal = val.ToInt();
            int iResult = ~iFlags & iVal;
            return ToEnum(val.GetType(), iResult);
        }

        public static bool HasFlag(this Enum value, params Enum[] flags)
        {
            bool suc = true;

            foreach (Enum e in flags)
                suc &= value.HasFlag(e);

            return suc;
        }
    }
}
