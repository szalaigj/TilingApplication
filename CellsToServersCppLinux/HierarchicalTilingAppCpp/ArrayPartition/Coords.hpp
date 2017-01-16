#ifndef COORDS_HPP_
#define COORDS_HPP_
#include <math.h>
#include <stdlib.h>
#include <string>
#include <iostream>
#include <sstream>
#include <vector>

namespace ArrayPartition
{
	class Coords
	{
	public:
		int * getExtendedIndicesArray() const;
		void setExtendedIndicesArray(int * inputExtendedIndicesArray);
		int getHeftOfRegion() const;
		void setHeftOfRegion(int inputHeftOfRegion);
		double differenceFromDelta(double delta);
		void printCoords(int spaceDimension, int serialNO);
		void writeToStringBuilder(int spaceDimension, std::stringstream& strBldr);
	private:
		int * extendedIndicesArray;
		int heftOfRegion;
	};
}

#endif /* COORDS_HPP_ */
