#include "RangeAuxData.hpp"

namespace Measure
{
	RangeAuxData::RangeAuxData(Vector_s& shells) : SimilarityAuxData(shells)
	{
	}

	int RangeAuxData::getMaxRange() const
	{
		return maxRange;
	}

	void RangeAuxData::setMaxRange(int inputMaxRange)
	{
		this->maxRange = inputMaxRange;
	}

	int RangeAuxData::getRange() const
	{
		return range;
	}

	void RangeAuxData::setRange(int inputRange)
	{
		this->range = inputRange;
	}

	RangeAuxData& RangeAuxData::operator= (const RangeAuxData& other)
	{
		this->setHistogram(other.getHistogram());
		this->setHistogramResolution(other.getHistogramResolution());
		this->setMaxRange(other.getMaxRange());
		this->setPointNO(other.getPointNO());
		this->setRange(other.getRange());
		this->setServerNO(other.getServerNO());
		this->setShells(other.getShells());
		this->setSpaceDimension(other.getSpaceDimension());
		return *this;
	}
}