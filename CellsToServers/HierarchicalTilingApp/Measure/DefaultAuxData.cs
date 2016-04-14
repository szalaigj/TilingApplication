using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp.Measure
{
    public class DefaultAuxData : IAuxData
    {
        public int SpaceDimension { get; set; }

        public int HistogramResolution { get; set; }

        public int ServerNO { get; set; }

        public int PointNO { get; set; }

        public int KNN { get; set; }

        public Array Histogram { get; set; }
    }
}
