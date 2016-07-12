#include "InputParser.hpp"

namespace DataUtilHandling
{
	ParsedData& InputParser::parseInputFile()
	{
		ParsedData * result;
		std::string filename, line;
		int spaceDimension, histogramResolution, serverNO, pointNO;
		double delta;
		int * histogram;
		std::cout << "Enter the input path and filename:" << std::endl;
		std::getline(std::cin, filename);
		std::ifstream inputfile(filename.c_str());
		if (inputfile.good())
		{
			std::getline(inputfile, line);
			spaceDimension = atoi(line.c_str());
			std::getline(inputfile, line);
			histogramResolution = atoi(line.c_str());
			std::getline(inputfile, line);
			serverNO = atoi(line.c_str());
			std::getline(inputfile, line);
			std::stringstream binStringstream(line);
			int binNO = (int)pow(histogramResolution, spaceDimension);
			histogram = parseHistogram(binStringstream, serverNO, binNO, pointNO, delta);
			std::cout << "Space dim: " << spaceDimension << ", resolution: " << histogramResolution 
				<< ", server no.: " << serverNO << std::endl;
			result = new ParsedData(spaceDimension, histogramResolution, serverNO, pointNO, delta,
					 histogram);
		}
		else
		{
			std::string errmsg("File ");
			errmsg.append(filename);
			errmsg.append(" does not exist.\n");
			throw std::runtime_error(errmsg);
		}
		return *result;
	}

	int * InputParser::parseHistogram(std::stringstream& binStringstream, int serverNO, int binNO,
		int& pointNO, double& delta)
	{
		std::string binStr;
		int * histogram = new int[binNO];
		pointNO = 0;
		int binIdx = 0;
		int binValue;
		int binMaxValue = 0;
		while (getline(binStringstream, binStr, ' '))
		{
			binValue = atoi(binStr.c_str());
			if (binMaxValue < binValue)
			{
				binMaxValue = binValue;
			}
			pointNO += binValue;
			histogram[binIdx] = binValue;
			binIdx++;
		}
		delta = (double)pointNO / (double)serverNO;
		if (binMaxValue > delta)
		{
			std::cout << "WARNING: There is a bin which has greater heft value than delta."
				<< std::endl;
		}
		return histogram;
	}
}