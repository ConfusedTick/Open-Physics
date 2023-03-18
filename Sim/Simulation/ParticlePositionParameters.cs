using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Sim.GlmMath;
using Sim.Particles;
using Sim.Map;
using SFML.System;
using Sim.Utils;

namespace Sim.Simulation
{

    /// <summary>
    /// Не лезь, тут прописана корневая физика движения, взаимодействия
    /// Для частицы. 
    /// Советую просто использовать и не думать как это работает
    /// </summary>

    public class ParticlePositionParameters
    {

        public ParticleBase Particle { get; set; }

        public double PreviousX { get; private set; }
        private double _x;
        public double X
        {
            get => _x;
            private set
            {
                PreviousX = _x;
                _x = value;
            }
        }

        public double PreviousY { get; private set; }
        private double _y;
        public double Y
        {
            get => _y;
            private set
            {
                PreviousY = _y;
                _y = value;
            }
        }

        public int PreviousAngle { get; private set; }

        private int _angle;
        public int Angle
        {
            get => _angle;
            private set
            {
                PreviousAngle = _angle;
                _angle = value;
            }
        }

        public double Speed { get; private set; }

        public double PreviousAcceleration { get; private set; }
        public double Acceleration { get; private set; }

        public double NetForce { get; private set; }

        public double Weight { get; private set; }

        public bool ApplyGravity { get; set; }

        public List<Force> PermanentForces { get; set; }
        public List<Force> TemporaryForces;
        public List<Force> OneTickForces;

        public bool Fixed;

        private bool UpdateOnNextTick;
        private bool ForceUpdateOnNextTick;

        public Size Size;

        public ParticlePositionParameters(double x, double y)
        {
            X = x;
            Y = y;
            PermanentForces = new List<Force> { };
            TemporaryForces = new List<Force> { };
            OneTickForces = new List<Force> { };
            ApplyGravity = true;
        }

        public ParticlePositionParameters(double x, double y, int angle = 0, double mass = 0d, double speed = 0d, double acceleration = 0d, double force = 0d, double sizeX = 0d, double sizeY = 0d, bool isFixed = false, ParticleBase particle = null)
        {
            UpdateOnNextTick = false;
            ForceUpdateOnNextTick = false;
            Particle = particle;
            PreviousAngle = angle;
            Speed = speed;
            NetForce = force;
            Acceleration = acceleration;
            Weight = 0;
            Size = new Size(sizeX, sizeY);
            Fixed = isFixed;
            PermanentForces = new List<Force> { };
            TemporaryForces = new List<Force> { };
            OneTickForces = new List<Force> { };
            PreviousAngle = angle;
            PreviousX = x;
            PreviousY = y;
            PreviousAcceleration = acceleration;
            ApplyGravity = true;
            _x = x;
            _y = y;
            _angle = angle;
            X = x;
            Y = y;
            Angle = angle;
        }

        /// <summary>
        /// Должен вызываться каждый тик, просчитывает информацию о позиции
        /// </summary>
        /// <returns>Требуется лм визуальное обновление на этом тике</returns>
        public bool Tick()
        {
            NetForce = 0d;
            if (Fixed) return false;
            UpdateOnNextTick = false;

            foreach (Force pf in PermanentForces.ToList())
            {
                TickForce(pf);
                AddForceToDirection(pf.Angle, pf.NetForce);
            }

            foreach (Force tf in TemporaryForces.ToList())
            {
                TickForce(tf);
                AddForceToDirection(tf.Angle, tf.NetForce);
            }

            foreach (Force df in OneTickForces.ToList())
            {
                TickForce(df);
                AddForceToDirection(df.Angle, df.NetForce);
                OneTickForces.Remove(df);
            }

            SetAcceleration(NetForce / Particle.Mass);
            IncreaseSpeed(Acceleration * Particle.Map.Physics.DeltaTime);
            
            bool saved = ForceUpdateOnNextTick;
            ForceUpdateOnNextTick = false;
            return UpdateOnNextTick || saved;
        }

        public void SetForceUpdateOnNextTick(bool s)
        {
            ForceUpdateOnNextTick = s;
        }

        public void TickForce(Force force)
        {
            force.Tick();
            double nX = (double)(X + (double)(force.Speed * (double)Trigonometrics.DCos(force.Angle)));
            double nY = (double)(Y + (double)(force.Speed * (double)Trigonometrics.DSin(force.Angle)));
            SetX(nX);
            SetY(nY);
        }

        public void AddForceToDirection(int angle, double force)
        {
            int betweenAngle = (int)Trigonometrics.Correct((int)(180d - (double)(Angle + angle)));
            double F = Math.Sqrt((NetForce * NetForce) + (force * force) - (2 * NetForce * force * Trigonometrics.DCos(betweenAngle)));
            int A = Angle + (int)Trigonometrics.RadToDeg(Math.Atan((double)((double)(force * (double)Trigonometrics.DSin(betweenAngle)) / (double)(NetForce + (double)(force * (double)Trigonometrics.DCos(betweenAngle))))));
            SetAngle(A);
            SetForce(F);
        }

        public void SetXForced(double newX)
        {
            if (!Utils.Utils.IsAcceptable(newX))
            {
                Logger.Exception(new ArgumentException("Vector2.SetXForced() - Unacceptable position"));
                return;
            }
            X = newX;
            ForceUpdateOnNextTick = true;
        }

        public void SetYForced(double newY)
        {
            if (!Utils.Utils.IsAcceptable(newY))
            {
                Logger.Exception(new ArgumentException("Vector2.SetYForced() - Unacceptable position"));
                return;
            }
            Y = newY;
            ForceUpdateOnNextTick = true;
        }

        public void SetX(double newX)
        {
            if (!Utils.Utils.IsAcceptable(newX))
            {
                Logger.Exception(new ArgumentException("Vector2.SetX() - Unacceptable position"));
                return;
            }
            if (X == newX)
            {
                return;
            }
            if (newX < 0 || newX > Size.Width || Fixed)
            {
                UpdateOnNextTick = true;
                HitBounds();
                if (Fixed)
                {
                    return;
                }
            }
            X = newX < Size.Width && newX > 0 ? newX : newX < 0 ? 0 : newX > Size.Width ? Size.Width : 0;
            if (PreviousX != X)
            {
                UpdateOnNextTick = true;
            }
        }

        public void SetY(double newY)
        {
            if (!Utils.Utils.IsAcceptable(newY))
            {
                Logger.Exception(new ArgumentException("Vector2.SetY() - Unacceptable position"));
                return;
            }
            if (Y == newY)
            {
                return;
            }
            if (newY < 0 || newY > Size.Height || Fixed)
            {
                UpdateOnNextTick = true;
                HitBounds();
                if (Fixed)
                {
                    return;
                }
            }
            Y = newY < Size.Height && newY > 0 ? newY : newY < 0 ? 0 : newY > Size.Height ? Size.Height : 0;
            if (PreviousY != Y)
            {
                UpdateOnNextTick = true;
            }
        }

        public void SetAngleForced(int angle)
        {
            if (!Utils.Utils.IsAcceptable(angle))
            {
                Logger.Exception(new ArgumentException("Vector2.SetAngleForced() - Unacceptable angle"));
                return;
            }
            PreviousAngle = angle;
            _angle = angle;
        }

        public void SetAngle(int angle)
        {
            if (!Utils.Utils.IsAcceptable(angle))
            {
                Logger.Exception(new ArgumentException("Vector2.SetAngle() - Unacceptable angle"));
                return;
            }

            int periodChange = (int)(angle / 360d);
            int realAngle = angle - (int)(periodChange * 360d);
            if (realAngle < 0) realAngle = (int)(360d + realAngle);
            Angle = realAngle;
        }

        public void SetSpeed(double newSpeed)
        {
            if (!Utils.Utils.IsAcceptable(newSpeed))
            {
                Logger.Exception(new ArgumentException("Vector2.SetSpeed() - Unacceptable speed"));
                return;
            }

            Speed = newSpeed;
        }

        public void SetAcceleration(double newAcceleration)
        {
            if (!Utils.Utils.IsAcceptable(newAcceleration))
            {
                Logger.Exception(new ArgumentException("Vector2.SetAcceleration() - Unacceptable acceleration"));
                return;
            }
            PreviousAcceleration = Acceleration;
            Acceleration = newAcceleration;
        }

        public void SetForce(double newForce)
        {
            if (!Utils.Utils.IsAcceptable(newForce))
            {
                Logger.Exception(new ArgumentException("Vector2.SetForce() - Unacceptable force"));
                return;
            }

            NetForce = newForce;
        }

        public void IncreaseAcceleration(double addAcceleration)
        {
            SetAcceleration(Acceleration + addAcceleration);
        }

        public void DecreaseAcceleration(double disAcceleration)
        {
            SetAcceleration(Acceleration - disAcceleration);
        }

        public void IncreaseSpeed(double addSpeed)
        {
            SetSpeed(Speed + addSpeed);
        }

        public void DecreaseSpeed(double disSpeed)
        {
            SetSpeed(Speed - disSpeed);
        }

        public double RecalculateWeight()
        {
            Weight = Fixed ? 0d : Particle.Mass * Particle.Map.Physics.GravityAcceleration;
            if ((PermanentForces.Count - 1) >= 0 && ApplyGravity) PermanentForces[0] = new Force(Particle.Map.Physics.GravityVectorAngle, Weight, this);
            if (!ApplyGravity && (PermanentForces.Count - 1) >= 0) PermanentForces.RemoveAt(0);
            return Weight;
        }

        /// <summary>
        /// Resets all (force, acceleration, speed)
        /// </summary>
        public void HitBounds()
        {
            // Resets all
            CompensateForces();
            SetForce(0d);
            SetAcceleration(0d);
            SetSpeed(0d);
            
        }

        public void Initialize()
        {
            if (ApplyGravity)
            {
                PermanentForces.Add(new Force(Particle.Map.Physics.GravityVectorAngle, Weight, this));
            }
        }

        // Compensate forces is only stopping Forces, its not stopping the whole object completely
        public void CompensateForces()
        {
            TemporaryForces.Clear();
            PermanentForces.ForEach(f => f.Reset());
        }

        public double[] GetPosition()
        {
            return new double[] { X, Y };
        }

        public Vector2f ToVector2()
        {
            return new Vector2f((float)X, (float)Y);
        }

        public void CollideWith(ParticlePositionParameters vector, double distance)
        {

            Logger.Log("Collision");
            double responseCoeff = 2d;

            Vector2f v = ToVector2() - vector.ToVector2();
            Vector2f n = v / (float)distance;

            double massSum = Particle.Mass + vector.Particle.Mass;
            double ratio1 = Particle.Mass / massSum;
            double ratio2 = vector.Particle.Mass / massSum;
            double delta = .5f * responseCoeff * Math.Abs(distance - Size.GetDefaultSize().Width);

            Vector2f add1 = n * (float)((float)ratio2 * (float)delta);
            Vector2f add2 = n * (float)((float)ratio1 * (float)delta);

            X += add1.X;
            Y += add1.Y;
            vector.X += add2.X;
            vector.Y += add2.Y;
            Logger.Log(ratio1.ToString());
            Logger.Log(ratio2.ToString());


        }

        public static bool operator ==(ParticlePositionParameters left, ParticlePositionParameters right)
        {
            return left.X <= right.X && left.X + left.Particle.Size.Width >= right.X &&
                left.Y <= right.Y && left.Y + left.Particle.Size.Height >= right.Y;
        }

        public static bool operator !=(ParticlePositionParameters left, ParticlePositionParameters right)
        {
            return !(left == right);
        }


    }
}
