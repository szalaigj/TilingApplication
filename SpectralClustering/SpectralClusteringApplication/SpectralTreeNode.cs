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

        public void writeToStringBuilder(StringBuilder strBldr)
        {
            // TODO: the following is no valid heft for cluster:
            int heft = 0;
            strBldr.Append(heft);
            foreach (var vertex in VertexList)
            {
                strBldr.Append(" ").Append(vertex);
            }
            strBldr.AppendLine();
        }
    }
}
