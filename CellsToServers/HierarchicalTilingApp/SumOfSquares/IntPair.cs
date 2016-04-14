using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp.SumOfSquares
{
    public class IntPair
    {
        public int X { get; set; }
        public int Y { get; set; }
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
