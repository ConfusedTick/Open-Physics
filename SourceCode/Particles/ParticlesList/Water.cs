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

    public class Water : ParticleBase
    {

        public new const int Id = 3;
        public new const string Name = "Water";
        public new readonly Size Size = Size.DefaultSize;
        
        public new const double Mass = 18d;
        public new const AggregationStates CurrentState = AggregationStates.Solid;
        public new const double Temperature = 20d;
        public new const double EmittingCoeff = 0.2d;
        public new const double AcceptanceCoeff = 0.15d;
        public new const double HeatCapacity = 4200d;
        public new const double MeltingPoint = 0d;
        public new const double MeltingHeat = 83600d;
        public new const double EvaporationPoint = 100d;
        public new const double EvaporationHeat = 40650d;

        public new const bool RequireRandomTick = true;
        public new const int RandomTickRarity = 100;

        public Water(MapBase map, Vector2 position, Flags parameters) : base(map, Id, Name, position, Colors.Blue, parameters, Size.DefaultSize, Mass, CurrentState, Temperature, EmittingCoeff, AcceptanceCoeff, HeatCapacity, MeltingPoint, MeltingHeat, EvaporationPoint, EvaporationHeat, RequireRandomTick)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            base.RandomTickRarity = RandomTickRarity;
        }

        public override void ChangeAggregationState(AggregationStates newState)
        {
            base.ChangeAggregationState(newState);
            if (newState == AggregationStates.Gas)
            {
                base.Name = "Steam";
                base.Color = Colors.Aquamarine;
            }
        }

        
        public override void RandomTick()
        {

        }

    }
}
