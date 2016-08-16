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
		this->objectiveValueArray = new double[sizeOfArrays];
		this->partitionArray = new Vector_coords(sizeOfArrays);
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
}