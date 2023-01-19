using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Events
{
    public class ParticleTemperatureChangedEventArgs : EventArgs
    {

        public readonly double NewTemperature;

        public ParticleTemperatureChangedEventArgs(double newTemperature) : base()
        {
            NewTemperature = newTemperature;
        }

    }
}
