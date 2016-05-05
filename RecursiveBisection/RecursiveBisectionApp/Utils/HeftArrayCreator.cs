using System;
using System.Collections.Generic;

namespace RecursiveBisectionApp.Utils
{
    public class HeftArrayCreator
    {
        private Transformator transformator;

        public HeftArrayCreator(Transformator transformator)
        {
            this.transformator = transformator;
        }
        public Array createHeftArray(int spaceDimension, int histogramResolution, Array array)
        {
            int[] lengthsHeftArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < 2 * spaceDimension; idx++)
            {
                lengthsHeftArray[idx] = histogramResolution;
            }
            Array heftArray = Array.CreateInstance(typeof(int), lengthsHeftArray);
            fillHeftArray(spaceDimension, array, heftArray);
            return heftArray;
        }

        private void fillHeftArray(int spaceDimension, Array array, Array heftArray)
        {
            for (int[] outerIndicesArray = transformator.determineFirstIndicesArray(array);
                    outerIndicesArray != null;
                    outerIndicesArray = transformator.determineNextIndicesArray(array, outerIndicesArray))
            {
                for (int[] innerIndicesArray = outerIndicesArray;
                    innerIndicesArray != null;
                    innerIndicesArray = transformator.determineNextIndicesArray(array, innerIndicesArray))
                {
                    int[] heftArrayIndeces = transformator.mergeIndicesArrays(spaceDimension,
                        outerIndicesArray, innerIndicesArray);
                    int binValue = 0;
                    for (int[] indicesArrayOfBin = transformator.determineFirstIndicesArray(heftArrayIndeces);
                        indicesArrayOfBin != null;
                        indicesArrayOfBin = transformator.determineNextIndicesArray(heftArrayIndeces, indicesArrayOfBin))
                    {
                        binValue += (int)array.GetValue(indicesArrayOfBin);
                    }
                    heftArray.SetValue(binValue, heftArrayIndeces);
                }
            }
        }
    }
}
