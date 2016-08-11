#include "Transformator.hpp"

namespace Transformation
{
	int Transformator::determineMaxRange(int spaceDimension, int histogramResolution)
	{
		double temp = pow((double)histogramResolution, (double)spaceDimension);
		temp /= (factorial(spaceDimension) * doubleFactorial(spaceDimension));
		temp *= pow(M_PI * spaceDimension / 2.0, spaceDimension / 2.0);
		int result = (int)ceil(temp);
		return result;
	}

	int Transformator::factorial(int input)
    { 
        int result = 1;
        for (int idx = 1; idx <= input; idx++)
        {
           result *= idx;
        }
        return result;
    }

	int Transformator::doubleFactorial(int input)
    {
       int result;
       if ((input % 2) == 0)
       {
           result = 1;
           for (int idx = 1; idx <= input/2; idx++)
           {
               result *= 2 * idx;
           }
       }
       else
       {
           result = 1;
           for (int idx = 1; idx <= (input + 1) / 2; idx++)
           {
               result *= (2 * idx - 1);
           }
       }
       return result;
   }
}