#ifndef BACKTRACKING_METHOD_HPP_
#define BACKTRACKING_METHOD_HPP_
#include <math.h>

#include "CornacchiaMethod.hpp"

namespace SumOfSquares
{
	class BacktrackingMethod
	{
	public:
		BacktrackingMethod(CornacchiaMethod& cornacchiaMethod);
		Vector_t& decomposeByBacktracking(int num, int squaresNO);
	private:
		Vector_t * innerDecomposeByBacktracking(int num, int squaresNO);
		Vector_t * innerDecomposeByBacktrackingWhenSquaresNOIsGreaterThanTwo(int num, int squaresNO);
		void processSubIntTuples(Vector_t * subIntTuples, int lastTerm, int squaresNO, 
			List_t * tempIntTuples);
		CornacchiaMethod& cornacchiaMethod;
	};
}

#endif /* BACKTRACKING_METHOD_HPP_ */