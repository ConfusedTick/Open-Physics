using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Commands
{
    public class Command
    {

        public string CommandName;
        public string[] Arguments;

        public Command(string command, string[] args)
        {
            CommandName = command;
            Arguments = args;
        }

        public override string ToString()
        {
            string outstr = CommandName;
            foreach (string arg in Arguments) outstr += " " + arg;
            return outstr;
        }

        public static Command ParseString(string inp)
        {
            string[] cmda = inp.Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);
            string cmdname = cmda[0];
            string[] cmdargs;
            if (cmda.Length == 1)
            {
                cmdargs = new string[] { };
            }
            else
            {
                cmdargs = cmda[1].Split(" ");
            }
            return new Command(cmdname, cmdargs);
        }

    }
}
