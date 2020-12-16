﻿using System;

namespace FRMLib.Scope.Controls
{
    public static class EnumExt
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
    }
}
