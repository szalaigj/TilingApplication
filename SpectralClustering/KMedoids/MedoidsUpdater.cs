using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMedoids
{
    public class MedoidsUpdater
    {
        public void updateMedoids(int K, Cluster[] clusters)
        {
            for (int clusterIdx = 0; clusterIdx < K; clusterIdx++)
            {
                clusters[clusterIdx].updateMedoid();
            }
        }
    }
}
