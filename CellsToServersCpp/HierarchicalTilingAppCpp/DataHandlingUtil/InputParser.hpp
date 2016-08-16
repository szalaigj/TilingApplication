#ifndef INPUT_PARSER_HPP_
#define INPUT_PARSER_HPP_

// standard includes
#include <iostream>
#include <math.h>
#include <fstream>
#include <string>
#include <sstream>
#include <stdexcept>
#include "ParsedData.hpp"

namespace DataHandlingUtil
{
	class InputParser
	{
	public:
		ParsedData& parseInputFile();
	private:
		int * parseHistogram(std::stringstream& binStringstream, int serverNO, int binNO,
			int& pointNO, double& delta);
	};
}
#endif /* INPUT_PARSER_HPP_ */