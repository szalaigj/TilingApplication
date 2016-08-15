#ifndef BASE_MEASURE_HPP_
#define BASE_MEASURE_HPP_

#include "IMeasure.hpp"
#include "BaseAuxData.hpp"
#include "../Transformation/Transformator.hpp"

using namespace Transformation;

namespace Measure
{
	template<typename T>
	class BaseMeasure : public IMeasure<T>
	{
	public:
		T& getAuxData();
		void setAuxData(T& inputAuxData);
		double computeMeasure(Vector_coords& partition);
		virtual double computeMeasureForRegion(Coords& coords) = 0;
		virtual double computeMeasureForBin(int * indicesArrayOfBin, int * indicesArrayOfRegion) = 0;
	protected:
		BaseMeasure(T& auxData, Transformator& transformator);
		Transformator& transformator;
		T& auxData;
	};

	// There was a compiler error ("unresolved external symbol") without inline keyword 
	// (source: http://stackoverflow.com/a/456716)
	template <class T>
	inline BaseMeasure<T>::BaseMeasure(T& auxData, Transformator& transformator) :
		auxData(auxData), transformator(transformator)
	{
		static_assert(std::is_base_of<BaseAuxData, T>::value, 
			"type parameter of this class must derive from BaseAuxData");
	}

	template <class T>
	inline T& BaseMeasure<T>::getAuxData()
	{
		return auxData;
	}
	
	template <class T>
	inline void BaseMeasure<T>::setAuxData(T& inputAuxData)
	{
		static_assert(std::is_base_of<BaseAuxData, T>::value, 
			"type parameter of this method must derive from BaseAuxData");
		this->auxData = inputAuxData;
	}
	
	template <class T>
	inline double BaseMeasure<T>::computeMeasure(Vector_coords& partition)
	{
		double measure = 0.0;
		for (Vector_coords::iterator itr = partition.begin(); itr != partition.end(); itr++)
		{
			Coords * coords = *itr;
			measure += computeMeasureForRegion(*coords);
		}
		measure = measure / (double)auxData.getServerNO();
		return measure;
	}
}

#endif /* BASE_MEASURE_HPP_ */