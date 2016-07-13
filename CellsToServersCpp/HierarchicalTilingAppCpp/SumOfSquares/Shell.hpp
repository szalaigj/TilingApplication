#ifndef SHELL_HPP_
#define SHELL_HPP_
#include <math.h>
#include <unordered_set>
#include "IntTuple.hpp"

namespace SumOfSquares
{
	typedef std::unordered_set<IntTuple, IntTupleEqualityComparer::GetHashCodeFn,
		IntTupleEqualityComparer::EqualsFn> Uset_t;

	class Shell
	{
	public:
		IntTuple * getIntTuples();
		void setIntTuplesWithSwapsAndSignChange(IntTuple * inputIntTuples);
	private:
		void addOppositeElements(Uset_t& tempIntTuples, IntTuple intTuple);
		void addSwapElements(Uset_t& tempIntTuples);
		IntTuple * intTuples;
	};
}

#endif /* SHELL_HPP_ */