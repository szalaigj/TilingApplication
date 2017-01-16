#ifndef LOAD_BALANCING_AUX_DATA_HPP_
#define LOAD_BALANCING_AUX_DATA_HPP_

#include "DefaultAuxData.hpp"

namespace Measure
{
	class LoadBalancingAuxData : public DefaultAuxData
	{
	public:
		double getDelta() const;
		void setDelta(double inputDelta);
		LoadBalancingAuxData& operator= (const LoadBalancingAuxData& other);
	private:
		double delta;
	};
}

#endif /* LOAD_BALANCING_AUX_DATA_HPP_ */