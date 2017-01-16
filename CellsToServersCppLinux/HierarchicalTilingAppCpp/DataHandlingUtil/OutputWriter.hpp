#ifndef OUTPUT_WRITER_HPP_
#define OUTPUT_WRITER_HPP_
#include <string>
#include <sstream>
#include <fstream>

#include "../ArrayPartition/Coords.hpp"

using namespace ArrayPartition;

namespace DataHandlingUtil
{
	class OutputWriter
	{
	public:
		void writeOutTiles(int spaceDimension, int serverNO, Coords ** partition,
			const std::string& inputTilesOutput);
		void writeOutServers(int serverNO, Coords ** partition, const std::string& inputServersOutput);
		void writeOutCellsToServers(int histogramResolution, int serverNO, Coords ** partition, 
			const std::string& inputCellsToServersOutput);
	private:
		void writeAllText(const std::string& path, std::stringstream& strBldr);
	};
}

#endif /* OUTPUT_WRITER_HPP_ */