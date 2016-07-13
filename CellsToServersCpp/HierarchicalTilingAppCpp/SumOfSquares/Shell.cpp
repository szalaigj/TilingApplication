#include "Shell.hpp"

namespace SumOfSquares
{
	void Shell::setIntTuplesWithSwapsAndSignChange(Vector_t& inputIntTuples)
	{
		Uset_t container;
		int spaceDimension = inputIntTuples[0]->getSpaceDimension();
		for (int idx = 0; idx < sizeof(inputIntTuples) / sizeof(inputIntTuples[0]); idx++)
		{
			IntTuple * intTuple = inputIntTuples[idx];
			container.insert(intTuple);
			addOppositeElements(container, intTuple);
			addSwapElements(container, spaceDimension);
		}
		this->intTuples = Vector_t();
		for (Uset_t::iterator itr = container.begin(); itr != container.end(); itr++)
		{
			this->intTuples.push_back(*itr);
		}
	}

	void Shell::addOppositeElements(Uset_t& tempIntTuples, IntTuple * intTuple)
	{
		int spaceDimension = intTuple->getSpaceDimension();
		int * tuple = intTuple->getTuple();
		// The following loop iterates over the all sign-combinations of the original tuple components.
		// E.g. for (x,y) these are (-x,y),(x,-y),(-x,-y)
		for (int signDealingOut = 1; signDealingOut < pow(2, spaceDimension); signDealingOut++)
		{
			int * currentTuple = new int[spaceDimension];
			for (int idx = 0; idx < spaceDimension; idx++)
			{
				int bit = (signDealingOut >> idx) & 1;
				if (bit == 0) // the bit 0 symbolizes '+'
					currentTuple[idx] = tuple[idx];
				else // the bit 0 symbolizes '-'
					currentTuple[idx] = -tuple[idx];
			}
			IntTuple * newIntTuple = new IntTuple(spaceDimension, currentTuple);
			tempIntTuples.insert(newIntTuple);
		}
	}

	void Shell::addSwapElements(Uset_t& tempIntTuples, int spaceDimension)
	{
		List_t swapElements;
		for (Uset_t::iterator itr = tempIntTuples.begin(); itr != tempIntTuples.end(); itr++)
		{
			IntTuple * tempIntTuple = *itr;
			std::list<int *> perms = permutateWithoutInitial(tempIntTuple->getSpaceDimension(), 
				tempIntTuple->getTuple());
			for(std::list<int *>::iterator itr = perms.begin(); itr != perms.end(); itr++)
			{
				IntTuple * newIntTuple = new IntTuple(spaceDimension, *itr);
				swapElements.push_back(newIntTuple);
			}
		}
		for(List_t::iterator itr = swapElements.begin(); itr != swapElements.end(); itr++)
		{
			tempIntTuples.insert(*itr);
		}
	}

	std::list<int *>& Shell::permutateWithoutInitial(int spaceDimension, int * tuple)
	{
		std::list<int> components(tuple, tuple + spaceDimension);
		std::list<int *> * perms = innerPermutate(components);
		std::list<int *>::iterator itr = perms->begin();
		perms->erase(itr);
		return *perms;
	}

	std::list<int *> * Shell::innerPermutate(std::list<int>& components)
	{
		std::list<int *> * result = new std::list<int *>();
		if (components.size() == 2)
		{
			std::list<int>::iterator itr = components.begin();
			int firstElement = *itr;
			std::advance(itr, 1);
			int secondElement = *itr;
			int perm1[2] = { firstElement, secondElement };
			int perm2[2] = { secondElement, firstElement };
			result->push_back(perm1);
			result->push_back(perm2);
		}
		else if (components.size() > 2)
		{
			for (std::list<int>::iterator itr = components.begin(); itr != components.end(); itr++)
			{
				int currentComponent = *itr;
				std::list<int> componentsWithoutCurrent(components);
				componentsWithoutCurrent.erase(itr);
				std::list<int *> * perms = innerPermutate(componentsWithoutCurrent);
				for (std::list<int *>::iterator subitr = perms->begin(); subitr != perms->end(); subitr++)
				{
					int * perm = *subitr;
					int * extendedPerm = new int[components.size()];
					extendedPerm[0] = currentComponent;
					for (size_t idx = 1; idx < components.size(); idx++)
					{
						extendedPerm[idx] = perm[idx - 1];
					}
					result->push_back(extendedPerm);
				}
			}
		}
		return result;
	}
}