#ifndef RANGE_AUX_DATA_HPP_
#define RANGE_AUX_DATA_HPP_

#include "SimilarityAuxData.hpp"

namespace Measure
{
	class RangeAuxData : public SimilarityAuxData
	{
	public:
		RangeAuxData(Vector_s& shells);
		int getMaxRange();
		void setMaxRange(int inputMaxRange);
		int getRange();
		void setRange(int inputRange);
	private:
		int maxRange;
		int range;
	};
}

#endif /* RANGE_AUX_DATA_HPP_ */