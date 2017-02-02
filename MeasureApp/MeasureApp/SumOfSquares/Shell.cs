using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.SumOfSquares
{
    public class Shell
    {
        private IntTuple[] intTuples;

        public IntTuple[] getIntTuples()
        {
            return intTuples;
        }

        public void setIntTuplesWithSwapsAndSignChange(IntTuple[] inputIntTuples, IntTupleEqualityComparer comparer)
        {
            HashSet<IntTuple> container = new HashSet<IntTuple>(comparer);
            foreach (var intTuple in inputIntTuples)
            {
                container.Add(intTuple);
                addOppositeElements(container, intTuple);
                addSwapElements(container);
            }
            this.intTuples = new IntTuple[container.Count];
            container.CopyTo(this.intTuples);
        }

        private void addOppositeElements(HashSet<IntTuple> tempIntTuples, IntTuple intTuple)
        {
            int dim = intTuple.Tuple.Length;
            // The following loop iterates over the all sign-combinations of the original tuple components.
            // E.g. for (x,y) these are (-x,y),(x,-y),(-x,-y)
            for (int signDealingOut = 1; signDealingOut < Math.Pow(2, dim); signDealingOut++)
            {
                int[] currentTuple = new int[dim];
                for (int idx = 0; idx < dim; idx++)
                {
                    int bit = (signDealingOut >> idx) & 1;
                    if (bit == 0) // the bit 0 symbolizes '+'
                        currentTuple[idx] = intTuple.Tuple[idx];
                    else // the bit 0 symbolizes '-'
                        currentTuple[idx] = -intTuple.Tuple[idx];
                }
                tempIntTuples.Add(new IntTuple() { Tuple = currentTuple });
            }
        }

        private void addSwapElements(HashSet<IntTuple> tempIntTuples)
        {
            List<IntTuple> swapElements = new List<IntTuple>();
            foreach (var tempIntTuple in tempIntTuples)
            {
                List<int[]> perms = permutateWithoutInitial(tempIntTuple.Tuple);
                foreach (var perm in perms)
                {
                    swapElements.Add(new IntTuple { Tuple = perm });
                }
            }
            tempIntTuples.UnionWith(swapElements);
        }

        private List<int[]> permutateWithoutInitial(int[] tuple)
        {
            List<int> components = new List<int>(tuple);
            List<int[]> perms = innerPermutate(components);
            perms.RemoveAt(0);
            return perms;
        }

        private List<int[]> innerPermutate(List<int> components)
        {
            List<int[]> result = new List<int[]>();
            if (components.Count == 2)
            {
                int[] perm1 = new int[] { components[0], components[1] };
                int[] perm2 = new int[] { components[1], components[0] };
                result.Add(perm1);
                result.Add(perm2);
            }
            else if (components.Count > 2)
            {
                for (int idx = 0; idx < components.Count; idx++)
                {
                    int currentComponent = components[idx];
                    List<int> componentsWithoutCurrent = new List<int>(components);
                    componentsWithoutCurrent.RemoveAt(idx);
                    List<int[]> perms = innerPermutate(componentsWithoutCurrent);
                    foreach (var perm in perms)
                    {
                        int[] extendedPerm = new int[components.Count];
                        extendedPerm[0] = currentComponent;
                        perm.CopyTo(extendedPerm, 1);
                        result.Add(extendedPerm);
                    }
                }
            }
            return result;
        }
    }
}
