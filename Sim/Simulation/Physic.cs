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

        public int GravityVectorAngle = 270;

        public double DeltaTime = 0.01d;
        public int Smoothness = 15;

        public double StartAcceleration { get; set; } = 50d;

        public double GravityAcceleration { get; set; } = 9.81d;

        public double StefanBoltzmannConst = 5.670374419d * (double)Math.Pow(10, -8);

        public double CasterDepthStep = 0.6d;

        public double MinTemperature = -273.25d;

        public double MaxTemperature = 2500;

        public double TemperatureDelta;

        public event EventHandler PhysicParametersChanged;

        public bool EnablePositionTick = false;

        public Physic()
        {
            TemperatureDelta = Math.Abs(MaxTemperature - MinTemperature);
        }

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

                    case "casterdepthstep":
                        CasterDepthStep = Convert.ToDouble(args[1]);
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
            if (CasterDepthStep > Size.GetDefaultSize().Width || CasterDepthStep > Size.GetDefaultSize().Height)
            {
                Logger.Log("Warning: depth step is bigger then standart size.", "Physics", '!', ConsoleColor.Yellow);
                string minvalname;
                double minval;
                if (Size.GetDefaultSize().Width > Size.GetDefaultSize().Height)
                {
                    minvalname = "height";
                    minval = Size.GetDefaultSize().Height;
                }
                else
                {
                    minvalname = "width";
                    minval = Size.GetDefaultSize().Width;
                }
                Logger.Log("Please, lower " + minvalname + " to " + (minval - 0.1d), "Physics", '!', ConsoleColor.Yellow);
            }
            Logger.Log("Physics loaded", "Physics");
            Update();
        }

        public void Update()
        {
            TemperatureDelta = Math.Abs(MaxTemperature - MinTemperature);
            Logger.Log("Physics parameters updated", "Physics");
            PhysicParametersChanged?.Invoke(this, new PhysicParametersChangedEventArgs(this));
        }

        public static double CelsToKel(double cels)
        {
            return (double)(cels + 273.15d);
        }


        public void Save(string filename)
        {
            try
            {
                File.WriteAllText(filename, string.Empty);
                File.AppendAllText(filename, "#" + DateTime.Now.ToString("D") + "\n");
                File.AppendAllText(filename, "gravityangle=" + GravityVectorAngle + "\n");
                File.AppendAllText(filename, "secondspertick=" + DeltaTime + "\n");
                File.AppendAllText(filename, "#Smoothness of real values (1 to 15), reduces lags\n");
                File.AppendAllText(filename, "smoothness=" + Smoothness + "\n");
                File.AppendAllText(filename, "startacceleration=" + StartAcceleration + "\n");
                File.AppendAllText(filename, "gravityacceleration=" + GravityAcceleration + "\n");
                File.AppendAllText(filename, "stefanboltzmannconst=" + StefanBoltzmannConst + "\n");
                File.AppendAllText(filename, "#Heat render parameters.\n");
                File.AppendAllText(filename, "#Ray cast ray numbers. Bigger value - more lags, but more accuracy.\n");
                File.AppendAllText(filename, "#Not used in RT render.\n");
                File.AppendAllText(filename, "#Ray caster depth step. Bigger value - less lags, but less accuracy.\n");
                File.AppendAllText(filename, "casterdepthstep=" + CasterDepthStep + "\n");
                File.AppendAllText(filename, "MinTemperature=" + MinTemperature.ToString() + "\n");
                File.AppendAllText(filename, "MaxTemperature=" + MaxTemperature.ToString() + "\n");
                Logger.Log("Physics parameters saved to " + filename, "Physics");

            }catch(Exception e)
            {
                Logger.Log("Failed to save physics parameters to " + filename, "Physics", textColor: ConsoleColor.Red);
                Logger.Log(e.Message, "Physics", textColor: ConsoleColor.Yellow);
            }
            
            //File.AppendAllText(filename, );
        }

    }
}
