using MeasureApp.Data;
using System;
using System.Collections.Generic;

namespace MeasureApp.Measure
{
    public interface IMeasure<T>
        where T : IAuxData
    {
        T AuxData { get; set; }

        double computeMeasure(BinGroup[] partition);

        double computeMeasureForRegion(BinGroup binGroup);

        double computeMeasureForBin(int[] indicesArrayOfBin, List<int> binList);
    }
}
