"""
Created 2016-04-21 by Janos Szalai-Gindl;
Executing: e.g. python plot_posterior_values.py posterior_values.dat
"""
import argparse as argp
import numpy as np
from matplotlib.pyplot import *

parser = argp.ArgumentParser()
parser.add_argument("data_file_name", help="The path and file name of simulated data file.", type = str)
parser.add_argument("--data_dir", default = "c:/temp/data/", help="The directory of the results", type=str)
parser.add_argument("--pdf_format", default = 'True', help="Would you like pdf format and high resolution for the figure output(s)?", type=str)
args = parser.parse_args()

data_dir = args.data_dir
data_file_name = args.data_file_name
pdf_format = eval(args.pdf_format)

post_values = np.loadtxt(data_dir + data_file_name)

rc('font', size=18)  # default for labels (not axis labels)
rc('font', family='serif')  # default for labels (not axis labels)
rc('figure.subplot', bottom=.125, top=.95, right=.95, left=0.125)
# Use square shape
rc('figure', figsize=(14, 10))
if(pdf_format):
  rc('savefig', dpi=150)
else:
  rc('savefig', dpi=100)

fig, ax =  subplots()
binNOs = np.arange(2, len(post_values) + 2)
ax.scatter(binNOs, post_values, color='r', marker='.')
ax.plot(binNOs, post_values)
ax.set_xlim([2, len(post_values) + 2])
if(pdf_format):
  savefig(data_dir + 'posterior_values.pdf', format='pdf')
else:
  savefig(data_dir + 'posterior_values.png')
