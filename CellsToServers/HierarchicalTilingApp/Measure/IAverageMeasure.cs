using HierarchicalTilingApp.ArrayPartition;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public interface IAverageMeasure
    {
        public double averageAllMeasures(Coords[] partition);
    }
}
