using HierarchicalTilingApp.SumOfSquares;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Transformation
{
    public class ShellBuilder
    {
        private BacktrackingMethod backtrackingMethod;

        public ShellBuilder(BacktrackingMethod backtrackingMethod)
        {
            this.backtrackingMethod = backtrackingMethod;
        }

        public Shell[] createShells(int shellNO, int spaceDimension)
        {
            List<Shell> shells = new List<Shell>();
            // The shellIdx is only a candidate shell index based on Fermat's theorem on sum of two squares
            // and Legendre's three-square theorem
            int shellIdx = 1;
            while (shells.Count < shellNO)
            {
                Shell currentShell = new Shell();
                IntTuple[] currentIntTuples = backtrackingMethod.decomposeByBacktracking(shellIdx, spaceDimension);
                if (currentIntTuples.Length != 0)
                {
                    currentShell.setIntTuplesWithSwapsAndSignChange(currentIntTuples, backtrackingMethod.getComparer());
                    shells.Add(currentShell);
                }
                shellIdx++;
            }
            return shells.ToArray();
        }
    }
}
