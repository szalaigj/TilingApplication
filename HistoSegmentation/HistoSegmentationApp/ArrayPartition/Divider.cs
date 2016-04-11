using HistoSegmentationApp.JensenShannonDiv;
using System;
using System.Collections.Generic;

namespace HistoSegmentationApp.ArrayPartition
{
    public class Divider
    {
        private Array array;
        private Array heftArray;
        private Array frequencyArray;
        private List<Coords> listOfLeaves;
        private IndexTransformator transformator;
        private JenShaDivComputer jenShaDivComputer;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private int slidingWindowSize;

        public Divider(Array array, Array heftArray, Array frequencyArray, IndexTransformator transformator, 
            JenShaDivComputer jenShaDivComputer, int spaceDimension, int histogramResolution, int serverNO,
            int slidingWindowSize)
        {
            this.array = array;
            this.heftArray = heftArray;
            this.frequencyArray = frequencyArray;
            this.transformator = transformator;
            this.jenShaDivComputer = jenShaDivComputer;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.slidingWindowSize = slidingWindowSize;
            int[] lengthsTileNumberArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < 2 * spaceDimension; idx++)
            {
                lengthsTileNumberArray[idx] = histogramResolution;
            }
            this.listOfLeaves = new List<Coords>();
        }

        public Coords[] determinePartition()
        {
            int[] indicesArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                indicesArray[2 * idx] = 0;
                indicesArray[2 * idx + 1] = histogramResolution - 1;
            }
            Coords coords = new Coords
            {
                IndicesArray = indicesArray,
                HeftOfRegion = (int)heftArray.GetValue(indicesArray),
                MaxDiv = 0.0
            };
            innerDetermineMaxDivAndChildren(coords);
            innerDeterminePartition(coords.FirstChild, coords.SecondChild);
            return listOfLeaves.ToArray();
        }

        private void innerDeterminePartition(Coords coords1, Coords coords2)
        {
            innerDetermineMaxDivAndChildren(coords1);
            innerDetermineMaxDivAndChildren(coords2);
            listOfLeaves.Add(coords1);
            listOfLeaves.Add(coords2);
            if (listOfLeaves.Count < serverNO)
            {
                listOfLeaves.Sort(new Comparison<Coords>((c1, c2) => c2.MaxDiv.CompareTo(c1.MaxDiv)));
                Coords leafWithMaxDiv = listOfLeaves[0];
                Coords firstChildOfLeafWithMaxDiv = leafWithMaxDiv.FirstChild;
                Coords secondChildOfLeafWithMaxDiv = leafWithMaxDiv.SecondChild;
                listOfLeaves.Remove(leafWithMaxDiv);
                innerDeterminePartition(firstChildOfLeafWithMaxDiv, secondChildOfLeafWithMaxDiv);
            }
        }

        private void innerDetermineMaxDivAndChildren(Coords coords)
        {
            int[] maxFirstChildIndicesArray, maxSecondChildIndicesArray;
            // The following is only for initialization:
            maxFirstChildIndicesArray = maxSecondChildIndicesArray = coords.IndicesArray;
            int cellNO = (int)Math.Pow(histogramResolution, spaceDimension);
            int[] movingIndicesArray = new int[spaceDimension];
            for (int movingIdx = 0; movingIdx < cellNO; movingIdx++)
            {
                transformator.transformCellIdxToIndicesArray(histogramResolution, movingIndicesArray, movingIdx);
                for (int splitDimIdx = 0; splitDimIdx < spaceDimension; splitDimIdx++)
                {
                    bool validMovingIndicesArray = transformator.validateIndicesArrays(spaceDimension, splitDimIdx,
                        coords.IndicesArray, movingIndicesArray);
                    if (validMovingIndicesArray)
                    {
                        int[] firstPartIndicesArray, secondPartIndicesArray;
                        int[] firstWindow, secondWindow;
                        transformator.splitIndicesArrays(spaceDimension, splitDimIdx, coords.IndicesArray,
                            movingIndicesArray, out firstPartIndicesArray, out secondPartIndicesArray);
                        transformator.determineWindowsOfSplitSides(spaceDimension, splitDimIdx, coords.IndicesArray,
                            movingIndicesArray, slidingWindowSize, histogramResolution,
                            out firstWindow, out secondWindow);
                        double currentDiv = determineCurrentDiv(firstWindow, secondWindow, firstPartIndicesArray, secondPartIndicesArray);
                        if (coords.MaxDiv < currentDiv)
                        {
                            coords.MaxDiv = currentDiv;
                            maxFirstChildIndicesArray = firstPartIndicesArray;
                            maxSecondChildIndicesArray = secondPartIndicesArray;
                        }
                    }
                }
            }
            Coords firstChild = new Coords
            {
                IndicesArray = maxFirstChildIndicesArray,
                HeftOfRegion = (int)heftArray.GetValue(maxFirstChildIndicesArray),
                MaxDiv = 0.0,
            };
            coords.FirstChild = firstChild;
            Coords secondChild = new Coords
            {
                IndicesArray = maxSecondChildIndicesArray,
                HeftOfRegion = (int)heftArray.GetValue(maxSecondChildIndicesArray),
                MaxDiv = 0.0,
            };
            coords.SecondChild = secondChild;
        }

        private double determineCurrentDiv(int[] firstWindow, int[] secondWindow, 
            int[] firstPartIndicesArray, int[] secondPartIndicesArray)
        {
            int firstPartHeftOfRegion = (int)heftArray.GetValue(firstPartIndicesArray);
            int secondPartHeftOfRegion = (int)heftArray.GetValue(secondPartIndicesArray);
            double currentDiv;
            if ((firstPartHeftOfRegion == 0) || (secondPartHeftOfRegion == 0))
            {
                currentDiv = 0.0;
            }
            else
            {
                double firstWeight = 0.5;
                double secondWeight = 0.5;
                //double firstPartHeftOfRegion = (double)((int)heftArray.GetValue(firstPartIndicesArray));
                //double secondPartHeftOfRegion = (double)((int)heftArray.GetValue(secondPartIndicesArray));
                //double firstWeight = firstPartHeftOfRegion / (firstPartHeftOfRegion + secondPartHeftOfRegion);
                //double secondWeight = secondPartHeftOfRegion / (firstPartHeftOfRegion + secondPartHeftOfRegion);
                //double firstPartHeftOfRegion = (double)((int)heftArray.GetValue(firstWindow));
                //double secondPartHeftOfRegion = (double)((int)heftArray.GetValue(secondWindow));
                //double firstWeight = firstPartHeftOfRegion / (firstPartHeftOfRegion + secondPartHeftOfRegion);
                //double secondWeight = secondPartHeftOfRegion / (firstPartHeftOfRegion + secondPartHeftOfRegion);
                //double firstVolume = transformator.determineWindowVolume(spaceDimension, firstWindow);
                //double secondVolume = transformator.determineWindowVolume(spaceDimension, secondWindow);
                //double firstWeight = firstVolume / (firstVolume + secondVolume);
                //double secondWeight = secondVolume / (firstVolume + secondVolume);
                double[] firstPartFrequenciesOfRegion = (double[])frequencyArray.GetValue(firstWindow);
                double[] secondPartFrequenciesOfRegion = (double[])frequencyArray.GetValue(secondWindow);
                currentDiv = jenShaDivComputer.computeDivergence(firstPartFrequenciesOfRegion,
                    secondPartFrequenciesOfRegion, firstWeight, secondWeight);
            }
            return currentDiv;
        }
    }
}
