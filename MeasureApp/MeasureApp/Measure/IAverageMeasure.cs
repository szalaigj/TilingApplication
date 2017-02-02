using MeasureApp.Data;
using System;
using System.Collections.Generic;

namespace MeasureApp.Measure
{
    public interface IAverageMeasure
    {
        double averageAllMeasures(BinGroup[] partition);
    }
}
