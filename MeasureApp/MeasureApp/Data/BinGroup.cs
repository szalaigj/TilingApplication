using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Data
{
    public class BinGroup
    {
        public List<int> BinList { get; set; }

        public int Heft { get; set; }

        public double differenceFromDelta(double delta)
        {
            return Math.Abs(delta - Heft);
        }
    }
}
