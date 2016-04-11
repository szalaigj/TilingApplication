using HistoSegmentationApp.ArrayPartition;
using HistoSegmentationApp.JensenShannonDiv;
using System;
using System.IO;
using System.Text;

namespace HistoSegmentationApp
{
    class Program
    {
        /// <summary>
        /// This implementation is based on the following articles:
        /// 1.  Gómez-Lopera, Juan Francisco, et al. (2000)
        ///     An analysis of edge detection by using the Jensen-Shannon divergence.
        ///     Journal of Mathematical Imaging and Vision 13.1: 35-56.
        ///     
        /// 2.  Katatbeh, Qutaibeh D., et al. (2015)
        ///     An Optimal Segmentation Method Using Jensen–Shannon Divergence via a Multi-Size Sliding Window Technique.
        ///     Entropy 17.12: 7996-8006.
        /// </summary>
        static void Main(string[] args)
        {
            IndexTransformator transformator = new IndexTransformator();
            InputParser inputParser = new InputParser(transformator);
            HeftArrayCreator heftArrayCreator = new HeftArrayCreator(transformator);
            int serverNO;
            int pointNO;

            int spaceDimension;
            int histogramResolution;
            int scaleNumber;
            int cellMaxValue;
            int slidingWindowSize;
            Array array;
            bool together = inputParser.determineTogetherOrSeparately();
            if (together)
            {
                array = inputParser.parseInputFile(out spaceDimension, out histogramResolution,
                    out serverNO, out pointNO, out scaleNumber, out cellMaxValue, out slidingWindowSize);
            }
            else
            {
                parseInputSeparately(inputParser, out serverNO, out pointNO, out spaceDimension,
                    out histogramResolution, out scaleNumber, out cellMaxValue, out array, out slidingWindowSize);
            }
            Console.WriteLine("Point no.: {0}", pointNO);

            ShannonEntropyComputer entropyComputer = new ShannonEntropyComputer();
            JenShaDivComputer jenShaDivComputer = new JenShaDivComputer(entropyComputer);
            FrequencyComputer frequencyComputer = new FrequencyComputer(array, transformator, cellMaxValue,
                scaleNumber);
            Array heftArray = heftArrayCreator.createHeftArray(spaceDimension, histogramResolution, array);
            Array frequencyArray = frequencyComputer.createFrequencyArray(spaceDimension, histogramResolution);

            Divider divider = new Divider(array, heftArray, frequencyArray, transformator, jenShaDivComputer,
                spaceDimension, histogramResolution, serverNO, slidingWindowSize);
            Coords[] partition = divider.determinePartition();
            writeOutTiles(serverNO, spaceDimension, partition);
        }

        private static void parseInputSeparately(InputParser inputParser, out int serverNO, out int pointNO,
            out int spaceDimension, out int histogramResolution, out int scaleNumber,
            out int cellMaxValue, out Array array, out int slidingWindowSize)
        {
            inputParser.parseInputSizes(out spaceDimension, out histogramResolution, out serverNO, out scaleNumber,
                out slidingWindowSize);
            Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}, scale no.: {3}, sliding window size: {4}",
                spaceDimension, histogramResolution, serverNO, scaleNumber, slidingWindowSize);
            int[] lengthsArray = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                lengthsArray[idx] = histogramResolution;
            }
            array = Array.CreateInstance(typeof(int), lengthsArray);
            inputParser.parseInputArray(serverNO, histogramResolution, array, out pointNO, out cellMaxValue);
        }

        private static void writeOutTiles(int serverNO, int spaceDimension, Coords[] partition)
        {
            StringBuilder strBldr = new StringBuilder();
            StringBuilder strBldrForServers = new StringBuilder();
            for (int idx = 0; idx < serverNO; idx++)
            {
                strBldrForServers.Append(partition[idx].HeftOfRegion);
                strBldrForServers.Append(" ").Append(idx);
                strBldrForServers.AppendLine();
                partition[idx].printCoords(spaceDimension, idx + 1);
                partition[idx].writeToStringBuilder(spaceDimension, strBldr);
            }
            string tilesOutput = @"c:\temp\data\tiles.dat";
            System.IO.File.WriteAllText(tilesOutput, strBldr.ToString());
            string serversOutput = @"c:\temp\data\servers.dat";
            System.IO.File.WriteAllText(serversOutput, strBldrForServers.ToString());
        }
    }
}
