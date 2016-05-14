using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpectralClusteringApplication
{
    public class EdgesTransformator
    {
        IntArrayEqualityComparer comparer;
        IndexTransformator transformator;
        Array array;
        List<int[]> initialIndicesList;
        int spaceDimension;
        int histogramResolution;
        int kNN;

        public EdgesTransformator(IndexTransformator transformator, Array array, int spaceDimension, 
            int histogramResolution, int kNN)
        {
            this.comparer = new IntArrayEqualityComparer();
            this.transformator = transformator;
            this.array = array;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.kNN = kNN;
            this.initialIndicesList = determineInitialIndicesList();
        }

        public Matrix<double> determineWeightMatrix(int vertexNO)
        {
            Matrix<double> weightMX = Matrix<double>.Build.Dense(vertexNO, vertexNO);
            List<int> verticesWithoutZeroHeft = new List<int>();
            determineVerticesWithoutZeroHeft(vertexNO, verticesWithoutZeroHeft);
            for (int idxOfFromNode = 0; idxOfFromNode < vertexNO; idxOfFromNode++)
            {
                int[] hstgramIndicesOfFromNode = new int[spaceDimension];
                transformator.transformCellIdxToIndicesArray(histogramResolution, 
                    hstgramIndicesOfFromNode, idxOfFromNode);
                int heftOfFromNode = (int)array.GetValue(hstgramIndicesOfFromNode);
                if (heftOfFromNode - 1 >= kNN)
                {
                    double edgeWeight = kNN;
                    weightMX[idxOfFromNode, idxOfFromNode] = edgeWeight;
                }
                else if (heftOfFromNode == 0)
                {
                    foreach (var vertexWithoutZeroHeft in verticesWithoutZeroHeft)
                    {
                        double edgeWeight = kNN;
                        weightMX[idxOfFromNode, vertexWithoutZeroHeft] = edgeWeight;
                    }
                }
                else
                {
                    determineEdgeWeightNoLoopEdgeCase(weightMX, idxOfFromNode, 
                        hstgramIndicesOfFromNode, heftOfFromNode);
                }
            }
            return weightMX;
        }

        private void determineVerticesWithoutZeroHeft(int vertexNO, List<int> verticesWithoutZeroHeft)
        {
            for (int vertexIdx = 0; vertexIdx < vertexNO; vertexIdx++)
            {
                int[] hstgramIndicesOfVertex = new int[spaceDimension];
                transformator.transformCellIdxToIndicesArray(histogramResolution,
                    hstgramIndicesOfVertex, vertexIdx);
                int heftOfHstgramIndices = (int)array.GetValue(hstgramIndicesOfVertex);
                if (heftOfHstgramIndices > 0)
                {
                    verticesWithoutZeroHeft.Add(vertexIdx);
                }
            }
        }

        private void determineEdgeWeightNoLoopEdgeCase(Matrix<double> weightMX, int idxOfFromNode, 
            int[] hstgramIndicesOfFromNode, int heftOfFromNode)
        {
            List<int[]> hstgramIndicesOfToNodeList =
            determineAdjacentIndices(hstgramIndicesOfFromNode);
            foreach (var hstgramIndicesOfToNode in hstgramIndicesOfToNodeList)
            {
                int heftOfToNode = (int)array.GetValue(hstgramIndicesOfToNode);
                if (heftOfToNode != 0)
                {
                    double edgeWeight = Math.Min(heftOfToNode, kNN - (heftOfFromNode - 1));
                    int idxOfToNode = transformator.transformIndicesArrayToCellIdx(histogramResolution,
                        hstgramIndicesOfToNode);
                    weightMX[idxOfFromNode, idxOfToNode] = edgeWeight;
                }
            }
        }

        public List<int[]> determineAdjacentIndices(int[] inputIndices)
        {
            List<int[]> result = new List<int[]>();
            foreach (var initialIndices in initialIndicesList)
            {
                int[] currentIndicesArray;
                if (determineIdxArrayRelativeTo(histogramResolution, inputIndices,
                        initialIndices, out currentIndicesArray))
                {
                    result.Add(currentIndicesArray);
                }
            }
            return result;
        }

        private bool determineIdxArrayRelativeTo(int histogramResolution, int[] inputIndices,
            int[] initialIndices, out int[] outputIndices)
        {
            outputIndices = new int[inputIndices.Length];
            for (int idx = 0; idx < inputIndices.Length; idx++)
            {
                outputIndices[idx] = initialIndices[idx] + inputIndices[idx];
            }
            return isValidIdxArray(histogramResolution, outputIndices);
        }

        private bool isValidIdxArray(int histogramResolution, int[] outputIndices)
        {
            bool isValid = true;
            foreach (var outComponent in outputIndices)
            {
                if (!((outComponent >= 0) && (outComponent < histogramResolution)))
                {
                    isValid = false;
                }
            }
            return isValid;
        } 

        private List<int[]> determineInitialIndicesList()
        {
            int[] baseIndices = new int[spaceDimension];
            baseIndices[0] = 1; // baseIndices = { 1, 0, ..., 0}
            HashSet<int[]> container = new HashSet<int[]>(comparer);
            container.Add(baseIndices);
            addOppositeElements(container, baseIndices);
            addSwapElements(container);
            List<int[]> result = new List<int[]>(container);
            return result;
        }

        private void addOppositeElements(HashSet<int[]> container, int[] baseIndices)
        {
            int dim = baseIndices.Length;
            // The following loop iterates over the all sign-combinations of the original tuple components.
            // E.g. for (x,y) these are (-x,y),(x,-y),(-x,-y)
            for (int signDealingOut = 1; signDealingOut < Math.Pow(2, dim); signDealingOut++)
            {
                int[] currentIndices = new int[dim];
                for (int idx = 0; idx < dim; idx++)
                {
                    int bit = (signDealingOut >> idx) & 1;
                    if (bit == 0) // the bit 0 symbolizes '+'
                        currentIndices[idx] = baseIndices[idx];
                    else // the bit 0 symbolizes '-'
                        currentIndices[idx] = -baseIndices[idx];
                }
                container.Add(currentIndices);
            }
        }

        private void addSwapElements(HashSet<int[]> container)
        {
            List<int[]> swapElements = new List<int[]>();
            foreach (var tempIndices in container)
            {
                List<int[]> perms = permutateWithoutInitial(tempIndices);
                foreach (var perm in perms)
                {
                    swapElements.Add(perm);
                }
            }
            container.UnionWith(swapElements);
        }

        private List<int[]> permutateWithoutInitial(int[] tempIndices)
        {
            List<int> components = new List<int>(tempIndices);
            List<int[]> perms = innerPermutate(components);
            perms.RemoveAt(0);
            return perms;
        }

        private List<int[]> innerPermutate(List<int> components)
        {
            List<int[]> result = new List<int[]>();
            if (components.Count == 2)
            {
                int[] perm1 = new int[] { components[0], components[1] };
                int[] perm2 = new int[] { components[1], components[0] };
                result.Add(perm1);
                result.Add(perm2);
            }
            else if (components.Count > 2)
            {
                for (int idx = 0; idx < components.Count; idx++)
                {
                    int currentComponent = components[idx];
                    List<int> componentsWithoutCurrent = new List<int>(components);
                    componentsWithoutCurrent.RemoveAt(idx);
                    List<int[]> perms = innerPermutate(componentsWithoutCurrent);
                    foreach (var perm in perms)
                    {
                        int[] extendedPerm = new int[components.Count];
                        extendedPerm[0] = currentComponent;
                        perm.CopyTo(extendedPerm, 1);
                        result.Add(extendedPerm);
                    }
                }
            }
            return result;
        }
    }

    public class IntArrayEqualityComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] it1, int[] it2)
        {
            bool isEqual = false;
            if (it1 == null && it2 == null)
                isEqual = true;
            else if ((it1 != null) && (it2 != null) && (it1.Length == it2.Length))
            {
                isEqual = true;
                for (int idx = 0; idx < it1.Length; idx++)
                {
                    if (it1[idx] != it2[idx])
                    {
                        isEqual = false;
                    }
                }
            }
            return isEqual;
        }

        public int GetHashCode(int[] it)
        {
            int hCode = it[0];
            for (int idx = 1; idx < it.Length; idx++)
            {
                hCode ^= it[idx];
            }
            return hCode.GetHashCode();
        }
    }
}
