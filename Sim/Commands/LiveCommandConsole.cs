using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sim.Map;

namespace Sim.Commands
{
    class LiveCommandConsole
    {

        public static MapBase CurrentMap { get { return Core.Map; } }

        public static bool IsOpened = false;

        public static Task StartNew()
        {
            Logger.Log("Opening live command executor window.");
            IsOpened = true;
            while (true)
            {
                ParseLine(Console.ReadLine());
            }
            return Task.CompletedTask;
        }

        static async Task ParseLine(string input)
        {
            try
            {
                LiveCommandExecutor.ExecuteCommand(Command.ParseString(input));
            }catch(Exception e)
            {
                Logger.Log("Can not execute command.");
                Logger.Exception(e);
            }
            
        }
    }
}
