#ifndef I_AVERAGE_MEASURE_HPP_
#define I_AVERAGE_MEASURE_HPP_

#include "../ArrayPartition/Coords.hpp"

using namespace ArrayPartition;

namespace Measure
{
	class IAverageMeasure
	{
	public:
		virtual double averageAllMeasures(int partitionSize, Coords ** partition) = 0;
	};
}

#endif /* I_AVERAGE_MEASURE_HPP_ */