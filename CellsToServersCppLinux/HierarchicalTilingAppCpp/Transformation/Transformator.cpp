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

	// For OpenMP solution 
	bool Transformator::calculateCellIdx(int arrayRank, int arrayResolution,
		int outerCellIdx, int innerCellIdx, int& cellIdx, int *& mergedArrayIndices)
	{
		bool validArrayIndices = true;
		cellIdx = 0;
		mergedArrayIndices = new int[2 * arrayRank];
		for (int dimIdx = arrayRank - 1; dimIdx >= 0; dimIdx--)
		{
			int outerArrayResPow = (int)pow((double)arrayResolution, dimIdx);
			int outerCoordVal = outerCellIdx / outerArrayResPow;
			int innerArrayResPow = (int)pow((double)arrayResolution, dimIdx);
			int innerCoordVal = innerCellIdx / innerArrayResPow;
			if (outerCoordVal <= innerCoordVal)
			{
				cellIdx += outerCoordVal * (int)pow((double)arrayResolution, 2 * dimIdx);
				cellIdx += innerCoordVal * (int)pow((double)arrayResolution, 2 * dimIdx + 1);
				mergedArrayIndices[2 * dimIdx] = outerCoordVal;
				mergedArrayIndices[2 * dimIdx + 1] = innerCoordVal;
				outerCellIdx -= outerCoordVal * outerArrayResPow;
				innerCellIdx -= innerCoordVal * innerArrayResPow;
			}
			else
			{
				validArrayIndices = false;
				delete [] mergedArrayIndices;
				break;
			}
		}
		return validArrayIndices;
	}

	int Transformator::calculateExtendedCellIdx(int arrayRank, int serverNO, int arrayResolution,
		int * extendedIndicesArray)
	{
		int extendedCellIdx = extendedIndicesArray[0];
		for (int dimIdx = 1; dimIdx < arrayRank; dimIdx++)
		{
			extendedCellIdx += extendedIndicesArray[dimIdx] * serverNO
				* (int)pow((double)arrayResolution, dimIdx - 1);
		}
		return extendedCellIdx;
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

	int * Transformator::extendIndicesArray(int spaceDimension, int * inputIndicesArray)
	{
		int * result = new int[2 * spaceDimension + 1];
		for (int idx = 0; idx < 2 * spaceDimension; idx++)
		{
			result[idx + 1] = inputIndicesArray[idx];
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
			if (nextIndicesArray[dimIdx] < histogramResolution)
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
			if (nextIndicesArray[dimIdx] < histogramResolution)
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

	int * Transformator::mergeIndicesArrays(int spaceDimension, int splitNO, int * outerIndicesArray,
			int * innerIndicesArray)
	{
		int * mergedArrayIndices = new int[2 * spaceDimension + 1];
		mergedArrayIndices[0] = splitNO;
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			mergedArrayIndices[2 * dimIdx + 1] = outerIndicesArray[dimIdx];
			mergedArrayIndices[2 * dimIdx + 2] = innerIndicesArray[dimIdx];
		}
		return mergedArrayIndices;
	}
	
	// For OpenMP solution
	bool Transformator::mergeIndicesArrays(int spaceDimension, int arrayResolution, int splitNO,
		int serverNO, int outerCellIdx, int innerCellIdx, int& extendedCellIdx, int *& mergedArrayIndices)
	{
		bool validArrayIndices = true;
		extendedCellIdx = splitNO;
		mergedArrayIndices = new int[2 * spaceDimension + 1];
		mergedArrayIndices[0] = splitNO;
		for (int dimIdx = spaceDimension - 1; dimIdx >= 0; dimIdx--)
		{
			int outerArrayResPow = (int)pow((double)arrayResolution, dimIdx);
			int outerCoordVal = outerCellIdx / outerArrayResPow;
			int innerArrayResPow = (int)pow((double)arrayResolution, dimIdx);
			int innerCoordVal = innerCellIdx / innerArrayResPow;
			if (outerCoordVal <= innerCoordVal)
			{
				extendedCellIdx += outerCoordVal * serverNO * 
					(int)pow((double)arrayResolution, 2 * dimIdx);
				extendedCellIdx += innerCoordVal * serverNO * 
					(int)pow((double)arrayResolution, 2 * dimIdx + 1);
				mergedArrayIndices[2 * dimIdx + 1] = outerCoordVal;
				mergedArrayIndices[2 * dimIdx + 2] = innerCoordVal;
				outerCellIdx -= outerCoordVal * outerArrayResPow;
				innerCellIdx -= innerCoordVal * innerArrayResPow;
			}
			else
			{
				validArrayIndices = false;
				delete [] mergedArrayIndices;
				break;
			}
		}
		return validArrayIndices;
	}

	int * Transformator::determineIndicesArray(int spaceDimension, int * extendedIndicesArray)
	{
		int * indicesArray = new int[2 * spaceDimension];
		for (int idx = 0; idx < spaceDimension; idx++)
		{
			indicesArray[2 * idx] = extendedIndicesArray[2 * idx + 1];
			indicesArray[2 * idx + 1] = extendedIndicesArray[2 * idx + 2];
		}
		return indicesArray;
	}

	Dictionary_s * Transformator::convertIntPairsOfShellsToListOfIdxArrays(int histogramResolution,
			int * inputIndicesArray, Vector_s shells)
	{
		Dictionary_s * result = new Dictionary_s();
		int shellIdx = 0;
		for (Vector_s::iterator itr = shells.begin(); itr != shells.end(); itr++)
		{
			Shell * shell = *itr;
			Shell_idxs * indicesArraysInCurrentShell = new Shell_idxs();
			Vector_t intTuples = shell->getIntTuples();
			for (Vector_t::iterator subItr = intTuples.begin(); subItr != intTuples.end(); subItr++)
			{
				IntTuple * intTuple = *subItr;
				int * currentIndicesArray = nullptr;
				if(intTuple->determineIdxArrayRelativeTo(histogramResolution, inputIndicesArray,
					currentIndicesArray))
				{
					indicesArraysInCurrentShell->push_back(currentIndicesArray);
				}
			}
			result->insert( std::pair<int, Shell_idxs *>(shellIdx, indicesArraysInCurrentShell) );
			shellIdx++;
		}
		return result;
	}

	bool Transformator::validateRegionHasEnoughBins(int spaceDimension, int * indicesArray, int splitNO)
	{
		bool hasEnoughBins = true;
		int binNOInThisRegion = 1;
		for (int idx = 0; idx < spaceDimension; idx++)
		{
			int lowerBound = indicesArray[2 * idx];
			int upperBound = indicesArray[2 * idx + 1];
			binNOInThisRegion *= (upperBound - lowerBound + 1);
		}
		if (binNOInThisRegion <= splitNO)
		{
			hasEnoughBins = false;
		}
		return hasEnoughBins;
	}

	void Transformator::splitIndicesArrays(int spaceDimension, int splitDimIdx, int * indicesArray,
		int componentInSplitDim, int *& firstPartIndicesArray, int *& secondPartIndicesArray)
	{
		firstPartIndicesArray = new int[2 * spaceDimension];
		secondPartIndicesArray = new int[2 * spaceDimension];
		for (int idx = 0; idx < spaceDimension; idx++)
		{
			int lowerBound = indicesArray[2 * idx];
			int upperBound = indicesArray[2 * idx + 1];
			firstPartIndicesArray[2 * idx] = lowerBound;
			secondPartIndicesArray[2 * idx + 1] = upperBound;
			if (idx == splitDimIdx)
			{
				firstPartIndicesArray[2 * idx + 1] = componentInSplitDim;
				secondPartIndicesArray[2 * idx] = componentInSplitDim + 1;
			}
			else
			{
				firstPartIndicesArray[2 * idx + 1] = upperBound;
				secondPartIndicesArray[2 * idx] = lowerBound;
			}
		}
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