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
using Sim.Commands;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace Sim
{
    /// <summary>
    /// Основной класс, загружающий игру
    /// </summary>
    public class Core
    {

        public static MapBase Map;
        public static MapBase PreviousMap;
        public static Physic Physics;


        public static Random Random = Utils.Utils.GetNewRandom();

        
        public static void InitializeGameStart(Size size)
        {
            Logger.Log("Preparing core...", "Core");
            if (!ParticleFactory.Initialized) ParticleFactory.Initialize();
            LoadAll(Path.Combine(Directory.GetCurrentDirectory(), "data/"));
            Map = new MapBase(size, Physics);
            Logger.Log("Core ready", "Core", '!', ConsoleColor.Green);
            Task.Run(LiveCommandConsole.StartNew);
        }

        public static void ChangeMap(MapBase newMap)
        {
            PreviousMap = Map;
            Map = newMap;
        }

        public static void LoadAll(string cfgDir)
        {
            Logger.Log("Loading configs from " + cfgDir, "Core");
            Physics = new Physic();

            string physicsfile = Path.Combine(cfgDir, "lastconf.txt");
            if (!File.Exists(physicsfile))
            {
                FileInfo fileInfo = new FileInfo(physicsfile);

                if (!fileInfo.Exists)
                {
                    _ = Directory.CreateDirectory(fileInfo.Directory.FullName);
                }

                File.Create(physicsfile).Close();
                Physics.Save(physicsfile);
            }

            Physics.Load(Path.Combine(cfgDir, "lastconf.txt"));

            Logger.Log("Configs loaded", "Core");
        }
    }
}
