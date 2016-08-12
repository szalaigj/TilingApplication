#ifndef HEFT_ARRAY_CREATOR_HPP_
#define HEFT_ARRAY_CREATOR_HPP_
#include <math.h>

#include "../Transformation/Transformator.hpp"

using namespace Transformation;

namespace ArrayPartition
{
	class HeftArrayCreator
	{
	public:
		HeftArrayCreator(Transformator& transformator);
		int * createHeftArray(int spaceDimension, int histogramResolution, int * histogram);
	private:
		void fillHeftArray(int spaceDimension, int histogramResolution, int * histogram, int * heftArray);
		Transformator& transformator;
	};
}

#endif /* HEFT_ARRAY_CREATOR_HPP_ */