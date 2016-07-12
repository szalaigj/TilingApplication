#ifndef PARSED_DATA_HPP_
#define PARSED_DATA_HPP_

namespace DataUtilHandling
{
	class ParsedData
	{
	public:
		ParsedData(int spaceDimension, int histogramResolution, int serverNO, int pointNO, double delta,
			int * histogram) : spaceDimension(spaceDimension), histogramResolution(histogramResolution),
			serverNO(serverNO), pointNO(pointNO), delta(delta), histogram(histogram)
		{
		}

		int getSpaceDimension();
		int getHistogramResolution();
		int getServerNO();
		int getPointNO();
		double getDelta();
		int * getHistogram();
	private:
		int spaceDimension;
		int histogramResolution;
		int serverNO;
		int pointNO;
		double delta;
		int * histogram;
	};
}

#endif /* PARSED_DATA_HPP_ */