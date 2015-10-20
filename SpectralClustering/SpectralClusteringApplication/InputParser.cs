using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class InputParser
    {
        public Matrix<double> parseWeightMatrix()
        {
            Matrix<double> weightMX = null;
            Console.WriteLine("Enter the input path and filename:");
            string filename = Console.ReadLine();
            bool exists = File.Exists(filename);
            if (exists)
            {
                string[] lines = File.ReadAllLines(filename);
                int nodeNO = int.Parse(lines[0]);
                weightMX = Matrix<double>.Build.Dense(nodeNO, nodeNO);
                for (int idx = 1; idx < lines.Length; idx++)
                {
                    string[] elements = lines[idx].Split(' ');
                    int idxOfFromNode = int.Parse(elements[0]);
                    int idxOfToNode = int.Parse(elements[1]);
                    double weight = double.Parse(elements[2], CultureInfo.InvariantCulture);
                    weightMX[idxOfFromNode, idxOfToNode] = weight;
                }
            }
            else
            {
                throw new ArgumentException("The path is not valid.");
            }
            return weightMX;
        }
    }
}
