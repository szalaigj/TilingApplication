#include "IntTuple.hpp"

namespace SumOfSquares
{
	IntTuple::IntTuple()
	{
	}

	IntTuple::IntTuple(int spaceDimension, int * tuple)
	{
		this->spaceDimension = spaceDimension;
		this->tuple = new int[spaceDimension];
		for (int idx = 0; idx < spaceDimension; idx++)
		{
			this->tuple[idx] = tuple[idx];
		}
	}

	IntTuple::~IntTuple()
	{
		if (tuple)
		{
			delete [] tuple;
		}
	}

	bool IntTuple::determineIdxArrayRelativeTo(int histogramResolution,	int * inputIndicesArray, 
		int * outputIndicesArray)
	{
		outputIndicesArray = new int[spaceDimension];
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			outputIndicesArray[dimIdx] = tuple[dimIdx] + inputIndicesArray[dimIdx];
		}
		return isValidIdxArray(histogramResolution, outputIndicesArray);
	}

	bool IntTuple::isValidIdxArray(int histogramResolution, int * outputIndicesArray)
	{
		bool isValid = true;
		for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
		{
			int outComponent = outputIndicesArray[dimIdx];
			if (!((outComponent >= 0) && (outComponent < histogramResolution)))
			{
				isValid = false;
			}
		}
		return isValid;
	}

	int * IntTuple::getTuple() const
	{
		return tuple;
	}

	int IntTuple::getSpaceDimension() const
	{
		return spaceDimension;
	}

	namespace IntTupleEqualityComparer
	{
		size_t GetHashCodeFn::operator() (IntTuple const * it) const
		{
			int * tuple = it->getTuple();
			size_t hCode = tuple[0];
			for (int idx = 1; idx < it->getSpaceDimension(); idx++)
			{
				hCode ^= tuple[idx];
			}
			size_t num = 352654597;
			num = (((num << 5) + num) + (num >> 27)) ^ hCode;
			return num * 1566083941;
		}

		bool EqualsFn::operator() (IntTuple const * it1, IntTuple const * it2) const
		{
			bool isEqual = false;
			if (!it1 && !it2)
				isEqual = true;
			else if (it1 && it2)
			{
				if (it1->getSpaceDimension() == it2->getSpaceDimension())
				{
					isEqual = true;
					int * tuple1 = it1->getTuple();
					int * tuple2 = it2->getTuple();
					for (int idx = 0; idx < it1->getSpaceDimension(); idx++)
					{
						if (tuple1[idx] != tuple2[idx])
						{
							isEqual = false;
						}
					}
				}
			}
			return isEqual;
		}
	}
}