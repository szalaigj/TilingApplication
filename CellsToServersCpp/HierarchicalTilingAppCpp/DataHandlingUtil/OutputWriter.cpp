#include "OutputWriter.hpp"

namespace DataHandlingUtil
{
	void OutputWriter::writeOutTiles(int spaceDimension, int serverNO, Coords ** partition,
		const std::string& inputTilesOutput)
	{
		std::stringstream strBldr;
		for (int serverIdx = 0; serverIdx < serverNO; serverIdx++)
		{
			Coords * coords = partition[serverIdx];
			coords->printCoords(spaceDimension, serverIdx + 1);
			coords->writeToStringBuilder(spaceDimension, strBldr);
		}
		writeAllText(inputTilesOutput, strBldr);
	}

	void OutputWriter::writeOutServers(int serverNO, Coords ** partition,
		const std::string& inputServersOutput)
	{
		std::stringstream strBldr;
		for (int serverIdx = 0; serverIdx < serverNO; serverIdx++)
		{
			Coords * coords = partition[serverIdx];
			strBldr << coords->getHeftOfRegion() << " " << serverIdx << std::endl;
		}
		writeAllText(inputServersOutput, strBldr);
	}

	void OutputWriter::writeOutCellsToServers(int histogramResolution, int serverNO, 
		Coords ** partition, const std::string& inputCellsToServersOutput)
	{
		std::stringstream strBldr;
		for (int serverIdx = 0; serverIdx < serverNO; serverIdx++)
		{
			Coords * coords = partition[serverIdx];
			int * extendedIndicesArray = coords->getExtendedIndicesArray();
			for (int x = extendedIndicesArray[1]; x <= extendedIndicesArray[2]; x++)
			{
				for (int y = extendedIndicesArray[3]; y <= extendedIndicesArray[4]; y++)
				{
					int cellIdx = x * histogramResolution + y;
					strBldr << cellIdx;
					if ((x != extendedIndicesArray[2]) || (y != extendedIndicesArray[4]))
						strBldr << " ";
				}
			}
			strBldr << std::endl;
		}
		writeAllText(inputCellsToServersOutput, strBldr);
	}

	void OutputWriter::writeAllText(const std::string& path, std::stringstream& strBldr)
	{
		std::ofstream outputFilePtr;
		outputFilePtr.open(path);
		outputFilePtr << strBldr.str().c_str();
		outputFilePtr.close();
	}
}