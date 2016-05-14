using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class SpectralTreeNodeComparer : IComparer<SpectralTreeNode>
    {
        public int Compare(SpectralTreeNode treeNode1, SpectralTreeNode treeNode2)
        {
            return treeNode1.NormCutValue.CompareTo(treeNode2.NormCutValue);
        }
    }

    public class SpectralTreeNode
    {
        public double NormCutValue { get; set; }
        public List<int> VertexList { get; set; }
        public SpectralTreeNode FirstChild { get; set; }
        public SpectralTreeNode SecondChild { get; set; }

        public void printCoords(int serialNO)
        {
            Console.Write("{0}. cluster: [", serialNO);
            foreach (var vertex in VertexList)
            {
                Console.Write(" {0} ", vertex);
            }
            Console.WriteLine("]");
        }

        public void writeToStringBuilder(IndexTransformator transformator, Array array,
            int spaceDimension, int histogramResolution, StringBuilder strBldr)
        {
            int heft = 0;
            string outputOfRelatedServer = "";
            foreach (var vertexIdx in VertexList)
            {
                int[] indicesArray = new int[spaceDimension];
                transformator.transformCellIdxToIndicesArray(histogramResolution, indicesArray, vertexIdx);
                heft += (int)array.GetValue(indicesArray);
                outputOfRelatedServer += " " + vertexIdx;
            }
            strBldr.Append(heft).Append(outputOfRelatedServer);
            strBldr.AppendLine();
        }
    }
}
