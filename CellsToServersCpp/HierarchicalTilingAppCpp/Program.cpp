// standard library includes
#include <iostream>
#include <time.h>
#include <math.h>

#include "DataHandlingUtil/InputParser.hpp"
#include "SumOfSquares/IntTuple.hpp"
#include "SumOfSquares/Shell.hpp"
#include "SumOfSquares/CornacchiaMethod.hpp"
#include "SumOfSquares/BacktrackingMethod.hpp"
#include "SumOfSquares/ShellBuilder.hpp"
#include "Transformation/Transformator.hpp"
#include "ArrayPartition/HeftArrayCreator.hpp"
#include "ArrayPartition/IterativeDivider.hpp"

int main(int argc, char** argv)
{
    clock_t begin, end;
    double elapsed_secs;
    DataHandlingUtil::InputParser parser;
    SumOfSquares::CornacchiaMethod cornacchiaMethod;
    SumOfSquares::BacktrackingMethod backtrackingMethod(cornacchiaMethod);
    Transformation::Transformator transformator;
    begin = clock();
    DataHandlingUtil::ParsedData parsedData = parser.parseInputFile();
	SumOfSquares::ShellBuilder shellBuilder(backtrackingMethod);
	ArrayPartition::HeftArrayCreator heftArrayCreator(transformator);
    int spaceDimension = parsedData.getSpaceDimension();
    int histogramResolution = parsedData.getHistogramResolution();
	double delta = parsedData.getDelta();
	int pointNO = parsedData.getPointNO();
	int serverNO = parsedData.getServerNO();
	int * histogram = parsedData.getHistogram();
    int * heftArray = heftArrayCreator.createHeftArray(spaceDimension, histogramResolution, histogram);

    size_t kNN = (size_t)ceil(delta);
    size_t maxShellNO = transformator.determineMaxRange(spaceDimension, histogramResolution);
    size_t maxRange = transformator.determineMaxRange(spaceDimension, histogramResolution / 2);
    SumOfSquares::Vector_s shellsForKNN = shellBuilder.createShells(maxShellNO, spaceDimension);
	SumOfSquares::Vector_s shellsForRange = shellBuilder.createShells(maxRange, spaceDimension);
    for (size_t idx = 0; idx < shellsForKNN.size(); idx++)
    {
        std::cout << "Shell " << idx << ":" << std::endl;
        SumOfSquares::Vector_t currentIntTuples = shellsForKNN[idx]->getIntTuples();
        for (size_t subidx = 0; subidx < currentIntTuples.size(); subidx++)
        {
            int * tuple = currentIntTuples[subidx]->getTuple();
            std::cout << "(" << tuple[0] << "," << tuple[1] << ")" << std::endl;
        }
    }

	ArrayPartition::IterativeDivider iterativeDivider(histogram, heftArray, transformator, parsedData,
		kNN, maxRange, shellsForKNN, shellsForRange);

    end = clock();
    elapsed_secs = double(end - begin) / CLOCKS_PER_SEC;
    std::cout << "Elapsed time (in min) of the input parsing" << " " << (elapsed_secs / 60.0) << std::endl;
    std::cout << "Press any character and press enter to continue..." << std::endl;
    char chr;
    std::cin >> chr;
}