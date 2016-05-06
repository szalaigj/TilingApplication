using System;
using System.Collections.Generic;

namespace RecursiveBisectionApp.Utils
{
    public class LoadBalancingMeasure
    {
        private int serverNO;
        private int pointNO;
        private double delta;

        public LoadBalancingMeasure(int serverNO, int pointNO, double delta)
        {
            this.serverNO = serverNO;
            this.pointNO = pointNO;
            this.delta = delta;
        }

        public double computeMeasure(Coords[] partition)
        {
            double measure = 0.0;
            foreach (var coords in partition)
            {
                measure += computeMeasureForRegion(coords);
            }
            measure = measure / serverNO;
            return measure;
        }

        private double computeMeasureForRegion(Coords coords)
        {
            return 1 - (coords.differenceFromDelta(delta) / (double)pointNO);
        }
    }
}
