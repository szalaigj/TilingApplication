#ifndef INT_TUPLE_HPP_
#define INT_TUPLE_HPP_

#include <unordered_set>
#include <list>

namespace SumOfSquares
{
	class IntTuple
	{
	public:
		IntTuple();
		IntTuple(int spaceDimension, int * tuple);
		~IntTuple();
		bool determineIdxArrayRelativeTo(int histogramResolution, int * inputIndicesArray,
			int * outputIndicesArray);
		int * getTuple() const;
		int getSpaceDimension() const;
	private:
		bool isValidIdxArray(int histogramResolution, int * outputIndicesArray);
		int spaceDimension;
		int * tuple;
	};

	namespace IntTupleEqualityComparer
	{
		class GetHashCodeFn
		{
		public:
			size_t operator() (IntTuple const * it) const;
		};
		class EqualsFn
		{
		public:
			bool operator() (IntTuple const * it1, IntTuple const * it2) const;
		};
	}

	typedef std::unordered_set<IntTuple *, IntTupleEqualityComparer::GetHashCodeFn,
		IntTupleEqualityComparer::EqualsFn> Uset_t;
	typedef std::list<IntTuple *> List_t;
	typedef std::vector<IntTuple *> Vector_t;
}

#endif /* INT_TUPLE_HPP_ */