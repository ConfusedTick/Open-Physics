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

    // НЕ ЛЕЗЬ
    internal class RayCasting
    {

        public static bool ShowRays = true;
        public static ulong DeleteRayThreshold = 5;
        public static double MaxDepth = 2000d;
        public static double FOVDelta = Trigonometrics.DegToRad(20d);

        public static double FirstAnglesCheck = 1.5d * Size.GetDefaultSize().Width;
        public static double SecondAnglesCheck = MaxDepth;

        public static List<KeyValuePair<ParticleBase, (double, double)>> TraceRay(MapBase map, MapPoint startpos, MapPoint endpos, List<ParticleBase> searchlist, ulong recurtion = 0, double externalcoef = 1)
        {
            List<KeyValuePair<ParticleBase, (double, double)>> outList = new List<KeyValuePair<ParticleBase, (double, double)>>();
            if (recurtion >= DeleteRayThreshold) return outList;
            double angle = Math.Atan2((double)(endpos.Y - startpos.Y), (double)(endpos.X - startpos.X));

            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            List<KeyValuePair<ParticleBase, (double, double)>> pred2;

            ParticleBase ignore = map.IsInParticleArea(startpos.X, startpos.Y);

            Dictionary<ParticleBase, double> angles = AngleToAll(ignore, searchlist);

            bool removed = false;

            for (double depth = 0; depth < MaxDepth; depth += map.Physics.CasterDepthStep)
            {

                double x = startpos.X + (double)(depth * cos);
                double y = startpos.Y + (double)(depth * sin);

                if (depth >= SecondAnglesCheck)
                {
                    removed = false;
                    angles = AngleToAll(x, y, searchlist);
                }
                
                if (depth >= FirstAnglesCheck && !removed)
                {
                    foreach (ParticleBase bs in angles.Keys)
                    {
                        if (Math.Abs(angles[bs] - angle) > FOVDelta)
                        {
                            searchlist.Remove(bs);
                        }
                    }
                    removed = true;
                    if (searchlist.Count == 0)
                    {
                        return outList;
                    }
                    if (searchlist.Count == 1)
                    {
                        outList.Add(new KeyValuePair<ParticleBase, (double, double)>(searchlist[0], (DistanceTo(ignore, searchlist[0]), 1d - searchlist[0].Transparency)));
                        return outList;
                    }
                }

                double coef;
                if (ShowRays) App.MainWindow.Dot(x, y);
                if (!map.IsAllowedPosition(x, y))
                {
                    break;
                }

                ParticleBase pred = MapBase.IsInParticleArea(x, y, searchlist);

                if (pred == null || pred == ignore)
                {
                    continue;
                }

                // Внешний коэффициент для дополнительных лучей
                coef = 1d - pred.Transparency;

                outList.Add(new KeyValuePair<ParticleBase, (double, double)>(pred, (depth, coef * externalcoef)));

                // Луч прозрачности
                if (pred.Transparency != .0d)
                {
                    pred2 = TraceRay(map, new MapPoint(x, y), new MapPoint((x + (cos * MaxDepth)), (y + (sin * MaxDepth))), searchlist, recurtion + 1, pred.Transparency * externalcoef);
                    outList.AddRange(pred2);
                }

                // Луч отражения
                if (pred.AcceptanceCoeff != 1d)
                {
                    pred2 = TraceRay(map, new MapPoint(x, y), new MapPoint((x + (double)(x * Math.Cos(angle + Math.PI) * MaxDepth)), (y + (double)(y * Math.Sin(angle + Math.PI) * MaxDepth))), map.Particles.ToList(), recurtion + 1, externalcoef * (1d - pred.AcceptanceCoeff));
                    outList.AddRange(pred2);
                }
                break;
            }
            return outList;
        }

        public static double AngleFromPointToPoint(MapPoint start, MapPoint end)
        {
            return Math.Atan2((double)(end.Y - start.Y), (double)(end.X - start.X));
        }

        public static double AngleFromPointToPoint(double startx, double starty, double endx, double endy)
        {
            return Math.Atan2(endy - starty, endx - startx);
        }

        public static Dictionary<ParticleBase, double> AngleToAll(ParticleBase start, List<ParticleBase> search)
        {
            search.Remove(start);
            Dictionary<ParticleBase, double> outlist = new Dictionary<ParticleBase, double>();
            foreach(ParticleBase particle in search)
            {
                outlist.Add(particle, AngleFromPointToPoint(start.Position.X, start.Position.Y, particle.Position.X, particle.Position.Y));
            }
            return outlist;
        }

        public static Dictionary<ParticleBase, double> AngleToAll(double x, double y, List<ParticleBase> search)
        {
            Dictionary<ParticleBase, double> outlist = new Dictionary<ParticleBase, double>();
            foreach(ParticleBase particle in search)
            {
                outlist.Add(particle, AngleFromPointToPoint(x, y, particle.Position.X, particle.Position.Y));
            }
            return outlist;
        }

        public static List<KeyValuePair<ParticleBase, (double, double)>> RayTraceAll(ParticleBase main, MapBase map)
        {
            List<KeyValuePair<ParticleBase, (double, double)>> outList = new List<KeyValuePair<ParticleBase, (double, double)>>();
            double[] mainCenter = main.CalculateMassCenter();
            List<KeyValuePair<ParticleBase, (double, double)>> predicate;
            foreach (ParticleBase particle in map.Particles.ToList())
            {
                if (particle == main)
                {
                    continue;
                }

                double[] particleCenter = particle.CalculateMassCenter();

                predicate = TraceRay(map, new MapPoint(mainCenter[0], mainCenter[1]), new MapPoint(particleCenter[0], particleCenter[1]), map.Particles.ToList(), 0, main.EmmitingCoeff);
                if (predicate.Count <= 0)
                {
                    continue;
                }

                outList.AddRange(predicate);
            }
            return outList;
        }

        public static double DistanceTo(ParticleBase start, ParticleBase end)
        {
            double[] startCenter = start.CalculateMassCenter();
            double[] endCenter = end.CalculateMassCenter();
            double deltax = endCenter[0] - startCenter[0];
            double deltay = endCenter[1] - startCenter[1];
            return Math.Sqrt((deltax * deltax) + (deltay * deltay));
        }

        public static Dictionary<ParticleBase, double> LazyCollisionRayTrace(ParticleBase main, MapBase map)
        {
            Dictionary<ParticleBase, double> outList = new Dictionary<ParticleBase, double>();
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
    }
}
