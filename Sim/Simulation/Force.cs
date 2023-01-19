using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Simulation
{
    public class Force
    {

        public int Angle;
        public double NetForce;

        public double Acceleration;
        public double Speed { get; protected set; }

        public Vector2 Vector;

        public Force(int angle, double force, Vector2 vector)
        {
            NetForce = force;
            Angle = angle;
            Acceleration = 0d;
            Speed = 0d;
            Vector = vector;
        }

        public void Tick()
        {
            Acceleration = NetForce / Vector.Particle.Mass;
            Speed += Acceleration * Vector.Particle.Map.Physics.DeltaTime;
        }

        public void Reset()
        {
            Acceleration = 0d;
            Speed = 0d;
        }

        public void SwitchToContinious()
        {
            NetForce = 0d;
        }

    }
}
