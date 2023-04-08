using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Particles
{
    public class ParticleException : Exception
    {

        public ParticleBase Particle;

        public string Message;

        public ParticleException(ParticleBase particle, string message) : base(message)
        {
            Particle = particle;
            Message = "Particle " + particle.Uid.ToString() + " at " + particle.Position.ToString() + ": " + message;
        }

    }
}
