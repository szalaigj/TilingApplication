// standard library includes
#include <iostream>
#include <time.h>
#include <math.h>

#include "DataHandlingUtil/InputParser.hpp"
#include "DataHandlingUtil/OutputWriter.hpp"
#include "SumOfSquares/IntTuple.hpp"
#include "SumOfSquares/Shell.hpp"
#include "SumOfSquares/CornacchiaMethod.hpp"
#include "SumOfSquares/BacktrackingMethod.hpp"
#include "SumOfSquares/ShellBuilder.hpp"
#include "Transformation/Transformator.hpp"
#include "ArrayPartition/HeftArrayCreator.hpp"
#include "ArrayPartition/IterativeDivider.hpp"

const std::string& tilesOutput = "u:/temp/data/hier_tiling/tiles.dat";
const std::string& serversOutput = "u:/temp/data/hier_tiling/servers.dat";
const std::string& cellsToServersOutput = "u:/temp/data/hier_tiling/cells_to_servers.dat";

int main(int argc, char** argv)
{
    clock_t begin, end;
    double elapsed_secs;
    DataHandlingUtil::InputParser parser;
	DataHandlingUtil::OutputWriter outputWriter;
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

	ArrayPartition::IterativeDivider iterativeDivider(histogram, heftArray, transformator, parsedData,
		kNN, maxRange, shellsForKNN, shellsForRange);
	Vector_coords partition;
	double objectiveValue = iterativeDivider.determineObjectiveValue(partition);
	std::cout << "Objective value: " << objectiveValue << std::endl;
	std::cout << "Sum of differences between tile hefts and delta: " << iterativeDivider.getDiffSum()
		<< std::endl;
	outputWriter.writeOutTiles(spaceDimension, partition, tilesOutput);
	outputWriter.writeOutServers(partition, serversOutput);
	outputWriter.writeOutCellsToServers(histogramResolution, partition, cellsToServersOutput);
	
    end = clock();
    elapsed_secs = double(end - begin) / CLOCKS_PER_SEC;
    std::cout << "Elapsed time (in min) of the input parsing" << " " << (elapsed_secs / 60.0) << std::endl;
    std::cout << "Press any character and press enter to continue..." << std::endl;
    char chr;
    std::cin >> chr;
}