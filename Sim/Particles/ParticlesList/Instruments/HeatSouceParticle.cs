using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Sim.Particles;
using Sim.Map;
using Sim.Simulation;
using Sim.Enums;

namespace Sim.Particles.ParticlesList.Instruments
{
    public class HeatSouceParticle : ParticleBase, IInstrument
    {

        public new const int Id = (int)ParticleIds.HEATSOURCE;
        public new const string Name = "Heat source";
        public new readonly Size Size = Size.GetDefaultSize();

        public new const double Mass = 1d;
        public new const AggregationStates CurrentState = AggregationStates.Solid;
        public new const double Temperature = 0d;
        public new const double EmittingCoeff = 0d;
        public new const double AcceptanceCoeff = 0d;
        public new const double HeatCapacity = 1d;
        public new const double MeltingPoint = 0d;
        public new const double MeltingHeat = 0d;
        public new const double EvaporationPoint = 0d;
        public new const double EvaporationHeat = 0d;

        public new const bool RequireRandomTick = false;

        public ParticleBase AffectedParticle;
        public double Affection;

        double IInstrument.Affection => Affection;

        public HeatSouceParticle(MapBase map, Vector2 position, Flags parameters, double heatFlux, ParticleBase affp = null) : base(map, Id, Name, position, Colors.White, parameters, Size.GetDefaultSize(), Mass, CurrentState, Temperature, EmittingCoeff, AcceptanceCoeff, HeatCapacity, MeltingPoint, MeltingHeat, EvaporationPoint, EvaporationHeat, RequireRandomTick)
        {
            AffectedParticle = affp;
            Affection = heatFlux;
        }

        public override void Initialize()
        {
            base.Position.ApplyGravity = false;
            base.Initialize();
        }

        public override void SetTemperature(double newtemp)
        {
            return;
        }

        public override void ChangeAggregationState(AggregationStates newState)
        {
            return;
        }

        public override bool IsInstrument()
        {
            return true;
        }

        public void AffectionTick()
        {
            if (AffectedParticle != null) AffectedParticle.ChangeTemperatureByHeatFlux(Affection);
        }
    }
}
