"""
Created 2016-06-16 by Janos Szalai-Gindl;
Executing: e.g. python plot_outer_data.py datafile.dat 3000
"""
import argparse as argp
from data_io_util import DataIOUtil
from matplotlib.pyplot import *

parser = argp.ArgumentParser()
parser.add_argument("data_file_name", help="The path and file name of simulated data file.", type = str)
parser.add_argument("--data_dir", default = "c:/temp/data/", help="The directory of the results", type=str)
parser.add_argument("--suffix", default = "", help="The suffix of sample-related output files", type=str)
parser.add_argument("--space_dim", default = 2, help="The space dimension", type=int)
parser.add_argument("--alpha", default = 0.333, help="The alpha value for plot", type=float)
parser.add_argument("--pdf_format", default = 'True', help="Would you like pdf format and high resolution for the figure output(s)?", type=str)

args = parser.parse_args()

data_io = DataIOUtil(args.data_dir)

rc('font', size=24)  # default for labels (not axis labels)
rc('font', family='serif')  # default for labels (not axis labels)
rc('figure.subplot', bottom=.125, top=.875, right=.885, left=0.175)
# Use square shape
rc('figure', figsize=(10, 10))
rc('savefig', dpi=150)

def plot_two_dimensional_data(coords):
    x_coord,y_coord = coords.T
    fig, ax = subplots()
    if(args.pdf_format == 'True'):
      ax.scatter(x_coord,y_coord, c=(0.4, 0.0, 0.4), marker = ".", linewidth=0, alpha=args.alpha)
    else:
      ax.scatter(x_coord,y_coord, c=(0.4, 0.0, 0.4), marker = ".", alpha=args.alpha)
    coordsMaxs = coords.max(axis=0)
    coordsMins = coords.min(axis=0)
    xlim([coordsMins[0], coordsMaxs[0]])
    ylim([coordsMins[1], coordsMaxs[1]])
    if(args.pdf_format == 'True'):
      savefig(args.data_dir + 'data' + args.suffix + '.pdf', format='pdf')
    else:
      savefig(args.data_dir + 'data' + args.suffix + '.png', format='png')

coords = data_io.load_data_from_disk(args.data_file_name, args.space_dim)
plot_two_dimensional_data(coords)