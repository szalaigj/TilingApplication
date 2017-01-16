#include "CornacchiaMethod.hpp"



namespace SumOfSquares
{
	Vector_t& CornacchiaMethod::applyCornacchiaMethod(int num)
	{
		Vector_t * intTuples = new Vector_t();
		if (num == 1)
		{
			IntTuple * intTuple1 = new IntTuple(2, tuple_1_0);
			intTuples->push_back(intTuple1);
			IntTuple * intTuple2 = new IntTuple(2, tuple_0_1);
			intTuples->push_back(intTuple2);
		} 
		else
		{
			List_t intTuplesOfFactors = listSumOfSquaresOfFactors(num);
			intTuples = applyFibonacciIdentity(intTuplesOfFactors);
		}
		return *intTuples;
	}

	List_t& CornacchiaMethod::listSumOfSquaresOfFactors(int num)
	{
		List_t * intTuplesOfFactors = new List_t();
		for (int factor = 2; num > 1; factor++)
		{
			if (num % factor == 0)
			{
				int exponent = innerListSumOfSquaresOfFactors(intTuplesOfFactors, factor, num);
				// If the prime factor is of the form 4k+3 then its exponent should be even
				// otherwise the num is not expressible as x^2 + y^2 
				// due to the corollary of Fermat's theorem on sums of two squares
				if (factor % 4 == 3)
				{
					if (exponent % 2 == 0)
					{
						int newTuple[2] = { 0, (int)pow(factor, exponent / 2) };
						IntTuple * newIntTuple = new IntTuple(2, newTuple);
						intTuplesOfFactors->push_back(newIntTuple);
					}
					else
					{
						intTuplesOfFactors = new List_t();
						break;
					}
				}
			}
		}
		return *intTuplesOfFactors;
	}

	int CornacchiaMethod::innerListSumOfSquaresOfFactors(List_t * intTuplesOfFactors, int factor,
		int& num)
	{
		int exponent = 0;
		while (num % factor == 0)
		{
			num /= factor;
			exponent++;
			if ((factor % 4 == 1) || (factor == 2))
			{
				List_t primitiveSolutions = applyCornacchiaMethodPrimitiveSolution(factor);
				for(List_t::iterator itr = primitiveSolutions.begin(); 
					itr != primitiveSolutions.end(); itr++)
				{
					intTuplesOfFactors->push_back(*itr);
				}
			}
		}
		return exponent;
	}

	List_t& CornacchiaMethod::applyCornacchiaMethodPrimitiveSolution(int num)
	{
		List_t * container = new List_t();
		if (num == 2)
		{
			IntTuple * newIntTuple = new IntTuple(2, tuple_1_1);
			container->push_back(newIntTuple);
		}
		else
		{
			for (int t = 1; t <= num / 2; t++)
			{
				if (t * t < std::numeric_limits<int>::max())
				{
					innerApplyCornacchiaMethodPrimitiveSolution(num, container, t);
				}
				else
				{
					std::string errmsg("Number ");
					errmsg.append("" + num);
					errmsg.append(" is too large.\n");
					throw std::runtime_error(errmsg);
				}
			}
		}
		return *container;
	}

	void CornacchiaMethod::innerApplyCornacchiaMethodPrimitiveSolution(int num, List_t * container, int t)
	{
		int x, y;
		if (t * t % num == num - 1)
		{
			int r1 = num;
			int r2 = t;
			int tmp;
			while (r2 * r2 >= num)
			{
				tmp = r2;
				r2 = (r1 % r2);
				r1 = tmp;
			}
			if (r1 * r1 > num)
			{
				x = r2;
				y = r1 % r2;
				chooseOrderedIntTuple(container, x, y);
			}
		}
	}

	Vector_t * CornacchiaMethod::applyFibonacciIdentity(List_t& intTuplesOfFactors)
	{
		Uset_t currentIntTuples;
		for(List_t::iterator itr = intTuplesOfFactors.begin(); itr != intTuplesOfFactors.end(); itr++)
		{
			IntTuple * intTuple = *itr;
			currentIntTuples = applyFibonacciIdentity(currentIntTuples, intTuple);
		}
		Vector_t * result = new Vector_t();
		for(Uset_t::iterator itr = currentIntTuples.begin(); itr != currentIntTuples.end(); itr++)
		{
			result->push_back(*itr);
		}
		return result;
	}

	Uset_t& CornacchiaMethod::applyFibonacciIdentity(Uset_t& currentIntTuples, IntTuple * newIntTuple)
	{
		Uset_t * result = new Uset_t();
		if (currentIntTuples.size() == 0)
			result->insert(newIntTuple);
		else
		{
			for(Uset_t::iterator itr = currentIntTuples.begin(); itr != currentIntTuples.end(); itr++)
			{
				IntTuple * currentIntTuple = *itr;
				int n1, n2;
				int a = currentIntTuple->getTuple()[0];
				int b = currentIntTuple->getTuple()[1];
				int c = newIntTuple->getTuple()[0];
				int d = newIntTuple->getTuple()[1];
				n1 = abs(a * c - b * d);
				n2 = abs(a * d + b * c);
				chooseOrderedIntTuple(result, n1, n2);
				n1 = abs(a * c + b * d);
				n2 = abs(a * d - b * c);
				chooseOrderedIntTuple(result, n1, n2);
			}
		}
		return *result;
	}
	
	void CornacchiaMethod::chooseOrderedIntTuple(List_t * result, int n1, int n2)
	{
		if (n1 < n2)
		{
			int * newTuple = new int[2]; newTuple[0] = n1; newTuple[1] = n2;
			IntTuple * newIntTuple = new IntTuple(2, newTuple);
			result->push_back(newIntTuple);
		}
		else
		{
			int * newTuple = new int[2]; newTuple[0] = n2; newTuple[1] = n1;
			IntTuple * newIntTuple = new IntTuple(2, newTuple);
			result->push_back(newIntTuple);
		}
	}
	
	void CornacchiaMethod::chooseOrderedIntTuple(Uset_t * result, int n1, int n2)
	{
		if (n1 < n2)
		{
			int * newTuple = new int[2]; newTuple[0] = n1; newTuple[1] = n2;
			IntTuple * newIntTuple = new IntTuple(2, newTuple);
			result->insert(newIntTuple);
		}
		else
		{
			int * newTuple = new int[2]; newTuple[0] = n2; newTuple[1] = n1;
			IntTuple * newIntTuple = new IntTuple(2, newTuple);
			result->insert(newIntTuple);
		}
	}
}