using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinsToServersIntLPApp.Histogram
{
    public class Bin
    {
        public int[] IndicesArray { get; set; }
        public int Heft { get; set; }

        public void printCoords(int spaceDimension, int serialNO)
        {
            Console.Write("{0}. bin: [", serialNO);
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                Console.Write(" {0} {1} ", IndicesArray[2 * idx], IndicesArray[2 * idx + 1]);
            }
            Console.WriteLine("] : {0} heft", Heft);
        }

        public void writeToStringBuilder(int spaceDimension, StringBuilder strBldr)
        {
            strBldr.Append(Heft);
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                // the following contains the lower and upper bound for bin which is the same:
                strBldr.Append(" ").Append(IndicesArray[idx]).Append(" ").Append(IndicesArray[idx]);
            }
            strBldr.AppendLine();
        }
    }
}
