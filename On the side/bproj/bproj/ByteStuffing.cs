﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace bproj
{
    public class ByteStuffing
    {
        enum BS : byte
        {
            SOF = (byte)'&',
            EOF = (byte)'%',
            ESC = (byte)'\\',
            NOP = (byte)'*',
        }

        bool startFound = false;
        bool esc = false;
        List<byte> dataBuffer = new List<byte>();

        public void Unstuff(List<byte> data)
        {
            int len = data.Count();
            for (int i = 0; i < len; i++)
            {
                bool record = false;

                if (esc)
                {
                    record = true;
                    esc = false;
                }
                else
                {
                    switch (data[i])
                    {
                        case (byte)BS.ESC:
                            esc = true;
                            break;
                        case (byte)BS.SOF:
                            startFound = true;
                            dataBuffer.Clear();
                            break;
                        case (byte)BS.EOF:
                            startFound = false;
                            OnFrameCollected(dataBuffer);
                            break;
                        case (byte)BS.NOP:
                            break;
                        default:
                            record = true;
                            break;
                    }
                }

                if (record && startFound)
                    dataBuffer.Add(data[i]);
            } 
        }

        public List<byte> Stuff(List<byte> frame)
        {
            List<byte> dataOut = new List<byte>();
            dataOut.Add((byte) BS.SOF);
            foreach(byte b in frame)
            {
                if(Enum.IsDefined(typeof(BS), b))
                    dataOut.Add((byte)BS.ESC);
                dataOut.Add(b);
            }

            dataOut.Add((byte)BS.EOF);
            return dataOut;           
        }


        public event Action<List<byte>> OnFrameCollected;
    }


}