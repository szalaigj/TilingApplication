#include "RangeAuxData.hpp"

namespace Measure
{
	RangeAuxData::RangeAuxData(Vector_s& shells) : SimilarityAuxData(shells)
	{
	}

	int RangeAuxData::getMaxRange()
	{
		return maxRange;
	}

	void RangeAuxData::setMaxRange(int inputMaxRange)
	{
		this->maxRange = inputMaxRange;
	}

	int RangeAuxData::getRange()
	{
		return range;
	}

	void RangeAuxData::setRange(int inputRange)
	{
		this->range = inputRange;
	}
}