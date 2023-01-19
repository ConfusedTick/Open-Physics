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


        public ParticleFactory(MapBase map)
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
            ParticleBase particle = null;
            switch (id)
            {
                case 0:
                    break;

                case 1:
                    particle = new Polonium210(Map, position, flags);
                    break;

                case 2:
                    particle = new HeatedParticle(Map, position, flags);
                    break;

                case 3:
                    particle = new Water(Map, position, flags);
                    break;

                default:
                    break;
            }
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
            ParticleBase particle = null;
            Vector2 prepared = new Vector2(x, y);
            switch (id)
            {
                case 0:
                    break;

                case 1:
                    particle = new Polonium210(Map, prepared, flags);
                    break;

                case 2:
                    particle = new HeatedParticle(Map, prepared, flags);
                    break;

                case 3:
                    particle = new Water(Map, prepared, flags);
                    break;

                default:
                    break;
            }
            if (particle != null) particle.InitPosition();
            return particle;
        }
    }
}
