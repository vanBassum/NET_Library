﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace STDLib.Commands
{

    public abstract class BaseCommand
    {
        public static List<BaseCommand> Commands { get; } = new List<BaseCommand>();
        public virtual string CMD { get; set; }
        public virtual string Description { get { return "No description available"; } }
        protected static bool Work { get; set; } = true;

        protected static int LongestCmd { get; private set; } = 0;

        public BaseCommand()
        {
            Commands.Add(this);
            CMD = this.GetType().Name;

            if (LongestCmd < this.CMD.Length)
                LongestCmd = this.CMD.Length;
        }

        public BaseCommand(string cmd)
        {
            Commands.Add(this);
            CMD = cmd;

            if (LongestCmd < this.CMD.Length)
                LongestCmd = this.CMD.Length;
        }



        public abstract void Execute();

        public static void Do()
        {
            //Static commands:
            Help help = new Help();
            Exit exit = new Exit();

            while (Work)
            {
                string input = Console.ReadLine();

                //Parse input for arguments

                BaseCommand cmd = Commands.FirstOrDefault(c => c.CMD.ToLower() == input.ToLower());

                if (cmd == null)
                {
                    Console.WriteLine("Command not found, use Help to get a list of all avaiable commands");
                }
                else
                {
                    cmd.Execute();
                }

            }
            

            Console.WriteLine("Bye");
        }

        public static void Register(string cmd, Action action)
        {
            ActionCMD actionCMD = new ActionCMD(cmd, action);
        }
    }

    public class ActionCMD : Commands.BaseCommand
    {
        Action exec;

        public ActionCMD(string cmd, Action exec) : base(cmd)
        {
            this.exec = exec;
        }

        public override void Execute()
        {
            exec.Invoke();
        }
    }
}
