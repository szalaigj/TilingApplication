#ifndef TRANSFORMATOR_HPP_
#define TRANSFORMATOR_HPP_
#define _USE_MATH_DEFINES
#include <math.h>

namespace Transformation
{
	class Transformator
	{
	public:
		int calculateCellIdx(int arrayRank, int arrayResolution, int * indicesArray);
		int * copyIndicesArray(int spaceDimension, int * inputIndicesArray);
		int * determineNextIndicesArray(int spaceDimension, int histogramResolution,
			int * previousIndicesArray);
		int * determineNextIndicesArray(int spaceDimension, int histogramResolution,
			int * lowerBoundArray, int * previousIndicesArray);
		int * determineFirstContainedIndicesArray(int spaceDimension, int * indicesArrayOfRegion);
		int * determineNextContainedIndicesArray(int spaceDimension, int * indicesArrayOfRegion,
			int * previousIndicesArray);
		int * mergeIndicesArrays(int spaceDimension, int * outerIndicesArray, int * innerIndicesArray);
		int determineMaxRange(int spaceDimension, int histogramResolution);
	private:
		int factorial(int input);
		int doubleFactorial(int input);
	};
}

#endif /* TRANSFORMATOR_HPP_ */