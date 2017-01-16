#include "SimilarityAuxData.hpp"

namespace Measure
{
	SimilarityAuxData::SimilarityAuxData(Vector_s& shells) : shells(shells)
	{
	}

	int SimilarityAuxData::getSpaceDimension() const
	{
		return spaceDimension;
	}
	
	void SimilarityAuxData::setSpaceDimension(int inputSpaceDimension)
	{
		this->spaceDimension = inputSpaceDimension;
	}

	int SimilarityAuxData::getHistogramResolution() const
	{
		return histogramResolution;
	}
	
	void SimilarityAuxData::setHistogramResolution(int inputHistogramResolution)
	{
		this->histogramResolution = inputHistogramResolution;
	}

	int * SimilarityAuxData::getHistogram() const
	{
		return histogram;
	}
	
	void SimilarityAuxData::setHistogram(int * inputHistogram)
	{
		this->histogram = inputHistogram;
	}

	Vector_s& SimilarityAuxData::getShells() const
	{
		return shells;
	}

	void SimilarityAuxData::setShells(Vector_s& inputShells)
	{
		this->shells = inputShells;
	}
}