using STDLib.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol.Devices
{
    public abstract class Device
    {
        public UInt16 ID { get; set; }
        public abstract SoftwareID SoftwareID { get; }
        public static JBVClient Client { get; set; }

        public static Map<SoftwareID, Type> Devices = new Map<SoftwareID, Type> {
            { SoftwareID.Router, typeof(STDLib.JBVProtocol.Devices.Router) },
            { SoftwareID.DPS50xx, typeof(STDLib.JBVProtocol.Devices.DPS50xx) },
        };

        static bool TryGetDevice(SoftwareID softwareID, out Type type)
        {
            return Devices.TryForward(softwareID, out type);
        }



        protected async Task<CMD> ExecuteCommand(CMD cmd, int timeoutMs)
        {
            TaskCompletionSource<CMD> tcs = new TaskCompletionSource<CMD>();

            var ct = new CancellationTokenSource(timeoutMs);
            ct.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            Client.OnCommand += (a, b) => tcs.TrySetResult(b);
            Client.Send(cmd, ID);

            try
            {
                return tcs.Task.Result;
            }
            catch
            {

            }
            return null;
            
        }


        
        public static List<T> GetDevices<T>(int timeout) where T : Device
        {
            List<T> result = new List<T>();

            SoftwareID softwareID = Activator.CreateInstance<T>().SoftwareID;

            Client.OnCommand += (a,b) => {
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
            cmd.SoftwareID = softwareID;
            Client.Send(cmd, 0);
            System.Threading.Thread.Sleep(timeout);

            return result;
        }
        
        
        public static List<Device> GetDevices(int timeout)
        {
            List<Device> result = new List<Device>();

            Client.OnCommand += (a, b) => {
                if (b is ReplySoftwareID reply)
                {
                    Type dev;
                    if(TryGetDevice(reply.SoftwareID, out dev))
                    {
                        Device t = (Device)Activator.CreateInstance(dev);
                        t.ID = reply.SID;
                        result.Add(t);
                    }
                    else
                    {

                    }
                }
            };

            RequestSoftwareID cmd = new RequestSoftwareID();
            cmd.SoftwareID = SoftwareID.Unknown;
            Client.Send(cmd, 0);

            System.Threading.Thread.Sleep(timeout);

            return result;
        }
        
    }
}

