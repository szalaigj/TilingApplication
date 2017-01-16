#ifndef OPENMP_DIVIDER_HPP_
#define OPENMP_DIVIDER_HPP_
#include "BaseDivider.hpp"

using namespace DataHandlingUtil;
using namespace Transformation;
using namespace Measure;

extern const size_t nThreads;

namespace ArrayPartition
{
	class OpenMPDivider : public BaseDivider
	{
	public:
		OpenMPDivider(int * histogram, int * inputHeftArray, 
			Transformator& transformator, ParsedData& parsedData, int kNN, int maxRange,
			Vector_s& shellsForKNN,	Vector_s& shellsForRange);
	protected:
		void fillObjectiveValueArray();
	};
}

#endif /* OPENMP_DIVIDER_HPP_ */