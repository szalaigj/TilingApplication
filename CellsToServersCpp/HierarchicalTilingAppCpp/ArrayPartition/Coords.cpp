#include "Coords.hpp"

namespace ArrayPartition
{
	int * Coords::getExtendedIndicesArray()
	{
		return extendedIndicesArray;
	}

	void Coords::setExtendedIndicesArray(int * inputExtendedIndicesArray)
	{
		this->extendedIndicesArray = inputExtendedIndicesArray;
	}

	int Coords::getHeftOfRegion()
	{
		return heftOfRegion;
	}

	void Coords::setHeftOfRegion(int inputHeftOfRegion)
	{
		this->heftOfRegion = inputHeftOfRegion;
	}

	double Coords::differenceFromDelta(double delta)
	{
		return abs(delta - heftOfRegion);
	}

	void Coords::printCoords(int spaceDimension, int serialNO)
	{
		std::cout << serialNO << ". tile: [";
		for (int idx = 0; idx < spaceDimension; idx++)
		{
			std::cout << " " << extendedIndicesArray[2 * idx + 1] << " " 
				<< extendedIndicesArray[2 * idx + 2] << " ";
		}
		std::cout << "] : " << heftOfRegion << " heft" << std::endl;
	}

	void Coords::writeToStringBuilder(int spaceDimension, std::stringstream& strBldr)
	{
		strBldr << heftOfRegion;
		for (int idx = 0; idx < spaceDimension; idx++)
		{
			strBldr << " " << extendedIndicesArray[2 * idx + 1] << " "
				<< extendedIndicesArray[2 * idx + 2];
		}
		strBldr << std::endl;
		//std::cout << strBldr.str();
	}
}