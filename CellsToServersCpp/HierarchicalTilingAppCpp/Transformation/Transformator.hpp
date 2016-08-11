#ifndef TRANSFORMATOR_HPP_
#define TRANSFORMATOR_HPP_
#define _USE_MATH_DEFINES
#include <math.h>

namespace Transformation
{
	class Transformator
	{
	public:
		int determineMaxRange(int spaceDimension, int histogramResolution);
	private:
		int factorial(int input);
		int doubleFactorial(int input);
	};
}

#endif /* TRANSFORMATOR_HPP_ */