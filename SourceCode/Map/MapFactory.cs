using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sim.Particles;
using Sim.Simulation;
using Sim.Enums;

namespace Sim.Map
{

    /**
    * [Int32]      - 4 bytes
    * [Double]     - 8 bytes 
    * 
    * Обычный Формат:
    * [String Format] - for this version: OPMF 
    * [Int32 MapWidth][Int32 MapHeight]
    * Repeating:
    * [Vector2 data] (-[Double X][Double Y][Int32 Angle][Double Accel]-)
    * [Particle data] (-[Int32 Id][Bool Fixed][Double Temperature][Double HeatBuffer][Double Mass][Byte AggregState]-)
    * 
    * Расширенный формат:
    * [String Format] - for this version EOPMF
    * [Int32 MapWidth][Int32 MapHeight]
    * [Physic PhysicParams]
    * Repeating:
    * [Vector2 Edata]
    * [Particle Edata]
    * [Flags data] // Flags data should be optional
    * 
    **/


    public class MapFactory
    {

        public const string FileFormatName = "OPMF";
        public const string ExtendedFileFormatName = "EOPMF";

        /// <summary>
        /// Проверяет, является ли файл файлом сохранения карты
        /// </summary>
        public bool IsMapFile(string fileName)
        {
            using BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open));
            if (reader.ReadString() != FileFormatName) return false;
            return true;
        }

        /// <summary>
        /// Проверяет, является ли файл файлом расширенного сохранения карыт
        /// </summary>
        public bool IsExtendedMapFile(string fileName)
        {
            using BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open));
            if (reader.ReadString() != ExtendedFileFormatName) return false;
            return true;
        }

        /// <summary>
        /// Загружает карту из файла
        /// </summary>
        /// <param name="fileName">Название файла</param>
        /// <param name="physics">Физика, используемая для этой карты</param>
        /// <returns>Загруженнная карта</returns>
        public static MapBase LoadMapFromFile(string fileName, Physic physics)
        {
            MapBase map;
            Size mapSize;
            List<ParticleBase> add = new List<ParticleBase>();
            ParticleFactory factory;
            ParticleBase particle;
            Vector2 pos;
            using (BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.OpenOrCreate)))
            {
                if (reader.ReadString() != FileFormatName)
                {
                    Logger.Exception(new FormatException("Invalid map file format."));
                    return null;
                }

                mapSize = new Size(reader.ReadInt32(), reader.ReadInt32());
                map = new MapBase(mapSize, physics);
                factory = new ParticleFactory(map);
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    pos = new Vector2(x: reader.ReadDouble(), y: reader.ReadDouble(), angle: reader.ReadInt32(), acceleration: reader.ReadDouble());
                    particle = factory.CreateParticle(id: reader.ReadInt32(), pos, flags: Flags.Empty);
                    particle.SetFixed(reader.ReadBoolean());
                    particle.SetTemperature(reader.ReadDouble());
                    particle.SetHeatBuffer(reader.ReadDouble());
                    particle.SetMass(reader.ReadDouble());
                    particle.ChangeAggregationState((AggregationStates)reader.ReadByte());
                    add.Add(particle);
                }
            }
            add.ForEach(part => map.AddParticle(part));
            return map;
        }

        /**
        public MapBase LoadMapFromFile(string fileName, Physic physics)
        {
            MapBase map;
            Size mapSize;
            List<ParticleBase> add = new List<ParticleBase>();
            ParticleFactory factory;
            ParticleBase particle;
            Vector2 pos;
            using (BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.OpenOrCreate)))
            {
                if (reader.ReadString() != FileFormatName) throw new FormatException("Invalid map file format.");

                mapSize = new Size(reader.ReadInt32(), reader.ReadInt32());
                map = new MapBase(mapSize, physics);
                factory = new ParticleFactory(map);
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    pos = new Vector2(x: reader.ReadDouble(), y: reader.ReadDouble(), angle: reader.ReadInt32(), acceleration: reader.ReadDouble());
                    particle = factory.CreateParticle(id: reader.ReadInt32(), pos, flags: Flags.Empty);
                    particle.SetFixed(reader.ReadBoolean());
                    particle.SetTemperature(reader.ReadDouble());
                    particle.SetHeatBuffer(reader.ReadDouble());
                    particle.SetMass(reader.ReadDouble());
                    particle.ChangeAggregationState((AggregationStates)reader.ReadByte());
                    add.Add(particle);
                }
            }
            add.ForEach(part => map.AddParticle(part));
            return map;
        }

        **/
    }
}
