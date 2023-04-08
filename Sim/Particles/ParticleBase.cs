using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Threading;
using Sim.GlmMath;
using Sim.Enums;
using Sim.Simulation.HeatRender;
using Sim.Map;
using Sim.Events;
using Sim.Simulation;
using Sim.Utils;

namespace Sim.Particles
{

    /// <summary>
    /// Этот класс должны наследовать все частицы
    /// И тут определена вся базовая логика для частиц
    /// 
    /// Для создания новых частиц требуется наследование 
    /// этого класса 
    /// </summary>
    public abstract class ParticleBase
    {

        /// <summary>
        /// Карта на которой находиться частица
        /// </summary>
        public Map.MapBase Map { get; protected set; }

        /// <summary>
        /// Последний созданный Uid
        /// </summary>
        private static int LastUid = 0;

        /// <summary>
        /// Uid частицы - уникален для каждого партикла
        /// </summary>
        public readonly int Uid;

        /// <summary>
        /// Id (тип) частицы
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// Название частицы
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Позиция частицы
        /// </summary>
        public ParticlePositionParameters Position;

        /// <summary>
        /// Количество тиков, которое прожила частица
        /// </summary>
        public ulong LifeTick { get; protected set; }

        /// <summary>
        /// Визуальный цвет партикла
        /// </summary>
        public Color Color { get; protected set; }

        /// <summary>
        /// Флаги частицы
        /// </summary>
        public Flags Flags;

        /// <summary>
        /// Размер частицы
        /// </summary>
        public readonly Size Size;

        /// <summary>
        /// Масса частицы
        /// </summary>
        public double Mass { get; protected set; }

        /// <summary>
        /// Размер карты на которрй находиться частица
        /// </summary>
        public Size MapSize { get; protected set; }

        /// <summary>
        /// Предшествующее агрегатное состояние
        /// </summary>
        public AggregationStates PreviousState;

        /// <summary>
        /// Текущие агрегатное состояние
        /// </summary>
        public AggregationStates CurrentState { get; protected set; }

        /// <summary>
        /// Текущая темепература
        /// </summary>
        public double Temperature { get; protected set; }

        /// <summary>
        /// Текущая темепература
        /// </summary>
        public double PreviousTemperature { get; protected set; }

        /// <summary>
        /// Зафиксирована ли частица на месте
        /// </summary>
        public bool Fixed { get; protected set; }

        /// <summary>
        /// Максимальная длина луча рендера тепла
        /// </summary>
        public readonly double MaxDepth;

        /// <summary>
        /// Длина половины окружности вписанной в квадрат партикла
        /// </summary>
        public readonly double halfLenght;

        /// <summary>
        /// Коэффициент излучения
        /// </summary>
        public double EmittingCoeff { get; protected set; }

        /// <summary>
        /// Коэффициент поглощения
        /// </summary>
        public double AcceptanceCoeff { get; protected set; }

        /// <summary>
        /// Прозрачность частицы
        /// </summary>
        public double Transparency { get; protected set; }

        /// <summary>
        /// Теплоемкость
        /// </summary>
        public double HeatCapacity { get; protected set; }

        /// <summary>
        /// Температура начала плавления
        /// </summary>
        public readonly double MeltingPoint;

        /// <summary>
        /// Теплота плавления
        /// </summary>
        public readonly double MeltingHeat;

        /// <summary>
        /// Температура начала испарения
        /// </summary>
        public readonly double EvaporationPoint;

        /// <summary>
        /// Тепло испарения
        /// </summary>
        public readonly double EvaporationHeat;

        /// <summary>
        /// Обновлять ли визуально частицу на текущем тике
        /// </summary>
        public bool UpdateOnNextTick = true;

        /// <summary>
        /// Нужны ли партиклу случайные тики
        /// </summary>
        public bool RequireRandomTick = false;

        /// <summary>
        /// Частота случайных тиков
        /// </summary>
        public int RandomTickRarity = 0;

        /// <summary>
        /// Тепловой буффер изменения температуры
        /// </summary>
        public double HeatBuffer { get; private set; }

        public event EventHandler ParticlePositionChanged;
        public event EventHandler ParticleAggregationStateChanged;
        public event EventHandler ParticleTemperatureChanged;
        public event EventHandler ParticleRemoved;
        public event EventHandler ParticleInitialized;
        public event EventHandler ParticleCollided;


        protected ParticleBase(MapBase map, int id, string name, ParticlePositionParameters position, Color color, Flags parameters, Size size, double mass, AggregationStates startState, double startTemperature, double emittingCoeff, double acceptanceCoeff, double transparency, double heatCapacity, double meltingPoint, double meltingHeat, double evaporationPoint, double evaporationHeat, bool requireRandomTicks)
        {
            Uid = LastUid + 1;
            LastUid += 1;
            Map = map;
            Id = id;
            Name = name;
            Position = position;
            Color = color;
            Flags = parameters;
            Size = size;
            Mass = mass;
            Position.Particle = this;
            MapSize = Map.Size;
            PreviousState = CurrentState;
            CurrentState = startState;
            Temperature = startTemperature;
            PreviousTemperature = startTemperature;
            EmittingCoeff = emittingCoeff;
            AcceptanceCoeff = acceptanceCoeff;
            HeatCapacity = heatCapacity;
            HeatBuffer = 0d;
            MeltingPoint = meltingPoint;
            MeltingHeat = meltingHeat;
            EvaporationPoint = evaporationPoint;
            EvaporationHeat = evaporationHeat;
            MaxDepth = Math.Sqrt((map.Size.Width * map.Size.Width) + (map.Size.Height * map.Size.Height));
            halfLenght = Math.PI * size.Width / 2;
            CalculateAggregationState(Temperature);
            RequireRandomTick = requireRandomTicks;
            Map.Physics.PhysicParametersChanged += MapPhysicsUpdated;
            Transparency = transparency;
            CheckSum();
        }

        private void MapPhysicsUpdated(object sender, EventArgs e)
        {
            Position.SetAngleForced(Map.Physics.GravityVectorAngle);
            _ = Position.RecalculateWeight();
        }

        /// <summary>
        /// Вызывается когда частица добовляется на карту
        /// </summary>
        public virtual void Initialize()
        {
            ParticleInitialized?.Invoke(this, EventArgs.Empty);
        }

        public virtual void CheckSum()
        {
            if (Transparency + AcceptanceCoeff > 1)
            {
                Logger.Exception(new ParticleException(this, "Checks sum is out of interval [0;+1]"));
                Environment.Exit(114);
            }
        }

        public virtual void DecayInto(ParticleBase[] particles)
        {
            Remove();

            foreach (ParticleBase bs in particles)
            {
                bs.InitPosition();
                bs.Position.SetXForced(Position.PreviousX);
                bs.Position.SetYForced(Position.PreviousY);
                Map.AddParticle(bs);
            }
        }

        protected ParticleBase()
        {
            Uid = LastUid + 1;
            LastUid += 1;
        }

        protected ParticleBase(Size mapSize, Size totalSize)
        {
            Uid = LastUid + 1;
            LastUid += 1;
            MaxDepth = Math.Sqrt((mapSize.Width * mapSize.Width) + (mapSize.Height * mapSize.Height));
            halfLenght = Math.PI * totalSize.Width / 2;
        }
        
        /// <summary>
        /// Фиксирует частицу на месте
        /// </summary>

        public virtual void Fix()
        {
            Fixed = true;
            Position.Fixed = true;
            _ = Position.RecalculateWeight();
            Position.HitBounds();
        }

        /// <summary>
        /// Отключает фиксирование частицы на месте 
        /// </summary>
        public virtual void UnFix()
        {
            Fixed = false;
            Position.Fixed = false;
            _ = Position.RecalculateWeight();
        }

        /// <summary>
        /// Фиксирует или разфиксирует частицу
        /// </summary>
        /// <param name="fix">Состояние частицы</param>
        public virtual void SetFixed(bool fix)
        {
            if (fix)
            {
                Fix();
            }
            else
            {
                UnFix();
            }
        }

        /// <summary>
        /// Увеличивает температуру
        /// </summary>
        /// <param name="addtemp">Температура которую нужно добавить</param>
        public virtual void IncreaseTemperature(double addtemp)
        {
            SetTemperature(Temperature + addtemp);
        }

        /// <summary>
        /// Устанавливает массу частицы
        /// </summary>
        /// <param name="newMass">Новая температура</param>
        public virtual void SetMass(double newMass)
        {
            if (!Utils.Utils.IsAcceptable(newMass))
            {
                Logger.Exception(new ArgumentException("ParticleBase.SetMass() - Unacceptable mass"));
                return;
            }

            Mass = newMass;
        }

        public virtual void SetHeatBuffer(double newBuffer)
        {
            if (!Utils.Utils.IsAcceptable(newBuffer))
            {
                Logger.Exception(new ArgumentException("ParticleBase.SetHeatBuffer() - Unacceptable heat buffer"));
                return;
            }
            HeatBuffer = newBuffer;
        }

        public virtual bool IsInstrument()
        {
            return false;
        }

        /// <summary>
        /// Изменяет температуру по тепловому потоку
        /// </summary>
        /// <param name="flux">Тепловой потом</param>
        public virtual void ChangeTemperatureByHeatFlux(double flux)
        {
            if (!Utils.Utils.IsAcceptable(flux))
            {
                Logger.Exception(new ArgumentException("ParticleBase.ChangeTemperatureByHeatFlux() - Unacceptable temperature"));
                return;
            }
            if (Math.Abs(flux) == 0d)
            {
                return;
            }

            double exp = 0d;

            // Учёт буффера тепла при уменьшении температуры

            if (HeatBuffer != 0)
            {
                if (HeatBuffer > 0 && flux <= 0) 
                {
                    if (Math.Abs(HeatBuffer) >= Math.Abs(flux))
                    {
                        double deltabuf = HeatBuffer - flux;
                        HeatBuffer -= flux;
                        ChangeTemperatureByHeatFlux(deltabuf);
                    }
                    if (Math.Abs(HeatBuffer) <= Math.Abs(flux))
                    {
                        double deltabuf = HeatBuffer;
                        HeatBuffer = 0;
                        ChangeTemperatureByHeatFlux(deltabuf);
                    }
                }
                if (HeatBuffer < 0 && flux >= 0)
                {
                    if (Math.Abs(HeatBuffer) >= Math.Abs(flux))
                    {
                        double deltabuf = Math.Abs(HeatBuffer) - Math.Abs(flux);
                        HeatBuffer += flux;
                        ChangeTemperatureByHeatFlux(deltabuf);
                    }
                    if (Math.Abs(HeatBuffer) <= Math.Abs(flux))
                    {
                        double deltabuf = HeatBuffer;
                        HeatBuffer = 0;
                        ChangeTemperatureByHeatFlux(deltabuf);
                    }
                }
            }

            exp = (double)(flux / (HeatCapacity * Mass));
            

            
            IncreaseTemperature(exp);
        }

        /// <summary>
        /// Вычисляет центр масс
        /// </summary>
        /// <returns>Массив double, первый элемент которого содержит x, а второй y</returns>
        public virtual double[] CalculateMassCenter()
        {
            double mcx = Position.X + (double)(Size.Width * 0.5d);
            double mcy = Position.Y + (double)(Size.Height * 0.5d);
            return new double[] { mcx, mcy };
        }

        /// <summary>
        /// Устанавлиает температуру
        /// </summary>
        /// <param name="newtemp">Новая температура</param>
        public virtual void SetTemperature(double newtemp)
        {
            if (!Utils.Utils.IsAcceptable(newtemp))
            {
                Logger.Exception(new ArgumentException("ParticleBase.SetTemperature() - Unacceptable temperature."));
                return;
            }

            ParticleTemperatureChanged?.Invoke(this, new ParticleTemperatureChangedEventArgs(newtemp));

            if (newtemp < Map.Physics.MinTemperature)
            {
                newtemp = Map.Physics.MinTemperature;
            }
            if (newtemp > Map.Physics.MaxTemperature)
            {
                newtemp = Map.Physics.MaxTemperature;
            }
            PreviousTemperature = Temperature;
            Temperature = newtemp;
            CalculateAggregationState(Temperature);
        }

        /// <summary>
        /// Изменяет агрегатное состояние
        /// </summary>
        /// <param name="newState">Новое состояние</param>
        public virtual void ChangeAggregationState(AggregationStates newState)
        {
            UpdateOnNextTick = true;
            PreviousState = CurrentState;
            CurrentState = newState;
            ParticleAggregationStateChanged?.Invoke(this, new ParticleAggregationStateChangedEventArgs(newState));
        }

        /// <summary>
        /// Корректирует агрегатное состояние
        /// </summary>
        /// <param name="temp">Температура</param>
        public virtual void CalculateAggregationState(double temp)
        {
            if (!Utils.Utils.IsAcceptable(temp))
            {
                Logger.Exception(new ArgumentException("ParticleBase.CalculateAggregationState() - Unacceptable temperature."));
                return;
            }

            double expheat = 0;
            double exptemp = temp;
            AggregationStates expstate = AggregationStates.Solid;
            if (temp > MeltingPoint && CurrentState == AggregationStates.Solid)
            {
                expheat = MeltingHeat * Mass;
                exptemp = MeltingPoint;
                expstate = AggregationStates.Liquid;
            }
            if (temp > EvaporationPoint && CurrentState == AggregationStates.Liquid)
            {
                expheat = EvaporationHeat * Mass;
                exptemp = EvaporationPoint;
                expstate = AggregationStates.Gas;
            }
            if (temp < EvaporationPoint && CurrentState == AggregationStates.Gas)
            {
                HeatBuffer = -EvaporationHeat * Mass;
                expheat = 0;
                exptemp = EvaporationPoint;
                expstate = AggregationStates.Liquid;
            }
            if (temp < MeltingPoint && CurrentState == AggregationStates.Liquid)
            {
                HeatBuffer = -MeltingHeat * Mass;
                expheat = 0;
                exptemp = MeltingPoint;
                expstate = AggregationStates.Solid;
            }

            double heatdelta = Math.Abs(Math.Abs(exptemp) - Math.Abs(temp)) * HeatCapacity * Mass;
            double bufferdelta;
            HeatBuffer += heatdelta;
            if (HeatBuffer > expheat)
            {
                bufferdelta = HeatBuffer - expheat;
                HeatBuffer = 0;
                ChangeAggregationState(expstate);
                ChangeTemperatureByHeatFlux(bufferdelta);
                return;
            }



            /**
            // Melting
            if (temp > MeltingPoint && CurrentState == AggregationStates.Solid)
            {
                double heatChange = (double)((double)((double)temp - MeltingPoint) * HeatCapacity * Mass);
                HeatBuffer += heatChange;
                Temperature = MeltingPoint;
                double tchange = (double)(HeatBuffer - MeltingHeat * Mass);
                if (tchange > 0)
                {
                    ChangeAggregationState(AggregationStates.Liquid);
                    HeatBuffer = 0;
                    ChangeTemperatureByHeatFlux(tchange);
                }
            }

            // Evaporation
            if (temp > EvaporationPoint && CurrentState == AggregationStates.Liquid)
            {
                double heatChange = (double)((double)((double)temp - EvaporationPoint) * HeatCapacity * Mass);
                HeatBuffer += heatChange;
                Temperature = EvaporationPoint;
                double tchange = (double)(HeatBuffer - EvaporationHeat * Mass);
                if (tchange > 0)
                {
                    ChangeAggregationState(AggregationStates.Gas);
                    HeatBuffer = 0;
                    ChangeTemperatureByHeatFlux(tchange);
                }
            }

            // Condensation
            if (temp < EvaporationPoint && CurrentState == AggregationStates.Gas)
            {
                double heatChange = (double)((double)((double)temp - EvaporationPoint) * HeatCapacity * Mass);
                HeatBuffer += heatChange;
                Temperature = EvaporationPoint;
                double tchange = (double)(HeatBuffer + EvaporationHeat * Mass);
                if (tchange < 0)
                {
                    ChangeAggregationState(AggregationStates.Liquid);
                    HeatBuffer = 0;
                    ChangeTemperatureByHeatFlux(tchange);
                }
            }

            // Crystalization
            if (temp < MeltingPoint && CurrentState == AggregationStates.Liquid)
            {
                double heatChange = (double)((double)((double)temp - MeltingPoint) * HeatCapacity * Mass);
                HeatBuffer += heatChange;
                Temperature = MeltingPoint;
                double tchange = (double)(HeatBuffer + MeltingHeat * Mass);
                if (tchange < 0)
                {
                    ChangeAggregationState(AggregationStates.Solid);
                    HeatBuffer = 0;
                    ChangeTemperatureByHeatFlux(tchange);
                }
            }
            **/
        }

        public virtual void Remove()
        {
            Map.RemoveParticle(this);
            ParticleRemoved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Инициализирует позицию для вектора
        /// Устанавливает ускорение на дефолтное ускорение и угол
        /// </summary>
        public virtual void InitPosition()
        {
            Position.Particle = this;
            Position.Size = Map.Size;
            Position.SetAngle(Map.Physics.GravityVectorAngle);
            Position.SetAcceleration(Map.Physics.StartAcceleration);
            _ = Position.RecalculateWeight();
            Position.Initialize();
        }

        /// <summary>
        /// Создаёт новый вектор для частицы
        /// </summary>
        /// <param name="x">Позиция x</param>
        /// <param name="y">Позиция y</param>
        public virtual void InitPosition(double x, double y)
        {
            Position = new ParticlePositionParameters(x, y, particle: this, sizeX: Map.Size.Width, sizeY: Map.Size.Height, mass: Mass, angle: Map.Physics.GravityVectorAngle, acceleration: Map.Physics.StartAcceleration);
            _ = Position.RecalculateWeight();
            Position.Initialize();
        }

        /// <summary>
        /// Случайный тик
        /// </summary>
        public virtual void RandomTick()
        {
        }

        /// <summary>
        /// Тик
        /// </summary>
        public virtual void Tick()
        {
            UpdateOnNextTick = RayCastingTick() | (PreviousState != CurrentState);
            if (Map.Physics.EnablePositionTick) UpdateOnNextTick |= Position.Tick();
            if (UpdateOnNextTick)
            {
                ParticlePositionChanged?.Invoke(this, new ParticleVisibleParametersEventArgs(this));
            }

            LifeTick++;

            UpdateOnNextTick = false;

        }

        /// <summary>
        /// Тик рендера тепла и столкновений
        /// </summary>
        /// <returns>Требуется ли обновления на следующем кадре</returns>
        protected virtual bool RayCastingTick()
        {
            double timeMult = 1d;
            if (EmittingCoeff <= 0d) return false;
            List<KeyValuePair<ParticleBase, (decimal, double)>> affected = RayCasting.RayTraceAll(this, Map);
            Dictionary<ParticleBase, double> collisions = RayCasting.LazyCollisionRayTrace(this, Map);
            decimal transitionCoeff;
            double heatFlux;


            foreach (ParticleBase pt in collisions.Keys)
            {
                if (collisions[pt] <= Size.GetDefaultSize().Width)
                {
                    CollideWith(pt, collisions[pt]);
                    //Idk, this just does not looks right...
                    //pt.CollideWith(this, collisions[pt]);
                }
            }

            foreach (KeyValuePair<ParticleBase, (decimal, double)> kvp in affected)
            {
                if (kvp.Key.AcceptanceCoeff <= 0) continue;
                transitionCoeff = ((decimal)(1 / (decimal)((decimal)Math.PI * (decimal)((decimal)kvp.Value.Item1 * (decimal)kvp.Value.Item1) * (decimal)halfLenght)));
                heatFlux = (double)((decimal)kvp.Value.Item2 * (decimal)((decimal)EmittingCoeff * (decimal)kvp.Key.AcceptanceCoeff * (decimal)Map.Physics.StefanBoltzmannConst) * (decimal)transitionCoeff * (decimal)(Math.Pow(Physic.CelsToKel(Temperature), 4) - (double)Math.Pow(Physic.CelsToKel(kvp.Key.Temperature), 4)));

                ChangeTemperatureByHeatFlux(-1 * (double)(heatFlux) * timeMult);
                kvp.Key.ChangeTemperatureByHeatFlux(heatFlux * timeMult);
                // I thought this would fix the precision error in ZeroPrecisionDistance
                //ChangeTemperatureByHeatFlux(-1 * (double)(heatFlux));
                //kvp.Key.ChangeTemperatureByHeatFlux((double)(heatFlux));
            }
            return UpdateOnNextTick;
        }

        /// <summary>
        /// Столкновение двух частиц
        /// </summary>
        /// <param name="particle">Вторая столкнувшаяся частица</param>
        public virtual void CollideWith(ParticleBase particle, double distance)
        {
            Position.CollideWith(particle.Position, distance);
            ParticleCollided?.Invoke(this, new ParticleCollidedEventArgs(this, particle));
        }
    }
}
