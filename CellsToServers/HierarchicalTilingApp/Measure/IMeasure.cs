using HierarchicalTilingApp.ArrayPartition;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public interface IMeasure<T>
        where T : IAuxData
    {
        T AuxData { get; set; }

        double computeMeasure(Coords[] partition);

        double computeMeasureForRegion(Coords coords);

        double computeMeasureForBin(int[] indicesArrayOfBin, int[] indicesArrayOfRegion);
    }
}
