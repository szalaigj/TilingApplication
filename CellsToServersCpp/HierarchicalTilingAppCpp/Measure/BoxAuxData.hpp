#ifndef BOX_AUX_DATA_HPP_
#define BOX_AUX_DATA_HPP_

#include "BaseAuxData.hpp"

namespace Measure
{
	class BoxAuxData : public BaseAuxData
	{
	public:
		int getSpaceDimension();
		void setSpaceDimension(int inputSpaceDimension);
		int getHistogramResolution();
		void setHistogramResolution(int inputHistogramResolution);
		int * getHistogram();
		void setHistogram(int * inputHistogram);
		int * getHeftArray();
		void setHeftArray(int * inputHeftArray);
		int * getIndicesArrayOfQueryRegion();
		void setIndicesArrayOfQueryRegion(int * inputIndicesArrayOfQueryRegion);
		double getVolumeOfQueryRegion();
		void setVolumeOfQueryRegion(double inputVolumeOfQueryRegion);
	private:
		int spaceDimension;
		int histogramResolution;
		int * histogram;
		int * heftArray;
		int * indicesArrayOfQueryRegion;
		double volumeOfQueryRegion;
	};
}

#endif /* BOX_AUX_DATA_HPP_ */