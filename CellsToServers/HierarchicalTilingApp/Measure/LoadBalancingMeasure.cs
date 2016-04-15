using HierarchicalTilingApp.ArrayPartition;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp.Measure
{
    public class LoadBalancingMeasure : BaseMeasure<LoadBalancingAuxData>
    {
        public LoadBalancingMeasure(LoadBalancingAuxData auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public override double computeMeasureForRegion(Coords coords)
        {
            return 1 - (coords.differenceFromDelta(AuxData.Delta) / (double)AuxData.PointNO);
        }

        public override double computeMeasureForBin(int[] indicesArrayOfBin, int[] indicesArrayOfRegion)
        {
            // This method is unused for load balancing measure
            throw new NotImplementedException();
        }
    }
}
