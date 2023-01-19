using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sim.GlmMath;
using Sim.Map;
using Sim.Particles;

namespace Sim.Simulation.HeatRender
{

    /// <summary>
    /// Отвечает за рендер тепла, возвращает словарь, который в качестве ключа содержит частицу с карты (не основную)
    /// а в качестве значения число, отображающее расстояние от этой частицы до основной. В случае RayTracingа и RayCastinga
    /// проводятся лучи, соответсвенно блоки, находящиеся за другими блоками, до которых не достанет луч, не будут учитываться возвращаемом словаре
    /// </summary>

    internal class RayCasting
    {

        public static Dictionary<ParticleBase, double> RayCast(double startX, double startY, int angle, int raysNumber, int depth, int fov, Map.MapBase map)
        {
            Dictionary<ParticleBase, double> outList = new Dictionary<ParticleBase, double>() { };
            double deltaAngle = fov / (double)raysNumber;
            double currentAngle = (double)(angle - (double)((double)fov / 2));
            for (int i = 0; i <= raysNumber; i++)
            {
                double sin = Trigonometrics.DSin(currentAngle);
                double cos = Trigonometrics.DCos(currentAngle);
                for (double d = 0; d <= depth; d += map.Physics.CasterDepthStep)
                {
                    double x = startX + (double)(d * cos);
                    double y = startY + (double)(d * sin);
                    if (!map.IsAllowedPosition(x, y))
                    {
                        break;
                    }

                    ParticleBase pred = map.IsInParticleArea(x, y);
                    if (pred != null && !outList.ContainsKey(pred))
                    {
                        outList.Add(pred, (double)Math.Abs(d));
                        break;
                    }
                }
                currentAngle += deltaAngle;
            }
            return outList;
        }

        public static Dictionary<ParticleBase, double> LazyRayTrace(ParticleBase main, Map.MapBase map)
        {
            Dictionary<ParticleBase, double> outList = new Dictionary<ParticleBase, double>() { };
            double[] mainCenter = main.CalculateMassCenter();
            foreach (ParticleBase particle in map.Particles)
            {
                if (main.Position == particle.Position) continue;
                if (particle == main)
                {
                    continue;
                }
                if (!outList.ContainsKey(particle))
                {
                    double[] particleCenter = particle.CalculateMassCenter();
                    double dif0 = particleCenter[0] - mainCenter[0];
                    double dif1 = particleCenter[1] - mainCenter[1];
                    outList.Add(particle, Math.Sqrt((dif0 * dif0) + (dif1 * dif1)));
                }
            }
            return outList;
        }

        public static Dictionary<ParticleBase, double> RayTrace(ParticleBase main, Map.MapBase map)
        {
            Dictionary<ParticleBase, double> outList = new Dictionary<ParticleBase, double>() { };
            double[] mainCenter = main.CalculateMassCenter();
            foreach (ParticleBase particle in map.Particles)
            {
                if (particle == main)
                {
                    continue;
                }

                double[] particleCenter = particle.CalculateMassCenter();
                double angle = Math.Atan2((double)(particleCenter[1] - mainCenter[1]), (double)(particleCenter[0] - mainCenter[0]));

                double sin = Math.Sin(angle);
                double cos = Math.Cos(angle);

                for (double depth = 0; depth < main.MaxDepth; depth += map.Physics.CasterDepthStep)
                {
                    double x = mainCenter[0] + (double)(depth * cos);
                    double y = mainCenter[1] + (double)(depth * sin);

                    if (!map.IsAllowedPosition(x, y))
                    {
                        break;
                    }

                    List<ParticleBase> preds = map.IsInParticleAreaAll(x, y);

                    if (preds.Count != 1)
                    {
                        if (preds.Count > 1)
                        {
                            break;
                        }

                        continue;
                    }

                    ParticleBase pred = preds[0];

                    if (pred == main)
                    {
                        continue;
                    }

                    if (outList.ContainsKey(pred))
                    {
                        break;
                    }

                    outList.Add(pred, Math.Abs(depth));
                    break;
                }
            }
            return outList;
        }
    }
}
