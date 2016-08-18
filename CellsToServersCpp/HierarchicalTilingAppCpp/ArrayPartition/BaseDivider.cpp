#include "BaseDivider.hpp"

namespace ArrayPartition
{
	BaseDivider::BaseDivider(int * histogram, int * inputHeftArray,
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
		this->partitionArray = new Coords**[sizeOfArrays]();
		this->hasEnoughBinsArray = new bool[sizeOfArrays]();
		this->setMeasureInstances(histogram, kNN, maxRange, shellsForKNN, shellsForRange);
	}

	void BaseDivider::setMeasureInstances(int * histogram, int kNN, int maxRange,
            Vector_s& shellsForKNN, Vector_s& shellsForRange)
	{
		KNNAuxData * kNNAuxData = new KNNAuxData(shellsForKNN);
		kNNAuxData->setKNN(kNN);
		kNNAuxData->setHistogram(histogram);
		kNNAuxData->setHistogramResolution(parsedData.getHistogramResolution());
		kNNAuxData->setPointNO(parsedData.getPointNO());
		kNNAuxData->setServerNO(parsedData.getServerNO());
		kNNAuxData->setSpaceDimension(parsedData.getSpaceDimension());
		this->kNNMeasure = new KNNMeasure(*kNNAuxData, transformator);

		RangeAuxData * rangeAuxData = new RangeAuxData(shellsForRange);
		rangeAuxData->setMaxRange(maxRange);
		rangeAuxData->setHistogram(histogram);
		rangeAuxData->setHistogramResolution(parsedData.getHistogramResolution());
		rangeAuxData->setPointNO(parsedData.getPointNO());
		rangeAuxData->setServerNO(parsedData.getServerNO());
		rangeAuxData->setSpaceDimension(parsedData.getSpaceDimension());
		this->rangeMeasure = new RangeMeasure(*rangeAuxData, transformator);

		LoadBalancingAuxData * lbAuxData = new LoadBalancingAuxData();
		lbAuxData->setDelta(parsedData.getDelta());
		lbAuxData->setPointNO(parsedData.getPointNO());
		lbAuxData->setServerNO(parsedData.getServerNO());
		this->lbMeasure = new LoadBalancingMeasure(*lbAuxData, transformator);

		BoxAuxData * boxAuxData = new BoxAuxData();
		boxAuxData->setHeftArray(this->heftArray);
		boxAuxData->setHistogram(histogram);
		boxAuxData->setHistogramResolution(parsedData.getHistogramResolution());
		boxAuxData->setServerNO(parsedData.getServerNO());
		boxAuxData->setSpaceDimension(parsedData.getSpaceDimension());
		this->boxMeasure = new BoxMeasure(*boxAuxData, transformator);
	}

	double BaseDivider::getDiffSum() const
	{
		return diffSum;
	}

	double BaseDivider::determineObjectiveValue(Coords **& partition)
	{
		int * extendedIndicesArray = determineExtendedIndicesArray();

		fillObjectiveValueArray();

		int extendedCellIdx = transformator.calculateExtendedCellIdx(2 * parsedData.getSpaceDimension() + 1,
			parsedData.getServerNO(), parsedData.getHistogramResolution(), extendedIndicesArray);
		bool hasEnoughBins = hasEnoughBinsArray[extendedCellIdx];
		double objectiveValue = objectiveValueArray[extendedCellIdx];
		if (!hasEnoughBins)
		{
			throw std::runtime_error("Not enough bins");
		}
		objectiveValue = objectiveValue / (double)parsedData.getServerNO();
		partition = partitionArray[extendedCellIdx];
		diffSum = determineCurrentDiffSum(partition);
		double measureOfKNN = kNNMeasure->computeMeasure(parsedData.getServerNO(), partition);
		std::cout << "k-NN measure of the partition: " << measureOfKNN << std::endl;
		double measureOfRange = rangeMeasure->averageAllMeasures(parsedData.getServerNO(), partition);
		std::cout << "Range measure of the partition: " << measureOfRange << std::endl;
		double measureOfLB = lbMeasure->computeMeasure(parsedData.getServerNO(), partition);
		std::cout << "Load balancing measure of the partition: " << measureOfLB << std::endl;
		double measureOfBox = boxMeasure->averageAllMeasures(parsedData.getServerNO(), partition);
		std::cout << "Box measure of the partition: " << measureOfBox << std::endl;
		return objectiveValue;
	}
	
	void BaseDivider::fillObjectiveValueArrayCore(int splitNO, int extendedCellIdx, int * extendedIndicesArray)
	{
		int spaceDimension = parsedData.getSpaceDimension();
		double objectiveValue = 0.0;
		bool hasEnoughBins;
		int * indicesArray = transformator.determineIndicesArray(spaceDimension, extendedIndicesArray);
		if (splitNO == 0)
		{
			hasEnoughBins = true;
			fillObjectiveValueWhenSplitNOIsZero(extendedIndicesArray, objectiveValue, indicesArray);
		}
		else
		{
			hasEnoughBins = transformator.validateRegionHasEnoughBins(spaceDimension, indicesArray, 
				splitNO);
			fillObjectiveValueWhenSplitNOIsLargerThenZero(extendedIndicesArray,	objectiveValue,
				indicesArray, splitNO);
		}
		objectiveValueArray[extendedCellIdx] = objectiveValue;
		hasEnoughBinsArray[extendedCellIdx] = hasEnoughBins;
		delete [] indicesArray;
	}

	void BaseDivider::fillObjectiveValueWhenSplitNOIsZero(int * extendedIndicesArray, 
		double& objectiveValue, int * indicesArray)
	{
		int spaceDimension = parsedData.getSpaceDimension();
		int currentCellIdx = transformator.calculateCellIdx(2 * spaceDimension,
			parsedData.getHistogramResolution(), indicesArray);
		int heftOfRegion = heftArray[currentCellIdx];
		Coords * coords = new Coords();
		coords->setExtendedIndicesArray(extendedIndicesArray);
		coords->setHeftOfRegion(heftOfRegion);
		Coords ** partition = new Coords*[1];
		partition[0] = coords;
		int extendedCellIdx = transformator.calculateExtendedCellIdx(2 * spaceDimension + 1,
			parsedData.getServerNO(), parsedData.getHistogramResolution(), extendedIndicesArray);
		partitionArray[extendedCellIdx] = partition;
		// If the heft of the current region is zero the objective value is zero 
		// because region with zero heft should not belong to a server.
		if (heftOfRegion != 0)
			objectiveValue = parsedData.getKNNMeasCoeff() * kNNMeasure->computeMeasureForRegion(coords) +
			parsedData.getLbMeasCoeff() * lbMeasure->computeMeasureForRegion(coords);
	}

	void BaseDivider::fillObjectiveValueWhenSplitNOIsLargerThenZero(int * extendedIndicesArray,
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

	void BaseDivider::fillObjectiveValueForSplitComponent(int * extendedIndicesArray, 
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
		}
		delete [] firstPartIndicesArray;
		delete [] secondPartIndicesArray;
	}

	void BaseDivider::fillObjectiveValueForFixedSplitNOComposition(int * extendedIndicesArray,
		double& objectiveValue, int splitNO, int * firstPartIndicesArray, int * secondPartIndicesArray,
		int firstSplitNO, int secondSplitNO)
	{
		double objectiveValueForFirstPart, objectiveValueForSecondPart;
		Coords ** firstPartPartition = nullptr;
		Coords ** secondPartPartition = nullptr;
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
			setPartitionByParts(extendedIndicesArray, splitNO, firstSplitNO, firstPartPartition,
				secondSplitNO, secondPartPartition);
		}
	}

	void BaseDivider::getValuesFromParts(int * firstPartIndicesArray, int * secondPartIndicesArray,
		int firstSplitNO, int secondSplitNO, double& objectiveValueForFirstPart,
		Coords **& firstPartPartition, bool& hasEnoughBinsForFirstPart, 
		double& objectiveValueForSecondPart, Coords **& secondPartPartition, 
		bool& hasEnoughBinsForSecondPart)
	{
		int spaceDimension = parsedData.getSpaceDimension();
		int histogramResolution = parsedData.getHistogramResolution();
		int serverNO = parsedData.getServerNO();
		int * firstPartExtendedIndicesArray = transformator.extendIndicesArray(spaceDimension,
			firstPartIndicesArray);
		firstPartExtendedIndicesArray[0] = firstSplitNO;
		int * secondPartExtendedIndicesArray = transformator.extendIndicesArray(spaceDimension,
			secondPartIndicesArray);
		secondPartExtendedIndicesArray[0] = secondSplitNO;
		int firstExtendedCellIdx = transformator.calculateExtendedCellIdx(2 * spaceDimension + 1,
			serverNO, histogramResolution, firstPartExtendedIndicesArray);
		int secondExtendedCellIdx = transformator.calculateExtendedCellIdx(2 * spaceDimension + 1,
			serverNO, histogramResolution, secondPartExtendedIndicesArray);
		objectiveValueForFirstPart = objectiveValueArray[firstExtendedCellIdx];
		firstPartPartition = partitionArray[firstExtendedCellIdx];
		hasEnoughBinsForFirstPart = hasEnoughBinsArray[firstExtendedCellIdx];
		objectiveValueForSecondPart = objectiveValueArray[secondExtendedCellIdx];
		secondPartPartition = partitionArray[secondExtendedCellIdx];
		hasEnoughBinsForSecondPart = hasEnoughBinsArray[secondExtendedCellIdx];
		delete [] firstPartExtendedIndicesArray;
		delete [] secondPartExtendedIndicesArray;
	}

	void BaseDivider::setPartitionByParts(int * extendedIndicesArray, int splitNO, int firstSplitNO,
		Coords **& firstPartPartition, int secondSplitNO, Coords **& secondPartPartition)
	{
		Coords ** partition = new Coords*[splitNO + 1];
		for (int idx = 0; idx < firstSplitNO + 1; idx++)
		{
			partition[idx] = firstPartPartition[idx];
		}
		for (int idx = firstSplitNO + 1; idx < splitNO + 1; idx++)
		{
			partition[idx] = secondPartPartition[idx - (firstSplitNO + 1)];
		}
		int extendedCellIdx = transformator.calculateExtendedCellIdx(
			2 * parsedData.getSpaceDimension() + 1, parsedData.getServerNO(),
			parsedData.getHistogramResolution(), extendedIndicesArray);
		partitionArray[extendedCellIdx] = partition;
	}

	int * BaseDivider::determineExtendedIndicesArray()
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
	
	double BaseDivider::determineCurrentDiffSum(Coords ** partition)
	{
		double diffSum = 0.0;
		for (int idx = 0; idx < parsedData.getServerNO(); idx++)
		{
			diffSum += (*partition[idx]).differenceFromDelta(parsedData.getDelta());
		}
		return diffSum;
	}
}