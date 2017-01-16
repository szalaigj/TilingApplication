#ifndef ITERATIVE_DIVIDER_HPP_
#define ITERATIVE_DIVIDER_HPP_
#include "BaseDivider.hpp"

using namespace DataHandlingUtil;
using namespace Transformation;
using namespace Measure;

namespace ArrayPartition
{
	class IterativeDivider : public BaseDivider
	{
	public:
		IterativeDivider(int * histogram, int * inputHeftArray, 
			Transformator& transformator, ParsedData& parsedData, int kNN, int maxRange,
			Vector_s& shellsForKNN,	Vector_s& shellsForRange);
	protected:
		void fillObjectiveValueArray();
	private:
		int * determineNextIndicesArray(int * previousIndicesArray);
		int * determineNextIndicesArray(int * lowerBoundArray, int * previousIndicesArray);
	};
}

#endif /* ITERATIVE_DIVIDER_HPP_ */