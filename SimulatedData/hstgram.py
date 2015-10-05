"""
Created 2015-10-03 by Janos Szalai-Gindl;
Executing: e.g. python hstgram.py data.dat 2 10
"""

import argparse as argp
from data_io_util import DataIOUtil
from hstgram_creator import HistogramCreator

parser = argp.ArgumentParser()
parser.add_argument("data_file_name", help="The path and file name of simulated data file.", type = str)
parser.add_argument("space_dim", help="The space dimension", type = int)
parser.add_argument("nhst_resolution", help="The histogram resolution", type=int)
parser.add_argument("--data_dir", default = "c:/temp/data/", help="The directory of the results", type=str)
parser.add_argument("--suffix", default = "", help="The suffix of sample-related output files", type=str)
parser.add_argument("--ndata", default = 3000, help="The number of samples", type=int)

args = parser.parse_args()

data_io = DataIOUtil(args.data_dir)
hstgram_cr = HistogramCreator()

coords = data_io.load_data_from_disk(args.data_file_name, args.space_dim)

hstgram = hstgram_cr.create_histogram(args.nhst_resolution, coords)
data_io.save_histogram_to_disk(args.suffix, args.ndata, hstgram)