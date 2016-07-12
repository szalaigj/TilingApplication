#ifndef INT_TUPLE_HPP_
#define INT_TUPLE_HPP_

namespace SumOfSquares
{
	class IntTuple
	{
	public:
		bool determineIdxArrayRelativeTo(int spaceDimension, int histogramResolution, 
			int * inputIndicesArray, int * outputIndicesArray);
		int * getTuple();
		void setTuple(int * tuple);
	private:
		bool isValidIdxArray(int spaceDimension, int histogramResolution, int * outputIndicesArray);
		int * tuple;
	};
}

#endif /* INT_TUPLE_HPP_ */