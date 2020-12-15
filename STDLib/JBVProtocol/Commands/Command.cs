using System;
using System.Collections.Generic;
using System.Linq;



namespace STDLib.JBVProtocol.Commands
{

    public abstract class Command
    {
        static bool isInitialized = false;
        Frame frame = new Frame();
        static Dictionary<UInt32, Type> CommandList = new Dictionary<uint, Type>() { };
        protected abstract bool IsBroadcast { get; }
        public abstract UInt32 CommandID { get; }
        public UInt16 TxID { get { return frame.TxID; } set { frame.TxID = value; } }
        public UInt16 RxID { get { return frame.RxID; } set { frame.RxID = value; } }
        public UInt16 Sequence { get { return frame.Sequence; } set { frame.Sequence = value; } }

        public abstract byte[] ToArray();
        public abstract void Populate(byte[] data);


        public Frame GetFrame()
        {
            if (IsBroadcast) frame.Options |= Frame.OPT.Broadcast;
            frame.CommandID = (UInt32)CommandID;
            frame.Data = ToArray();
            frame.DataLength = (UInt16)frame.Data.Length;
            return frame;
        }


        public static Command Create(UInt32 commandId)
        {
            if (!isInitialized) InitList();
            return (Command)Activator.CreateInstance(CommandList[commandId]);
        }

        public static Command Create(Frame frame)
        {
            if (!isInitialized) InitList();
            Command cmd = null;
            Type type;

            if (CommandList.TryGetValue(frame.CommandID, out type))
            {
                cmd = (Command)Activator.CreateInstance(type);
                cmd.frame = frame;
                //cmd.IsBroadcast = frame.Options.HasFlag(Frame.OPT.Broadcast);
                cmd.Populate(frame.Data);
            }

            return cmd;
        }


        public static void InitList()
        {
            var type = typeof(Command);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p != type);

            foreach (Type t in types)
            {
                Command instance = (Command)Activator.CreateInstance(t);
                if (CommandList.ContainsKey((UInt32)instance.CommandID))
                    throw new Exception("Duplicate command!");
                else
                {
                    CommandList[(UInt32)instance.CommandID] = t;
                    Logger.LOGI($"Command added '{t.Name}'");
                }
            }
            isInitialized = true;
        }
    }

}