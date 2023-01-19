using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Particles
{

    // Interface all unstable particles should use
    // Unstable particles, decays
    // All unstable particles should require random tick (RequireRandomTick = true;)
    // And have RandomTickRarity, RandomTick()
    public interface IUnstable
    {

        public static bool RequireRandomTicks { get; }
        public static int RandomTicksRarity { get; }

        // ParticleBase RandomTick() does not have any realization
        public void RandomTick();

    }
}
