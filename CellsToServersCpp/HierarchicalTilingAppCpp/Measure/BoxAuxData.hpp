#ifndef BOX_AUX_DATA_HPP_
#define BOX_AUX_DATA_HPP_

#include "BaseAuxData.hpp"

namespace Measure
{
	class BoxAuxData : public BaseAuxData
	{
	public:
		int getSpaceDimension() const;
		void setSpaceDimension(int inputSpaceDimension);
		int getHistogramResolution() const;
		void setHistogramResolution(int inputHistogramResolution);
		int * getHistogram() const;
		void setHistogram(int * inputHistogram);
		int * getHeftArray() const;
		void setHeftArray(int * inputHeftArray);
		int * getIndicesArrayOfQueryRegion() const;
		void setIndicesArrayOfQueryRegion(int * inputIndicesArrayOfQueryRegion);
		double getVolumeOfQueryRegion() const;
		void setVolumeOfQueryRegion(double inputVolumeOfQueryRegion);
		BoxAuxData& operator= (const BoxAuxData& other);
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