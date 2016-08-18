#include "OpenMPDivider.hpp"

namespace ArrayPartition
{
	OpenMPDivider::OpenMPDivider(int * histogram, int * inputHeftArray,
		Transformator& transformator, ParsedData& parsedData, int kNN, int maxRange,
		Vector_s& shellsForKNN, Vector_s& shellsForRange) : BaseDivider(histogram, inputHeftArray, 
		transformator, parsedData, kNN, maxRange, shellsForKNN, shellsForRange)
	{
	}
	void OpenMPDivider::fillObjectiveValueArray()
	{
		int spaceDimension = parsedData.getSpaceDimension();
		int histogramResolution = parsedData.getHistogramResolution();
		int serverNO = parsedData.getServerNO();
		for (int splitNO = 0; splitNO < serverNO; splitNO++)
		{
			int cellNO = (int)pow((double)histogramResolution, spaceDimension);
			#pragma omp parallel for
			for (int outerCellIdx = 0; outerCellIdx < cellNO; outerCellIdx++)
			{
				for (int innerCellIdx = outerCellIdx; innerCellIdx < cellNO; innerCellIdx++)
				{
					int extendedCellIdx;
					int * extendedIndicesArray = nullptr;
					bool validArrayIndices = transformator.mergeIndicesArrays(spaceDimension, 
						histogramResolution, splitNO, serverNO, outerCellIdx, innerCellIdx, 
						extendedCellIdx, extendedIndicesArray);
					if (validArrayIndices)
					{
						fillObjectiveValueArrayCore(splitNO, extendedCellIdx, extendedIndicesArray);
					}
				}
			}
		}
	}
}