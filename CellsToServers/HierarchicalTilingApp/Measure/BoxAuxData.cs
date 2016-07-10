﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp.Measure
{
    public class BoxAuxData : BaseAuxData
    {
        public int SpaceDimension { get; set; }

        public int HistogramResolution { get; set; }

        public Array Array { get; set; }

        public Array HeftArray { get; set; }

        public int[] IndicesArrayOfQueryRegion { get; set; }

        public double VolumeOfQueryRegion { get; set; }
    }
}
