#ifndef RANGE_MEASURE_HPP_
#define RANGE_MEASURE_HPP_

#include "IAverageMeasure.hpp"
#include "SimilarityMeasure.hpp"
#include "RangeAuxData.hpp"
#include "../SumOfSquares/Shell.hpp"

using namespace SumOfSquares;

namespace Measure
{
	class RangeMeasure : public SimilarityMeasure<RangeAuxData>, public IAverageMeasure
	{
	public:
		RangeMeasure(RangeAuxData& auxData, Transformator& transformator);
		double averageAllMeasures(Vector_coords& partition);
		double computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion);
	private:
		double iterateOverShells(int * indicesArrayOfRegion, Dictionary_s * dictOfShells);
	};

	// There was a compiler error ("unresolved external symbol") without inline keyword 
	// (source: http://stackoverflow.com/a/456716)
	inline RangeMeasure::RangeMeasure(RangeAuxData& auxData, Transformator& transformator) :
		SimilarityMeasure<RangeAuxData>(auxData, transformator)
	{
	}

	inline double RangeMeasure::averageAllMeasures(Vector_coords& partition)
	{
		double result = 0.0;
		int maxRange = getAuxData().getMaxRange();
		for (int rangeIdx = 1; rangeIdx <= maxRange; rangeIdx++)
		{
			getAuxData().setRange(rangeIdx);
			result += computeMeasure(partition);
		}
		result /= (double)maxRange;
		return result;
	}
	
	inline double RangeMeasure::computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion)
	{
		double measureForBin = 0.0;
		int spaceDimension = getAuxData().getSpaceDimension();
		int histogramResolution = getAuxData().getHistogramResolution();
		Vector_s shells = getAuxData().getShells();
		int currentCellIdx = transformator.calculateCellIdx(spaceDimension, histogramResolution, 
			indicesArrayOfBin);
		int binValue = (int)(getAuxData().getHistogram())[currentCellIdx];
		if (binValue > 0)
		{
			Vector_s::const_iterator first = shells.begin();
			Vector_s::const_iterator last = shells.begin() + getAuxData().getRange();
			Vector_s currentShells(first, last);
			Dictionary_s * dictOfShells = transformator.convertIntPairsOfShellsToListOfIdxArrays(
                        histogramResolution, indicesArrayOfBin, currentShells);
			measureForBin = iterateOverShells(indicesArrayOfRegion, dictOfShells);
		}
		return measureForBin;
	}

	inline double RangeMeasure::iterateOverShells(int * indicesArrayOfRegion, Dictionary_s * dictOfShells)
	{
		int pointsInServer = 0;
		int pointsOutServer = 0;
		for(Dictionary_s::iterator itr = dictOfShells->begin(); itr != dictOfShells->end(); itr++)
		{
			int shellIdx = itr->first;
			Shell_idxs * idxArraysOnCurrentShell = itr->second;
			for (Shell_idxs::iterator itr = idxArraysOnCurrentShell->begin(); 
				itr != idxArraysOnCurrentShell->end(); itr++)
			{
				int * idxArray = *itr;
				int currentCellIdx = transformator.calculateCellIdx(getAuxData().getSpaceDimension(),
					getAuxData().getHistogramResolution(), idxArray);
				int currentBinValue = (int)(getAuxData().getHistogram())[currentCellIdx];
				if (isIdxArrayInTheRegion(idxArray, indicesArrayOfRegion))
				{
					pointsInServer += currentBinValue;
				}
				else
				{
					pointsOutServer += currentBinValue;
				}
			}
		}
		double measureForBin = (double)pointsInServer / (double)(pointsInServer + pointsOutServer);
		return measureForBin;
	}
}
#endif /* RANGE_MEASURE_HPP_ */