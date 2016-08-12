#include "Transformator.hpp"

namespace Transformation
{
	int Transformator::calculateCellIdx(int arrayRank, int arrayResolution, int * indicesArray)
	{
		int cellIdx = 0;
		for (int dimIdx = 0; dimIdx < arrayRank; dimIdx++)
		{
			cellIdx += indicesArray[dimIdx] * (int)pow((double)arrayResolution, dimIdx);
		}
		return cellIdx;
	}

	int * Transformator::copyIndicesArray(int spaceDimension, int * inputIndicesArray)
	{
		int * result = new int[spaceDimension];
		for (int idx = 0; idx < spaceDimension; idx++)
		{
			result[idx] = inputIndicesArray[idx];
		}
		return result;
	}

	int * Transformator::determineNextIndicesArray(int spaceDimension, int histogramResolution,
		int * previousIndicesArray)
	{
		int * nextIndicesArray = copyIndicesArray(spaceDimension, previousIndicesArray);
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			nextIndicesArray[dimIdx]++;
			if (nextIndicesArray[dimIdx] <= histogramResolution)
				return nextIndicesArray;
			nextIndicesArray[dimIdx] = 0;
		}
		return nullptr;
	}

	int * Transformator::determineNextIndicesArray(int spaceDimension, int histogramResolution,
		int * lowerBoundArray, int * previousIndicesArray)
	{
		int * nextIndicesArray = copyIndicesArray(spaceDimension, previousIndicesArray);
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			nextIndicesArray[dimIdx]++;
			if (nextIndicesArray[dimIdx] <= histogramResolution)
				return nextIndicesArray;
			nextIndicesArray[dimIdx] = lowerBoundArray[dimIdx];
		}
		return nullptr;
	}

	int * Transformator::determineFirstContainedIndicesArray(int spaceDimension,
		int * indicesArrayOfRegion)
	{
		int * firstIndicesArray = new int[spaceDimension];
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			firstIndicesArray[dimIdx] = indicesArrayOfRegion[2 * dimIdx];
		}
		return firstIndicesArray;
	}

	int * Transformator::determineNextContainedIndicesArray(int spaceDimension,
		int * indicesArrayOfRegion,	int * previousIndicesArray)
	{
		int * nextIndicesArray = copyIndicesArray(spaceDimension, previousIndicesArray);
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			nextIndicesArray[dimIdx]++;
			int lowerBoundForCurrentDim = indicesArrayOfRegion[2 * dimIdx];
			int upperBoundForCurrentDim = indicesArrayOfRegion[2 * dimIdx + 1];
			if (nextIndicesArray[dimIdx] <= upperBoundForCurrentDim)
				return nextIndicesArray;
			nextIndicesArray[dimIdx] = lowerBoundForCurrentDim;
		}
		return nullptr;
	}

	int * Transformator::mergeIndicesArrays(int spaceDimension, int * outerIndicesArray,
		int * innerIndicesArray)
	{
		int * mergedArrayIndices = new int[2 * spaceDimension];
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			mergedArrayIndices[2 * dimIdx] = outerIndicesArray[dimIdx];
			mergedArrayIndices[2 * dimIdx + 1] = innerIndicesArray[dimIdx];
		}
		return mergedArrayIndices;
	}

	int Transformator::determineMaxRange(int spaceDimension, int histogramResolution)
	{
		double temp = pow((double)histogramResolution, (double)spaceDimension);
		temp /= (factorial(spaceDimension) * doubleFactorial(spaceDimension));
		temp *= pow(M_PI * spaceDimension / 2.0, spaceDimension / 2.0);
		int result = (int)ceil(temp);
		return result;
	}

	int Transformator::factorial(int input)
    { 
        int result = 1;
        for (int idx = 1; idx <= input; idx++)
        {
           result *= idx;
        }
        return result;
    }

	int Transformator::doubleFactorial(int input)
    {
       int result;
       if ((input % 2) == 0)
       {
           result = 1;
           for (int idx = 1; idx <= input/2; idx++)
           {
               result *= 2 * idx;
           }
       }
       else
       {
           result = 1;
           for (int idx = 1; idx <= (input + 1) / 2; idx++)
           {
               result *= (2 * idx - 1);
           }
       }
       return result;
   }
}