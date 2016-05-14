using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class NormCutComparer : IComparer<NormCutLeaf>
    {
        public int Compare(NormCutLeaf l1, NormCutLeaf l2)
        {
            return l1.NormCutValue.CompareTo(l2.NormCutValue);
        }
    }

    public class NormCutLeaf
    {
        public double NormCutValue { get; set; }
        public List<int> NodeList { get; set; }
        public List<int> PotentialFirstPart { get; set; }
        public List<int> PotentialSecondPart { get; set; }
    }
}
