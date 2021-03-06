#ifndef CORNACCHIA_METHOD_HPP_
#define CORNACCHIA_METHOD_HPP_
#include <math.h>
#include <limits>

#include "IntTuple.hpp"

const int tuple_1_0[2] = { 1, 0 };
const int tuple_0_1[2] = { 0, 1 };
const int tuple_1_1[2] = { 1, 1 };

namespace SumOfSquares
{
	class CornacchiaMethod
	{
	public:
		Vector_t& applyCornacchiaMethod(int num);
	private:
		List_t& listSumOfSquaresOfFactors(int num);
		int innerListSumOfSquaresOfFactors(List_t * intTuplesOfFactors, int factor, int& num);
		List_t& applyCornacchiaMethodPrimitiveSolution(int num);
		void innerApplyCornacchiaMethodPrimitiveSolution(int num, List_t * container, int t);
		Vector_t * applyFibonacciIdentity(List_t& intTuplesOfFactors);
		Uset_t& applyFibonacciIdentity(Uset_t& currentIntTuples, IntTuple * newIntTuple);
		void chooseOrderedIntTuple(List_t * result, int n1, int n2);
		void chooseOrderedIntTuple(Uset_t * result, int n1, int n2);
	};
}

#endif /* CORNACCHIA_METHOD_HPP_ */