#ifndef ITERATIVE_DIVIDER_HPP_
#define ITERATIVE_DIVIDER_HPP_

#include "../DataHandlingUtil/ParsedData.hpp"
#include "../Transformation/Transformator.hpp"
#include "../Measure/IncludingAllMeasures.hpp"

using namespace DataHandlingUtil;
using namespace Transformation;
using namespace Measure;

namespace ArrayPartition
{
	class IterativeDivider
	{
	public:
		IterativeDivider(int * histogram, int * inputHeftArray, 
			Transformator& transformator, ParsedData& parsedData, int kNN, int maxRange,
			Vector_s& shellsForKNN,	Vector_s& shellsForRange);
		double getDiffSum() const;
	private:
		void setMeasureInstances(int * histogram, int kNN, int maxRange, 
            Vector_s& shellsForKNN, Vector_s& shellsForRange);
		int * heftArray;
		double * objectiveValueArray;
		Vector_coords * partitionArray;
		bool * hasEnoughBinsArray;
		ParsedData& parsedData;
		Transformator& transformator;
		KNNMeasure * kNNMeasure;
		RangeMeasure * rangeMeasure;
		LoadBalancingMeasure * lbMeasure;
		BoxMeasure * boxMeasure;
		double diffSum;
	};
}

#endif /* ITERATIVE_DIVIDER_HPP_ */