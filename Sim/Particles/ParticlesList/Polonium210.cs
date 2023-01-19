using System;
using System.Collections.Generic;
using System.Windows.Media;
using Sim.Map;
using Sim.Enums;
using Sim.Simulation;
using System.Text;

namespace Sim.Particles.ParticlesList
{
    public class Polonium210 : ParticleBase, IUnstable
    {

        public new const int Id = 1;
        public new const string Name = "Po210";
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

        public static bool RequireRandomTicks { get; } = true;
        public static int RandomTicksRarity { get; } = 100;

        public Polonium210(Map.MapBase map, Vector2 position, Flags parameters) : base(map, Id, Name, position, Colors.Blue, parameters + Flags.Dev, Size.DefaultSize, Mass, CurrentState, Temperature, EmittingCoeff, AcceptanceCoeff, HeatCapacity, MeltingPoint, MeltingHeat, EvaporationPoint, EvaporationHeat, RequireRandomTicks)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            base.RandomTickRarity = RandomTicksRarity;
        }

        public override void RandomTick()
        {
            
        }
    }
}
