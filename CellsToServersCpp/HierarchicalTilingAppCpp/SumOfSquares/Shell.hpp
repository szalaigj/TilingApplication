#ifndef SHELL_HPP_
#define SHELL_HPP_
#include <math.h>
#include <unordered_set>
#include <list>
#include "IntTuple.hpp"

namespace SumOfSquares
{
	typedef std::unordered_set<IntTuple, IntTupleEqualityComparer::GetHashCodeFn,
		IntTupleEqualityComparer::EqualsFn> Uset_t;
	typedef std::list<IntTuple> List_t;

	class Shell
	{
	public:
		~Shell();
		IntTuple * getIntTuples();
		void setIntTuplesWithSwapsAndSignChange(IntTuple * inputIntTuples);
	private:
		void addOppositeElements(Uset_t& tempIntTuples, IntTuple intTuple);
		void addSwapElements(Uset_t& tempIntTuples, int spaceDimension);
		std::list<int *>& permutateWithoutInitial(int spaceDimension, int * tuple);
		std::list<int *>& innerPermutate(std::list<int>& components);
		IntTuple * intTuples;
	};
}

#endif /* SHELL_HPP_ */