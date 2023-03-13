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
        
        BETA = -2,
        BETA_PARTICLE = -2,
        
        ALPHA = -1,
        ALPHA_PARTICLE = -1,

        ICE = 1,
        WATER = 1,
        STEAM = 1,

        HYDROGEN = 8,

        DEUTERIUM = 102,
        TRITIUM = 103,
        
        SODIUM = 2,
        NATRIUM = 2,
        POTASSIUM = 3,

        CARBON = 4,
        DIAMOND = 4,
        GRAPHITE = 4,

        COPPER = 5,
        CUPRUM = 5,
        
        FERRUM = 6,
        IRON = 6,

        ALLIMINIUM = 7,


        // Instruments (IInstruments)
        HEATSOURCE = -255,
        HEAT_SOURCE = -255,
    }
}
