#include "IntTuple.hpp"

namespace SumOfSquares
{
	bool IntTuple::determineIdxArrayRelativeTo(int spaceDimension, int histogramResolution, 
		int * inputIndicesArray, int * outputIndicesArray)
	{
		outputIndicesArray = new int[spaceDimension];
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			outputIndicesArray[dimIdx] = tuple[dimIdx] + inputIndicesArray[dimIdx];
		}
		return isValidIdxArray(spaceDimension, histogramResolution, outputIndicesArray);
	}

	bool IntTuple::isValidIdxArray(int spaceDimension, int histogramResolution, 
		int * outputIndicesArray)
	{
		bool isValid = true;
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			int outComponent = outputIndicesArray[dimIdx];
			if (!((outComponent >= 0) && (outComponent < histogramResolution)))
			{
				isValid = false;
			}
		}
		return isValid;
	}

	int * IntTuple::getTuple()
	{
		return tuple;
	}

	void IntTuple::setTuple(int * tuple)
	{
		this->tuple = tuple;
	}
}