using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.SumOfSquares
{
    /// <summary>
    /// This class is based on Peter Schorn's (Institut für Theoretische Informatik, ETH Zürich) technical report
    /// links:  ftp://ftp.inf.ethz.ch/pub/software/xyz/papers/SquareDecomposition.ps
    ///         http://schorn.ch/lagrange.html
    /// </summary>
    public class BacktrackingMethod
    {
        private CornacchiaMethod cornacchiaMethod;

        public BacktrackingMethod(CornacchiaMethod cornacchiaMethod)
        {
            this.cornacchiaMethod = cornacchiaMethod;
        }

        public IntTupleEqualityComparer getComparer()
        {
            return cornacchiaMethod.getComparer();
        }

        /// <summary>
        /// This method return such int tuple where the components of this tuple have ascending order.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="squaresNO"></param>
        /// <returns></returns>
        public IntTuple[] decomposeByBacktracking(int num, int squaresNO)
        {
            IntTuple[] decompositions;
            if (squaresNO == 1)
            {
                int floorOfSquareOfN = (int)Math.Floor(Math.Sqrt(num));
                if (floorOfSquareOfN * floorOfSquareOfN == num)
                {
                    decompositions = new IntTuple[1];
                    decompositions[0] = new IntTuple() { Tuple = new int[] {floorOfSquareOfN} };
                }
                else
                    decompositions = new IntTuple[0];
            }
            else if (squaresNO >= 2)
            {
                decompositions = innerDecomposeByBacktracking(num, squaresNO);
            }
            else
                decompositions = new IntTuple[0];
            return decompositions;
        }

        private IntTuple[] innerDecomposeByBacktracking(int num, int squaresNO)
        {
            IntTuple[] intTuples;
            if (num == 0)
            {
                int[] tuple = new int[squaresNO];
                for (int idx = 0; idx < squaresNO; idx++)
                {
                    tuple[idx] = 0;
                }
                intTuples = new IntTuple[1] { new IntTuple() { Tuple = tuple } };
            } 
            else if (squaresNO == 2)
                intTuples = cornacchiaMethod.applyCornacchiaMethod(num);
            else
            {
                intTuples = innerDecomposeByBacktrackingWhenSquaresNOIsGreaterThanTwo(num, squaresNO);
            }
            return intTuples;
        }

        private IntTuple[] innerDecomposeByBacktrackingWhenSquaresNOIsGreaterThanTwo(int num, int squaresNO)
        {
            IntTuple[] intTuples;
            List<IntTuple> tempIntTuples = new List<IntTuple>();
            int floorOfSquareOfN = (int)Math.Floor(Math.Sqrt(num));
            // This method return such int tuple where the components of this tuple has ascending order.
            // Thus num-lastTerm*lastTerm <= (squaresNO-1)*(lastTerm*lastTerm) --> sqrt(num/squaresNO) <= lastTerm.
            // (E.g. num = x^2 + y^2 + z^2 (x<=y<=z), num - z^2 = x^2 + y^2 <= 2 * z^2 ...)
            int lowerLimit = (int)(floorOfSquareOfN / Math.Sqrt(squaresNO));
            for (int lastTerm = floorOfSquareOfN; lastTerm >= lowerLimit; lastTerm--)
            {
                IntTuple[] subIntTuples = innerDecomposeByBacktracking(num - lastTerm * lastTerm, squaresNO - 1);
                foreach (var subIntTuple in subIntTuples)
                {
                    // the components of this tuple must have ascending order
                    if (subIntTuple.Tuple[squaresNO - 2] <= lastTerm)
                    {
                        int[] tuple = new int[squaresNO];
                        subIntTuple.Tuple.CopyTo(tuple, 0);
                        tuple[squaresNO - 1] = lastTerm;
                        tempIntTuples.Add(new IntTuple() { Tuple = tuple });
                    }
                }
            }
            intTuples = tempIntTuples.ToArray();
            return intTuples;
        }
    }
}
