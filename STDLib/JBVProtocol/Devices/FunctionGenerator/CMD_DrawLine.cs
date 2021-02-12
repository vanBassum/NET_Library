using STDLib.JBVProtocol.Commands;
using System.Drawing;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol.FunctionGenerator
{
    public partial class FunctionGenerator
    {
        public class CMD_DrawLine : Command
        {
            protected override bool IsBroadcast => false;
            public override UInt32 CommandID => 3;

            public UInt16 X1;
            public UInt16 Y1;
            public UInt16 X2;
            public UInt16 Y2;

            public Color Color { get; set; }


            public override void Populate(byte[] data)
            {
                throw new NotImplementedException();
                Color = Color.FromArgb(data[0], data[1], data[2]);
            }

            public override byte[] ToArray()
            {
                List<byte> data = new List<byte>();

                data.AddRange(BitConverter.GetBytes(X1));
                data.AddRange(BitConverter.GetBytes(Y1));
                data.AddRange(BitConverter.GetBytes(X2));
                data.AddRange(BitConverter.GetBytes(Y2));
                data.Add(Color.R);
                data.Add(Color.G);
                data.Add(Color.B);
                return data.ToArray();
            }
        }


    }

}