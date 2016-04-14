using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.SumOfSquares
{
    public class CornacchiaMethod
    {
        public IntPair[] applyCornacchiaMethod(int num)
        {
            List<IntPair> intPairsOfFactors = listSumOfSquaresOfFactors(num);
            return applyFibonacciIdentity(intPairsOfFactors);
        }

        private IntPair[] applyCornacchiaMethodForPrime(int prime)
        {
            List<IntPair> container = new List<IntPair>();
            if (prime == 2)
            {
                container.Add(new IntPair { X = 1, Y = 1 });
            }
            else
            {
                for (int t = 1; t <= prime / 2; t++)
                {
                    innerApplyCornacchiaMethodForPrime(prime, container, t);
                }
            }
            return container.ToArray();
        }

        private void innerApplyCornacchiaMethodForPrime(int prime, List<IntPair> container, int t)
        {
            int x, y;
            if ((t * t) % prime == prime - 1)
            {
                int r1 = prime;
                int r2 = t;
                int tmp;
                while (r2 * r2 >= prime)
                {
                    tmp = r2;
                    r2 = (r1 % r2);
                    r1 = tmp;
                }
                if (r1 * r1 > prime)
                {
                    x = r2;
                    y = r1 % r2;
                    chooseOrderedIntPair(container, x, y);
                }
            }
        }

        private List<IntPair> listSumOfSquaresOfFactors(int num)
        {
            List<IntPair> intPairsOfFactors = new List<IntPair>();
            for (int factor = 2; num > 1; factor++)
            {
                if (num % factor == 0)
                {
                    int exponent = 0;
                    while (num % factor == 0)
                    {
                        num /= factor;
                        exponent++;
                        if ((factor % 4 == 1) || (factor == 2))
                        {
                            intPairsOfFactors.AddRange(applyCornacchiaMethodForPrime(factor));
                        }
                    }
                    // If the prime factor is of the form 4k+3 then its exponent should be even
                    // otherwise the num is not expressible as x^2 + y^2 
                    // due to the corollary of Fermat's theorem on sums of two squares
                    if (factor % 4 == 3)
                    {
                        if (exponent % 2 == 0)
                        {
                            intPairsOfFactors.Add(new IntPair { X = 0, Y = (int)Math.Pow(factor, exponent/2) });
                        }
                        else
                        {
                            intPairsOfFactors = new List<IntPair>();
                            break;
                        }
                    }
                }
            }
            return intPairsOfFactors;
        }

        private IntPair[] applyFibonacciIdentity(List<IntPair> intPairsOfFactors)
        {
            IntPairEqualityComparer comparer = new IntPairEqualityComparer();
            HashSet<IntPair> currentIntPairs = new HashSet<IntPair>(comparer);
            foreach (var intPair in intPairsOfFactors)
            {
                currentIntPairs = applyFibonacciIdentity(currentIntPairs, intPair, comparer);
            }
            IntPair[] result = new IntPair[currentIntPairs.Count];
            currentIntPairs.CopyTo(result);
            return result;
        }

        private HashSet<IntPair> applyFibonacciIdentity(HashSet<IntPair> currentIntPairs, IntPair newIntPair, 
            IntPairEqualityComparer comparer)
        {
            HashSet<IntPair> result = new HashSet<IntPair>(comparer);
            if (currentIntPairs.Count == 0)
                result.Add(newIntPair);
            else
            {
                foreach (var currentIntPair in currentIntPairs)
                {
                    int n1, n2;
                    n1 = (int)Math.Abs(currentIntPair.X * newIntPair.X - currentIntPair.Y * newIntPair.Y);
                    n2 = (int)Math.Abs(currentIntPair.X * newIntPair.Y + currentIntPair.Y * newIntPair.X);
                    chooseOrderedIntPair(result, n1, n2);
                    n1 = (int)Math.Abs(currentIntPair.X * newIntPair.X + currentIntPair.Y * newIntPair.Y);
                    n2 = (int)Math.Abs(currentIntPair.X * newIntPair.Y - currentIntPair.Y * newIntPair.X);
                    chooseOrderedIntPair(result, n1, n2);
                }
            }
            return result;
        }

        private void chooseOrderedIntPair(ICollection<IntPair> result, int n1, int n2)
        {
            if (n1 < n2)
            {
                result.Add(new IntPair { X = n1, Y = n2 });
            }
            else
            {
                result.Add(new IntPair { X = n2, Y = n1 });
            }
        }
    }
}
