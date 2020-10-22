using STDLib.Misc;
using System;
using System.Linq;

namespace STDLib.JBVProtocol
{
    public static class CommandList
    {

        static Map<UInt16, Type> CMDList = new Map<UInt16, Type> 
        {
            { 0, typeof(ReplyNACK) },
            { 1, typeof(ReplyACK) },
            { 2, typeof(RequestSoftwareID) },
            { 3, typeof(ReplySoftwareID) },
            { 5, typeof(RequestSetLED) },
        };

        
        public static bool TryGetType(UInt16 cmdId, out Type type)
        {
            return CMDList.TryForward(cmdId, out type);
        }

        public static Type GetCommand(UInt16 cmdId)
        {
            return CMDList.Forward[cmdId];
        }


        public static bool TryGetID(Type type, out UInt16 cmdId)
        {
            return CMDList.TryReverse(type, out cmdId);
        }

        public static UInt16 GetID(Type cmd)
        {
            return CMDList.Reverse[cmd];
        }

    }

}
