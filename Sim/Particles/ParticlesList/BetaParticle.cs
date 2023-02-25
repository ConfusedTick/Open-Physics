﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Sim.Particles;
using Sim.Map;
using Sim.Simulation;
using Sim.Enums;

namespace Sim.Particles.ParticlesList
{
    
    public class BetaParticle : ParticleBase
    {

        public new const int Id = (int)ParticleIds.BETA;
        public new const string Name = "Beta particle";
        public new readonly Size Size = Size.DefaultSize;

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

        public new const bool RequireRandomTick = true;
        public new const int RandomTickRarity = 1000;

        public BetaParticle(MapBase map, Vector2 position, Flags parameters) : base(map, Id, Name, position, Colors.Brown, parameters, Size.DefaultSize, Mass, CurrentState, Temperature, EmittingCoeff, AcceptanceCoeff, HeatCapacity, MeltingPoint, MeltingHeat, EvaporationPoint, EvaporationHeat, RequireRandomTick)
        {
        }

        public override void Initialize()
        {
            base.Position.ApplyGravity = false;
            base.Initialize();
            base.RandomTickRarity = RandomTickRarity;
            base.Position.RecalculateWeight();
            base.Position.TemporaryForces.Add(new Force(Core.Random.Next(0, 360), Core.Random.Next(3, 6), base.Position));
        }

        public override void RandomTick()
        {
            base.Remove();
        }

        public override void CollideWith(ParticleBase particle)
        {
            if (particle.Mass > 4) base.Remove();
            return;
        }

        public override void SetTemperature(double newtemp)
        {
            return;
        }

        public override void ChangeAggregationState(AggregationStates newState)
        {
            return;
        }

    }

}