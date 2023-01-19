using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Enums
{
    /// <summary>
    /// Виды агрегатных состояний блока 
    /// </summary>
    public enum AggregationStates : byte
    {
        /// <summary>
        /// Твердое состояние
        /// </summary>
        Solid = 0,
        /// <summary>
        /// Жидкое состояние
        /// </summary>
        Liquid = 1,
        /// <summary>
        /// Газообразное состояние
        /// </summary>
        Gas = 2,

    }
}
