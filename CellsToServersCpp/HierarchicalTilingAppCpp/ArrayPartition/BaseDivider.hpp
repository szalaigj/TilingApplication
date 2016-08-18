#ifndef BASE_DIVIDER_HPP_
#define BASE_DIVIDER_HPP_
#include <stdexcept>
#include <iostream>

#include "../DataHandlingUtil/ParsedData.hpp"
#include "../Transformation/Transformator.hpp"
#include "../Measure/IncludingAllMeasures.hpp"

using namespace DataHandlingUtil;
using namespace Transformation;
using namespace Measure;

namespace ArrayPartition
{
	class BaseDivider
	{
	public:
		BaseDivider(int * histogram, int * inputHeftArray, 
			Transformator& transformator, ParsedData& parsedData, int kNN, int maxRange,
			Vector_s& shellsForKNN,	Vector_s& shellsForRange);
		double getDiffSum() const;
		double determineObjectiveValue(Coords **& partition);
	protected:
		virtual void fillObjectiveValueArray() = 0;
		void setMeasureInstances(int * histogram, int kNN, int maxRange, 
            Vector_s& shellsForKNN, Vector_s& shellsForRange);
		void fillObjectiveValueArrayCore(int splitNO, int extendedCellIdx, int * extendedIndicesArray);
		void fillObjectiveValueWhenSplitNOIsZero(int * extendedIndicesArray, double& objectiveValue,
            int * indicesArray);
		void fillObjectiveValueWhenSplitNOIsLargerThenZero(int * extendedIndicesArray,
			double& objectiveValue, int * indicesArray, int splitNO);
		void fillObjectiveValueForSplitComponent(int * extendedIndicesArray, double& objectiveValue,
            int * indicesArray, int splitNO, int splitDimIdx, int componentInSplitDim);
		void fillObjectiveValueForFixedSplitNOComposition(int * extendedIndicesArray,
			double& objectiveValue, int splitNO, int * firstPartIndicesArray, int * secondPartIndicesArray,
			int firstSplitNO, int secondSplitNO);
		void getValuesFromParts(int * firstPartIndicesArray, int * secondPartIndicesArray,
			int firstSplitNO, int secondSplitNO, double& objectiveValueForFirstPart,
			Coords **& firstPartPartition, bool& hasEnoughBinsForFirstPart, 
			double& objectiveValueForSecondPart, Coords **& secondPartPartition, 
			bool& hasEnoughBinsForSecondPart);
		void setPartitionByParts(int * extendedIndicesArray, int splitNO, int firstSplitNO,
			Coords **& firstPartPartition, int secondSplitNO, Coords **& secondPartPartition);

		int * determineExtendedIndicesArray();
		double determineCurrentDiffSum(Coords ** partition);
		int * heftArray;
		double * objectiveValueArray;
		Coords *** partitionArray;
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

#endif /* BASE_DIVIDER_HPP_ */