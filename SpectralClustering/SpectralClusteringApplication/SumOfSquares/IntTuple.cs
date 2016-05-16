using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication.SumOfSquares
{
    public class IntTuple
    {
        public int[] Tuple { get; set; }

        public bool determineIdxArrayRelativeTo(int histogramResolution, int[] inputIndicesArray,
            out int[] outputIndicesArray)
        {
            outputIndicesArray = new int[inputIndicesArray.Length];
            for (int idx = 0; idx < inputIndicesArray.Length; idx++)
            {
                outputIndicesArray[idx] = Tuple[idx] + inputIndicesArray[idx];
            }
            return isValidIdxArray(histogramResolution, outputIndicesArray);
        }

        private bool isValidIdxArray(int histogramResolution, int[] outputIndicesArray)
        {
            bool isValid = true;
            foreach (var outComponent in outputIndicesArray)
            {
                if (!((outComponent >= 0) && (outComponent < histogramResolution)))
                {
                    isValid = false;
                }
            }
            return isValid;
        }
    }
    public class IntTupleEqualityComparer : IEqualityComparer<IntTuple>
    {
        public bool Equals(IntTuple it1, IntTuple it2)
        {
            bool isEqual = false;
            if (it1 == null && it2 == null)
                isEqual = true;
            else if ((it1 != null) && (it2 != null) && (it1.Tuple.Length == it2.Tuple.Length))
            {
                isEqual = true;
                for (int idx = 0; idx < it1.Tuple.Length; idx++)
                {
                    if (it1.Tuple[idx] != it2.Tuple[idx])
                    {
                        isEqual = false;
                    }
                }
            }
            return isEqual;
        }

        public int GetHashCode(IntTuple it)
        {
            int hCode = it.Tuple[0];
            for (int idx = 1; idx < it.Tuple.Length; idx++)
            {
                hCode ^= it.Tuple[idx];
            }
            return hCode.GetHashCode();
        }
    }
}
