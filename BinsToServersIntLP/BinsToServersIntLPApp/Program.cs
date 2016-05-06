using BinsToServersIntLPApp.Histogram;
using BinsToServersIntLPApp.LPProblem;
using System;
using System.IO;
using System.Text;

namespace BinsToServersIntLPApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // IMPORTANT NOTE:
            //     please check the Debug or Release folder contains lpsolve55.dll and build on x86 platform.
            IndexTransformator transformator = new IndexTransformator();
            InputParser inputParser = new InputParser(transformator);
            BinsCreator binsCreator = new BinsCreator(transformator);
            LPModelFileCreator lpModelFileCreator = new LPModelFileCreator();
            LPSolver lpSolver = new LPSolver();
            int serverNO;
            int pointNO;
            double delta;
            int spaceDimension;
            int histogramResolution;
            Array array;
            int[] binHefts;
            int binNO;
            binCreationPhase(inputParser, binsCreator, out serverNO, out pointNO, out delta, out spaceDimension, 
                out histogramResolution, out array, out binHefts, out binNO);
            lpProblemPhase(inputParser, serverNO, pointNO, delta, binNO, binHefts, lpModelFileCreator, lpSolver);
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        private static void binCreationPhase(InputParser inputParser, BinsCreator binsCreator, out int serverNO, 
            out int pointNO, out double delta, out int spaceDimension, out int histogramResolution, out Array array, 
            out int[] binHefts, out int binNO)
        {
            bool together = inputParser.determineTogetherOrSeparately();
            if (together)
            {
                array = inputParser.parseInputFile(out spaceDimension, out histogramResolution, out serverNO, 
                    out pointNO, out delta);
            }
            else
            {
                parseInputSeparately(inputParser, out serverNO, out pointNO, out delta, out spaceDimension, 
                    out histogramResolution, out array);
            }
            Console.WriteLine("Point no.: {0}", pointNO);
            Console.WriteLine("Delta: {0}", delta);
            Bin[] bins = binsCreator.createBinsFromHistogram(spaceDimension, histogramResolution, array);
            binNO = (int)Math.Pow(histogramResolution, spaceDimension);
            binHefts = writeOutBins(spaceDimension, binNO, bins);
        }

        private static void parseInputSeparately(InputParser inputParser, out int serverNO, out int pointNO, 
            out double delta, out int spaceDimension, out int histogramResolution, out Array array)
        {
            inputParser.parseInputSizes(out spaceDimension, out histogramResolution, out serverNO);
            Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}", spaceDimension,
                histogramResolution, serverNO);
            int[] lengthsArray = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                lengthsArray[idx] = histogramResolution;
            }
            array = Array.CreateInstance(typeof(int), lengthsArray);
            inputParser.parseInputArray(serverNO, histogramResolution, array, out pointNO, out delta);
        }

        private static int[] writeOutBins(int spaceDimension, int binNO, Bin[] bins)
        {
            int[] binHefts;
            binHefts = new int[binNO];
            StringBuilder strBldr = new StringBuilder();
            for (int cellIdx = 0; cellIdx < binNO; cellIdx++)
            {
                binHefts[cellIdx] = bins[cellIdx].Heft;
                bins[cellIdx].writeToStringBuilder(spaceDimension, strBldr);
            }
            string binsOutput = @"c:\temp\data\bins.dat";
            System.IO.File.WriteAllText(binsOutput, strBldr.ToString());
            return binHefts;
        }

        private static void lpProblemPhase(InputParser inputParser, int serverNO, int pointNO, double delta,
            int binNO, int[] binHefts, LPModelFileCreator lpModelFileCreator, LPSolver lpSolver)
        {
            int timeoutSec = inputParser.parseInputTimeout();
            bool deleteOutputLP = inputParser.parseInputDeleteOutputLP();

            string outputFilename = lpModelFileCreator.createOutputLPFile(serverNO, binNO, pointNO,
                binHefts, delta);

            lpSolver.solveLP(serverNO, binNO, binHefts, timeoutSec, outputFilename);

            if (deleteOutputLP)
            {
                File.Delete(outputFilename);
            }
        }
    }
}
