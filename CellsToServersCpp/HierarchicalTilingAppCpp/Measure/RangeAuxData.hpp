#ifndef RANGE_AUX_DATA_HPP_
#define RANGE_AUX_DATA_HPP_

#include "SimilarityAuxData.hpp"

namespace Measure
{
	class RangeAuxData : public SimilarityAuxData
	{
	public:
		RangeAuxData(Vector_s& shells);
		int getMaxRange() const;
		void setMaxRange(int inputMaxRange);
		int getRange() const;
		void setRange(int inputRange);
		RangeAuxData& operator= (const RangeAuxData& other);
	private:
		int maxRange;
		int range;
	};
}

#endif /* RANGE_AUX_DATA_HPP_ */