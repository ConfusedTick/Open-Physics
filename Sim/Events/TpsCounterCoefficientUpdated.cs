using System;
using System.Collections.Generic;
using System.Text;
using Sim.Simulation;

namespace Sim.Events
{
    public class TpsCounterCoefficientUpdated : EventArgs
    {

        public readonly double TpsCoefficient;
        
        public TpsCounterCoefficientUpdated(double coeff)
        {
            TpsCoefficient = coeff;
        }

    }
}
