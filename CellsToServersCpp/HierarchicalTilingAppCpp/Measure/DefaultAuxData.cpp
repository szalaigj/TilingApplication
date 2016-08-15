#include "DefaultAuxData.hpp"

namespace Measure
{
	int DefaultAuxData::getPointNO() const
	{
		return pointNO;
	}
	
	void DefaultAuxData::setPointNO(int inputPointNO)
	{
		this->pointNO = inputPointNO;
	}
}