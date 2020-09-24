using STDLib.JBVProtocol.IO;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol
{
    public abstract class CMD
    {
        public UInt16 SID { get; set; }
        public UInt32 CMDID { get { return CommandList.GetID(this.GetType()); } }
        public abstract Frame GetFrame();
        public abstract void Populate(List<byte[]> arg);

        public bool IsRequest { get { return (CMDID & 0x1) > 0; } }

        public static CMD FromFrame(Frame frame)
        {
            List<byte[]> parts = new List<byte[]>();
            Framing fr = new Framing();
            fr.OnFrameCollected += (a, b) => parts = b;
            fr.Unstuff(frame.PAY);

            if (parts.Count > 0)
            {
                //First part always contains the command!
                UInt32 icmd = BitConverter.ToUInt32(parts[0], 0);

                Type cmdType = CommandList.GetCommand(icmd);
                object instance = Activator.CreateInstance(cmdType);

                if (instance is CMD cmd)
                {
                    cmd.Populate(parts);
                    cmd.SID = frame.SID;
                    return cmd;
                }
            }
            throw new Exception("Couln't convert frame to command");
        }
    }



}
