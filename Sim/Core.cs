using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.IO;
using Sim.GlmMath;
using Sim.Particles;
using Sim.Particles.ParticlesList;
using Sim.Simulation;
using Sim.Map;
using System.Windows.Threading;
using System.Threading;

namespace Sim
{
    public class Core
    {

        public static Core LastInstance;

        private Map.MapBase Map { get; set; }

        public TimeSpan TPS;

        public ulong TickId { get; private set; }

        public static Random Random = new Random();

        public Map.MapBase Prepare()
        {
            LastInstance = this;

            ParticleFactory.Initialize();

            Logger.Log("Preparing core...", "Core");

            LoadAll(Path.Combine(Directory.GetCurrentDirectory(), "data/"));

            LoadMap();

            Logger.Log("Core ready", "Core", '!', ConsoleColor.Green);
            return Map;
        }

        public void LoadMap()
        {
        }

        public void LoadAll(string cfgDir)
        {
            Logger.Log("Loading configs from " + cfgDir, "Core");
            Physic physics = new Physic();

            string physicsfile = Path.Combine(cfgDir, "lastconf.txt");
            if (!File.Exists(physicsfile))
            {
                FileInfo fileInfo = new FileInfo(physicsfile);

                if (!fileInfo.Exists)
                {
                    _ = Directory.CreateDirectory(fileInfo.Directory.FullName);
                }

                File.Create(physicsfile).Close();
                physics.Save(physicsfile);
            }

            physics.Load(Path.Combine(cfgDir, "lastconf.txt"));

            Size ms = new Size(80, 80);
            MapBase mp = new MapBase(ms, physics);
            Map = mp;

            Logger.Log("Configs loaded", "Core");
        }

        public void PrintInfo()
        {

        }


        

    }
}
