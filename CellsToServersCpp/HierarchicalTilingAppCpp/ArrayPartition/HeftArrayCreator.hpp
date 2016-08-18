#ifndef HEFT_ARRAY_CREATOR_HPP_
#define HEFT_ARRAY_CREATOR_HPP_
#include <math.h>

#include "../Transformation/Transformator.hpp"

using namespace Transformation;

namespace ArrayPartition
{
	class BaseHeftArrayCreator
	{
	public:
		BaseHeftArrayCreator(Transformator& transformator);
		int * createHeftArray(int spaceDimension, int histogramResolution, int * histogram);
	protected:
		Transformator& transformator;
		virtual void fillHeftArray(int spaceDimension, int histogramResolution, int * histogram,
			int * heftArray) = 0;
	};

	class HeftArrayCreator : public BaseHeftArrayCreator
	{
	public:
		HeftArrayCreator(Transformator& transformator);
	protected:
		void fillHeftArray(int spaceDimension, int histogramResolution, int * histogram, int * heftArray);
	};

	class HeftArrayCreatorOpenMP : public BaseHeftArrayCreator
	{
	public:
		HeftArrayCreatorOpenMP(Transformator& transformator);
	protected:
		void fillHeftArray(int spaceDimension, int histogramResolution, int * histogram, int * heftArray);
	};
}

#endif /* HEFT_ARRAY_CREATOR_HPP_ */