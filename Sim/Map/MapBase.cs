using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Threading;
using Sim.Particles;
using Sim.Particles.ParticlesList.Instruments;
using Sim.Simulation;
using Sim.Enums;
using Sim.Events;
using Sim.Utils;
using System.IO;

namespace Sim.Map
{
    public partial class MapBase
    {

        /// <summary>
        /// Все частицы карты
        /// </summary>
        public readonly List<ParticleBase> Particles = new List<ParticleBase> { };

        /// <summary>
        /// Все инструменты частицы
        /// </summary>
        public readonly List<ParticleBase> Instruments = new List<ParticleBase> { };

        /// <summary>
        /// Словарь всех частиц карты (ключ - Uid)
        /// </summary>
        public readonly Dictionary<int, ParticleBase> ParticlesDictionary = new Dictionary<int, ParticleBase>();

        /// <summary>
        /// Размер карты
        /// </summary>
        public readonly Size Size;

        /// <summary>
        /// Физические параметры
        /// </summary>
        public readonly Physic Physics;

        /// <summary>
        /// Таймер обновления
        /// </summary>
        private readonly DispatcherTimer Ticker = new DispatcherTimer();

        /// <summary>
        /// Запущен ли тикер обновлений для этой карты
        /// </summary>
        public bool IsTicking = false;

        public ulong TickId { get; private set; } = 0;

        /// <summary>
        /// Класс рандома используемый для этой карты
        /// </summary>
        public Random Random;

        public EventHandler ParticleAdded;

        public TpsCounter TpsCounter;

        public double Performance = 1d;


        public MapBase(Size size, Physic physic)
        {
            Size = size;
            Physics = physic;
            physic.PhysicParametersChanged += PhysicParametersChanged;
            Random = Utils.Utils.GetNewRandom();
            // fffffff - time in MICROseconds to be more random
            Ticker.Tick += Tick;
            Ticker.Interval = TimeSpan.FromSeconds(physic.DeltaTime);

            TpsCounter = new TpsCounter(Ticker); 
            TpsCounter.CoefficientUpdated += TpsUpdated; 

            if (!ParticleFactory.Initialized) ParticleFactory.Initialize();
        }


        public void TpsUpdated(object sender, EventArgs args)
        {
            Performance = ((TpsCounterCoefficientUpdated)args).TpsCoefficient;
        }

        /// <summary>
        /// Запускает таймер обновления частиц
        /// </summary>
        public void StartTicking()
        {
            Ticker.Start();
            IsTicking = true;
            Logger.Log("Ticking started", "Map");
        }

        /// <summary>
        /// Останавливает таймер обновления частиц
        /// </summary>
        public void StopTicking()
        {
            Ticker.Stop();
            IsTicking = false;
            Logger.Log("Ticking stopped", "Map");
        }

        // Ивент
        private void PhysicParametersChanged(object sender, EventArgs e)
        {
            RecalculateAllWeight();
        }

        /// <summary>
        /// Перевычислеяет вес всех частиц
        /// </summary>
        public void RecalculateAllWeight()
        {
            Particles.ForEach(part => part.Position.RecalculateWeight());
        }

        /// <summary>
        /// Очищает карту
        /// </summary>
        public void Clear()
        {
            Particles.Clear();
            ParticlesDictionary.Clear();
        }

        /// <summary>
        /// Добавляет частицу
        /// </summary>
        /// <param name="particle">Частица</param>
        public void AddParticle(ParticleBase particle)
        {
            if (!ParticleFactory.Particles.ContainsKey(particle.Id))
            {
                Logger.Log("Particle with " + particle.Id + " is not registered in particle factory.");
                return;
            }
            if (!Particles.Contains(particle) && IsAllowedPosition(particle.Position))
            {
                Particles.Add(particle);
                ParticlesDictionary.Add(particle.Uid, particle);
                particle.Initialize();
                ParticleAdded?.Invoke(this, new ParticleAddedEventArgs(particle));
            }
        }

        public void AddInstrument(ParticleBase instrument)
        {
            if (!ParticleFactory.Particles.ContainsKey(instrument.Id))
            {
                Logger.Log(" with " + instrument.Id + " is not registered in particle factory.");
                return;
            }
            if (!Instruments.Contains(instrument) && IsAllowedPosition(instrument.Position))
            {
                Instruments.Add(instrument);
                instrument.Initialize();
            }
        }

        /// <summary>
        /// Удаляет частицу 
        /// </summary>
        /// <param name="remPart">Удаляемая частица</param>
        public void RemoveParticle(ParticleBase remPart)
        {
            Particles.RemoveAll(part => part == remPart);
            ParticlesDictionary.Remove(remPart.Uid);
        }
        
        /// <summary>
        /// Получить частицу карты на определенной позиции
        /// </summary>
        /// <param name="x">Позиция x</param>
        /// <param name="y">Позиция y</param>
        /// <returns>Частица на этой позиции</returns>
        public ParticleBase GetParticle(double x, double y)
        {
            return Particles.Find(bl => bl.Position.X == x && bl.Position.Y == y);
        }

        /// <summary>
        /// Получить частицу карты на определенной позиции
        /// </summary>
        /// <param name="x">Позиция x</param>
        /// <param name="y">Позиция y</param>
        /// <returns>Частица на этой позиции</returns>
        public ParticleBase GetParticle(int x, int y)
        {
            return Particles.Find(bl => bl.Position.X == x && bl.Position.Y == y);
        }

        /// <summary>
        /// Проверяет, есть ли частица на указаном месте карты
        /// </summary>
        /// <param name="x">Позиция x</param>
        /// <param name="y">Позиция y</param>
        /// <returns>Значение</returns>
        public bool ExistParticle(int x, int y)
        {
            return GetParticle(x, y) != null;
        }

        /// <summary>
        /// Проверяет, есть ли частица на указаном месте карты
        /// </summary>
        /// <param name="x">Позиция x</param>
        /// <param name="y">Позиция y</param>
        /// <returns>Значение</returns>
        public bool ExistParticle(double x, double y)
        {
            return GetParticle(x, y) != null;
        }

        public ParticleBase IsInParticleArea(double x, double y)
        {
            return Particles.Find(part => part.Position.X < x && part.Position.X + part.Size.Width > x &&
                part.Position.Y < y && part.Position.Y + part.Size.Height > y);
        }

        public static ParticleBase IsInParticleArea(double x, double y, List<ParticleBase> list)
        {
            return list.Find(part => part.Position.X < x && part.Position.X + part.Size.Width > x &&
                part.Position.Y < y && part.Position.Y + part.Size.Height > y);
        }

        public List<ParticleBase> IsInParticleAreaAll(double x, double y)
        {
            return Particles.FindAll(part => part.Position.X < x && (double)(part.Position.X + part.Size.Width) > x &&
                part.Position.Y < y && (double)(part.Position.Y + part.Size.Height) > y);
        }

        public static List<ParticleBase> IsInParticleAreaAll(double x, double y, List<ParticleBase> list)
        {
            return list.FindAll(part => part.Position.X < x && (double)(part.Position.X + part.Size.Width) > x &&
                part.Position.Y < y && (double)(part.Position.Y + part.Size.Height) > y);
        }

        public bool IsAllowedPosition(double x, double y)
        {
            return x <= Size.Width && x >= 0d && y <= Size.Height && y >= 0d;
        }

        public bool IsAllowedPosition(ParticlePositionParameters vec)
        {
            return vec.X <= Size.Width && vec.X >= 0d && vec.Y <= Size.Height && vec.Y >= 0d;
        }

        public bool ExistParticle(ParticleBase part)
        {
            return Particles.Contains(part);
        }

        public bool IsTouchingEachOthers(ParticleBase part1, ParticleBase part2)
        {
            ParticlePositionParameters pos1 = part1.Position;
            ParticlePositionParameters pos2 = part2.Position;
            return ((double)pos2.X >= (double)pos1.X) && ((double)pos2.X <= (double)((double)pos1.X + part1.Size.Width)) && ((double)pos2.Y >= (double)pos1.Y) && ((double)pos2.Y <= (double)((double)pos1.Y + part1.Size.Height)); 

        }
        public void Tick(object sender, EventArgs e)
        {
            ParticleBase particle;
            
            for (int i = 0; i < Particles.Count; i++)
            {
                particle = Particles[i];
                particle.Tick();
                if (particle.RequireRandomTick) if (Random.Next(0, particle.RandomTickRarity) == 1) particle.RandomTick();
            }

            ParticleBase instrument;

            for (int i = 0; i < Instruments.Count; i++)
            {
                instrument = Instruments[i];
                ((IInstrument)instrument).AffectionTick();
                if (instrument.RequireRandomTick) if (Random.Next(0, instrument.RandomTickRarity) == 1) instrument.RandomTick();
            }

            TickId++;
        }

        /**
         * 
         * [Int32]      - 4 bytes
         * [Double]     - 8 bytes 
         * 
         * Формат:
         * 
         * [Int32 MapWidth][Int32 MapHeight]
         * [Vector2 data] (-[Double X][Double Y][Int32 Angle][Double Accel]-)
         * [Particle data] (-[Int32 Id][Bool Fixed][Double Temperature][Double HeatBuffer][Double Mass][Byte AggregState]-)
         * 
         **/


        /// <summary>
        /// Сохраняет карту в файл
        /// </summary>
        /// <param name="savefile">Файл</param>
        public void Save(string savefile)
        {
            using BinaryWriter writer = new BinaryWriter(new FileStream(savefile, FileMode.OpenOrCreate));
            List<ParticleBase> all = Particles.ToList();
            all.AddRange(Instruments);
            // Open physics map format
            writer.Write("OPMF");

            writer.Write((int)Size.Width);
            writer.Write((int)Size.Height);
            ParticlePositionParameters pos;

            foreach (ParticleBase particle in all)
            {
                pos = particle.Position;
                // Vector2 data
                writer.Write(pos.X);
                writer.Write(pos.Y);
                writer.Write(pos.Angle);
                writer.Write(pos.Acceleration);

                // Particle data
                if (particle.IsInstrument())
                {
                    writer.Write((byte)(1));
                    writer.Write((double)(((IInstrument)particle).Affection));
                }
                else
                {
                    writer.Write((byte)(0));
                }
                writer.Write(particle.Id);
                writer.Write(particle.Fixed);
                writer.Write(particle.Temperature);
                writer.Write(particle.HeatBuffer);
                writer.Write(particle.Mass);
                writer.Write((byte)particle.CurrentState);
            }
        }

        public static MapBase Load(string file, Physic physics)
        {
            MapBase map;
            Size mapSize;
            List<ParticleBase> add = new List<ParticleBase>();
            ParticleFactory factory;
            ParticleBase particle;
            ParticlePositionParameters pos;
            byte isInstrument;
            int id;
            double affect;
            using (BinaryReader reader = new BinaryReader(new FileStream(file, FileMode.OpenOrCreate)))
            {
                if (reader.ReadString() != "OPMF")
                {
                    Logger.Exception(new FormatException("Invalid map file format."));
                    Logger.Log("Exception while loading map from " + file, "Map", '!', ConsoleColor.Red);
                    Logger.Log("This can lead to major malfunctions. Application will be closed.", textColor: ConsoleColor.Red);
                    Environment.Exit(100);
                    return null;
                }

                mapSize = new Size(reader.ReadInt32(), reader.ReadInt32());
                map = new MapBase(mapSize, physics);
                factory = ParticleFactory.GetFactory(map);
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    pos = new ParticlePositionParameters(x: reader.ReadDouble(), y: reader.ReadDouble(), angle: reader.ReadInt32(), acceleration: reader.ReadDouble());
                    isInstrument = reader.ReadByte();
                    
                    
                    if (isInstrument == 0)
                    {
                        id = reader.ReadInt32();
                        particle = factory.CreateParticle(id: id, pos, flags: Flags.Empty);
                    }
                    else
                    {
                        affect = reader.ReadDouble();
                        id = reader.ReadInt32();
                        particle = factory.CreateInstrument(id: id, add.Find(p => p.Position.X == pos.X && p.Position.Y == pos.Y), Flags.Empty, affect: affect);
                    }
                    if (particle == null)
                    {
                        Logger.Exception(new FormatException("Can not create particle, while loading new map, skipping particle initialization"));
                        reader.BaseStream.Seek(26, SeekOrigin.Current);
                        continue;
                    }
                    particle.SetFixed(reader.ReadBoolean());
                    particle.SetTemperature(reader.ReadDouble());
                    particle.SetHeatBuffer(reader.ReadDouble());
                    particle.SetMass(reader.ReadDouble());
                    particle.ChangeAggregationState((AggregationStates)reader.ReadByte());
                    add.Add(particle);
                }
            }
            foreach(ParticleBase pt in add)
            {
                if (pt.IsInstrument()) map.AddInstrument(pt);
                else map.AddParticle(pt);
            }
            return map;
        }
    }
}
