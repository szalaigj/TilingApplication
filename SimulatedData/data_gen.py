"""
Created 2015-09-22 by Janos Szalai-Gindl;
Executing: e.g. python data_gen.py
"""
import argparse as argp
from data_io_util import DataIOUtil
from data_gen_util import DataGenUtil
from hstgram_creator import HistogramCreator

parser = argp.ArgumentParser()
parser.add_argument("--data_dir", default = "c:/temp/data/", help="The directory of the results", type=str)
parser.add_argument("--suffix", default = "", help="The suffix of sample-related output files", type=str)
parser.add_argument("--ndata", default = 3000, help="The number of samples", type=int)
parser.add_argument("--nserver", default = 10, help="The server number", type=int)
parser.add_argument("--nhst_resolution", default = 0, help="The histogram resolution", type=int)
parser.add_argument("--pdf_format", default = 'True', help="Would you like pdf format and high resolution for the figure output(s)?", type=str)

args = parser.parse_args()

data_io = DataIOUtil(args.data_dir)
utl = DataGenUtil(args)
hstgram_cr = HistogramCreator()

#coords = utl.multivariate_normal_data_coords([0,0], [[1,0],[0,1]]) # diagonal covariance, points lie on x or y-axis
#coords = utl.multivariate_exponential_scale_0_5_data_coords(2)
#coords = utl.multivariate_chisquare_df_3_data_coords(2)
#coords = utl.multivariate_uniform_data_coords(2,-3,3)
#coords = utl.multivariate_normal_mixture_K_data_coords(2,9,[[-4,-4],[-4,0],[0,-4],[-4,4],[0,0],[4,-4],[0,4],[4,0],[4,4]],[[[0.25,0],[0,0.25]],[[0.25,0],[0,0.25]],[[0.25,0],[0,0.25]],[[0.25,0],[0,0.25]],[[0.5,0],[0,0.5]],[[0.25,0],[0,0.25]],[[0.25,0],[0,0.25]],[[0.25,0],[0,0.25]],[[0.25,0],[0,0.25]]],[0.0625,0.0625,0.0625,0.0625,0.5,0.0625,0.0625,0.0625,0.0625])
#coords = utl.multivariate_normal_mixture_K_data_coords(2,9,[[-6,-6],[-6,0],[0,-6],[-6,6],[0,0],[6,-6],[0,6],[6,0],[6,6]],[[[0.1,0],[0,0.1]] for x in range(9)],[1.0/9.0 for x in range(9)])
#K=20
#coords = utl.multivariate_normal_mixture_K_data_coords(2,K,[[-4,-4],[-6,-2],[-2,-6],[-2.5,-2.5],[-0.75,-6],[0.75,-2],[4,-4],[6,-2],[2,-6],[2.5,-2.5],[-4,4],[-6,2],[-2,6],[-2.5,2.5],[-0.75,2],[0.75,6],[4,4],[6,2],[2,6],[2.5,2.5]],[[[0.15,0],[0,0.15]] for x in range(K)],[1.0/float(K) for x in range(K)])
K=16
coords = utl.multivariate_normal_mixture_K_data_coords(2,K,[[-4,4],[-4,2],[-4,0],[-4,-1],[-2,4],[-2,2],[-3,0],[0,4],[4,-4],[4,-2],[4,0],[4,1],[2,-4],[2,-2],[3,0],[0,-4]],[[[0.25,0],[0,0.25]] for x in range(K)],[1.0/float(K) for x in range(K)])
print coords
data_io.save_data_to_disk(args.suffix, coords)
utl.plot_two_dimensional_data(coords)
histogram_resolution = args.nhst_resolution
if(histogram_resolution == 0):
  histogram_resolution = np.ceil(np.sqrt((2*space_dim + 1)*nserver))
hstgram = hstgram_cr.create_histogram(histogram_resolution, coords)
data_io.save_histogram_to_disk(args.suffix, args.ndata, hstgram)