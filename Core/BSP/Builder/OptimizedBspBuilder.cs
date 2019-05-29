﻿using Helion.BSP.Node;
using Helion.Map;

namespace Helion.BSP.Builder
{
    /// <summary>
    /// An optimized version of the BSP builder that should be used for when we
    /// are not debugging (as in used in map building, world building, etc).
    /// </summary>
    public class OptimizedBspBuilder : BspBuilder
    {
        public OptimizedBspBuilder(ValidMapEntryCollection map) : this(map, new BspConfig())
        {
        }

        public OptimizedBspBuilder(ValidMapEntryCollection map, BspConfig config) :
            base(map, config)
        {
        }

        /// <summary>
        /// Builds the entire tree and returns the root node upon completion.
        /// </summary>
        /// <returns>The root node of the built tree.</returns>
        public BspNode Build()
        {
            while (!Done)
                Execute();
            return Root;
        }
    }
}
