#ifndef INT_TUPLE_HPP_
#define INT_TUPLE_HPP_

namespace SumOfSquares
{
	class IntTuple
	{
	public:
		bool determineIdxArrayRelativeTo(int histogramResolution, int * inputIndicesArray,
			int * outputIndicesArray);
		int * getTuple() const;
		int getSpaceDimension() const;
		void setTuple(int spaceDimension, int * tuple);
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
			size_t operator() (IntTuple const& it) const;
		};
		class EqualsFn
		{
		public:
			bool operator() (IntTuple const& it1, IntTuple const& it2) const;
		};
	}
}

#endif /* INT_TUPLE_HPP_ */