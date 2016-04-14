using HierarchicalTilingApp.ArrayPartition;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public interface IMeasure<T>
        where T : IAuxData
    {
        double computeMeasure(Coords[] partition);

        double computeMeasureForRegion(int[] indicesArrayOfRegion);

        double computeMeasureForBin(int[] indicesArrayOfBin, int[] indicesArrayOfRegion);
    }
}
