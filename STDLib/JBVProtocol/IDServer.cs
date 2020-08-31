using STDLib.Commands;
using STDLib.JBVProtocol.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.JBVProtocol
{
    /// <summary>
    /// The ID server manages the ids of all devices connected.
    /// </summary>
    public partial class IDServer
    {
        public UInt16 ID { get; } = 0;
        List<Lease> Leases { get; } = new List<Lease>();
        Connection connection;
        System.Timers.Timer leaseRefreshTimer = new System.Timers.Timer();

        public IDServer(Connection connection)
        {
            Leases.Add(new Lease { ID = ID, Key = null, Expire = null });
            this.connection = connection;
            connection.OnFrameReceived += Connection_OnFrameReceived;
            leaseRefreshTimer.Elapsed += LeaseRefreshTimer_Elapsed;
            leaseRefreshTimer.Interval = 1000;
            leaseRefreshTimer.Start();

            Leases leases = new Leases(PrintLeases);
        }

        void PrintLeases()
        {
            Console.WriteLine("Current leases:");
            foreach(Lease l in Leases)
            {
                Console.WriteLine($" - {l}");
            }
        }


        private void LeaseRefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock(Leases)
            {
                for (int i = 0; i < Leases.Count; i++)
                {
                    Lease lease = Leases[i];
                    if (lease.Expire < DateTime.Now)
                    {
                        Leases.RemoveAt(i);
                        i--;
                        Console.WriteLine($"Lease timeout: {lease}");
                    }
                }
            }
        }

        private void Connection_OnFrameReceived(object sender, Frame e)
        {
            if (e.IDInfo)
            {
                Guid guid;  //Guid.NewGuid()

                if (Guid.TryParse(Encoding.ASCII.GetString(e.PAY), out guid))
                {
                    //Some client asked for an id.

                    if (e.SID == 0)
                    {
                        //Create a new ID for the client.
                        GiveClient_NewID(guid);
                    }
                    else
                    {
                        //The client asked for a specific id.
                        if(!GiveClient_SpecificID(e.SID, guid))
                        {
                            //Id wasn't available.
                            GiveClient_NewID(guid);
                        }
                    }
                }
                
            }
        }


        void GiveClient_NewID(Guid guid)
        {
            lock(Leases)
            {
                UInt16 id = 0;
                for (id = 0; id<0xFFFF; id++)
                {
                    int ind = Leases.FindIndex(a => a.ID == id);
                    if (ind == -1)
                    {
                        //Free id found.
                        Lease lease = new Lease();
                        lease.ID = id;
                        lease.Key = guid;
                        lease.Expire = DateTime.Now.AddHours(2);
                        Leases.Add(lease);
                        SendAnswer(lease);
                        break;
                    }
                }
                if(id == 0xFFFF)
                {
                    //TODO: No free id's
                }
            }
        }

        void GiveClient_ExtendLease(int ind)
        {
            lock (Leases)
            {
                Leases[ind].Expire = DateTime.Now.AddHours(2);
                SendAnswer(Leases[ind]);
            }
        }

        bool GiveClient_SpecificID(UInt16 id, Guid guid)
        {
            lock (Leases)
            {
                //The client requests a specific id.
                int ind = Leases.FindIndex(a => a.ID == id);
                if (ind == -1)
                {
                    //ID free, add to lease table and tell client the id is oke.
                    Lease lease = new Lease();
                    lease.ID = id;
                    lease.Key = guid;
                    lease.Expire = DateTime.Now.AddHours(2);
                    Leases.Add(lease);
                    SendAnswer(lease);
                    return true;
                }
                else
                {
                    if (Leases[ind].Key == guid)
                    {
                        //Extend lease
                        GiveClient_ExtendLease(ind);
                        return true;
                    }
                    else
                    {
                        //ID already taken.

                        /* Specifically chosen not to do this. If a lease has expired some thread should request an extention. 
                         * If not replied within x the lease should be removed from the list.
                        if(Leases[ind].Expire < DateTime.Now)
                        {
                            //Lease was expired, reclaim the id for the new client.
                            Leases[ind].ID = id;
                            Leases[ind].Key = guid;
                            Leases[ind].Expire = DateTime.Now.AddHours(2);
                            SendAnswer(Leases[ind]);
                        }
                        */
                        return false;
                    }
                }
            }
        }

        void SendAnswer(Lease newLease)
        {
            Console.WriteLine("Lease accepted " + newLease.ToString());
            Frame reply = new Frame();
            reply.Broadcast = true;
            reply.IDInfo = true;
            reply.SID = ID;
            reply.PAY = Encoding.ASCII.GetBytes(newLease.ToString());
            reply.RID = newLease.ID;
            connection.SendFrame(reply);
        }
    }

    public class Leases : BaseCommand
    {
        Action exec;

        public Leases(Action exec)
        {
            this.exec = exec;
        }

        public override void Execute()
        {
            exec.Invoke();
        }
    }

}
