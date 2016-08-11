#include "ShellBuilder.hpp"

namespace SumOfSquares
{
	ShellBuilder::ShellBuilder(BacktrackingMethod& backtrackingMethod) :
		backtrackingMethod(backtrackingMethod)
	{
	}

	Vector_s& ShellBuilder::createShells(size_t shellNO, int spaceDimension)
	{
		Vector_s * shells = new Vector_s();
		// The shellIdx is only a candidate shell index based on Fermat's theorem on sum of two squares
		// and Legendre's three-square theorem
		int shellIdx = 1;
		while (shells->size() < shellNO)
		{
			Shell * currentShell = new Shell();
			Vector_t currentIntTuples = backtrackingMethod.decomposeByBacktracking(shellIdx, 
				spaceDimension);
			if (currentIntTuples.size() != 0)
			{
				currentShell->setIntTuplesWithSwapsAndSignChange(currentIntTuples);
				shells->push_back(currentShell);
			}
			shellIdx++;
		}
		return *shells;
	}
}