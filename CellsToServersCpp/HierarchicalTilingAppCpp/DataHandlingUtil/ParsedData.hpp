#ifndef PARSED_DATA_HPP_
#define PARSED_DATA_HPP_

namespace DataHandlingUtil
{
	class ParsedData
	{
	public:
		ParsedData(int spaceDimension, int histogramResolution, int serverNO, int pointNO, double delta,
			double kNNMeasCoeff, double lbMeasCoeff, int * histogram) : spaceDimension(spaceDimension), 
			histogramResolution(histogramResolution), serverNO(serverNO), pointNO(pointNO), 
			kNNMeasCoeff(kNNMeasCoeff), lbMeasCoeff(lbMeasCoeff), delta(delta), histogram(histogram)
		{
		}
		~ParsedData();

		int getSpaceDimension() const;
		int getHistogramResolution() const;
		int getServerNO() const;
		int getPointNO() const;
		double getDelta() const;
		double getKNNMeasCoeff() const;
		double getLbMeasCoeff() const;
		int * getHistogram() const;
	private:
		int spaceDimension;
		int histogramResolution;
		int serverNO;
		int pointNO;
		double delta;
		double kNNMeasCoeff;
		double lbMeasCoeff;
		int * histogram;
	};
}

#endif /* PARSED_DATA_HPP_ */