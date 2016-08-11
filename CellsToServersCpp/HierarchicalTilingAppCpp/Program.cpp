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

int main(int argc, char** argv)
{
    clock_t begin, end;
    double elapsed_secs;
    DataUtilHandling::InputParser parser;
    SumOfSquares::CornacchiaMethod cornacchiaMethod;
    SumOfSquares::BacktrackingMethod backtrackingMethod(cornacchiaMethod);
    Transformation::Transformator transformator;
    begin = clock();
    DataUtilHandling::ParsedData parsedData = parser.parseInputFile();
    //SumOfSquares::Vector_t intTuples = backtrackingMethod.decomposeByBacktracking(14, 3);
    //for (size_t idx = 0; idx < intTuples.size(); idx++)
    //{
    //    int * tuple = intTuples[idx]->getTuple();
    //    std::cout << "(" << tuple[0] << "," << tuple[1] << "," << tuple[2] << ")" << std::endl;
    //}
    SumOfSquares::ShellBuilder shellBuilder(backtrackingMethod);
    int spaceDimension = parsedData.getSpaceDimension();
    int histogramResolution = parsedData.getHistogramResolution();
    size_t kNN = (size_t)ceil(parsedData.getDelta());
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
    end = clock();
    elapsed_secs = double(end - begin) / CLOCKS_PER_SEC;
    std::cout << "Elapsed time (in min) of the input parsing" << " " << (elapsed_secs / 60.0) << std::endl;
    int * histogram = parsedData.getHistogram();
    int bin = histogram[0];
    std::cout << "Press any character and press enter to continue..." << std::endl;
    char chr;
    std::cin >> chr;
}