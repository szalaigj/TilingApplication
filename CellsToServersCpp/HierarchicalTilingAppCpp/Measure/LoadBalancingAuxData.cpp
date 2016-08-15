#include "LoadBalancingAuxData.hpp"

namespace Measure
{
	double LoadBalancingAuxData::getDelta() const
	{
		return delta;
	}
	
	void LoadBalancingAuxData::setDelta(double inputDelta)
	{
		this->delta = inputDelta;
	}

	LoadBalancingAuxData& LoadBalancingAuxData::operator= (const LoadBalancingAuxData& other)
	{
		this->setDelta(other.getDelta());
		this->setPointNO(other.getPointNO());
		this->setServerNO(other.getServerNO());
		return *this;
	}
}