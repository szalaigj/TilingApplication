using MeasureApp.Data;
using MeasureApp.Transformation;
using System;
using System.Collections.Generic;

namespace MeasureApp.Measure
{
    public abstract class BaseMeasure<T> : IMeasure<T>
        where T : BaseAuxData
    {
        protected Transformator transformator;

        public T AuxData { get; set; }

        protected BaseMeasure(T auxData, Transformator transformator)
        {
            this.transformator = transformator;
            this.AuxData = auxData;
        }

        public double computeMeasure(BinGroup[] partition)
        {
            double measure = 0.0;
            foreach (var coords in partition)
            {
                measure += computeMeasureForRegion(coords);
            }
            measure = measure / (double)AuxData.ServerNO;
            return measure;
        }

        public abstract double computeMeasureForRegion(BinGroup binGroup);

        public abstract double computeMeasureForBin(int[] indicesArrayOfBin, List<int> binList);
    }
}
