#include "IterativeDivider.hpp"

namespace ArrayPartition
{
	IterativeDivider::IterativeDivider(int * histogram, int * inputHeftArray,
		Transformator& transformator, ParsedData& parsedData, int kNN, int maxRange,
		Vector_s& shellsForKNN, Vector_s& shellsForRange)
		: transformator(transformator), parsedData(parsedData)
	{
		this->heftArray = inputHeftArray;
		int spaceDimension = parsedData.getSpaceDimension();
		int histogramResolution = parsedData.getHistogramResolution();
		int serverNO = parsedData.getServerNO();
		int sizeOfArrays = serverNO * (int)pow((double)histogramResolution, 2 * spaceDimension);
		this->objectiveValueArray = new double[sizeOfArrays]();
		this->partitionArray = new Vector_coords*[sizeOfArrays];
		this->hasEnoughBinsArray = new bool[sizeOfArrays];
		this->setMeasureInstances(histogram, kNN, maxRange, shellsForKNN, shellsForRange);
	}

	void IterativeDivider::setMeasureInstances(int * histogram, int kNN, int maxRange,
            Vector_s& shellsForKNN, Vector_s& shellsForRange)
	{
		KNNAuxData kNNAuxData(shellsForKNN);
		kNNAuxData.setKNN(kNN);
		kNNAuxData.setHistogram(histogram);
		kNNAuxData.setHistogramResolution(parsedData.getHistogramResolution());
		kNNAuxData.setPointNO(parsedData.getPointNO());
		kNNAuxData.setServerNO(parsedData.getServerNO());
		kNNAuxData.setSpaceDimension(parsedData.getSpaceDimension());
		this->kNNMeasure = new KNNMeasure(kNNAuxData, transformator);

		RangeAuxData rangeAuxData(shellsForRange);
		rangeAuxData.setMaxRange(maxRange);
		rangeAuxData.setHistogram(histogram);
		rangeAuxData.setHistogramResolution(parsedData.getHistogramResolution());
		rangeAuxData.setPointNO(parsedData.getPointNO());
		rangeAuxData.setServerNO(parsedData.getServerNO());
		rangeAuxData.setSpaceDimension(parsedData.getSpaceDimension());
		this->rangeMeasure = new RangeMeasure(rangeAuxData, transformator);

		LoadBalancingAuxData lbAuxData;
		lbAuxData.setDelta(parsedData.getDelta());
		lbAuxData.setPointNO(parsedData.getPointNO());
		lbAuxData.setServerNO(parsedData.getServerNO());
		this->lbMeasure = new LoadBalancingMeasure(lbAuxData, transformator);

		BoxAuxData boxAuxData;
		boxAuxData.setHeftArray(this->heftArray);
		boxAuxData.setHistogram(histogram);
		boxAuxData.setHistogramResolution(parsedData.getHistogramResolution());
		boxAuxData.setServerNO(parsedData.getServerNO());
		boxAuxData.setSpaceDimension(parsedData.getSpaceDimension());
		this->boxMeasure = new BoxMeasure(boxAuxData, transformator);
	}

	double IterativeDivider::getDiffSum() const
	{
		return diffSum;
	}

	double IterativeDivider::determineObjectiveValue(Vector_coords * partition)
	{
		int * extendedIndicesArray = determineExtendedIndicesArray();

		fillObjectiveValueArray();

		int extendedCellIdx = transformator.calculateExtendedCellIdx(2 * parsedData.getSpaceDimension() + 1,
			parsedData.getHistogramResolution(), extendedIndicesArray);
		bool hasEnoughBins = hasEnoughBinsArray[extendedCellIdx];
		double objectiveValue = objectiveValueArray[extendedCellIdx];
		if (!hasEnoughBins)
		{
			throw std::runtime_error("Not enough bins");
		}
		objectiveValue = objectiveValue / (double)parsedData.getServerNO();
		partition = partitionArray[extendedCellIdx];
		diffSum = determineCurrentDiffSum(*partition);
		double measureOfKNN = kNNMeasure->computeMeasure(*partition);
		std::cout << "k-NN measure of the partition: " << measureOfKNN << std::endl;
		double measureOfRange = rangeMeasure->computeMeasure(*partition);
		std::cout << "Range measure of the partition: " << measureOfRange << std::endl;
		double measureOfLB = lbMeasure->computeMeasure(*partition);
		std::cout << "Load balancing measure of the partition: " << measureOfLB << std::endl;
		double measureOfBox = boxMeasure->computeMeasure(*partition);
		std::cout << "Box measure of the partition: " << measureOfBox << std::endl;
		return objectiveValue;
	}

	void IterativeDivider::fillObjectiveValueArray()
	{
		int spaceDimension = parsedData.getSpaceDimension();
		for (int splitNO = 0; splitNO < parsedData.getServerNO(); splitNO++)
		{
			
			int * outerIndicesArray = new int[spaceDimension]();
			for (;// the following expression is based on the logic of the method determineNextIndicesArray
				outerIndicesArray != nullptr;
				// retrieving the next indices array
				outerIndicesArray = determineNextIndicesArray(outerIndicesArray)
			)
			{
				int * innerIndicesArray;
				for (innerIndicesArray = transformator.copyIndicesArray(spaceDimension,	outerIndicesArray);
					innerIndicesArray != nullptr;
					innerIndicesArray = determineNextIndicesArray(outerIndicesArray, innerIndicesArray)
				)
				{
					int * extendedIndicesArray = transformator.mergeIndicesArrays(spaceDimension,
						splitNO, outerIndicesArray, innerIndicesArray);
					double objectiveValue = 0.0;
					bool hasEnoughBins;
					int * indicesArray = transformator.determineIndicesArray(spaceDimension, 
						extendedIndicesArray);
					if (splitNO == 0)
					{
						hasEnoughBins = true;
						fillObjectiveValueWhenSplitNOIsZero(extendedIndicesArray, objectiveValue,
							indicesArray);
					}
					else
					{
						hasEnoughBins = transformator.validateRegionHasEnoughBins(spaceDimension, 
							indicesArray, splitNO);
						fillObjectiveValueWhenSplitNOIsLargerThenZero(extendedIndicesArray,	objectiveValue,
							indicesArray, splitNO);
					}
					int extendedCellIdx = transformator.calculateExtendedCellIdx(2 * spaceDimension + 1,
						parsedData.getHistogramResolution(), extendedIndicesArray);
					objectiveValueArray[extendedCellIdx] = objectiveValue;
					hasEnoughBinsArray[extendedCellIdx] = hasEnoughBins;
					delete [] extendedIndicesArray;
					delete [] indicesArray;
				}
				delete [] innerIndicesArray;
			}
			delete [] outerIndicesArray;
		}
	}

	void IterativeDivider::fillObjectiveValueWhenSplitNOIsZero(int * extendedIndicesArray, 
		double& objectiveValue, int * indicesArray)
	{
		int spaceDimension = parsedData.getSpaceDimension();
		int currentCellIdx = transformator.calculateCellIdx(spaceDimension,
			parsedData.getHistogramResolution(), indicesArray);
		int heftOfRegion = heftArray[currentCellIdx];
		Coords * coords = new Coords();
		coords->setExtendedIndicesArray(extendedIndicesArray);
		coords->setHeftOfRegion(heftOfRegion);
		Vector_coords * partition = new Vector_coords();
		partition->push_back(coords);
		int extendedCellIdx = transformator.calculateExtendedCellIdx(2 * spaceDimension + 1,
			parsedData.getHistogramResolution(), extendedIndicesArray);
		partitionArray[extendedCellIdx] = partition;
		// If the heft of the current region is zero the objective value is zero 
		// because region with zero heft should not belong to a server.
		if (heftOfRegion != 0)
			objectiveValue = parsedData.getKNNMeasCoeff() * kNNMeasure->computeMeasureForRegion(*coords) +
			parsedData.getLbMeasCoeff() * lbMeasure->computeMeasureForRegion(*coords);
	}

	void IterativeDivider::fillObjectiveValueWhenSplitNOIsLargerThenZero(int * extendedIndicesArray,
		double& objectiveValue, int * indicesArray, int splitNO)
	{
		for (int splitDimIdx = 0; splitDimIdx < parsedData.getSpaceDimension(); splitDimIdx++)
		{
			for (int componentInSplitDim = indicesArray[2 * splitDimIdx];
				componentInSplitDim < indicesArray[2 * splitDimIdx + 1]; componentInSplitDim++)
			{
				fillObjectiveValueForSplitComponent(extendedIndicesArray, objectiveValue, indicesArray,
					splitNO, splitDimIdx, componentInSplitDim);
			}
		}
	}

	void IterativeDivider::fillObjectiveValueForSplitComponent(int * extendedIndicesArray, 
		double& objectiveValue, int * indicesArray, int splitNO, int splitDimIdx, int componentInSplitDim)
	{
		int * firstPartIndicesArray = nullptr;
		int * secondPartIndicesArray = nullptr;
		transformator.splitIndicesArrays(parsedData.getSpaceDimension(), splitDimIdx, indicesArray,
			componentInSplitDim, firstPartIndicesArray, secondPartIndicesArray);
		for (int firstSplitNO = 0; firstSplitNO <= splitNO - 1; firstSplitNO++)
		{
			int secondSplitNO = (splitNO - 1) - firstSplitNO;
			fillObjectiveValueForFixedSplitNOComposition(extendedIndicesArray, objectiveValue, 
				splitNO, firstPartIndicesArray, secondPartIndicesArray, firstSplitNO, secondSplitNO);
			delete [] firstPartIndicesArray;
			delete [] secondPartIndicesArray;
		}
	}

	void IterativeDivider::fillObjectiveValueForFixedSplitNOComposition(int * extendedIndicesArray,
		double& objectiveValue, int splitNO, int * firstPartIndicesArray, int * secondPartIndicesArray,
		int firstSplitNO, int secondSplitNO)
	{
		double objectiveValueForFirstPart, objectiveValueForSecondPart;
		Vector_coords * firstPartPartition = nullptr;
		Vector_coords * secondPartPartition = nullptr;
		bool hasEnoughBinsForFirstPart, hasEnoughBinsForSecondPart;
		getValuesFromParts(firstPartIndicesArray, secondPartIndicesArray, firstSplitNO, secondSplitNO, 
			objectiveValueForFirstPart, firstPartPartition, hasEnoughBinsForFirstPart, 
			objectiveValueForSecondPart, secondPartPartition, hasEnoughBinsForSecondPart);
		double currentObjectiveValue = 0.0;
		// If the objective value of a part of the current region is zero
		// then the current objective value is zero because this case is applied
		// when a part of the current region has zero heft which would belong to a server.
		if ((objectiveValueForFirstPart != 0.0) && (objectiveValueForSecondPart != 0.0))
		{
			currentObjectiveValue = objectiveValueForFirstPart + objectiveValueForSecondPart;
		}
		if ((currentObjectiveValue >= objectiveValue)
			&& hasEnoughBinsForFirstPart && hasEnoughBinsForSecondPart)
		{
			objectiveValue = currentObjectiveValue;
			Vector_coords * partition = new Vector_coords();
			setPartitionByParts(extendedIndicesArray, partition, *firstPartPartition, 
				*secondPartPartition);
		}
	}

	void IterativeDivider::getValuesFromParts(int * firstPartIndicesArray, int * secondPartIndicesArray,
		int firstSplitNO, int secondSplitNO, double& objectiveValueForFirstPart,
		Vector_coords * firstPartPartition, bool& hasEnoughBinsForFirstPart, 
		double& objectiveValueForSecondPart, Vector_coords * secondPartPartition, 
		bool& hasEnoughBinsForSecondPart)
	{
		int spaceDimension = parsedData.getSpaceDimension();
		int histogramResolution = parsedData.getHistogramResolution();
		int * firstPartExtendedIndicesArray = transformator.extendIndicesArray(spaceDimension,
			firstPartIndicesArray);
		firstPartExtendedIndicesArray[0] = firstSplitNO;
		int * secondPartExtendedIndicesArray = transformator.extendIndicesArray(spaceDimension,
			secondPartIndicesArray);
		firstPartExtendedIndicesArray[0] = secondSplitNO;
		int firstExtendedCellIdx = transformator.calculateExtendedCellIdx(2 * spaceDimension + 1,
			histogramResolution, firstPartExtendedIndicesArray);
		int secondExtendedCellIdx = transformator.calculateExtendedCellIdx(2 * spaceDimension + 1,
			histogramResolution, secondPartExtendedIndicesArray);
		objectiveValueForFirstPart = objectiveValueArray[firstExtendedCellIdx];
		firstPartPartition = partitionArray[firstExtendedCellIdx];
		hasEnoughBinsForFirstPart = hasEnoughBinsArray[firstExtendedCellIdx];
		objectiveValueForSecondPart = objectiveValueArray[secondExtendedCellIdx];
		secondPartPartition = partitionArray[secondExtendedCellIdx];
		hasEnoughBinsForSecondPart = hasEnoughBinsArray[secondExtendedCellIdx];
		delete [] firstPartExtendedIndicesArray;
		delete [] secondPartExtendedIndicesArray;
	}

	void IterativeDivider::setPartitionByParts(int * extendedIndicesArray, Vector_coords * partition,
           Vector_coords& firstPartPartition, Vector_coords& secondPartPartition)
	{
		for(Vector_coords::iterator itr = firstPartPartition.begin(); itr != firstPartPartition.end();
			itr++)
		{
			partition->push_back(*itr);
		}
		for(Vector_coords::iterator itr = secondPartPartition.begin(); itr != secondPartPartition.end();
			itr++)
		{
			partition->push_back(*itr);
		}
		int extendedCellIdx = transformator.calculateExtendedCellIdx(
			2 * parsedData.getSpaceDimension() + 1, parsedData.getHistogramResolution(),
			extendedIndicesArray);
		partitionArray[extendedCellIdx] = partition;
	}

	int * IterativeDivider::determineExtendedIndicesArray()
	{
		int * extendedIndicesArray = new int[2 * parsedData.getSpaceDimension() + 1];
		extendedIndicesArray[0] = parsedData.getServerNO() - 1;
		for (int idx = 0; idx < parsedData.getSpaceDimension(); idx++)
		{
			extendedIndicesArray[2 * idx + 1] = 0;
			extendedIndicesArray[2 * idx + 2] = parsedData.getHistogramResolution() - 1;
		}
		return extendedIndicesArray;
	}

	int * IterativeDivider::determineNextIndicesArray(int * previousIndicesArray)
	{
		int * nextIndicesArray = transformator.copyIndicesArray(parsedData.getSpaceDimension(),
			previousIndicesArray);
		for (int dimIdx = 0; dimIdx < parsedData.getSpaceDimension(); dimIdx++)
		{
			nextIndicesArray[dimIdx]++;
			if (nextIndicesArray[dimIdx] < parsedData.getHistogramResolution())
				return nextIndicesArray;
			nextIndicesArray[dimIdx] = 0;
		}
		return nullptr;
	}

	int * IterativeDivider::determineNextIndicesArray(int * lowerBoundArray, int * previousIndicesArray)
	{
		int * nextIndicesArray = transformator.copyIndicesArray(parsedData.getSpaceDimension(),
			previousIndicesArray);
		for (int dimIdx = 0; dimIdx < parsedData.getSpaceDimension(); dimIdx++)
		{
			nextIndicesArray[dimIdx]++;
			if (nextIndicesArray[dimIdx] < parsedData.getHistogramResolution())
				return nextIndicesArray;
			nextIndicesArray[dimIdx] = lowerBoundArray[dimIdx];
		}
		return nullptr;
	}

	double IterativeDivider::determineCurrentDiffSum(Vector_coords& partition)
	{
		double diffSum = 0.0;
		for (size_t idx = 0; idx < partition.size(); idx++)
		{
			diffSum += (*partition[idx]).differenceFromDelta(parsedData.getDelta());
		}
		return diffSum;
	}
}