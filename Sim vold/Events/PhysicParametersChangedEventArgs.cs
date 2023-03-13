using System;
using System.Collections.Generic;
using System.Text;
using Sim.Simulation;

namespace Sim.Events
{
    class PhysicParametersChangedEventArgs : EventArgs
    {

        public readonly Physic Physic;
        
        public PhysicParametersChangedEventArgs(Physic physic)
        {
            Physic = physic;
        }

    }
}
