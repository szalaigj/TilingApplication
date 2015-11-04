using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMedoids
{
    public class Cluster
    {
        private Matrix<double> distanceMX;
        
        public int MedoidIdx { get; private set; }

        public List<int> Members { get; private set; }

        public Cluster(Matrix<double> distanceMX, int medoidIdx)
        {
            this.distanceMX = distanceMX;
            this.MedoidIdx = medoidIdx;
            this.Members = new List<int>();
        }

        public double calculateCost()
        {
            double cost = 0.0;
            foreach (var item in Members)
            {
                cost += distanceMX[MedoidIdx, item];
            }
            return cost;
        }

        public void updateMedoid()
        {
            double minCost = calculateCost();
            int minMedoidIdx = MedoidIdx;
            for (int idx = 0; idx < Members.Count; idx++)
            {
                MedoidIdx = idx;
                double currentCost = calculateCost();
                if (currentCost < minCost)
                {
                    minCost = currentCost;
                    minMedoidIdx = idx;
                }
            }
            MedoidIdx = minMedoidIdx;
        }

        public void printCluster()
        {
            string str = "";
            foreach (var item in Members)
            {
                str += " " + item;
            }
            Console.Out.WriteLine("Members:" + str + " medoid: " + MedoidIdx);
        }
    }
}
