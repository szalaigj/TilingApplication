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
		int cellNO = (int)pow((double)histogramResolution, spaceDimension);
		double iterNOForCurrentSplitNO = (cellNO * (cellNO + 1)) / 2.0;
		int stepSize = (int)iterNOForCurrentSplitNO / 20;
		int localCountMax = stepSize / nThreads;
		for (int splitNO = 0; splitNO < serverNO; splitNO++)
		{
// The following solution for the progress bar is based on 
//http://stackoverflow.com/questions/28050669/can-i-report-progress-for-openmp-tasks
			int count = 0;
#pragma omp parallel num_threads(nThreads)
			{
			int reportedCount = 0;
			int localCount = 0;
#pragma omp for
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
					// update local and global progress counters
					if (localCount >= localCountMax)
					{
#pragma omp atomic
						count += localCountMax;
						localCount = 0;
					}
					else
					{
						++localCount;
					}
					// report progress
#pragma omp critical
					if (count - reportedCount >= stepSize)
					{
						std::cout << "Progress for splitNO " << (splitNO + 1) <<": " << 
							((int)((100.0*count)/iterNOForCurrentSplitNO)) << "%\r";
						std::cout.flush();
						reportedCount = count;
					}
				}
			}
			}
			std::cout << "Progress for splitNO " << (splitNO + 1) <<": 100%" << std::endl;
		}
	}
}