﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnuthRuleBinNumsApp.Utils
{
    public class FurthestPointFromLineMethod
    {
        public int findBestKneePoint(double[] xData, double[] yData)
        {
            double x1 = xData[0];
            double x2 = xData[xData.Length - 1];
            double y1 = yData[0];
            double y2 = yData[yData.Length - 1];
            double maxDistance = 0.0;
            int maxDistanceIdx = 0;
            for (int idx = 0; idx < xData.Length; idx++)
            {
                double currentDist = computeDistance(xData[idx],yData[idx],x1,x2,y1,y2);
                if (maxDistance < currentDist)
                {
                    maxDistance = currentDist;
                    maxDistanceIdx = idx;
                }
            }
            return maxDistanceIdx;
        }

        /// <summary>
        /// Compute the distance from the data point (x0, y0) to a line 
        /// which is defined by two points (x1, y1) and (x2, y2)
        /// See: e.g. https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        /// </summary>
        private double computeDistance(double x0, double y0, double x1, double x2, double y1, double y2)
        {
            double dist = Math.Abs(x0 * (y2 - y1) - (x2 - x1) * y0 + x2 * y1 - y2 * x1);
            dist /= Math.Sqrt((x2 - x1)*(x2 - x1) + (y2 - y1) * (y2 - y1));
            return dist;
        }
    }
}
