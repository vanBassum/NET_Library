using STDLib.Misc;
using System;

namespace STDLib.JBVProtocol
{
    public static class CommandList
    {

        static Map<UInt32, Type> CMDList = new Map<uint, Type> 
        {
            //Uneven numbers are requests, even numbers are reply!
            { 1, typeof(RequestSoftwareID) },
            { 2, typeof(ReplySoftwareID) },
        };

    
        public static Type GetCommand(UInt32 cmdId)
        {
            return CMDList.Forward[cmdId];
        }

        public static UInt32 GetID(Type cmd)
        {
            return CMDList.Reverse[cmd];
        }

    }

}
