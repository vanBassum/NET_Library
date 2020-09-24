using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.JBVProtocol.Devices
{
    public abstract class Device
    {
        public UInt16 ID { get; set; }
        public abstract SoftwareID SoftwareID { get; }
        public static JBVClient Client { get; set; }
        

        public static List<T> GetDevices<T>(int timeout) where T : Device
        {
            List<T> result = new List<T>();

            SoftwareID softwareID = Activator.CreateInstance<T>().SoftwareID;

            Client.OnReplyRecieved += (a,b) => {
                if(b is ReplySoftwareID reply)
                {
                    if(reply.SoftwareID == softwareID)
                    {
                        T t = Activator.CreateInstance<T>();
                        t.ID = reply.SID;
                        result.Add(t);
                    }
                }   
            };

            RequestSoftwareID cmd = new RequestSoftwareID();
            cmd.SofwareID = softwareID;
            Client.Send(cmd);

            System.Threading.Thread.Sleep(timeout);

            return result;
        }
    }
}
