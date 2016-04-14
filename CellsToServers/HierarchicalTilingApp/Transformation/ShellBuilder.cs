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

        /// <summary>
        /// This method returns shell array with maximum maxShellNO element number.
        /// Exact element number depends on how many primes of the form 4k+3 there are until maxShellNO.
        /// </summary>
        /// <param name="maxShellNO"></param>
        /// <returns></returns>
        public Shell[] createShellsInTwoDimSpace(int maxShellNO)
        {
            List<Shell> shells = new List<Shell>();
            // The shellIdx is only a candidate shell index because of the primes of the form 4k+3
            for (int shellIdx = 1; shellIdx <= maxShellNO; shellIdx++)
            {
                Shell currentShell = new Shell();
                IntPair[] currentIntPairs = cornacchiaMethod.applyCornacchiaMethod(shellIdx);
                if (currentIntPairs.Length != 0)
                {
                    currentShell.setIntPairsWithSwapsAndSignChange(currentIntPairs, cornacchiaMethod.getComparer());
                    shells.Add(currentShell);
                }
            }
            return shells.ToArray();
        }
    }
}
