#ifndef SIMILARITY_AUX_DATA_HPP_
#define SIMILARITY_AUX_DATA_HPP_

#include "DefaultAuxData.hpp"
#include "../SumOfSquares/Shell.hpp"

using namespace SumOfSquares;

namespace Measure
{
	class SimilarityAuxData : public DefaultAuxData
	{
	public:
		SimilarityAuxData(Vector_s& shells);
		int getSpaceDimension() const;
		void setSpaceDimension(int inputSpaceDimension);
		int getHistogramResolution() const;
		void setHistogramResolution(int inputHistogramResolution);
		int * getHistogram() const;
		void setHistogram(int * inputHistogram);
		Vector_s& getShells() const;
		void setShells(Vector_s& inputShells);
	private:
		int spaceDimension;
		int histogramResolution;
		int * histogram;
		Vector_s& shells;
	};
}

#endif /* SIMILARITY_AUX_DATA_HPP_ */