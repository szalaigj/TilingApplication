using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp.SumOfSquares
{
    public class Shell
    {
        private IntPair[] intPairs;

        public void setIntPairsWithSwapsAndSignChange(IntPair[] inputIntPairs, IntPairEqualityComparer comparer)
        {
            HashSet<IntPair> container = new HashSet<IntPair>(comparer);
            foreach (var intPair in inputIntPairs)
            {
                container.Add(intPair);
                addOppositeElements(container, intPair);
                addSwapElements(container);
            }
            this.intPairs = new IntPair[container.Count];
            container.CopyTo(this.intPairs);
        }

        private void addOppositeElements(HashSet<IntPair> tempIntPairs, IntPair intPair)
        {
            int x = intPair.X;
            int y = intPair.Y;
            tempIntPairs.Add(new IntPair { X = -x, Y = y });
            tempIntPairs.Add(new IntPair { X = x, Y = -y });
            tempIntPairs.Add(new IntPair { X = -x, Y = -y });
        }

        private void addSwapElements(HashSet<IntPair> tempIntPairs)
        {
            List<IntPair> swapElements = new List<IntPair>();
            foreach (var tempIntPair in tempIntPairs)
            {
                swapElements.Add(new IntPair { X = tempIntPair.Y, Y = tempIntPair.X });
            }
            tempIntPairs.UnionWith(swapElements);
        }

        public IntPair[] getIntPairs()
        {
            return intPairs;
        }
    }
}
