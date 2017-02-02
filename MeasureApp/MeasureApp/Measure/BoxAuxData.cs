using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Measure
{
    public class BoxAuxData : BaseAuxData
    {
        public int SpaceDimension { get; set; }

        public int HistogramResolution { get; set; }

        public Array Histogram { get; set; }

        public int[] IndicesArrayOfQueryRegion { get; set; }

        public double VolumeOfQueryRegion { get; set; }
    }
}
