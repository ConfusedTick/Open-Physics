using System;
using System.Collections.Generic;
using System.Text;
using Sim.Particles;

namespace Sim.Events
{
    class ParticleAddedEventArgs : EventArgs 
    {

        public readonly ParticleBase Particle;

        public ParticleAddedEventArgs(ParticleBase particle) : base()
        {
            Particle = particle;
        }

    }
}
