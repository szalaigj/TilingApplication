#include "OutputWriter.hpp"

namespace DataHandlingUtil
{
	void OutputWriter::writeOutTiles(int spaceDimension, Vector_coords& partition, 
		const std::string& inputTilesOutput)
	{
		std::stringstream strBldr;
		int serialNO = 1;
		for(Vector_coords::iterator itr = partition.begin(); itr != partition.end(); itr++)
		{
			Coords * coords = *itr;
			coords->printCoords(spaceDimension, serialNO);
			coords->writeToStringBuilder(spaceDimension, strBldr);
			serialNO++;
		}
		writeAllText(inputTilesOutput, strBldr);
	}

	void OutputWriter::writeOutServers(Vector_coords& partition, const std::string& inputServersOutput)
	{
		std::stringstream strBldr;
		int serverIdx = 0;
		for(Vector_coords::iterator itr = partition.begin(); itr != partition.end(); itr++)
		{
			Coords * coords = *itr;
			strBldr << coords->getHeftOfRegion() << " " << serverIdx << std::endl;
			serverIdx++;
		}
		writeAllText(inputServersOutput, strBldr);
	}

	void OutputWriter::writeOutCellsToServers(int histogramResolution, Vector_coords& partition,
		const std::string& inputCellsToServersOutput)
	{
		std::stringstream strBldr;
		int serverIdx = 0;
		for(Vector_coords::iterator itr = partition.begin(); itr != partition.end(); itr++)
		{
			Coords * coords = *itr;
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
			serverIdx++;
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