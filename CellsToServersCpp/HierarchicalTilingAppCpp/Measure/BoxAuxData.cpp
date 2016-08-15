#include "BoxAuxData.hpp"

namespace Measure
{
	int BoxAuxData::getSpaceDimension()
	{
		return spaceDimension;
	}
	
	void BoxAuxData::setSpaceDimension(int inputSpaceDimension)
	{
		this->spaceDimension = inputSpaceDimension;
	}

	int BoxAuxData::getHistogramResolution()
	{
		return histogramResolution;
	}
	
	void BoxAuxData::setHistogramResolution(int inputHistogramResolution)
	{
		this->histogramResolution = inputHistogramResolution;
	}

	int * BoxAuxData::getHistogram()
	{
		return histogram;
	}
	
	void BoxAuxData::setHistogram(int * inputHistogram)
	{
		this->histogram = inputHistogram;
	}

	int * BoxAuxData::getHeftArray()
	{
		return heftArray;
	}
	
	void BoxAuxData::setHeftArray(int * inputHeftArray)
	{
		this->heftArray = inputHeftArray;
	}

	int * BoxAuxData::getIndicesArrayOfQueryRegion()
	{
		return indicesArrayOfQueryRegion;
	}
	
	void BoxAuxData::setIndicesArrayOfQueryRegion(int * inputIndicesArrayOfQueryRegion)
	{
		this->indicesArrayOfQueryRegion = inputIndicesArrayOfQueryRegion;
	}

	double BoxAuxData::getVolumeOfQueryRegion()
	{
		return volumeOfQueryRegion;
	}
	
	void BoxAuxData::setVolumeOfQueryRegion(double inputVolumeOfQueryRegion)
	{
		this->volumeOfQueryRegion = inputVolumeOfQueryRegion;
	}
}