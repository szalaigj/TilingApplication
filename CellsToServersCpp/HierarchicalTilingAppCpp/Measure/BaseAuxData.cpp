#include "BaseAuxData.hpp"

namespace Measure
{
	int BaseAuxData::getServerNO() const
	{
		return serverNO;
	}

	void BaseAuxData::setServerNO(int inputServerNO)
	{
		this->serverNO = inputServerNO;
	}
}