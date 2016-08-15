#include "KNNAuxData.hpp"

namespace Measure
{
	KNNAuxData::KNNAuxData(Vector_s& shells) : SimilarityAuxData(shells)
	{
	}

	int KNNAuxData::getKNN() const
	{
		return kNN;
	}

	void KNNAuxData::setKNN(int inputKNN)
	{
		this->kNN = inputKNN;
	}

	KNNAuxData& KNNAuxData::operator= (const KNNAuxData& other)
	{
		this->setHistogram(other.getHistogram());
		this->setHistogramResolution(other.getHistogramResolution());
		this->setKNN(other.getKNN());
		this->setPointNO(other.getPointNO());
		this->setServerNO(other.getServerNO());
		this->setShells(other.getShells());
		this->setSpaceDimension(other.getSpaceDimension());
		return *this;
	}
}