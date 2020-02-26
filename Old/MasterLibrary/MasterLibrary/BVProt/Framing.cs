using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.BVProt
{

    public interface IFrame
    {
        void Parse(List<List<byte>> data);
        List<List<byte>> Parse();
    }

    public class Framing<T> where T : IFrame, new()
    {
        public event Action<T> OnFrameRecieved;
        bool startFound = false;
        bool esc = false;
        readonly List<List<byte>> frameBuffer = new List<List<byte>>();
        readonly Stream rawStream;
        readonly Task reader;
        public Framing(Stream rawStream)
        {
            this.rawStream = rawStream;
            reader = new Task(ReadAsync);
            
        }

        public void StartTask()
        {
            reader.Start();
        }

        async void ReadAsync()
        {
            while(true)
            {
                byte[] data = new byte[256];
                int len = await rawStream.ReadAsync(data, 0, data.Length);

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
                                frameBuffer.Clear();
                                frameBuffer.Add(new List<byte>());
                                break;
                            case (byte)BS.SEP:
                                frameBuffer.Add(new List<byte>());
                                break;
                            case (byte)BS.EOF:
                                startFound = false;
                                T frame = new T();
                                frame.Parse(frameBuffer);
                                OnFrameRecieved?.Invoke(frame);
                                break;
                            case (byte)BS.NOP:
                                break;
                            default:
                                record = true;
                                break;
                        }
                    }
                    if (record && startFound)
                        frameBuffer.Last().Add(data[i]);
                }              
            }
        }

        public void SendFrame(IFrame frame)
        {
            rawStream.WriteByte((byte)BS.SOF);
            List<List<byte>> frameBuffer = frame.Parse();
            foreach (List<byte> data in frameBuffer)
            {
                foreach(byte b in data)
                {
                    if(Enum.IsDefined(typeof(BS), b))
                        rawStream.WriteByte((byte)BS.ESC);
                    rawStream.WriteByte(b);
                }

                if(data != frameBuffer.Last())
                    rawStream.WriteByte((byte)BS.SEP);
            }

            rawStream.WriteByte((byte)BS.EOF);
        }

        enum BS : byte
        {
            SOF = (byte)'&',
            EOF = (byte)'%',
            ESC = (byte)'\\',
            NOP = (byte)'*',
            SEP = (byte)'|',
        }
    }
}
