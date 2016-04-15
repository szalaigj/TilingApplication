using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp.Measure
{
    public class KNNAuxData : DefaultAuxData
    {
        public int KNN { get; set; }

        public int SpaceDimension { get; set; }

        public int HistogramResolution { get; set; }

        public Array Histogram { get; set; }
    }
}
