using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sim.Events;
using Sim.Enums;
using Sim.Map;
using Sim.Simulation;

namespace Sim.Simulation
{
    public class Physic : ICloneable
    {
        public string Path;

        public int GravityVectorAngle = 90;

        public double DeltaTime = 0.01d;
        public int Smoothness = 15;

        public double StartAcceleration { get; set; } = 50d;
        public double GravityAcceleration { get; set; } = 0.981d;

        public double StefanBoltzmannConst = 5.670374419d * (double)Math.Pow(10, -8);

        public int RaycastRayNumbers = 2000;

        public double CasterDepthStep = 0.9d;

        public HeatRadiationRenders HeatRender = HeatRadiationRenders.RT;

        public double MinTemperature = -273.25d;

        public double MaxTemperature = 10000000;

        public event EventHandler PhysicParametersChanged;

        public bool EnablePositionTick = true;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Load(string file)
        {
            Path = file;
            string[] strs = File.ReadAllLines(file);
            string[] args;
            foreach (string str in strs)
            {
                if (str.StartsWith("#"))
                {
                    continue;
                }

                if (str.Replace(" ", "") == null)
                {
                    continue;
                }

                args = str.Replace(" ", "").Split("=");
                switch (args[0].ToLower())
                {
                    case "startacceleration":
                        StartAcceleration = Convert.ToDouble(args[1]);
                        break;

                    case "gravityacceleration":
                        GravityAcceleration = Convert.ToDouble(args[1]);
                        break;

                    case "smoothness":
                        Smoothness = Convert.ToInt32(args[1]);
                        break;

                    case "secondspertick":
                        DeltaTime = Convert.ToDouble(args[1]);
                        break;

                    case "gravityangle":
                        GravityVectorAngle = Convert.ToInt32(args[1]);
                        break;

                    case "ignoresymbols":
                        char[] symbols = args[1].ToCharArray();
                        foreach (char symbol in symbols)
                        {
                            Logger.Ignore(symbol);
                        }

                        break;

                    case "ignoreprocesses":
                        string[] processes = args[1].Split(",");
                        foreach (string process in processes)
                        {
                            Logger.Ignore(process);
                        }

                        break;

                    case "stefanboltzmannconst":
                        StefanBoltzmannConst = Convert.ToDouble(args[1]);
                        break;

                    case "raycastraynumbers":
                        RaycastRayNumbers = Convert.ToInt32(args[1]);
                        break;

                    case "casterdepthstep":
                        CasterDepthStep = Convert.ToDouble(args[1]);
                        break;

                    case "heatradiationrender":
                        switch (args[1].ToLower())
                        {
                            case "rc":
                                HeatRender = HeatRadiationRenders.RC;
                                break;

                            case "rt":
                                HeatRender = HeatRadiationRenders.RT;
                                break;

                            case "lrt":
                                HeatRender = HeatRadiationRenders.LRT;
                                break;

                            case "none":
                                HeatRender = HeatRadiationRenders.NONE;
                                break;

                            default:
                                try
                                {
                                    HeatRender = (HeatRadiationRenders)Convert.ToByte(args[1]);
                                }
                                catch (Exception)
                                {
                                    Logger.Log("Undefined value of parameter: " + args[0], "Physics", '!', ConsoleColor.Red);
                                }
                                break;
                        }
                        break;


                    case "mintemperature":
                        MinTemperature = Convert.ToDouble(args[1]);
                        break;


                    case "maxtemperature":
                        MaxTemperature = Convert.ToDouble(args[1]);
                        break;

                    default:
                        Logger.Log("Undefined parameter: " + args[0], "Physics", '!', ConsoleColor.Red);
                        break;
                }
            }
            if (CasterDepthStep > Size.DefaultSize.Width || CasterDepthStep > Size.DefaultSize.Height)
            {
                Logger.Log("Warning: depth step is bigger then standart size.", "Physics", '!', ConsoleColor.Yellow);
                double minval;
                if (Size.DefaultSize.Width > Size.DefaultSize.Height)
                {
                    minval = Size.DefaultSize.Height;
                }
                else
                {
                    minval = Size.DefaultSize.Width;
                }
                Logger.Log("Please, lower it to " + (minval - 0.1d), "Physics", '!', ConsoleColor.Yellow);
            }
            Logger.Log("Physics loaded", "Physics");
            Update();
        }

        public void Update()
        {
            Logger.Log("Physics parameters updated", "Physics");
            PhysicParametersChanged?.Invoke(this, new PhysicParametersChangedEventArgs());
        }

        public static double CelsToKel(double cels)
        {
            return (double)(cels + 273.15d);
        }


        public void Save(string filename)
        {
            File.WriteAllText(filename, string.Empty);
            File.AppendAllText(filename, "#" + DateTime.Now.ToString("D") + "\n");
            File.AppendAllText(filename, "gravityangle=" + GravityVectorAngle + "\n");
            File.AppendAllText(filename, "secondspertick=" + DeltaTime + "\n");
            File.AppendAllText(filename, "#Smoothness of displaying values (1 to 15), reduces lags\n");
            File.AppendAllText(filename, "smoothness=" + Smoothness + "\n");
            File.AppendAllText(filename, "startacceleration=" + StartAcceleration + "\n");
            File.AppendAllText(filename, "gravityacceleration=" + GravityAcceleration + "\n");
            File.AppendAllText(filename, "stefanboltzmannconst=" + StefanBoltzmannConst + "\n");
            File.AppendAllText(filename, "#Heat render parameters.\n");
            File.AppendAllText(filename, "#Ray cast ray numbers. Bigger value - more lags, but more accuracy.\n");
            File.AppendAllText(filename, "#Not used in RT render.\n");
            File.AppendAllText(filename, "raycastraynumbers=" + RaycastRayNumbers + "\n");
            File.AppendAllText(filename, "#Ray caster depth step. Bigger value - less lags, but less accuracy.\n");
            File.AppendAllText(filename, "casterdepthstep=" + CasterDepthStep + "\n");
            File.AppendAllText(filename, "#Heat radiation render: Ray Tracing(RT), Ray Casting(RC), None(NONE)\n");
            File.AppendAllText(filename, "#RT faster then RC, NONE is the fastest, just no heat render \n");
            File.AppendAllText(filename, "heatradiationrender=" + HeatRender.ToString() + "\n");
            File.AppendAllText(filename, "MinTemperature=" + MinTemperature.ToString() + "\n");
            File.AppendAllText(filename, "MaxTemperature=" + MaxTemperature.ToString() + "\n");
            Logger.Log("Physics parameters saved to " + filename, "Physics");
            //File.AppendAllText(filename, );
        }

    }
}
