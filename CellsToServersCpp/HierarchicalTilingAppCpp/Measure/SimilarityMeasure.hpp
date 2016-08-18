#ifndef SIMILARITY_MEASURE_HPP_
#define SIMILARITY_MEASURE_HPP_

#include "BaseMeasure.hpp"
#include "SimilarityAuxData.hpp"

namespace Measure
{
	template<typename T>
	class SimilarityMeasure : public BaseMeasure<T>
	{
	public:
		double computeMeasureForRegion(Coords * coords);
	protected:
		SimilarityMeasure(T& auxData, Transformator& transformator);
		bool isIdxArrayInTheRegion(int * idxArray, int * indicesArrayOfRegion);
		void cleanUpDictOfShells(Dictionary_s * dictOfShells);
	};

	// There was a compiler error ("unresolved external symbol") without inline keyword 
	// (source: http://stackoverflow.com/a/456716)
	template <class T>
	inline SimilarityMeasure<T>::SimilarityMeasure(T& auxData, Transformator& transformator) :
		BaseMeasure<T>(auxData, transformator)
	{
		static_assert(std::is_base_of<SimilarityAuxData, T>::value, 
			"type parameter of this class must derive from SimilarityAuxData");
	}

	template <class T>
	inline double SimilarityMeasure<T>::computeMeasureForRegion(Coords * coords)
	{
		double measureForRegion = 0.0;
		int spaceDimension = getAuxData().getSpaceDimension();
		int histogramResolution = getAuxData().getHistogramResolution();
		int * indicesArrayOfRegion =
			transformator.determineIndicesArray(spaceDimension, coords->getExtendedIndicesArray());
		int binNOInRegionWithoutZeroHeft = 0;
		int * indicesArrayOfBin;
		for (indicesArrayOfBin =
			transformator.determineFirstContainedIndicesArray(spaceDimension, indicesArrayOfRegion);
			indicesArrayOfBin != nullptr;
			indicesArrayOfBin = transformator.determineNextContainedIndicesArray(spaceDimension, 
				indicesArrayOfRegion, indicesArrayOfBin))
		{
			int currentCellIdx = transformator.calculateCellIdx(spaceDimension,
						histogramResolution, indicesArrayOfBin);
			int binValue = (int)(getAuxData().getHistogram())[currentCellIdx];
			if (binValue > 0)
			{
				binNOInRegionWithoutZeroHeft++;
				measureForRegion += computeMeasureForBin(indicesArrayOfBin, indicesArrayOfRegion);
			}
		}
		delete [] indicesArrayOfBin;
		measureForRegion = measureForRegion / (double)binNOInRegionWithoutZeroHeft;
		delete [] indicesArrayOfRegion;
		return measureForRegion;
	}

	template <class T>
	inline bool SimilarityMeasure<T>::isIdxArrayInTheRegion(int * idxArray, int * indicesArrayOfRegion)
	{
		bool result = true;
		int spaceDimension = getAuxData().getSpaceDimension();
		for (int idx = 0; idx < spaceDimension; idx++)
		{
			int lowerBound = indicesArrayOfRegion[2 * idx];
			int upperBound = indicesArrayOfRegion[2 * idx + 1];
			if (!((lowerBound <= idxArray[idx]) && (idxArray[idx] <= upperBound)))
			{
				result = false;
			}
		}
		return result;
	}

	template <class T>
	inline void SimilarityMeasure<T>::cleanUpDictOfShells(Dictionary_s * dictOfShells)
	{
		for(Dictionary_s::iterator itr = dictOfShells->begin(); itr != dictOfShells->end(); itr++)
		{
			Shell_idxs * idxArraysOnCurrentShell = itr->second;
			for (Shell_idxs::iterator subItr = idxArraysOnCurrentShell->begin(); 
				subItr != idxArraysOnCurrentShell->end(); subItr++)
			{
				int * currentIndicesArray = *subItr;
				delete [] currentIndicesArray;
			}
			delete idxArraysOnCurrentShell;
		}
		delete dictOfShells;
	}
}

#endif /* SIMILARITY_MEASURE_HPP_ */