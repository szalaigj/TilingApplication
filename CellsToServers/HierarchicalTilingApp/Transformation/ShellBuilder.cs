using HierarchicalTilingApp.SumOfSquares;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Transformation
{
    public class ShellBuilder
    {
        private CornacchiaMethod cornacchiaMethod;

        public ShellBuilder(CornacchiaMethod cornacchiaMethod)
        {
            this.cornacchiaMethod = cornacchiaMethod;
        }

        public Shell[] createShellsInTwoDimSpace(int shellNO)
        {
            List<Shell> shells = new List<Shell>();
            // The shellIdx is only a candidate shell index because of the primes of the form 4k+3
            int shellIdx = 1;
            while (shells.Count < shellNO)
            {
                Shell currentShell = new Shell();
                IntPair[] currentIntPairs = cornacchiaMethod.applyCornacchiaMethod(shellIdx);
                if (currentIntPairs.Length != 0)
                {
                    currentShell.setIntPairsWithSwapsAndSignChange(currentIntPairs, cornacchiaMethod.getComparer());
                    shells.Add(currentShell);
                }
                shellIdx++;
            }
            return shells.ToArray();
        }
    }
}
