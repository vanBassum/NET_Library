﻿using STDLib.Commands;
using STDLib.JBVProtocol.IO;
using STDLib.JBVProtocol.IO.CMD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BaseCommand = STDLib.JBVProtocol.IO.CMD.BaseCommand;

namespace STDLib.JBVProtocol
{
    /// <summary>
    /// The ID server manages the ids of all devices connected.
    /// </summary>
    public partial class LeaseServer
    {
        public int LeaseTimeout = 60 * 60 * 2;
        public UInt16 ID { get; } = 0;
        List<Lease> Leases { get; } = new List<Lease>();
        Connection connection;
        System.Timers.Timer leaseRefreshTimer = new System.Timers.Timer();

        public LeaseServer(Connection connection)
        {
            Leases.Add(new Lease { ID = ID, Key = Guid.NewGuid(), Expire = DateTime.MaxValue });
            this.connection = connection;
            connection.OnFrameReceived += Connection_OnFrameReceived;
            leaseRefreshTimer.Elapsed += LeaseRefreshTimer_Elapsed;
            leaseRefreshTimer.Interval = 1000;
            leaseRefreshTimer.Start();

            STDLib.Commands.BaseCommand.Register("Leases", PrintLeases);
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

            if(e.Command)
            {
                BaseCommand bcmd = BaseCommand.GetCommand(e.PAY);

                switch (bcmd)
                {
                    case CMD_RequestLease cmd:  //Some client asked for an id.
                        GiveClient_NewID(cmd.Key);
                        break;
                }
            }
        }


        void GiveClient_NewID(Guid guid)
        {
            lock(Leases)
            {
                //First check if the guid already exists in the list with leases.
                //If so, extend the existing lease instead of creating a new one.

                Lease lease = Leases.FirstOrDefault(l=>l.Key == guid);
                if(lease != null)
                {
                    lease.Expire = DateTime.Now.AddSeconds(LeaseTimeout);
                    SendAnswer(lease);
                }
                else
                {
                    UInt16 id = 0;
                    for (id = 0; id < 0xFFFF; id++)
                    {
                        int ind = Leases.FindIndex(a => a.ID == id);
                        if (ind == -1)
                        {
                            //Free id found.
                            lease = new Lease();
                            lease.ID = id;
                            lease.Key = guid;
                            lease.Expire = DateTime.Now.AddSeconds(LeaseTimeout);
                            Leases.Add(lease);
                            SendAnswer(lease);
                            break;
                        }
                    }
                    if (id == 0xFFFF)
                    {
                        //TODO: No free id's
                    }

                }
            }
        }


        void SendAnswer(Lease newLease)
        {
            Console.WriteLine("Lease accepted " + newLease.ToString());
            CMD_ReplyLease cmd = new CMD_ReplyLease(newLease);
            Frame tx = cmd.CreateCommandFrame(ID);
            connection.SendFrame(tx);
        }
    }
}