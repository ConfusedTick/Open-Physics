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
        public Vector2 Position;

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
        /// Зафиксирована ли частица на месте
        /// </summary>
        public bool Fixed { get; protected set; }

        /// <summary>
        /// Максимальная длина луча рендера тепла
        /// </summary>
        public readonly double MaxDepth;

        /// <summary>
        /// Длина половина партикла
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


        protected ParticleBase(Map.MapBase map, int id, string name, Vector2 position, Color color, Flags parameters, Size size, double mass, AggregationStates startState, double startTemperature, double emittingCoeff, double acceptanceCoeff, double heatCapacity, double meltingPoint, double meltingHeat, double evaporationPoint, double evaporationHeat, bool requireRandomTicks)
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
            //Initialize();
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

            double exp = (double)(flux / (double)(HeatCapacity * Mass));
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

            // Melting
            if (temp > MeltingPoint && CurrentState == AggregationStates.Solid)
            {
                double heatChange = (double)((double)((double)temp - MeltingPoint) * HeatCapacity);
                HeatBuffer += heatChange;
                Temperature = MeltingPoint;
                double tchange = (double)(HeatBuffer - MeltingHeat);
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
                double heatChange = (double)((double)((double)temp - EvaporationPoint) * HeatCapacity);
                HeatBuffer += heatChange;
                Temperature = EvaporationPoint;
                double tchange = (double)(HeatBuffer - EvaporationHeat);
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
                double heatChange = (double)((double)((double)temp - EvaporationPoint) * HeatCapacity);
                HeatBuffer += heatChange;
                Temperature = EvaporationPoint;
                double tchange = (double)(HeatBuffer + EvaporationHeat);
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
                double heatChange = (double)((double)((double)temp - MeltingPoint) * HeatCapacity);
                HeatBuffer += heatChange;
                Temperature = MeltingPoint;
                double tchange = (double)(HeatBuffer + MeltingHeat);
                if (tchange < 0)
                {
                    ChangeAggregationState(AggregationStates.Solid);
                    HeatBuffer = 0;
                    ChangeTemperatureByHeatFlux(tchange);
                }
            }


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
            Position = new Vector2(x, y, particle: this, sizeX: Map.Size.Width, sizeY: Map.Size.Height, mass: Mass, angle: Map.Physics.GravityVectorAngle, acceleration: Map.Physics.StartAcceleration);
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
                ParticlePositionChanged?.Invoke(this, new ParticleVisibleParametersEventArgs());
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
            if (EmittingCoeff <= 0d) return false;

            Dictionary<ParticleBase, double> affected;// = new Dictionary<ParticleBase, double>() { };

            bool skipHeatRender = false;

            switch (Map.Physics.HeatRender)
            {
                // Может вообще убрать, медленно работает ray casting
                // EDIT: Он теперь вообще выдаёт ошибку :)
                // TODO: Исправить
                case HeatRadiationRenders.RC:
                    double[] mc = CalculateMassCenter();
                    affected = RayCasting.RayCast(mc[0], mc[1], Position.Angle, Map.Physics.RaycastRayNumbers, Convert.ToInt32(MaxDepth) + 10, 360, Map);
                    break;

                case HeatRadiationRenders.RT:
                    affected = RayCasting.RayTrace(this, Map);
                    break;

                case HeatRadiationRenders.LRT:
                    affected = RayCasting.LazyRayTrace(this, Map);
                    break;

                case HeatRadiationRenders.NONE:
                    affected = RayCasting.LazyRayTrace(this, Map);
                    skipHeatRender = true;
                    break;

                default:
                    return false;
            }
            double transitionCoeff;
            double heatFlux;
            foreach (ParticleBase affect in affected.Keys)
            {
                if (affected[affect] <= Size.Width) CollideWith(affect);
                if (skipHeatRender) continue;
                if (affect.AcceptanceCoeff <= 0) continue;
                transitionCoeff = (double)(1 / (double)(Math.PI * (double)(affected[affect] * affected[affect]) * halfLenght));
                heatFlux = (double)((double)EmittingCoeff * (double)affect.AcceptanceCoeff * (double)Map.Physics.StefanBoltzmannConst) * (double)transitionCoeff * (double)(Math.Pow(Physic.CelsToKel(Temperature), 4) - Math.Pow(Physic.CelsToKel(affect.Temperature), 4));
                ChangeTemperatureByHeatFlux(-heatFlux);
                affect.ChangeTemperatureByHeatFlux(heatFlux);
            }
            return UpdateOnNextTick;
        }

        /// <summary>
        /// Столкновение двух частиц
        /// </summary>
        /// <param name="particle">Вторая столкнившаяся частица</param>
        public virtual void CollideWith(ParticleBase particle)
        {
            Position.CollideWith(particle.Position);
            ParticleCollided?.Invoke(this, new ParticleCollidedEventArgs(this, particle));
        }
    }
}
