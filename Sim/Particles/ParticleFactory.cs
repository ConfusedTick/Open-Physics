using System;
using System.Collections.Generic;
using System.Linq;
using Sim.Map;
using Sim.Simulation;
using Sim.Particles.ParticlesList;
using Sim.Particles.ParticlesList.Instruments;

namespace Sim.Particles
{
    public class ParticleFactory
    {
        /// <summary>
        /// Карта, для которой создана фабрика
        /// </summary>
        public Map.MapBase Map { get; protected set; }

        public static Dictionary<int, Type> Particles = new Dictionary<int, Type>() { };
        public static Dictionary<MapBase, ParticleFactory> Factories = new Dictionary<MapBase, ParticleFactory>() { };
        public static bool Initialized = false;

        /// <summary>
        /// Возвращает фабрику, отвечающую за соотвественную карту
        /// </summary>
        /// <param name="map">Карта</param>
        /// <returns>Фабрика</returns>
        public static ParticleFactory GetFactory(MapBase map)
        {
            if (!Factories.ContainsKey(map)) return new ParticleFactory(map);
            return Factories[map];
        }

        /// <summary>
        /// Инициализирует коллекцию всех частиц по умолчанию
        /// </summary>
        public static void Initialize()
        {
            if (Initialized)
            {
                Logger.Exception(new InvalidOperationException("Particle factory already initialized."));
                return;
            }
            RegisterParticle(typeof(BetaParticle));
            RegisterParticle(typeof(AlphaParticle));
            RegisterParticle(typeof(Hydrogen));
            RegisterParticle(typeof(Water));
            RegisterParticle(typeof(HeatSouceParticle));
            Initialized = true;
        }

        /// <summary>
        /// Добавляет новую частицу в базу всех частиц
        /// </summary>
        /// <param name="id">Номер частицы</param>
        /// <param name="type">Класс частицы (должен наследовать ParticleBase)</param>
        public static void RegisterParticle(int id, Type type)
        {
            if (type.IsAssignableFrom(typeof(ParticleBase)))
            {
                Logger.Exception(new ArgumentException("Particle is not type of ParticleBase."));
                return;
            }
            if (Particles.ContainsKey(id))
            {
                Logger.Exception(new ArgumentException("Particle with id " + id.ToString() + " is already registered."));
                return;
            }
            Logger.Log("Registered particle with id " + id.ToString(), "ParticleFactoryRegisterParticle");
            Particles.Add(id, type);
        }

        public static void RegisterParticle(Type type)
        {
            RegisterParticle((int)type.GetField("Id").GetValue(type), type);
        }

        /// <summary>
        /// Удаляет частицу из списка зарегестрированных
        /// </summary>
        /// <param name="id">Айди частицы</param>
        public static void UnRegisterParticle(int id)
        {
            Logger.Exception(new InvalidOperationException("Trying to unregister block, only possible in dev build."));
            Particles.Remove(id);
        }

        /// <summary>
        /// Создаёт новую фабрику для карты
        /// </summary>
        /// <param name="map">Карта</param>
        private ParticleFactory(MapBase map)
        {
            Map = map;
            Factories.Add(map, this);
            if (!Initialized) Initialize();
        }

        /// <summary>
        /// Создаёт и добавляет новую частицу на карту
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="position">Позиция</param>
        /// <param name="flags">Дефолтные флаги</param>
        /// <returns>Добавленная на карту частица</returns>
        public ParticleBase AddNewParticle(int id, Vector2 position, Flags flags)
        {
            ParticleBase particle = CreateParticle(id, position, flags);
            Map.AddParticle(particle);
            return particle;
        }

        public ParticleBase AddNewParticle(ParticleIds id, Vector2 position, Flags flags)
        {
            return AddNewParticle((int)id, position, flags);
        }

        /// <summary>
        /// Создаёт и добавляет новую частицу на карту
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="x">Позиция x</param>
        /// <param name="y">Позиция y</param>
        /// <param name="flags">Дефолтные флаги</param>
        /// <returns>Добавленная на карту частица</returns>
        public ParticleBase AddNewParticle(int id, double x, double y, Flags flags)
        {
            if (!Particles.ContainsKey(id))
            {
                Logger.Exception(new InvalidOperationException("Particle with id " + id + " is not registered in the ParticleFactory"));
                return null;
            }
            ParticleBase particle = CreateParticle(id, x, y, flags);
            Map.AddParticle(particle);
            return particle;
        }

        public ParticleBase AddNewInstrument(int id, ParticleBase target, Flags flags, double affect)
        {
            if (!Particles.ContainsKey(id))
            {
                Logger.Exception(new InvalidOperationException("Instrument with id " + id + " is not registered in the ParticleFactory"));
                return null;
            }
            
            ParticleBase inst = CreateInstrument(id, target, flags, affect);
            Map.AddInstrument(inst);
            return inst;
        }

        public ParticleBase AddNewParticle(ParticleIds id, double x, double y, Flags flags)
        {
            return AddNewParticle((int)id, x, y, flags);
        }

        /// <summary>
        /// Создаёт новую частицу
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="position">Позиция</param>
        /// <param name="flags">Дефолтные флаги</param>
        /// <returns>Созданная частица</returns>
        public ParticleBase CreateParticle(int id, Vector2 position, Flags flags)
        {
            if (!Particles.ContainsKey(id))
            {
                Logger.Exception(new InvalidOperationException("Particle with id " + id + " is not registered in the ParticleFactory"));
                return null;
            }
            ParticleBase particle = (ParticleBase)Activator.CreateInstance(Particles[id], new object[] { Map, position, Flags.Empty });
            if (particle != null) particle.InitPosition();
            return particle;
        }

        
        public ParticleBase CreateInstrument(int id, ParticleBase target, Flags flags, double affect)
        {
            if (!Particles.ContainsKey(id))
            {
                Logger.Exception(new InvalidOperationException("Particle with id " + id + " is not registered in the ParticleFactory"));
                return null;
            }
            
            ParticleBase instrument = (ParticleBase)Activator.CreateInstance(Particles[id], new object[] { Map, target.Position, flags, affect, target});
            return instrument;
        }
        

        public ParticleBase CreateInstrument(ParticleIds id, ParticleBase target, Flags flags, double affect)
        {
            return CreateInstrument((int)id, target, flags, affect);
        }

        public ParticleBase CreateParticle(ParticleIds id, Vector2 position, Flags flags)
        {
            return CreateParticle(id, position, flags);
        }

        /// <summary>
        /// Создаёт новую частицу
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="x">Позиция x</param>
        /// <param name="y">Позиция y</param>
        /// <param name="flags">Дефолтный флаги</param>
        /// <returns>Созданная частица</returns>
        public ParticleBase CreateParticle(int id, double x, double y, Flags flags)
        {
            Vector2 prepared = new Vector2(x, y);
            ParticleBase particle = (ParticleBase)Activator.CreateInstance(Particles[id], new object[] { Map, prepared, Flags.Empty });
            if (particle != null) particle.InitPosition();
            return particle;
        }
        public ParticleBase CreateParticle(ParticleIds id, double x, double y, Flags flags)
        {
            return CreateParticle((int)id, x, y, flags);
        }
    }
}
