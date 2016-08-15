#ifndef SHELL_HPP_
#define SHELL_HPP_
#include <math.h>
#include <map>

#include "IntTuple.hpp"

namespace SumOfSquares
{	
	class Shell
	{
	public:
		Shell();
		Vector_t& getIntTuples();
		void setIntTuplesWithSwapsAndSignChange(Vector_t& inputIntTuples);
	private:
		Vector_t& createIntTuples();
		void addOppositeElements(Uset_t& tempIntTuples, IntTuple * intTuple);
		void addSwapElements(Uset_t& tempIntTuples, int spaceDimension);
		std::list<int *>& permutateWithoutInitial(int spaceDimension, int * tuple);
		std::list<int *> * innerPermutate(std::list<int>& components);
		Vector_t& intTuples;
	};

	typedef std::vector<Shell *> Vector_s;
	typedef std::vector<int *> Shell_idxs;
	typedef std::map<int, Shell_idxs *> Dictionary_s;
}

#endif /* SHELL_HPP_ */