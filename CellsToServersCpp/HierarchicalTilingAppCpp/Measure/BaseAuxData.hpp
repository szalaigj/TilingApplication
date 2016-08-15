#ifndef BASE_AUX_DATA_HPP_
#define BASE_AUX_DATA_HPP_

namespace Measure
{
	class BaseAuxData
	{
	public:
		int getServerNO() const;
		void setServerNO(int inputServerNO);
	private:
		int serverNO;
	};
}

#endif /* BASE_AUX_DATA_HPP_ */