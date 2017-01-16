#include "ParsedData.hpp"

namespace DataHandlingUtil
{
	ParsedData::~ParsedData()
	{
		if (histogram)
		{
			delete [] histogram;
		}
	}

	int ParsedData::getSpaceDimension() const
	{
		return spaceDimension;
	}

	int ParsedData::getHistogramResolution() const
	{
		return histogramResolution;
	}

	int ParsedData::getServerNO() const
	{
		return serverNO;
	}

	int ParsedData::getPointNO() const
	{
		return pointNO;
	}

	double ParsedData::getDelta() const
	{
		return delta;
	}

	double ParsedData::getKNNMeasCoeff() const
	{
		return kNNMeasCoeff;
	}

	double ParsedData::getLbMeasCoeff() const
	{
		return lbMeasCoeff;
	}

	int * ParsedData::getHistogram() const
	{
		return histogram;
	}
}