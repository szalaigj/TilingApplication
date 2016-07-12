// standard library includes
#include <iostream>
#include <time.h>

#include "DataHandlingUtil/InputParser.hpp"

int main(int argc, char** argv)
{
	clock_t begin, end;
	double elapsed_secs;
	DataUtilHandling::InputParser parser;
	begin = clock();
	DataUtilHandling::ParsedData parsedData = parser.parseInputFile();
	end = clock();
	elapsed_secs = double(end - begin) / CLOCKS_PER_SEC;
	std::cout << "Elapsed time (in min) of the input parsing" << " " << (elapsed_secs / 60.0) << std::endl;
	int * histogram = parsedData.getHistogram();
	int bin = histogram[0];
	std::cout << "Press any character and press enter to continue..." << std::endl;
	char chr;
	std::cin >> chr;
}