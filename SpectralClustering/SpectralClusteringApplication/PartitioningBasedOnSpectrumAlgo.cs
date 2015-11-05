using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class PartitioningBasedOnSpectrumAlgo
    {
        public Dictionary<string, List<int>> apply(int K, int nodeNO, Matrix<double> theta)
        {
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            Evd<double> evdOfTheta = theta.Evd();
            int minNecessaryLevel = (int)Math.Log(K, 2.0);
            for (int levelIdx = minNecessaryLevel; (levelIdx < nodeNO * nodeNO) && (dict.Keys.Count < K); levelIdx++)
            {
                dict.Clear();
                for (int idx = 0; idx < nodeNO; idx++)
                {
                    // The following vector should be reversed because the original ordering of eigen values is ascending:
                    Vector<double> currentRow = evdOfTheta.EigenVectors.Row(idx);
                    // the first eigen vector is unnecessary for the following so ...Take(currentRow.Count - 1)...
                    List<double> currentRowList = currentRow.Take(currentRow.Count - 1).Reverse().Take(levelIdx).ToList();
                    string str = "";
                    foreach (var item in currentRowList)
                    {
                        str = determineSign(str, item);
                    }
                    List<int> currentElements;
                    if (dict.TryGetValue(str, out currentElements))
                    {
                        currentElements.Add(idx);
                        dict[str] = currentElements;
                    }
                    else
                    {
                        dict[str] = new List<int>(new int[] { idx });
                    }
                }
            }
            //for (int idx = 0; idx < nodeNO; idx++)
            //{
            //    // The following vector should be reversed because the original ordering of eigen values is ascending:
            //    Vector<double> currentRow = evdOfTheta.EigenVectors.Row(idx);
            //    // the first eigen vector is unnecessary for the following so ...Take(currentRow.Count - 1)...
            //    List<double> currentRowList = currentRow.Take(currentRow.Count - 1).Reverse().Take(K - 1).ToList();
            //    string str = "";
            //    foreach (var item in currentRowList)
            //    {
            //        if (item > 0)
            //        {
            //            str += "+";
            //        }
            //        else
            //        {
            //            str += "-";
            //        }
            //    }
            //    List<int> currentElements;
            //    if (dict.TryGetValue(str, out currentElements))
            //    {
            //        currentElements.Add(idx);
            //        dict[str] = currentElements;
            //    }
            //    else
            //    {
            //        dict[str] = new List<int>(new int[] { idx });
            //    }
            //}
            return dict;
        }

        private string determineSign(string str, double item)
        {
            if (item > 0)
            {
                str += "+";
            }
            else
            {
                str += "-";
            }
            return str;
        }
    }
}
