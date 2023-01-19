using System;
using System.Collections.Generic;
using System.Text;
using Sim.Particles;

namespace Sim.Events
{
    public class ParticleCollidedEventArgs : EventArgs
    {

        public readonly ParticleBase Particle1;
        public readonly ParticleBase Particle2;

        public ParticleCollidedEventArgs(ParticleBase particle1, ParticleBase particle2) : base()
        {
            Particle1 = particle1;
            Particle2 = particle2;
        }

    }
}
