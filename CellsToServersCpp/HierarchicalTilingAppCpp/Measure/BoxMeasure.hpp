#ifndef BOX_MEASURE_HPP_
#define BOX_MEASURE_HPP_
#include <math.h>
#include <algorithm>

#include "IAverageMeasure.hpp"
#include "BaseMeasure.hpp"
#include "BoxAuxData.hpp"

namespace Measure
{
	class BoxMeasure : public BaseMeasure<BoxAuxData>, public IAverageMeasure
	{
	public:
		BoxMeasure(BoxAuxData& auxData, Transformator& transformator);
		double averageAllMeasures(int partitionSize, Coords ** partition);
		double computeMeasureForRegion(Coords * coords);
		double computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion);
	private:
		double computeVolumeOfQueryRegion();
		int computeHeftOfIntersection(int * indicesArrayOfServer, int * indicesArrayOfQueryRegion);
	};

	// There was a compiler error ("unresolved external symbol") without inline keyword 
	// (source: http://stackoverflow.com/a/456716)
	inline BoxMeasure::BoxMeasure(BoxAuxData& auxData, Transformator& transformator) :
		BaseMeasure<BoxAuxData>(auxData, transformator)
	{
	}

	inline double BoxMeasure::averageAllMeasures(int partitionSize, Coords ** partition)
	{
		double result = 0.0;
		int spaceDimension = getAuxData().getSpaceDimension();
		int histogramResolution = getAuxData().getHistogramResolution();
		// initializing the elements of the outerIndicesArray to zeros:
		int * outerIndicesArray = new int[spaceDimension]();
		for (;// the following expression is based on the logic of the method determineNextIndicesArray
			outerIndicesArray != nullptr;
			// retrieving the next indices array
			outerIndicesArray = transformator.determineNextIndicesArray(spaceDimension,
				histogramResolution, outerIndicesArray)
		)
		{
			int * innerIndicesArray;
			for (innerIndicesArray = transformator.copyIndicesArray(spaceDimension, 
				outerIndicesArray);
				innerIndicesArray != nullptr;
				innerIndicesArray = transformator.determineNextIndicesArray(spaceDimension,
				histogramResolution, outerIndicesArray, innerIndicesArray)
			)
			{
				int * currentIndicesArrayOfQueryRegion = transformator.mergeIndicesArrays(spaceDimension,
					outerIndicesArray, innerIndicesArray);
				getAuxData().setIndicesArrayOfQueryRegion(currentIndicesArrayOfQueryRegion);
				getAuxData().setVolumeOfQueryRegion(computeVolumeOfQueryRegion());
				result += computeMeasure(partitionSize, partition);
				delete [] currentIndicesArrayOfQueryRegion;
			}
			delete [] innerIndicesArray;
		}
		delete [] outerIndicesArray;
		result *= (pow(2.0, spaceDimension) / (pow((double)histogramResolution, spaceDimension) 
			* pow((double)histogramResolution + 1.0, spaceDimension)));
		return result;
	}

	inline double BoxMeasure::computeVolumeOfQueryRegion()
	{
		double volumeOfQueryRegion = 1.0;
		for (int dimIdx = 0; dimIdx < getAuxData().getSpaceDimension(); dimIdx++)
		{
			int lowerBoundForCurrentDim = (getAuxData().getIndicesArrayOfQueryRegion())[2 * dimIdx];
			int upperBoundForCurrentDim = (getAuxData().getIndicesArrayOfQueryRegion())[2 * dimIdx + 1];
			volumeOfQueryRegion *= (upperBoundForCurrentDim - lowerBoundForCurrentDim + 1);
		}
		return volumeOfQueryRegion;
	}

	inline double BoxMeasure::computeMeasureForRegion(Coords * coords)
	{
		int * indicesArrayOfServer = transformator.determineIndicesArray(
			getAuxData().getSpaceDimension(), coords->getExtendedIndicesArray());
		int heftOfIntersection = computeHeftOfIntersection(indicesArrayOfServer, 
			getAuxData().getIndicesArrayOfQueryRegion());
		return 1 - (double)heftOfIntersection / (double)coords->getHeftOfRegion();
	}

	inline int BoxMeasure::computeHeftOfIntersection(int * indicesArrayOfServer, 
		int * indicesArrayOfQueryRegion)
	{
		int heftOfIntersection = 0;
		bool intersected = true;
		int spaceDimension = getAuxData().getSpaceDimension();
		int * indicesArrayOfIntersection = new int[2 * spaceDimension];
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			int lowerBoundForCurrentDimOfServer = indicesArrayOfServer[2 * dimIdx];
			int upperBoundForCurrentDimOfServer = indicesArrayOfServer[2 * dimIdx + 1];
			int lowerBoundForCurrentDimOfQueryRegion = indicesArrayOfQueryRegion[2 * dimIdx];
			int upperBoundForCurrentDimOfQueryRegion = indicesArrayOfQueryRegion[2 * dimIdx + 1];
			if ((upperBoundForCurrentDimOfServer < lowerBoundForCurrentDimOfQueryRegion) ||
				(upperBoundForCurrentDimOfQueryRegion < lowerBoundForCurrentDimOfServer))
			{
				intersected = false;
				break;
			}
			indicesArrayOfIntersection[2 * dimIdx] = std::max(lowerBoundForCurrentDimOfServer,
				lowerBoundForCurrentDimOfQueryRegion);
			indicesArrayOfIntersection[2 * dimIdx + 1] = std::min(upperBoundForCurrentDimOfServer,
				upperBoundForCurrentDimOfQueryRegion);
		}
		if (intersected)
		{
			int currentHeftCellIdx = transformator.calculateCellIdx(2 * spaceDimension,
					getAuxData().getHistogramResolution(), indicesArrayOfIntersection);
			heftOfIntersection = (int)(getAuxData().getHeftArray())[currentHeftCellIdx];
		}
		return heftOfIntersection;
	}

	inline double BoxMeasure::computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion)
	{
		// This method is unused for box measure
		 throw std::logic_error("Not Implemented");
	}
}
#endif /* BOX_MEASURE_HPP_ */