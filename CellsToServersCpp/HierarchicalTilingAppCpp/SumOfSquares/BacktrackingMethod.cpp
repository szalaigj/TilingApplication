#include "BacktrackingMethod.hpp"

namespace SumOfSquares
{
	BacktrackingMethod::BacktrackingMethod(CornacchiaMethod& cornacchiaMethod) : 
		cornacchiaMethod(cornacchiaMethod)
	{
	}

	Vector_t& BacktrackingMethod::decomposeByBacktracking(int num, int squaresNO)
	{
		Vector_t * decompositions = new Vector_t();
		if (squaresNO == 1)
		{
			int floorOfSquareOfN = (int)floor(sqrt(num));
			if (floorOfSquareOfN * floorOfSquareOfN == num)
			{
				int * newTuple = new int[1]; newTuple[0] = floorOfSquareOfN;
				IntTuple * newIntTuple = new IntTuple(1, newTuple);
				decompositions->push_back(newIntTuple);
			}
		}
		else if (squaresNO >= 2)
		{
			decompositions = innerDecomposeByBacktracking(num, squaresNO);
		}
		return *decompositions;
	}

	Vector_t * BacktrackingMethod::innerDecomposeByBacktracking(int num, int squaresNO)
	{
		Vector_t * intTuples = new Vector_t();
		if (num == 0)
		{
			int * newTuple = new int[squaresNO];
			for (int idx = 0; idx < squaresNO; idx++)
			{
				newTuple[idx] = 0;
			}
			IntTuple * newIntTuple = new IntTuple(squaresNO, newTuple);
			intTuples->push_back(newIntTuple);
		} 
		else if (squaresNO == 2)
			intTuples = &(cornacchiaMethod.applyCornacchiaMethod(num));
		else
		{
			intTuples = innerDecomposeByBacktrackingWhenSquaresNOIsGreaterThanTwo(num, squaresNO);
		}
		return intTuples;
	}

	Vector_t * BacktrackingMethod::innerDecomposeByBacktrackingWhenSquaresNOIsGreaterThanTwo(int num,
		int squaresNO)
	{
		Vector_t * intTuples = new Vector_t();
		List_t * tempIntTuples = new List_t();
		int floorOfSquareOfN = (int)floor(sqrt(num));
		// This method return such int tuple where the components of this tuple has ascending order.
		// Thus num-lastTerm*lastTerm <= (squaresNO-1)*(lastTerm*lastTerm) 
		// --> sqrt(num/squaresNO) <= lastTerm.
		// (E.g. num = x^2 + y^2 + z^2 (x<=y<=z), num - z^2 = x^2 + y^2 <= 2 * z^2 ...)
		int lowerLimit = (int)(floorOfSquareOfN / sqrt(squaresNO));
		for (int lastTerm = floorOfSquareOfN; lastTerm >= lowerLimit; lastTerm--)
		{
			Vector_t * subIntTuples = innerDecomposeByBacktracking(num - lastTerm * lastTerm,
				squaresNO - 1);
			processSubIntTuples(subIntTuples, lastTerm, squaresNO, tempIntTuples);
		}
		for(List_t::iterator itr = tempIntTuples->begin(); itr != tempIntTuples->end(); itr++)
		{
			intTuples->push_back(*itr);
		}
		return intTuples;
	}

	void BacktrackingMethod::processSubIntTuples(Vector_t * subIntTuples, int lastTerm, int squaresNO,
		List_t * tempIntTuples)
	{
		for(Vector_t::iterator itr = subIntTuples->begin(); itr != subIntTuples->end(); itr++)
		{
			IntTuple * subIntTuple = *itr;
			int * currentTuple = subIntTuple->getTuple();
			// the components of this tuple must have ascending order
			if (currentTuple[squaresNO - 2] <= lastTerm)
			{
				int * newTuple = new int[squaresNO];
				for (int idx = 0; idx < squaresNO; idx++)
				{
					newTuple[idx] = currentTuple[idx];
				}
				newTuple[squaresNO - 1] = lastTerm;
				IntTuple * newIntTuple = new IntTuple(squaresNO, newTuple);
				tempIntTuples->push_back(newIntTuple);
			}
		}
	}
}