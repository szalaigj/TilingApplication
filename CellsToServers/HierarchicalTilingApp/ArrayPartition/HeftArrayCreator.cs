using HierarchicalTilingApp.Transformation;
using System;

namespace HierarchicalTilingApp.ArrayPartition
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
                    innerIndicesArray = transformator.determineNextIndicesArray(array, outerIndicesArray, innerIndicesArray))
                {
                    int[] heftArrayIndeces = transformator.mergeIndicesArrays(spaceDimension, 
                        outerIndicesArray, innerIndicesArray);
                    int binValue = 0;
                    for (int[] indicesArrayOfBin = transformator.determineFirstContainedIndicesArray(heftArrayIndeces);
                        indicesArrayOfBin != null;
                        indicesArrayOfBin = transformator.determineNextContainedIndicesArray(heftArrayIndeces, indicesArrayOfBin))
                    {
                        binValue += (int)array.GetValue(indicesArrayOfBin);
                    }
                    heftArray.SetValue(binValue, heftArrayIndeces);
                }
            }
        }
    }
}
