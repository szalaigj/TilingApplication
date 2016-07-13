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

        /// <summary>
        /// This is an another approach. However, this is slower than the other.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<IntTuple>[] applyCornacchiaMethodForRange(int limit)
        {
            List<IntTuple>[] dictOfSolutions = new List<IntTuple>[limit + 1];
            for (int num = 2; num <= limit; num++)
            {
                IntTuple[] currentTuples = applyCornacchiaMethodPrimitiveSolution(num);
                if (dictOfSolutions[num] != null)
                    dictOfSolutions[num].AddRange(currentTuples);
                else
                {
                    dictOfSolutions[num] = new List<IntTuple>(currentTuples);
                }
                addNumSquareForLaterNum(limit, dictOfSolutions, num);
                addNumMultipleForLaterNum(limit, dictOfSolutions, num, currentTuples);
            }
            return dictOfSolutions;
        }

        private void addNumSquareForLaterNum(int limit, List<IntTuple>[] dictOfSolutions, int num)
        { 
            if (num <= Math.Sqrt(limit))
            {
                int laterNum = num * num;
                IntTuple tupleForLaterNum = new IntTuple()
                {
                    Tuple = new int[] { 0, num }
                };
                updateTuplesOfLaterNum(dictOfSolutions, laterNum, tupleForLaterNum);
            }
        }

        private void addNumMultipleForLaterNum(int limit, List<IntTuple>[] dictOfSolutions, int num, 
            IntTuple[] currentTuples)
        {
            foreach (var intTuple in currentTuples)
            {
                int k = 2;
                while (num <= (double)limit / (double)(k * k))
                {
                    int laterNum = num * k * k;
                    IntTuple tupleForLaterNum = new IntTuple()
                    {
                        Tuple = new int[] { intTuple.Tuple[0] * k, intTuple.Tuple[1] * k }
                    };
                    updateTuplesOfLaterNum(dictOfSolutions, laterNum, tupleForLaterNum);
                    k++;
                    laterNum = num * k * k;
                }
            }
        }

        private void updateTuplesOfLaterNum(List<IntTuple>[] dictOfSolutions, int laterNum,
            IntTuple tupleForLaterNum)
        {
            if (dictOfSolutions[laterNum] != null)
                dictOfSolutions[laterNum].Add(tupleForLaterNum);
            else
            {
                dictOfSolutions[laterNum] = new List<IntTuple>();
                dictOfSolutions[laterNum].Add(tupleForLaterNum);
            }
        }

        private IntTuple[] applyCornacchiaMethodPrimitiveSolution(int num)
        {
            List<IntTuple> container = new List<IntTuple>();
            if (num == 2)
            {
                container.Add(new IntTuple { Tuple = new int[] { 1, 1 } });
            }
            else
            {
                for (int t = 1; t <= num / 2; t++)
                {
                    if (t * t < int.MaxValue)
                    {
                        innerApplyCornacchiaMethodPrimitiveSolution(num, container, t);
                    }
                    else
                    {
                        throw new NotSupportedException("Number " + num + " is too large.");
                    }
                }
            }
            return container.ToArray();
        }

        private void innerApplyCornacchiaMethodPrimitiveSolution(int num, List<IntTuple> container, int t)
        {
            int x, y;
            if (t * t % num == num - 1)
            {
                int r1 = num;
                int r2 = t;
                int tmp;
                while (r2 * r2 >= num)
                {
                    tmp = r2;
                    r2 = (r1 % r2);
                    r1 = tmp;
                }
                if (r1 * r1 > num)
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
                            intTuplesOfFactors.AddRange(applyCornacchiaMethodPrimitiveSolution(factor));
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
