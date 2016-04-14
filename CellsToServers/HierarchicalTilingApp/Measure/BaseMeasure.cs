using HierarchicalTilingApp.ArrayPartition;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public abstract class BaseMeasure : IMeasure<DefaultAuxData>
    {
        public DefaultAuxData AuxData { get; set; }

        public abstract double computeMeasure(Coords[] partition);

        public abstract double computeMeasureForRegion(int[] indicesArrayOfRegion);

        public abstract double computeMeasureForBin(int[] indicesArrayOfBin, int[] indicesArrayOfRegion);
    }
}
