using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp.SumOfSquares
{
    public class BacktrackingMethod
    {
        /// <summary>
        /// This method return such int tuple where the components of this tuple have ascending order.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="squaresNO"></param>
        /// <returns></returns>
        public IntTuple[] decomposeByBacktracking(int num, int squaresNO)
        {
            List<IntTuple> decompositions = new List<IntTuple>();
            List<int> components = new List<int>();
            innerDecomposeByBacktracking(num, squaresNO, components);
            // The components contains decomposition in bulk style
            // therefore it should be partitioned by squaresNO in the followings
            int idx = 0;
            int[] tmpDecomposition = new int[squaresNO];
            foreach (var component in components)
            {
                tmpDecomposition[idx] = component;
                if (idx == squaresNO - 1)
                {
                    decompositions.Add(new IntTuple() { Tuple = tmpDecomposition });
                    tmpDecomposition = new int[squaresNO];
                    idx = 0;
                }
                else
                    idx++;
            }
            return decompositions.ToArray();
        }

        private bool innerDecomposeByBacktracking(int num, int squaresNO, List<int> components)
        {
            bool result = true;
            int floorOfSquareOfN = (int)Math.Floor(Math.Sqrt(num));
            if (floorOfSquareOfN * floorOfSquareOfN == num)
                components.Add(floorOfSquareOfN);
            else if (squaresNO == 1)
                result = false;
            else
            {
                // This method return such int tuple where the components of this tuple have ascending order.
                // Thus num - i * i <= i * i --> sqrt(num/2) <= i.
                for (int i = floorOfSquareOfN; i >= (double)floorOfSquareOfN / Math.Sqrt(2.0); i--)
                {
                    bool subResult = innerDecomposeByBacktracking(num - i * i, squaresNO - 1, components);
                    if (subResult)
                    {
                        components.Add(i);
                    }
                }
                result = false;
            }
            return result;
        }
    }
}
