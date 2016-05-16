using SpectralClusteringApplication.SumOfSquares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class IndexTransformator
    {
        public void transformCellIdxToIndicesArray(int histogramResolution, int[] indicesArray, int cellIdx)
        {
            indicesArray[0] =
                cellIdx / (int)Math.Pow(histogramResolution, indicesArray.Length - 1);
            for (int coordIdx = 1; coordIdx < indicesArray.Length; coordIdx++)
            {
                indicesArray[coordIdx] = cellIdx;
                for (int subCoordIdx = coordIdx - 1; subCoordIdx >= 0; subCoordIdx--)
                {
                    indicesArray[coordIdx] -=
                        (int)Math.Pow(histogramResolution, indicesArray.Length - (subCoordIdx + 1))
                        * indicesArray[subCoordIdx];
                }
                indicesArray[coordIdx] =
                    indicesArray[coordIdx] /
                    (int)Math.Pow(histogramResolution, indicesArray.Length - (coordIdx + 1));
            }
        }

        public int transformIndicesArrayToCellIdx(int histogramResolution, int[] indicesArray)
        {
            int cellIdx = 0;
            for (int coordIdx = 0; coordIdx < indicesArray.Length; coordIdx++)
            {
                cellIdx += (int)Math.Pow(histogramResolution, indicesArray.Length - (coordIdx + 1)) 
                    * indicesArray[coordIdx];
            }
            return cellIdx;
        }

        public Dictionary<int, List<int[]>> convertIntPairsOfShellsToListOfIdxArrays(int histogramResolution,
            int[] inputIndicesArray, Shell[] shells)
        {
            Dictionary<int, List<int[]>> result = new Dictionary<int, List<int[]>>();
            int shellIdx = 0;
            foreach (var shell in shells)
            {
                List<int[]> indicesArraysInCurrentShell = new List<int[]>();
                IntTuple[] intTuples = shell.getIntTuples();
                foreach (var intTuple in intTuples)
                {
                    int[] currentIndicesArray;
                    if (intTuple.determineIdxArrayRelativeTo(histogramResolution, inputIndicesArray,
                        out currentIndicesArray))
                    {
                        indicesArraysInCurrentShell.Add(currentIndicesArray);
                    }
                }
                result.Add(shellIdx, indicesArraysInCurrentShell);
                shellIdx++;
            }
            return result;
        }
    }
}
