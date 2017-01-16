#include "Coords.hpp"

namespace ArrayPartition
{
	int * Coords::getExtendedIndicesArray() const
	{
		return extendedIndicesArray;
	}

	void Coords::setExtendedIndicesArray(int * inputExtendedIndicesArray)
	{
		this->extendedIndicesArray = inputExtendedIndicesArray;
	}

	int Coords::getHeftOfRegion() const
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
		for (int idx = spaceDimension - 1; idx >= 0; idx--)
		{
			std::cout << " " << extendedIndicesArray[2 * idx + 1] << " " 
				<< extendedIndicesArray[2 * idx + 2] << " ";
		}
		std::cout << "] : " << heftOfRegion << " heft" << std::endl;
	}

	void Coords::writeToStringBuilder(int spaceDimension, std::stringstream& strBldr)
	{
		strBldr << heftOfRegion;
		for (int idx = spaceDimension - 1; idx >= 0; idx--)
		{
			strBldr << " " << extendedIndicesArray[2 * idx + 1] << " "
				<< extendedIndicesArray[2 * idx + 2];
		}
		strBldr << std::endl;
	}
}