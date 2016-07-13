#include "Shell.hpp"

namespace SumOfSquares
{
	void Shell::setIntTuplesWithSwapsAndSignChange(IntTuple * inputIntTuples)
	{
		Uset_t container;
		int spaceDimension = inputIntTuples[0].getSpaceDimension();
		for (int idx = 0; idx < sizeof(inputIntTuples) / sizeof(inputIntTuples[0]); idx++)
		{
			IntTuple intTuple = inputIntTuples[idx];
			container.insert(intTuple);
			addOppositeElements(container, intTuple);
			addSwapElements(container);
		}
		this->intTuples = new IntTuple[container.size()];
		int idx = 0;
		for (std::unordered_set<IntTuple>::iterator itr = container.begin(); itr != container.end(); ++itr)
		{
			this->intTuples[idx] = *itr;
			idx++;
		}
	}

	void Shell::addOppositeElements(Uset_t& tempIntTuples, IntTuple intTuple)
	{
		int dim = intTuple.getSpaceDimension();
		int * tuple = intTuple.getTuple();
		// The following loop iterates over the all sign-combinations of the original tuple components.
		// E.g. for (x,y) these are (-x,y),(x,-y),(-x,-y)
		for (int signDealingOut = 1; signDealingOut < pow(2, dim); signDealingOut++)
		{
			int * currentTuple = new int[dim];
			for (int idx = 0; idx < dim; idx++)
			{
				int bit = (signDealingOut >> idx) & 1;
				if (bit == 0) // the bit 0 symbolizes '+'
					currentTuple[idx] = tuple[idx];
				else // the bit 0 symbolizes '-'
					currentTuple[idx] = -tuple[idx];
			}
			IntTuple newIntTuple;
			newIntTuple.setTuple(dim, currentTuple);
			tempIntTuples.insert(newIntTuple);
		}
	}

	void Shell::addSwapElements(Uset_t& tempIntTuples)
	{
		// TODO
	}
}