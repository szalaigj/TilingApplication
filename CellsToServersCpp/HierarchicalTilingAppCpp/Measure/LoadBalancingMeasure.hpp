#ifndef LOAD_BALANCING_MEASURE_HPP_
#define LOAD_BALANCING_MEASURE_HPP_

#include "BaseMeasure.hpp"
#include "LoadBalancingAuxData.hpp"

using namespace Transformation;

namespace Measure
{
	class LoadBalancingMeasure : public BaseMeasure<LoadBalancingAuxData>
	{
	public:
		LoadBalancingMeasure(LoadBalancingAuxData& auxData, Transformator& transformator);
		double computeMeasureForRegion(Coords& coords);
		double computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion);
	};

	// There was a compiler error ("unresolved external symbol") without inline keyword 
	// (source: http://stackoverflow.com/a/456716)
	inline LoadBalancingMeasure::LoadBalancingMeasure(LoadBalancingAuxData& auxData, Transformator& transformator) :
		BaseMeasure<LoadBalancingAuxData>(auxData, transformator)
	{
	}

	inline double LoadBalancingMeasure::computeMeasureForRegion(Coords& coords)
	{
		return 1 - (coords.differenceFromDelta(auxData.getDelta()) / (double)auxData.getPointNO());
	}

	inline double LoadBalancingMeasure::computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion)
	{
		// This method is unused for load balancing measure
		 throw std::logic_error("Not Implemented");
	}
}
#endif /* LOAD_BALANCING_MEASURE_HPP_ */