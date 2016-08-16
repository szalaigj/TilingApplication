#ifndef ITERATIVE_DIVIDER_HPP_
#define ITERATIVE_DIVIDER_HPP_
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
	class IterativeDivider
	{
	public:
		IterativeDivider(int * histogram, int * inputHeftArray, 
			Transformator& transformator, ParsedData& parsedData, int kNN, int maxRange,
			Vector_s& shellsForKNN,	Vector_s& shellsForRange);
		double getDiffSum() const;
		double determineObjectiveValue(Vector_coords * partition);
	private:
		void setMeasureInstances(int * histogram, int kNN, int maxRange, 
            Vector_s& shellsForKNN, Vector_s& shellsForRange);
		void fillObjectiveValueArray();
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
			Vector_coords *& firstPartPartition, bool& hasEnoughBinsForFirstPart, 
			double& objectiveValueForSecondPart, Vector_coords *& secondPartPartition, 
			bool& hasEnoughBinsForSecondPart);
		void setPartitionByParts(int * extendedIndicesArray, Vector_coords& firstPartPartition,
			Vector_coords& secondPartPartition);

		int * determineExtendedIndicesArray();
		int * determineNextIndicesArray(int * previousIndicesArray);
		int * determineNextIndicesArray(int * lowerBoundArray, int * previousIndicesArray);
		double determineCurrentDiffSum(Vector_coords& partition);
		int * heftArray;
		double * objectiveValueArray;
		Vector_coords ** partitionArray;
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