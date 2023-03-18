using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Sim;
using Sim.Map;
using Sim.Simulation;
using Sim.Enums;

namespace Sim.Particles
{

    public class Hydrogen : ParticleBase
    {

        public new const int Id = (int)ParticleIds.HYDROGEN;
        public new const string Name = "Hydrogen";
        public new readonly Size Size = Size.GetDefaultSize();
        
        public new const double Mass = 1d;
        public new const AggregationStates CurrentState = AggregationStates.Gas;
        public new const double Temperature = 20d;
        public new const double EmittingCoeff = 0.001d;
        public new const double AcceptanceCoeff = 0.001d;
        public new const double HeatCapacity = 15d;
        public new const double MeltingPoint = -259d;
        public new const double MeltingHeat = 1d;
        public new const double EvaporationPoint = -252d;
        public new const double EvaporationHeat = 1d;

        public new const bool RequireRandomTick = false;

        public readonly Dictionary<AggregationStates, Color> StateColors = new Dictionary<AggregationStates, Color>() 
        {
            { AggregationStates.Solid, Colors.GhostWhite },
            { AggregationStates.Liquid, Colors.GhostWhite },
            { AggregationStates.Gas, Colors.GhostWhite },
        };

        public readonly Dictionary<AggregationStates, string> StateNames = new Dictionary<AggregationStates, string>()
        {
            { AggregationStates.Solid, "Solid hydrogen" },
            { AggregationStates.Liquid, "Liquid hydrogen"},
            { AggregationStates.Gas, "Hydrogen" },
        };

        public Hydrogen(MapBase map, ParticlePositionParameters position, Flags parameters) : base(map, Id, Name, position, Colors.Blue, parameters, Size.GetDefaultSize(), Mass, CurrentState, Temperature, EmittingCoeff, AcceptanceCoeff, HeatCapacity, MeltingPoint, MeltingHeat, EvaporationPoint, EvaporationHeat, RequireRandomTick)
        {
        }

        public override void ChangeAggregationState(AggregationStates newState)
        {
            base.ChangeAggregationState(newState);
            base.Name = StateNames[newState];
            base.Color = StateColors[newState];
        }
    }
}
