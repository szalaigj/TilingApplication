#include "ParsedData.hpp"

namespace DataUtilHandling
{
	ParsedData::~ParsedData()
	{
		if (histogram)
		{
			delete [] histogram;
		}
	}

	int ParsedData::getSpaceDimension()
	{
		return spaceDimension;
	}

	int ParsedData::getHistogramResolution()
	{
		return histogramResolution;
	}

	int ParsedData::getServerNO()
	{
		return serverNO;
	}

	int ParsedData::getPointNO()
	{
		return pointNO;
	}

	double ParsedData::getDelta()
	{
		return delta;
	}

	int * ParsedData::getHistogram()
	{
		return histogram;
	}
}