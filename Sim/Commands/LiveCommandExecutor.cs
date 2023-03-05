using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sim.Map;
using Sim.Particles;

namespace Sim.Commands
{
    public class LiveCommandExecutor
    {

        public static MapBase Map { get { return LiveCommandConsole.CurrentMap; } } 

        public static void ExecuteCommand(Command cmd)
        {
            switch (cmd.CommandName)
            {
                case "execute":
                case "start":
                    ExecuteScript(cmd.Arguments[0]);
                    break;

                case "info":
                    Logger.PrintInformation();
                    break;

                case "save":
                    switch (cmd.Arguments[0])
                    {
                        case "map":
                            Map.Save(cmd.Arguments[1]);
                            break;

                        case "physics":
                            Map.Physics.Save(cmd.Arguments[1]);
                            break;
                    }
                    break;

                case "load":
                    switch (cmd.Arguments[0])
                    {
                        case "map":
                            App.MainWindow.ChangeMap(MapBase.Load(cmd.Arguments[1], Map.Physics));
                            break;

                        case "physics":
                            Map.Physics.Load(cmd.Arguments[1]);
                            break;

                        default:
                            Logger.Log("Element " + cmd.Arguments[0] + " is not found.", "LCE", textColor: ConsoleColor.DarkRed);
                            return;
                    }
                    break;

                case "remove":
                    switch (cmd.Arguments[0])
                    {
                        case "glvalue":
                            GlobalValues.RemoveValue(cmd.Arguments[1]);
                            break;

                        case "particle":
                            int x, y;
                            x = Convert.ToInt32(cmd.Arguments[1]);
                            y = Convert.ToInt32(cmd.Arguments[2]);
                            ParticleBase partl = Map.GetParticle(x, y);
                            if (partl == null)
                            {
                                Logger.Log("Particle on " + x.ToString() + ", " + y.ToString() + " is not found.", "LCE");
                                return;
                            }
                            partl.Remove();
                            break;
                    }
                    break;

                case "get":
                    switch (cmd.Arguments[0])
                    {

                        case "glvalue":
                            Logger.Log("Value of " + cmd.Arguments[1] + ": " + GlobalValues.GetValue(cmd.Arguments[1]));
                            break;

                        case "particle":
                            int x, y;
                            x = Convert.ToInt32(cmd.Arguments[1]);
                            y = Convert.ToInt32(cmd.Arguments[2]);
                            ParticleBase partl = Map.GetParticle(x, y);
                            if (partl == null)
                            {
                                Logger.Log("Particle on " + x.ToString() + ", " + y.ToString() + " is not found.", "LCE");
                                return;
                            }
                            switch (cmd.Arguments[3])
                            {

                                case "id":
                                    Logger.Log(partl.Id.ToString(), "LCE");
                                    break;

                                case "uid":
                                    Logger.Log(partl.Uid.ToString(), "LCE");
                                    break;

                                case "name":
                                    Logger.Log(partl.Name.ToString(), "LCE");
                                    break;

                                case "mass":
                                    Logger.Log(partl.Mass.ToString(), "LCE");
                                    break;

                                case "heatbuffer":
                                    Logger.Log(partl.HeatBuffer.ToString(), "LCE");
                                    break;

                                case "heatcapacity":
                                    Logger.Log(partl.HeatCapacity.ToString(), "LCE");
                                    break;

                                default:
                                    Logger.Log("Parameter " + cmd.Arguments[3] + " is not found.", "LCE", textColor: ConsoleColor.DarkRed);
                                    return;

                            }
                            break;

                        default:
                            Logger.Log("Argument " + cmd.Arguments[0] + " is not defined.", "LCE", textColor: ConsoleColor.DarkRed);
                            return;

                    }
                    break;

                case "set":
                    switch (cmd.Arguments[0])
                    {

                        case "glvalue":
                            GlobalValues.SetValue(cmd.Arguments[1], cmd.Arguments[2]);
                            break;

                        case "physics":
                            break;

                        case "particle":
                            int x, y;
                            x = Convert.ToInt32(cmd.Arguments[1]);
                            y = Convert.ToInt32(cmd.Arguments[2]);
                            ParticleBase partl = Map.GetParticle(x, y);
                            if (partl == null)
                            {
                                Logger.Log("Particle on " + x.ToString() + ", " + y.ToString() + " is not found.", "LCE");
                                return;
                            }
                            switch (cmd.Arguments[3])
                            {

                                case "x":
                                    partl.Position.SetX(Convert.ToDouble(cmd.Arguments[4]));
                                    partl.Position.SetForceUpdateOnNextTick(true);
                                    break;

                                case "y":
                                    partl.Position.SetY(Convert.ToDouble(cmd.Arguments[4]));
                                    partl.Position.SetForceUpdateOnNextTick(true);
                                    break;

                                case "temperature":
                                    partl.SetTemperature(Convert.ToInt32(cmd.Arguments[4]));
                                    break;

                                case "mass":
                                    partl.SetMass(Convert.ToInt32(cmd.Arguments[4]));
                                    break;

                                case "fix":
                                    partl.SetFixed(Utils.Utils.ToBool(cmd.Arguments[4]));
                                    break;

                                case "heatbuffer":
                                    partl.SetHeatBuffer(Convert.ToInt32(cmd.Arguments[4]));
                                    break;

                                default:
                                    Logger.Log("Parameter " + cmd.Arguments[3] + " is not found.", "LCE", textColor: ConsoleColor.DarkRed);
                                    return;

                            }
                            break;

                        default:
                            Logger.Log("Argument " + cmd.Arguments[0] + " is not defined.", "LCE", textColor: ConsoleColor.DarkRed);
                            return;

                    }
                    break;

                case "if":
                    string val = cmd.Arguments[0];
                    switch (cmd.Arguments[1])
                    {
                        case "exists":
                            if (GlobalValues.ContainsValue(val))
                            {
                                ExecuteScript(cmd.Arguments[2]);
                            }
                            break;

                        case "nexists":
                            if (!GlobalValues.ContainsValue(val))
                            {
                                ExecuteScript(cmd.Arguments[2]);
                            }
                            break;

                        case "equals":
                            if (GlobalValues.GetValue(val) == cmd.Arguments[2])
                            {
                                ExecuteScript(cmd.Arguments[3]);
                            }
                            break;

                        case "nequals":
                            if (GlobalValues.GetValue(val) == cmd.Arguments[2])
                            {
                                ExecuteScript(cmd.Arguments[3]);
                            }
                            break;

                        default:
                            Logger.Log("Argument " + cmd.Arguments[0] + " is not defined.", "LCE", textColor: ConsoleColor.DarkRed);
                            return;

                    }
                    break;

                default:
                    Logger.Log("Command " + cmd.CommandName + " is not found.", "LCE", textColor: ConsoleColor.DarkRed);
                    return;
            }
            Logger.Log("Executed.", "LCE");
        }

        public static void ExecuteScript(string scriptname)
        {
            List<Command> execute = new List<Command>();
            Command cmd;
            foreach (string l in File.ReadAllLines(scriptname))
            {
                cmd = Command.ParseString(l);
                if ((cmd.CommandName == "start" || cmd.CommandName == "execute") && cmd.Arguments[0] == scriptname)
                {
                    Logger.Log("Script " + scriptname + " is trying to execute itself.", "LCE", '!', ConsoleColor.DarkYellow);
                    continue;
                }
                execute.Add(cmd);
            }
            execute.ForEach(comm => ExecuteCommand(comm));
        }
    }
}
