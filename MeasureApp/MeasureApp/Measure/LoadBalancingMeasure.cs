using MeasureApp.Data;
using MeasureApp.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Measure
{
    public class LoadBalancingMeasure : BaseMeasure<LoadBalancingAuxData>
    {
        public LoadBalancingMeasure(LoadBalancingAuxData auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public override double computeMeasureForRegion(BinGroup binGroup)
        {
            return 1 - (binGroup.differenceFromDelta(AuxData.Delta) / (double)AuxData.PointNO);
        }

        public override double computeMeasureForBin(int[] indicesArrayOfBin, List<int> binList)
        {
            // This method is unused for load balancing measure
            throw new NotImplementedException();
        }
    }
}
