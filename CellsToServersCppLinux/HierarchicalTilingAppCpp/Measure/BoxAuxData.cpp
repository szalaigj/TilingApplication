#include "BoxAuxData.hpp"

namespace Measure
{
	int BoxAuxData::getSpaceDimension() const
	{
		return spaceDimension;
	}
	
	void BoxAuxData::setSpaceDimension(int inputSpaceDimension)
	{
		this->spaceDimension = inputSpaceDimension;
	}

	int BoxAuxData::getHistogramResolution() const
	{
		return histogramResolution;
	}
	
	void BoxAuxData::setHistogramResolution(int inputHistogramResolution)
	{
		this->histogramResolution = inputHistogramResolution;
	}

	int * BoxAuxData::getHistogram() const
	{
		return histogram;
	}
	
	void BoxAuxData::setHistogram(int * inputHistogram)
	{
		this->histogram = inputHistogram;
	}

	int * BoxAuxData::getHeftArray() const
	{
		return heftArray;
	}
	
	void BoxAuxData::setHeftArray(int * inputHeftArray)
	{
		this->heftArray = inputHeftArray;
	}

	int * BoxAuxData::getIndicesArrayOfQueryRegion() const
	{
		return indicesArrayOfQueryRegion;
	}
	
	void BoxAuxData::setIndicesArrayOfQueryRegion(int * inputIndicesArrayOfQueryRegion)
	{
		this->indicesArrayOfQueryRegion = inputIndicesArrayOfQueryRegion;
	}

	double BoxAuxData::getVolumeOfQueryRegion() const
	{
		return volumeOfQueryRegion;
	}
	
	void BoxAuxData::setVolumeOfQueryRegion(double inputVolumeOfQueryRegion)
	{
		this->volumeOfQueryRegion = inputVolumeOfQueryRegion;
	}

	BoxAuxData& BoxAuxData::operator= (const BoxAuxData& other)
	{
		this->setHeftArray(other.getHeftArray());
		this->setHistogram(other.getHistogram());
		this->setHistogramResolution(other.getHistogramResolution());
		this->setIndicesArrayOfQueryRegion(other.getIndicesArrayOfQueryRegion());
		this->setServerNO(other.getServerNO());
		this->setSpaceDimension(other.getSpaceDimension());
		this->setVolumeOfQueryRegion(other.getVolumeOfQueryRegion());
		return *this;
	}
}