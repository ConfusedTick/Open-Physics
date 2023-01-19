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

        public const bool IsDevBuild = true;

        private Map.MapBase Map { get; set; }

        public TimeSpan TPS;

        public ulong TickId { get; private set; }

        public static Random Random = new Random();

        public Map.MapBase Prepare()
        {
            LastInstance = this;

            Logger.Log("Preparing core...", "Core");

            LoadAll(Path.Combine(Directory.GetCurrentDirectory(), "data/"));

            LoadMap();

            //Map.StartTicking();


            Logger.Log("Core ready", "Core", '!', ConsoleColor.Green);
            return Map;
        }

        public void LoadMap()
        {
            Water water = new Water(Map, new Vector2(10, 22), Flags.Empty);
            water.InitPosition();
            Map.AddParticle(water);

            HeatedParticle heater = new HeatedParticle(Map, new Vector2(50, 30), Flags.Empty);
            heater.InitPosition();
            Map.AddParticle(heater);


            water = new Water(Map, new Vector2(22, 50), Flags.Empty);
            water.InitPosition();
            Map.AddParticle(water);

            heater = new HeatedParticle(Map, new Vector2(45, 23), Flags.Empty);
            heater.InitPosition();
            Map.AddParticle(heater);

            water = new Water(Map, new Vector2(31, 45), Flags.Empty);
            water.InitPosition();
            Map.AddParticle(water);

            heater = new HeatedParticle(Map, new Vector2(21, 21), Flags.Empty);
            heater.InitPosition();
            Map.AddParticle(heater);

            water = new Water(Map, new Vector2(5, 5), Flags.Empty);
            water.InitPosition();
            Map.AddParticle(water);

            heater = new HeatedParticle(Map, new Vector2(3, 10), Flags.Empty);
            heater.InitPosition();
            Map.AddParticle(heater);
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
