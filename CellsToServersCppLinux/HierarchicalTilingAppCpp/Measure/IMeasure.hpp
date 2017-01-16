#ifndef I_MEASURE_HPP_
#define I_MEASURE_HPP_

#include "../ArrayPartition/Coords.hpp"

using namespace ArrayPartition;

namespace Measure
{
	template<typename T>
	class IMeasure
	{
	public:
		virtual T& getAuxData() = 0;
		virtual void setAuxData(T& inputAuxData) = 0;
		virtual double computeMeasure(int partitionSize, Coords ** partition) = 0;
		virtual double computeMeasureForRegion(Coords * coords) = 0;
		virtual double computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion) = 0;
	};
}

#endif /* I_MEASURE_HPP_ */