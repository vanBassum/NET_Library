using STDLib.JBVProtocol.IO;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace STDLib.JBVProtocol.Connections
{
    public class SerialConnection : Connection
    {
        SerialPort serial = new SerialPort();


        public SerialConnection(string com, int baud)
        {
            serial.DataReceived += Serial_DataReceived;
            serial.PortName = com;
            serial.BaudRate = baud;
            serial.Open();
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if(sender is SerialPort port)
            {
                int length = port.BytesToRead;
                byte[] data = new byte[length];
                port.Read(data,0,length);
                HandleData(data);
            }
        }

        protected override void SendData(byte[] data)
        {
            serial.Write(data, 0, data.Length);
        }
    }
}
