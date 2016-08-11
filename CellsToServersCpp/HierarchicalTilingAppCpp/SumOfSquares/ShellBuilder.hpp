#ifndef SHELL_BUILDER_HPP_
#define SHELL_BUILDER_HPP_

#include "BacktrackingMethod.hpp"
#include "IntTuple.hpp"
#include "Shell.hpp"

namespace SumOfSquares
{
	class ShellBuilder
	{
	public:
		ShellBuilder(BacktrackingMethod& backtrackingMethod);
		Vector_s& createShells(size_t shellNO, int spaceDimension);
	private:
		BacktrackingMethod& backtrackingMethod;
	};
}

#endif /* SHELL_BUILDER_HPP_ */