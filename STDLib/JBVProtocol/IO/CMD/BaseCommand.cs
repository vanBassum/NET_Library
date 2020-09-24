using STDLib.Misc;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol.IO.CMD
{
    public abstract class BaseCommand
    {
        protected static Map<Commands, Type> CommandTable = new Map<Commands, Type> {

            //{Commands.Unknown           ,typeof()},
            {Commands.RequestID         ,typeof(CMD_RequestID)},
            {Commands.ReplyID           ,typeof(CMD_ReplyID)},
            {Commands.RequestLease      ,typeof(CMD_RequestLease)},
            {Commands.ReplyLease        ,typeof(CMD_ReplyLease)},
        };

        public enum Commands : byte
        {
            Unknown = 0,
            RequestID = 1,              
            ReplyID = 2,
            RequestLease = 3,           
            ReplyLease = 4,
        }

        public abstract byte[] ToArray();
        public abstract void FromArray(byte[] data);


        public static BaseCommand GetCommand(byte[] data)
        {
            Commands cmd = (Commands)data[0];
            BaseCommand bcmd = (BaseCommand)Activator.CreateInstance(CommandTable.Forward[cmd]);
            bcmd.FromArray(data);
            return bcmd;
        }
    }
}
