"""
Created 2016-04-21 by Janos Szalai-Gindl;
Executing: e.g. python plot_posterior_values.py posterior_values.dat --knee_points True --knee_point_l_method 17 --knee_point_fpfl_method 9
"""
import argparse as argp
import numpy as np
from matplotlib.pyplot import *

parser = argp.ArgumentParser()
parser.add_argument("data_file_name", help="The path and file name of simulated data file.", type = str)
parser.add_argument("--data_dir", default = "c:/temp/data/", help="The directory of the results", type=str)
parser.add_argument("--min_hist_res", default = 2, help="The minimum value of histogram resolution", type = int)
parser.add_argument("--pdf_format", default = 'True', help="Would you like pdf format and high resolution for the figure output(s)?", type=str)
parser.add_argument("--knee_points", default = 'False', help="Would you like to add knee points (L method and furthest point from line method) for the figure output(s)?", type=str)
parser.add_argument("--knee_point_l_method", default = 2, help="The knee point from L method", type = int)
parser.add_argument("--knee_point_fpfl_method", default = 2, help="The knee point from furthest point from line method", type = int)
args = parser.parse_args()

data_file_name = args.data_file_name
data_dir = args.data_dir
min_hist_res = args.min_hist_res
pdf_format = eval(args.pdf_format)
knee_points = eval(args.knee_points)
knee_point_l_method = args.knee_point_l_method
knee_point_fpfl_method = args.knee_point_fpfl_method

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

max_post_est_loc = np.argmax(post_values) + min_hist_res

fig, ax =  subplots()
binNOs = np.arange(min_hist_res, len(post_values) + min_hist_res)
ax.scatter(binNOs, post_values, color='r', marker='.')
ax.plot(binNOs, post_values, linewidth=2)
ax.axvline(x = max_post_est_loc, color='black', linewidth=2, linestyle='-')
ax.annotate(str(max_post_est_loc), xy=(max_post_est_loc, post_values[max_post_est_loc - min_hist_res]),  xycoords='data', xytext=(30, 30), textcoords='offset points', color='black', arrowprops=dict(arrowstyle="->"), zorder=4, fontsize=18)
if knee_points:
  ax.axvline(x = knee_point_l_method, color='brown', linewidth=2, linestyle='-')
  ax.annotate(str(knee_point_l_method), xy=(knee_point_l_method, post_values[knee_point_l_method - min_hist_res]),  xycoords='data', xytext=(30, 30), textcoords='offset points', color='brown', arrowprops=dict(arrowstyle="->"), zorder=4, fontsize=18)
  ax.axvline(x = knee_point_fpfl_method, color='green', linewidth=2, linestyle='-')
  ax.annotate(str(knee_point_fpfl_method), xy=(knee_point_fpfl_method, post_values[knee_point_fpfl_method - min_hist_res]),  xycoords='data', xytext=(30, 30), textcoords='offset points', color='green', arrowprops=dict(arrowstyle="->"), zorder=4, fontsize=18)
ax.set_xlim([min_hist_res, len(post_values) + min_hist_res - 1])
if(pdf_format):
  savefig(data_dir + 'posterior_values.pdf', format='pdf')
else:
  savefig(data_dir + 'posterior_values.png')
