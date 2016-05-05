using System;
using System.Collections.Generic;

namespace RecursiveBisectionApp.Utils
{
    public class BinaryDecomposer
    {
        private Array array;
        private Array heftArray;
        private Transformator transformator;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private int pointNO;

        public BinaryDecomposer(Array array, Array heftArray, Transformator transformator, int spaceDimension, 
            int histogramResolution, int serverNO, int pointNO)
        {
            this.array = array;
            this.heftArray = heftArray;
            this.transformator = transformator;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.pointNO = pointNO;
        }

        internal Coords[] decompose()
        {
            throw new NotImplementedException();
        }
    }
}
