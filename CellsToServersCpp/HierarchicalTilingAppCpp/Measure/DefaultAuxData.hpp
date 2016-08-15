#ifndef DEFAULT_AUX_DATA_HPP_
#define DEFAULT_AUX_DATA_HPP_

#include "BaseAuxData.hpp"

namespace Measure
{
	class DefaultAuxData : public BaseAuxData
	{
	public:
		int getPointNO() const;
		void setPointNO(int inputPointNO);
	private:
		int pointNO;
	};
}

#endif /* DEFAULT_AUX_DATA_HPP_ */