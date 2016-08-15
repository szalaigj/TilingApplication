#ifndef COORDS_HPP_
#define COORDS_HPP_
#include <math.h>
#include <string>
#include <iostream>
#include <sstream>
#include <vector>

namespace ArrayPartition
{
	class Coords
	{
	public:
		int * getExtendedIndicesArray();
		void setExtendedIndicesArray(int * inputExtendedIndicesArray);
		int getHeftOfRegion();
		void setHeftOfRegion(int inputHeftOfRegion);
		double differenceFromDelta(double delta);
		void printCoords(int spaceDimension, int serialNO);
		void writeToStringBuilder(int spaceDimension, std::stringstream& strBldr);
	private:
		int * extendedIndicesArray;
		int heftOfRegion;
	};

	typedef std::vector<Coords *> Vector_coords;
}

#endif /* COORDS_HPP_ */