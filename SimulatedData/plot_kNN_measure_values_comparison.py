"""
Created 2017-02-02 by Janos Szalai-Gindl;
Executing: e.g. python plot_kNN_measure_values_comparison.py central_road_intsect_servers_res_31_SPC_kNN_measure_values.dat central_road_intsect_servers_res_31_kNN1_lb0_kNN_measure_values.dat 709 709 --data_dir U:\_Data\_bulk_load\
or
python plot_kNN_measure_values_comparison.py central_road_intsect_servers_res_31_SPC_kNN_measure_values.dat central_road_intsect_servers_res_31_kNN1_lb0_kNN_measure_values.dat 2402 709 --data_dir U:\_Data\_bulk_load\
"""
import argparse as argp
from matplotlib.pyplot import *

parser = argp.ArgumentParser()
parser.add_argument("data_file_name1", help="The path and file name of kNN measure values data file.", type = str)
parser.add_argument("data_file_name2", help="The path and file name of kNN measure values data file.", type = str)
parser.add_argument("kmax", help="k max", type=int)
parser.add_argument("kexec", help="k max", type=int)
parser.add_argument("--data_dir", default = "c:/temp/data/", help="The directory of the results", type=str)
parser.add_argument("--kmin", default = 2, help="k min", type=int)
parser.add_argument("--pdf_format", default = 'True', help="Would you like pdf format and high resolution for the figure output(s)?", type=str)

args = parser.parse_args()
data_file_name1 = args.data_file_name1
data_file_name2 = args.data_file_name2
kmax = args.kmax
kmin = args.kmin
kexec = args.kexec
data_dir = args.data_dir
pdf_format = args.pdf_format

rc('font', size=10)  # default for labels (not axis labels)
rc('font', family='serif')  # default for labels (not axis labels)
rc('figure.subplot', bottom=.125, top=.875, right=.885, left=0.175)
# Use square shape
rc('figure', figsize=(3.5, 3.5))
rc('savefig', dpi=150)

kNN_measure_v1 = np.loadtxt(data_dir + data_file_name1)
kNN_measure_v2 = np.loadtxt(data_dir + data_file_name2)

fig, ax = subplots()
ax.xaxis.set_ticks([kmin,kmin+int((kmax-kmin)/3),kmin+int(2*(kmax-kmin)/3),kmax])
if(args.pdf_format == 'True'):
  ax.axvline(x = kexec, color='blue', linewidth=1.5, linestyle='-', zorder = 1)
  ax.scatter(range(kmin,kmax+1),kNN_measure_v1, c=(1.0, 0.0, 0.0), marker = ".", s=3, linewidth=0, alpha=0.95, zorder = 2)
  ax.scatter(range(kmin,kmax+1),kNN_measure_v2, c=(0.0, 1.0, 0.0), marker = ".", s=3, linewidth=0, alpha=0.95, zorder = 2) 
else:
  ax.scatter(range(kmin,kmax+1),kNN_measure_v1, c=(1.0, 0.0, 0.0), marker = ".")
  ax.scatter(range(kmin,kmax+1),kNN_measure_v2, c=(0.0, 1.0, 0.0), marker = ".")
xlim([kmin, kmax])
ylim([0.0, 1])
if(args.pdf_format == 'True'):
  savefig(args.data_dir + 'kNN_measure_values' + '.pdf', format='pdf')
else:
  savefig(args.data_dir + 'kNN_measure_values' + '.png', format='png')