using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.SumOfSquares
{
    public class CornacchiaMethod
    {
        private IntTupleEqualityComparer comparer;

        public CornacchiaMethod(IntTupleEqualityComparer comparer)
        {
            this.comparer = comparer;
        }

        public IntTupleEqualityComparer getComparer()
        {
            return comparer;
        }

        /// <summary>
        /// This method return such int tuple where the components of this tuple have ascending order.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IntTuple[] applyCornacchiaMethod(int num)
        {
            IntTuple[] intTuples;
            if (num == 1)
	        {
                intTuples = new IntTuple[]
                { 
                    new IntTuple() { Tuple = new int[] { 1, 0 } }, 
                    new IntTuple() { Tuple = new int[] { 0, 1 } } 
                };
	        } 
            else
	        {
                List<IntTuple> intTuplesOfFactors = listSumOfSquaresOfFactors(num);
                intTuples = applyFibonacciIdentity(intTuplesOfFactors);
	        }
            return intTuples;
        }

        private IntTuple[] applyCornacchiaMethodForPrime(int prime)
        {
            List<IntTuple> container = new List<IntTuple>();
            if (prime == 2)
            {
                container.Add(new IntTuple { Tuple = new int[] { 1, 1 } });
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

        private void innerApplyCornacchiaMethodForPrime(int prime, List<IntTuple> container, int t)
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
                    chooseOrderedIntTuple(container, x, y);
                }
            }
        }

        private List<IntTuple> listSumOfSquaresOfFactors(int num)
        {
            List<IntTuple> intTuplesOfFactors = new List<IntTuple>();
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
                            intTuplesOfFactors.AddRange(applyCornacchiaMethodForPrime(factor));
                        }
                    }
                    // If the prime factor is of the form 4k+3 then its exponent should be even
                    // otherwise the num is not expressible as x^2 + y^2 
                    // due to the corollary of Fermat's theorem on sums of two squares
                    if (factor % 4 == 3)
                    {
                        if (exponent % 2 == 0)
                        {
                            intTuplesOfFactors.Add( 
                                new IntTuple { Tuple = new int[] { 0, (int)Math.Pow(factor, exponent / 2) } }
                                );
                        }
                        else
                        {
                            intTuplesOfFactors = new List<IntTuple>();
                            break;
                        }
                    }
                }
            }
            return intTuplesOfFactors;
        }

        private IntTuple[] applyFibonacciIdentity(List<IntTuple> intTuplesOfFactors)
        {
            HashSet<IntTuple> currentIntTuples = new HashSet<IntTuple>(comparer);
            foreach (var intTuple in intTuplesOfFactors)
            {
                currentIntTuples = applyFibonacciIdentity(currentIntTuples, intTuple);
            }
            IntTuple[] result = new IntTuple[currentIntTuples.Count];
            currentIntTuples.CopyTo(result);
            return result;
        }

        private HashSet<IntTuple> applyFibonacciIdentity(HashSet<IntTuple> currentIntTuples, IntTuple newIntTuple)
        {
            HashSet<IntTuple> result = new HashSet<IntTuple>(comparer);
            if (currentIntTuples.Count == 0)
                result.Add(newIntTuple);
            else
            {
                foreach (var currentIntTuple in currentIntTuples)
                {
                    int n1, n2;
                    int a = currentIntTuple.Tuple[0];
                    int b = currentIntTuple.Tuple[1];
                    int c = newIntTuple.Tuple[0];
                    int d = newIntTuple.Tuple[1];
                    n1 = (int)Math.Abs(a * c - b * d);
                    n2 = (int)Math.Abs(a * d + b * c);
                    chooseOrderedIntTuple(result, n1, n2);
                    n1 = (int)Math.Abs(a * c + b * d);
                    n2 = (int)Math.Abs(a * d - b * c);
                    chooseOrderedIntTuple(result, n1, n2);
                }
            }
            return result;
        }

        private void chooseOrderedIntTuple(ICollection<IntTuple> result, int n1, int n2)
        {
            if (n1 < n2)
            {
                result.Add(new IntTuple { Tuple = new int[] { n1, n2 } });
            }
            else
            {
                result.Add(new IntTuple { Tuple = new int[] { n2, n1 } });
            }
        }
    }
}
