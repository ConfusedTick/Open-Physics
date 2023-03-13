using System;
using System.Collections.Generic;
using System.Text;
using Sim.Enums;

namespace Sim.Events
{
    public class ParticleAggregationStateChangedEventArgs : EventArgs
    {

        public readonly AggregationStates NewState;

        public ParticleAggregationStateChangedEventArgs(AggregationStates newState) : base()
        {
            NewState = newState;
        }

    }
}
