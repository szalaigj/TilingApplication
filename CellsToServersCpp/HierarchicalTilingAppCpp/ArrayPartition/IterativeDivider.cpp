#include "IterativeDivider.hpp"

namespace ArrayPartition
{
	IterativeDivider::IterativeDivider(int * histogram, int * inputHeftArray,
		Transformator& transformator, ParsedData& parsedData, int kNN, int maxRange,
		Vector_s& shellsForKNN, Vector_s& shellsForRange) : BaseDivider(histogram, inputHeftArray, 
		transformator, parsedData, kNN, maxRange, shellsForKNN, shellsForRange)
	{
	}
	void IterativeDivider::fillObjectiveValueArray()
	{
		int spaceDimension = parsedData.getSpaceDimension();
		for (int splitNO = 0; splitNO < parsedData.getServerNO(); splitNO++)
		{
			int * outerIndicesArray = new int[spaceDimension]();
			for (;// the following expression is based on the logic of the method determineNextIndicesArray
				outerIndicesArray != nullptr;
				// retrieving the next indices array
				outerIndicesArray = determineNextIndicesArray(outerIndicesArray)
			)
			{
				int * innerIndicesArray;
				for (innerIndicesArray = transformator.copyIndicesArray(spaceDimension,	outerIndicesArray);
					innerIndicesArray != nullptr;
					innerIndicesArray = determineNextIndicesArray(outerIndicesArray, innerIndicesArray)
				)
				{
					int * extendedIndicesArray = transformator.mergeIndicesArrays(spaceDimension,
						splitNO, outerIndicesArray, innerIndicesArray);
					int extendedCellIdx = transformator.calculateExtendedCellIdx(2 * spaceDimension + 1,
						parsedData.getServerNO(), parsedData.getHistogramResolution(), extendedIndicesArray);
					fillObjectiveValueArrayCore(splitNO, extendedCellIdx, extendedIndicesArray);
				}
				delete [] innerIndicesArray;
			}
			delete [] outerIndicesArray;
		}
	}

	int * IterativeDivider::determineNextIndicesArray(int * previousIndicesArray)
	{
		int * nextIndicesArray = transformator.copyIndicesArray(parsedData.getSpaceDimension(),
			previousIndicesArray);
		for (int dimIdx = 0; dimIdx < parsedData.getSpaceDimension(); dimIdx++)
		{
			nextIndicesArray[dimIdx]++;
			if (nextIndicesArray[dimIdx] < parsedData.getHistogramResolution())
				return nextIndicesArray;
			nextIndicesArray[dimIdx] = 0;
		}
		return nullptr;
	}

	int * IterativeDivider::determineNextIndicesArray(int * lowerBoundArray, int * previousIndicesArray)
	{
		int * nextIndicesArray = transformator.copyIndicesArray(parsedData.getSpaceDimension(),
			previousIndicesArray);
		for (int dimIdx = 0; dimIdx < parsedData.getSpaceDimension(); dimIdx++)
		{
			nextIndicesArray[dimIdx]++;
			if (nextIndicesArray[dimIdx] < parsedData.getHistogramResolution())
				return nextIndicesArray;
			nextIndicesArray[dimIdx] = lowerBoundArray[dimIdx];
		}
		return nullptr;
	}
}