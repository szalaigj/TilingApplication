#ifndef KNN_AUX_DATA_HPP_
#define KNN_AUX_DATA_HPP_

#include "SimilarityAuxData.hpp"

namespace Measure
{
	class KNNAuxData : public SimilarityAuxData
	{
	public:
		KNNAuxData(Vector_s& shells);
		int getKNN() const;
		void setKNN(int inputKNN);
		KNNAuxData& operator= (const KNNAuxData& other);
	private:
		int kNN;
	};
}

#endif /* KNN_AUX_DATA_HPP_ */