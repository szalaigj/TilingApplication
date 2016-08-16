#include "HeftArrayCreator.hpp"

namespace ArrayPartition
{
	HeftArrayCreator::HeftArrayCreator(Transformator& transformator) :
		transformator(transformator)
	{
	}

	int * HeftArrayCreator::createHeftArray(int spaceDimension, int histogramResolution, int * histogram)
	{
		int elementNO = (int)pow((double)histogramResolution, 2 * spaceDimension);
		// initializing the elements of the heftArray to zeros:
		int * heftArray = new int[elementNO]();
		fillHeftArray(spaceDimension, histogramResolution, histogram, heftArray);
		return heftArray;
	}

	void HeftArrayCreator::fillHeftArray(int spaceDimension, int histogramResolution, int * histogram,
		int * heftArray)
	{
		int cellNO = (int)pow((double)histogramResolution, spaceDimension);
		// initializing the elements of the outerIndicesArray to zeros:
		int * outerIndicesArray = new int[spaceDimension]();
		for (;// the following expression is based on the logic of the method determineNextIndicesArray
			outerIndicesArray != nullptr;
			// retrieving the next indices array
			outerIndicesArray = transformator.determineNextIndicesArray(spaceDimension,
				histogramResolution, outerIndicesArray)
		)
		{
			int * innerIndicesArray;
			for (innerIndicesArray = transformator.copyIndicesArray(spaceDimension, 
				outerIndicesArray);
				innerIndicesArray != nullptr;
				innerIndicesArray = transformator.determineNextIndicesArray(spaceDimension,
				histogramResolution, outerIndicesArray, innerIndicesArray)
			)
			{
				int * heftArrayIndices = transformator.mergeIndicesArrays(spaceDimension, 
                        outerIndicesArray, innerIndicesArray);
				int binValue = 0;
				int * indicesArrayOfBin;
				for (indicesArrayOfBin = transformator.determineFirstContainedIndicesArray(
					spaceDimension, heftArrayIndices);
					indicesArrayOfBin != nullptr;
					indicesArrayOfBin = transformator.determineNextContainedIndicesArray(
						spaceDimension, heftArrayIndices, indicesArrayOfBin)
				)
				{
					int currentCellIdx = transformator.calculateCellIdx(spaceDimension,
						histogramResolution, indicesArrayOfBin);
					binValue += histogram[currentCellIdx];
				}
				delete [] indicesArrayOfBin;
				int currentHeftCellIdx = transformator.calculateCellIdx(2 * spaceDimension,
					histogramResolution, heftArrayIndices);
				heftArray[currentHeftCellIdx] = binValue;
				delete [] heftArrayIndices;
			}
			delete [] innerIndicesArray;
		}
		delete [] outerIndicesArray;
	}
}