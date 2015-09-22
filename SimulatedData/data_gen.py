"""
Created 2015-09-22 by Janos Szalai-Gindl;
Executing: e.g. python data_gen.py
"""
import argparse as argp
from data_gen_util import DataGenUtil

parser = argp.ArgumentParser()
parser.add_argument("--data_dir", default = "c:/temp/data/", help="The directory of the results", type=str)
parser.add_argument("--suffix", default = "", help="The suffix of sample-related output files", type=str)
parser.add_argument("--ndata", default = 3000, help="The number of samples", type=int)
parser.add_argument("--nserver", default = 10, help="The server number", type=int)
parser.add_argument("--nhst_resolution", default = 0, help="The histogram resolution", type=int)
parser.add_argument("--pdf_format", default = 'True', help="Would you like pdf format and high resolution for the figure output(s)?", type=str)

args = parser.parse_args()

utl = DataGenUtil(args)

coords = utl.multivariate_normal_data_coords([0,0], [[1,0],[0,1]]) # diagonal covariance, points lie on x or y-axis
print coords
utl.save_data_to_disk(coords)
utl.plot_two_dimensional_data(coords)
hstgram = utl.create_histogram(args.nhst_resolution, args.nserver, coords)
utl.save_histogram_to_disk(hstgram)