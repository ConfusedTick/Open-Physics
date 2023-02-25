using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Sim.Particles;
using Sim.GUI;
using Sim.Map;

namespace Sim
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static MainWindow MainWindow { get; private set; }

        public MapBase Map => Core.Map;

        public App()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            File.Delete(Logger.SaveFile);
            Logger.Log("New session. Local time: " + DateTime.Now.ToString("h:mm:ss"), "System", '!', ConsoleColor.Magenta);
            Core.InitializeGameStart(new Sim.Map.Size(80, 80));

            MainWindow = new MainWindow(Map);
            MainWindow.Show();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Logger.Log("Exiting application...", "System", '!', ConsoleColor.Magenta);
            Map.Physics.Save(Path.Combine(Directory.GetCurrentDirectory(), "data/", "lastconf.txt"));
        }
    }
}
