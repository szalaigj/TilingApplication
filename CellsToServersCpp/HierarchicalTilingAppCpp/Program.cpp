// standard library includes
#include <iostream>
#include <time.h>

#include "DataHandlingUtil/InputParser.hpp"
#include "SumOfSquares/CornacchiaMethod.hpp"
#include "SumOfSquares/BacktrackingMethod.hpp"

int main(int argc, char** argv)
{
	clock_t begin, end;
	double elapsed_secs;
	DataUtilHandling::InputParser parser;
	SumOfSquares::CornacchiaMethod cornacchiaMethod;
	SumOfSquares::BacktrackingMethod backtrackingMethod(cornacchiaMethod);
	begin = clock();
	DataUtilHandling::ParsedData parsedData = parser.parseInputFile();
	SumOfSquares::Vector_t intTuples = backtrackingMethod.decomposeByBacktracking(14, 3);
	for (size_t idx = 0; idx < intTuples.size(); idx++)
	{
		int * tuple = intTuples[idx]->getTuple();
		std::cout << "(" << tuple[0] << "," << tuple[1] << "," << tuple[2] << ")" << std::endl;
	}
	end = clock();
	elapsed_secs = double(end - begin) / CLOCKS_PER_SEC;
	std::cout << "Elapsed time (in min) of the input parsing" << " " << (elapsed_secs / 60.0) << std::endl;
	int * histogram = parsedData.getHistogram();
	int bin = histogram[0];
	std::cout << "Press any character and press enter to continue..." << std::endl;
	char chr;
	std::cin >> chr;
}