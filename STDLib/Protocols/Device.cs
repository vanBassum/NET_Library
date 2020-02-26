using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace STDLib
{
    public class Device
    {
        Dictionary<UInt16, TaskCompletionSource<Command>> pending = new Dictionary<ushort, TaskCompletionSource<Command>>();
        public UInt16 ID { get; private set; } = 0;
        public Func<Command, Command> RequestHandler = new Func<Command, Command>((a) => { return new Command { CMD = a.CMD, IsError = true, Data = Encoding.ASCII.GetBytes("This client doenst support request handling.") }; });
        public ICommunicator Comminucator { get; private set; }

        public event EventHandler<Package> OnPackageRecieved;


        public Device()
        {

        }

        public Device(UInt16 id)
        {
            ID = id;
        }


        public void SendPackage(Package package)
        {
            Comminucator.SendPackage(package);
        }
        

        public void SetComminucator(ICommunicator com)
        {
            Comminucator = com;
            Comminucator.OnPackageRecieved += (object sender, Package e) =>
            {
                OnPackageRecieved?.Invoke(this, e);
                if (!e.IsReply && (e.RID == ID || e.RID == 0 || e.RID == 0xFFFF))
                {
                    Package pack = new Package();
                    pack.Command = HandleRequest(e.Command);
                    pack.RID = e.SID;
                    pack.SID = 0;
                    pack.TID = e.TID;
                    pack.IsReply = true;
                    Comminucator.SendPackage(pack);
                }
            };
        }

        Command HandleRequest(Command cmd)
        {
            Command result = new Command { IsError = true, CMD = cmd.CMD };
            switch (cmd.CMD)
            {
                case 0:
                    ID = BitConverter.ToUInt16(cmd.Data, 0);
                    result.IsError = false;
                    break;
                default:
                    RequestHandler(cmd);
                    break;
            }
            return result;
        }


        public async Task<Command> SendRequest(Command cmd, CancellationTokenSource cts = null)
        {
            cmd.IsError = false;
            UInt16 tid = SendRequestAsync(cmd);
            cts?.Token.Register(() => 
            { 

                lock (pending) 
                    if(pending.ContainsKey(tid))
                        pending[tid].TrySetCanceled(); 
            });



            Command result = await pending[tid].Task;
            lock (pending)
                pending.Remove(tid);
            return result;
        }

        private UInt16 SendRequestAsync(Command cmd)
        {
            Package package = new Package();
            package.TID = 0;
            lock (pending)
            {

                while (pending.ContainsKey(package.TID) && package.TID != 0xFFFF)
                    package.TID++;
                if (package.TID == 0xFFFF)
                    throw new Exception("No unique TID's left");
                pending[package.TID] = new TaskCompletionSource<Command>();
            }
            package.SID = 0;
            package.RID = ID;
            package.Command = cmd;
            Comminucator.OnPackageRecieved += (object sender, Package e) =>
            {
                lock (pending)
                {
                    if (e.IsReply)
                        pending[e.TID].SetResult(e.Command);
                }
            };

            Comminucator.SendPackage(package);
            return package.TID;
        }





    }


}
