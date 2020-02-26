using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace MasterLibrary.Ethernet
{
    //Class to escape en deescape data so that you recieve 1 event per 1 package
    public class TcpSocketClientEscaped : TcpSocketClient
    {
        
        public event OnDataRecievedHandler OnPackageRecieved;

        public void SendPackage(byte[] data)
        {
            List<byte> escData = new List<byte> { };

            escData.Add((byte)Ch.SOF);
            foreach (byte b in data)
            {
                if (Enum.IsDefined(typeof(Ch), b))
                    escData.Add((byte)Ch.ESC);

                escData.Add(b);
            }
            escData.Add((byte)Ch.EOF);

            SendDataSync(escData.ToArray());
        }

        private List<byte> recData = new List<byte>();
        private RecState recState = RecState.WaitForStart;

        public TcpSocketClientEscaped()
        {
        }

        public TcpSocketClientEscaped(Socket s) : base(s)
        {
        }

        public override void DataRecieved(byte[] escData)
        {

            for(int i = 0; i<escData.Length; i++)
            {

                switch(recState)
                {
                    case RecState.WaitForStart:
                        if (escData[i] == (byte)Ch.SOF)
                        {
                            recData.Clear();
                            recState = RecState.Recording;
                        }
                        break;
                    case RecState.Escaped:
                        recData.Add(escData[i]);
                        recState = RecState.Recording;
                        break;
                    case RecState.Recording:
                        if (escData[i] == (byte)Ch.ESC)
                            recState = RecState.Escaped;
                        else if (escData[i] == (byte)Ch.EOF)
                        {
                            OnPackageRecieved?.Invoke(this, recData.ToArray());
                            recState = RecState.WaitForStart;
                        }
                        else
                        {
                            recData.Add(escData[i]);
                        }
                        break;
                }
            }
        }

        enum RecState
        {
            WaitForStart,
            Recording,
            Escaped
        }
        enum Ch : byte
        {
            ESC,
            SOF,
            EOF
        }
    }
    
}