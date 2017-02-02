using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.SumOfSquares
{
    public class IntPair
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool determineIdxArrayRelativeTo(int histogramResolution, int[] inputIndicesArray,
            out int[] outputIndicesArray)
        {
            int outputX = X + inputIndicesArray[0];
            int outputY = Y + inputIndicesArray[1];
            outputIndicesArray = new int[] { outputX, outputY };
            return isValidIdxArray(histogramResolution, outputX, outputY);
        }

        private bool isValidIdxArray(int histogramResolution, int outputX, int outputY)
        {
            if ((outputX >= 0) && (outputX < histogramResolution) && (outputY >= 0) && (outputY < histogramResolution))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class IntPairEqualityComparer : IEqualityComparer<IntPair>
    {
        public bool Equals(IntPair ip1, IntPair ip2)
        {
            if (ip1 == null && ip2 == null)
                return true;
            else if (ip1 == null || ip2 == null)
                return false;
            else if (ip1.X == ip2.X && ip1.Y == ip2.Y)
                return true;
            else
                return false;
        }

        public int GetHashCode(IntPair ip)
        {
            int hCode = ip.X ^ ip.Y;
            return hCode.GetHashCode();
        }
    }
}
