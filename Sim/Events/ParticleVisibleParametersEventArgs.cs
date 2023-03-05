using System;
using System.Collections.Generic;
using System.Text;
using Sim.Particles;

namespace Sim.Events
{
    public class ParticleVisibleParametersEventArgs : EventArgs
    {
        public readonly ParticleBase Particle;

        public ParticleVisibleParametersEventArgs(ParticleBase particle)
        {
            Particle = particle;
        }
    }
}
