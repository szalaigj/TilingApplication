using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnuthRuleBinNumsApp.Utils
{
    /// <summary>
    /// This class is based on the following article:
    /// 
    /// Stan Salvador and Philip Chan (2004)
    /// Determining the number of clusters/segments in hierarchical clustering/segmentation algorithms
    /// Tools with Artificial Intelligence, ICTAI 2004. 16th IEEE International Conference on (pp. 576-584)
    /// </summary>
    public class LMethod
    {
        public int iterativeRefinementOfTheKnee(double[] xData, double[] yData)
        {
            int cutoff = xData.Length;
            int lastKnee = xData.Length;
            int currentKnee = xData.Length;
            do
            {
                lastKnee = currentKnee;
                currentKnee = findBestKneePoint(xData, yData, cutoff);
                cutoff = 2 * currentKnee;
            } while (currentKnee < lastKnee);
            return currentKnee;
        }

        public int findBestKneePoint(double[] xData, double[] yData, int cutoff)
        {
            int result = 2;
            double maxRMSE = double.MaxValue;
            double[] firstLineXPoints, secondLineXPoints, firstLineYPoints, secondLineYPoints;
            // each line must contain at least two points so the initial index of knee point is 2 
            // and the last one is less than cutoff - 2
            for (int kneePointIdx = 2; kneePointIdx < cutoff - 2; kneePointIdx++)
            {
                firstLineXPoints = new double[kneePointIdx];
                firstLineYPoints = new double[kneePointIdx];
                secondLineXPoints = new double[cutoff - kneePointIdx];
                secondLineYPoints = new double[cutoff - kneePointIdx];
                Array.Copy(xData, firstLineXPoints, kneePointIdx);
                Array.Copy(yData, firstLineYPoints, kneePointIdx);
                Array.Copy(xData, kneePointIdx, secondLineXPoints, 0, cutoff - kneePointIdx);
                Array.Copy(yData, kneePointIdx, secondLineYPoints, 0, cutoff - kneePointIdx);
                double rmse1 = fitLine(firstLineXPoints, firstLineYPoints);
                double rmse2 = fitLine(secondLineXPoints, secondLineYPoints);
                double currentRMSE = ((double)kneePointIdx / (double)cutoff) * rmse1
                    + ((double)(cutoff - kneePointIdx) / (double)cutoff) * rmse2;
                if (currentRMSE < maxRMSE)
                {
                    maxRMSE = currentRMSE;
                    result = kneePointIdx;
                }
            }
            return result;
        }

        private double fitLine(double[] xPoints, double[] yPoints)
        {
            Tuple<double, double> p = Fit.Line(xPoints, yPoints);
            // y : x -> a + b*x
            double a = p.Item1;
            double b = p.Item2;
            double[] yApprox = xPoints.Select(x => a + b * x).ToArray();
            double rootMeanSquaredError = Math.Sqrt(Distance.MSE(yApprox, yPoints));
            return rootMeanSquaredError;
        }
    }
}
