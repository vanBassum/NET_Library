using STDLib.JBVProtocol.IO;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol
{
    public abstract class CMD
    {
        public byte SeqNo { get; set; }
        protected virtual byte[] Data { get; set; } = new byte[0];
        public UInt16 CMDID { get { return CommandList.GetID(this.GetType()); } }
        public virtual bool IsBroadcast { get; } = false;
        public UInt16 SID { get; set; }


        public Frame ToFrame()
        {
            Frame f = new Frame();
            f.PAY = new byte[Data.Length + 3];
            f.PAY[0] = (byte)(CMDID >> 8);
            f.PAY[1] = (byte)(CMDID);
            f.PAY[2] = SeqNo;
            f.Broadcast = IsBroadcast;
            f.Command = false;
            Array.Copy(Data, 0, f.PAY, 3, Data.Length);
            return f;
        }

        public static CMD FromFrame(Frame frame)
        {
            if(frame.LEN >= 3)
            {
                UInt16 cmdId = (UInt16)(frame.PAY[0]<<8);
                cmdId += frame.PAY[1];

                Type cmdType;

                if (CommandList.TryGetType(cmdId, out cmdType))
                {
                    object instance = Activator.CreateInstance(cmdType);
                    if (instance is CMD cmd)
                    {
                        cmd.SeqNo = frame.PAY[2];
                        byte[] data = new byte[frame.LEN - 3];
                        Array.Copy(frame.PAY, 3, data, 0, data.Length);
                        cmd.Data = data;
                        cmd.SID = frame.SID;
                        return cmd;
                    }
                }
                else
                    return null;
            }
            throw new Exception("Couln't convert frame to command");
        }
    }
}
