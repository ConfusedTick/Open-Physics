using System;
using System.Collections.Generic;
using System.Text;
using Sim.Particles.ParticlesList;

namespace Sim.Particles
{
    
    // Here you can find all particle ids for all default simulator blocks
    // Not every id currently has the particle associated with it
    public enum ParticleIds : int
    {
        BETA, BETA_PARTICLE = -2,
        ALPHA, ALPHA_PARTICLE = -1,

        WATER, STEAM, ICE = 1,
        DEUTERIUM = 102,
        TRITIUM = 103,
        
        SODIUM, NATRIUM = 2,
        POTASSIUM = 3,

        CARBON, DIAMOND, GRAPHITE = 4,


    }
}
