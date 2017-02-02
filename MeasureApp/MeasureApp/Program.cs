using MeasureApp.Data;
using MeasureApp.Measure;
using MeasureApp.SumOfSquares;
using MeasureApp.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IntTupleEqualityComparer comparer = new IntTupleEqualityComparer();
            CornacchiaMethod cornacchiaMethod = new CornacchiaMethod(comparer);
            BacktrackingMethod backtrackingMethod = new BacktrackingMethod(cornacchiaMethod);
            ShellBuilder shellBuilder = new ShellBuilder(backtrackingMethod);
            Transformator transformator = new Transformator(shellBuilder);
            InputParser inputParser = new InputParser(transformator);
            int serverNO;
            int pointNO;
            double delta;
            int spaceDimension;
            int histogramResolution;
            Array array;
            bool together = inputParser.determineTogetherOrSeparately();
            if (together)
            {
                array = inputParser.parseInputFile(out spaceDimension, out histogramResolution,
                    out serverNO, out pointNO, out delta);
            }
            else
            {
                parseInputSeparately(inputParser, out serverNO, out pointNO, out delta, out spaceDimension,
                    out histogramResolution, out array);
            }
            BinGroup[] binGroups = inputParser.parseServerAssignments(spaceDimension, histogramResolution, serverNO);
            //Console.WriteLine("Enter the k parameter value for kNN measure:");
            //int kNN = int.Parse(Console.ReadLine());
            int kNN = (int)Math.Ceiling(delta);
            computeMeasures(shellBuilder, transformator, serverNO, pointNO, delta, spaceDimension, histogramResolution,
                array, kNN, binGroups);
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        private static void parseInputSeparately(InputParser inputParser, out int serverNO, out int pointNO, 
            out double delta, out int spaceDimension, out int histogramResolution, out Array array)
        {
            inputParser.parseInputSizes(out spaceDimension, out histogramResolution, out serverNO);
            Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}", spaceDimension, histogramResolution,
                serverNO);
            int[] lengthsArray = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                lengthsArray[idx] = histogramResolution;
            }
            array = Array.CreateInstance(typeof(int), lengthsArray);
            inputParser.parseInputArray(serverNO, histogramResolution, array, out pointNO, out delta);
        }

        private static void computeMeasures(ShellBuilder shellBuilder, Transformator transformator, int serverNO, 
            int pointNO, double delta, int spaceDimension, int histogramResolution, Array array, int kNN, 
            BinGroup[] binGroups)
        {
            int maxShellNO = transformator.determineMaxRange(spaceDimension, histogramResolution);
            Shell[] shellsForKNN = shellBuilder.createShells(maxShellNO, spaceDimension);
            KNNAuxData kNNAuxData = new KNNAuxData()
            {
                SpaceDimension = spaceDimension,
                HistogramResolution = histogramResolution,
                ServerNO = serverNO,
                PointNO = pointNO,
                KNN = kNN,
                Histogram = array,
                Shells = shellsForKNN
            };
            KNNMeasure kNNMeasure = new KNNMeasure(kNNAuxData, transformator);
            LoadBalancingAuxData lbAuxData = new LoadBalancingAuxData()
            {
                ServerNO = serverNO,
                PointNO = pointNO,
                Delta = delta
            };
            LoadBalancingMeasure lbMeasure = new LoadBalancingMeasure(lbAuxData, transformator);

            BoxAuxData boxAuxData = new BoxAuxData()
            {
                SpaceDimension = spaceDimension,
                HistogramResolution = histogramResolution,
                ServerNO = serverNO,
                Histogram = array
            };
            BoxMeasure boxMeasure = new BoxMeasure(boxAuxData, transformator);

            double measureOfKNN = kNNMeasure.computeMeasure(binGroups);
            Console.WriteLine("k-NN measure of the partition: {0}", measureOfKNN);
            double measureOfLB = lbMeasure.computeMeasure(binGroups);
            Console.WriteLine("Load balancing measure of the partition: {0}", measureOfLB);
            double measureOfBox = boxMeasure.averageAllMeasures(binGroups);
            Console.WriteLine("Box measure of the partition: {0}", measureOfBox);
        }
    }
}
