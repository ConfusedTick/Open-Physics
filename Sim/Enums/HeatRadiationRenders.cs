using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Enums
{
    /// <summary>
    /// Виды рендера тепла. 
    /// </summary>
    public enum HeatRadiationRenders : byte
    {
        /// <summary>
        /// Ray casting 
        /// </summary>
        RC = 1,
        /// <summary>
        /// Ray tracing
        /// </summary>
        RT = 2,
        /// <summary>
        /// Lasy ray tracing
        /// </summary>
        LRT = 3,
        /// <summary>
        /// None
        /// </summary>
        NONE = 255,

    }
}
