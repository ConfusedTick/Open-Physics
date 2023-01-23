using System;
using System.Collections.Generic;
using System.Text;
using Sim.Map;
using Sim.Simulation;
using Sim.Particles.ParticlesList;

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
            if (!Factories.ContainsKey(map)) Factories.Add(map, new ParticleFactory(map));
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
            RegisterParticle((int)ParticleIds.ALPHA, typeof(AlphaParticle));
            RegisterParticle((int)ParticleIds.WATER, typeof(Water));
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
            Particles.Add(id, type);
        }

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
            ParticleBase particle = CreateParticle(id, x, y, flags);
            Map.AddParticle(particle);
            return particle;
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
            ParticleBase particle = (ParticleBase)Activator.CreateInstance(Particles[id], new object[] { Map, position, Flags.Empty});
            if (particle != null) particle.InitPosition();
            return particle;
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
    }
}
