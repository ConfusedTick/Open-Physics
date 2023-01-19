using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Sim
{
    class Logger
    {

        public static List<string> ignoreProcesses = new List<string>() { };

        public static List<char> ignoreLogSymbol = new List<char>() { };

        public static bool Save = true;
        public static string SaveFile = Path.Combine(Directory.GetCurrentDirectory(), "logs.lf");

        public static void Ignore(char logSymbol)
        {
            if (!ignoreLogSymbol.Contains(logSymbol))
            {
                ignoreLogSymbol.Add(logSymbol);
            }
        }

        public static void Ignore(string ignore)
        {
            if (!ignoreProcesses.Contains(ignore))
            {
                ignoreProcesses.Add(ignore);
            }
        }

        public static void Exception(Exception e)
        {
            if (Core.IsDevBuild) Log(e.Message, "System", '!', ConsoleColor.Red);
            else throw e;
        }

        public static void Log(string message, string processName = "System", char logSymbol = '*', ConsoleColor textColor = ConsoleColor.DarkGray)
        {
            if (ignoreProcesses.Contains(processName) || ignoreLogSymbol.Contains(logSymbol)) return;
            Console.ForegroundColor = textColor;
            string text = "[" + DateTime.Now.ToString("h:mm:ss") + "]" + "[" + processName + "][" + logSymbol + "] " + message;
            
            Console.WriteLine(text);

            if (Save) File.AppendAllText(SaveFile, text + "\n");

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void LoadIgnoring(string file)
        {
            string[] data = File.ReadAllLines(file);
            string[] args;

            foreach (string str in data)
            {
                if (str.StartsWith("#"))
                {
                    continue;
                }

                args = str.Replace(" ", "").Split("=");
                switch (args[0].ToLower())
                {
                    case "ignoresymbols":
                        char[] symbols = args[1].ToCharArray();
                        foreach (char symbol in symbols)
                        {
                            Ignore(symbol);
                        }

                        break;

                    case "ignoreprocesses":
                        string[] processes = args[1].Split(",");
                        foreach (string process in processes)
                        {
                            Ignore(process);
                        }

                        break;

                    default:
                        break;
                }
            }
        }
    }
}
