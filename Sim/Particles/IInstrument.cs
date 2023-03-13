using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Particles
{
    
    // Interface all Instrument particles should use
    // They does not collide or affect heat rays
    public interface IInstrument
    {

        public double Affection { get; }

        // If instrument always true
        public bool IsInstrument()
        {
            return true;
        }

        // Tick that does all affection of a particle
        void AffectionTick()
        {
        }

    }
}
