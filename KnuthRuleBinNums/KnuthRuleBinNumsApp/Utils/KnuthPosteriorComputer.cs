using System;
using System.Collections.Generic;
using MathNet.Numerics;

namespace KnuthRuleBinNumsApp.Utils
{
    /// <summary>
    /// This implementation is based on the article:
    ///     Knuth, Kevin H. (2013)
    ///     Optimal data-based binning for histograms
    ///     arXiv preprint physics/0605197v2
    /// </summary>
    public class KnuthPosteriorComputer
    {
        public double evaluate(double binNO, int pointNO, int[] binHefts)
        {
            double result = pointNO * Math.Log(binNO) + SpecialFunctions.GammaLn(binNO / 2.0) - binNO
                * SpecialFunctions.GammaLn(1.0 / 2.0) - SpecialFunctions.GammaLn(pointNO + binNO / 2.0);
            foreach (var binHeft in binHefts)
            {
                result += SpecialFunctions.GammaLn(binHeft + 1.0 / 2.0);
            }
            return result;
        }
    }
}
