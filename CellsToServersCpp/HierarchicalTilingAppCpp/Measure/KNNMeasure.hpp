#ifndef KNN_MEASURE_HPP_
#define KNN_MEASURE_HPP_

#include "SimilarityMeasure.hpp"
#include "KNNAuxData.hpp"
#include "../SumOfSquares/Shell.hpp"

using namespace SumOfSquares;

namespace Measure
{
	class KNNMeasure : public SimilarityMeasure<KNNAuxData>
	{
	public:
		KNNMeasure(KNNAuxData& auxData, Transformator& transformator);
		double computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion);
	private:
		void iterateOverShells(int * indicesArrayOfRegion, int kNN, int& nnInServer, int& nnOutserver,
			Dictionary_s * dictOfShells);
		void updateNNCountsWithBinsOnCurrentShell(int * indicesArrayOfRegion, int& nnInServer, 
            int& tmpNNOutServer, Shell_idxs * idxArraysOnCurrentShell);
	};

	// There was a compiler error ("unresolved external symbol") without inline keyword 
	// (source: http://stackoverflow.com/a/456716)
	inline KNNMeasure::KNNMeasure(KNNAuxData& auxData, Transformator& transformator) :
		SimilarityMeasure<KNNAuxData>(auxData, transformator)
	{
	}

	inline double KNNMeasure::computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion)
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
			int kNN = (getAuxData().getKNN() < getAuxData().getPointNO()) ? getAuxData().getKNN()
				: getAuxData().getPointNO() - 1;
			int nnInServer = binValue - 1;
			int nnOutserver = 0;
			if (kNN - binValue + 1 > 0)
			{
				Dictionary_s * dictOfShells = transformator.convertIntPairsOfShellsToListOfIdxArrays(
					histogramResolution, indicesArrayOfBin, shells);
				iterateOverShells(indicesArrayOfRegion, kNN, nnInServer, nnOutserver, dictOfShells);
				delete dictOfShells;
			}
			//nnInServer should not be greater than (kNN - nnOutserver) because nnOutserver has been 'commited':
			nnInServer = (nnInServer <= kNN - nnOutserver) ? nnInServer : kNN - nnOutserver;
			measureForBin = (double)nnInServer / (double)kNN;
		}
		return measureForBin;
	}

	inline void KNNMeasure::iterateOverShells(int * indicesArrayOfRegion, int kNN, int& nnInServer,
		int& nnOutserver, Dictionary_s * dictOfShells)
	{
		for(Dictionary_s::iterator itr = dictOfShells->begin(); itr != dictOfShells->end(); itr++)
		{
			int shellIdx = itr->first;
			Shell_idxs * idxArraysOnCurrentShell = itr->second;
			int tmpNNOutServer = 0;
			updateNNCountsWithBinsOnCurrentShell(indicesArrayOfRegion, nnInServer,
				tmpNNOutServer, idxArraysOnCurrentShell);
			if (nnInServer + nnOutserver >= kNN)
			{
				break;
			}
			else
			{
				if (nnInServer + nnOutserver + tmpNNOutServer >= kNN)
				{
					nnOutserver = kNN - nnInServer;
					break;
				}
				else
				{
					nnOutserver += tmpNNOutServer;
				}
			}
		}
	}

	inline void KNNMeasure::updateNNCountsWithBinsOnCurrentShell(int * indicesArrayOfRegion,
		int& nnInServer, int& tmpNNOutServer, Shell_idxs * idxArraysOnCurrentShell)
	{
		for (Shell_idxs::iterator itr = idxArraysOnCurrentShell->begin(); 
			itr != idxArraysOnCurrentShell->end(); itr++)
		{
			int * idxArray = *itr;
			int currentCellIdx = transformator.calculateCellIdx(getAuxData().getSpaceDimension(),
						getAuxData().getHistogramResolution(), idxArray);
			int currentBinValue = (int)(getAuxData().getHistogram())[currentCellIdx];
			if (isIdxArrayInTheRegion(idxArray, indicesArrayOfRegion))
			{
				nnInServer += currentBinValue;
			}
			else
			{
				tmpNNOutServer += currentBinValue;
			}
		}
	}
}
#endif /* KNN_MEASURE_HPP_ */