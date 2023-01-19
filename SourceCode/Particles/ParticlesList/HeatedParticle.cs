using System;
using System.Collections.Generic;
using System.Windows.Media;
using Sim.Map;
using Sim.Enums;
using Sim.Simulation;
using System.Text;

namespace Sim.Particles.ParticlesList
{
    public class HeatedParticle : ParticleBase
    {

        public new const int Id = 2;
        public new const string Name = "Heated_01";
        public new readonly Size Size = Size.DefaultSize;

        public new const double Mass = 20d;
        public new const AggregationStates CurrentState = AggregationStates.Solid;
        public new const double Temperature = 2000d;
        public new const double EmittingCoeff = 1d;
        public new const double AcceptanceCoeff = 0.0d;
        public new const double HeatCapacity = 1500d;
        public new const double MeltingPoint = 5000d;
        public new const double MeltingHeat = 15000d;
        public new const double EvaporationPoint = 7500d;
        public new const double EvaporationHeat = 35000d;

        public new const bool RequireRandomTick = false;
        public new const int RandomTickRarity = 10;

        public HeatedParticle(MapBase map, Vector2 position, Flags parameters) : base(map, Id, Name, position, Colors.YellowGreen, parameters, Size.DefaultSize, Mass, CurrentState, Temperature, EmittingCoeff, AcceptanceCoeff, HeatCapacity, MeltingPoint, MeltingHeat, EvaporationPoint, EvaporationHeat, RequireRandomTick)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            base.RandomTickRarity = RandomTickRarity;
        }

        public override void RandomTick()
        {
        }

    }
}
