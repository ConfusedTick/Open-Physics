using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Sim;
using Sim.Map;
using Sim.Simulation;
using Sim.Enums;

namespace Sim.Particles.ParticlesList
{

    public class Water : ParticleBase, ILiquid
    {

        public new const int Id = (int)ParticleIds.WATER;
        public new const string Name = "Water";
        public new readonly Size Size = Size.GetDefaultSize();
        
        public new const double Mass = 18d;
        public new const AggregationStates CurrentState = AggregationStates.Liquid;
        public new const double Temperature = 20d;
        public new const double EmittingCoeff = 1d;
        public new const double AcceptanceCoeff = 0.5d;
        public new const double Transparency = 0.0d;
        public new const double HeatCapacity = 4200d;
        public new const double MeltingPoint = 0d;
        public new const double MeltingHeat = 83600d;
        public new const double EvaporationPoint = 100d;
        public new const double EvaporationHeat = 40650d;

        public new const bool RequireRandomTick = false;

        public readonly Dictionary<AggregationStates, Color> StateColors = new Dictionary<AggregationStates, Color>() 
        {
            { AggregationStates.Solid, Colors.Blue },
            { AggregationStates.Liquid, Colors.MediumAquamarine },
            { AggregationStates.Gas, Colors.Aquamarine },
        };

        public readonly Dictionary<AggregationStates, string> StateNames = new Dictionary<AggregationStates, string>()
        {
            { AggregationStates.Solid, "Ice" },
            { AggregationStates.Liquid, "Water"},
            { AggregationStates.Gas, "Steam" },
        };

        public Water(MapBase map, ParticlePositionParameters position, Flags parameters) : base(map, Id, Name, position, Colors.Blue, parameters, Size.GetDefaultSize(), Mass, CurrentState, Temperature, EmittingCoeff, AcceptanceCoeff,  Transparency, HeatCapacity, MeltingPoint, MeltingHeat, EvaporationPoint, EvaporationHeat, RequireRandomTick)
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
