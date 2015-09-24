"""
Created 2015-09-22 by Janos Szalai-Gindl;
Executing: e.g. python plot_result.py data.dat tiles.dat servers.dat 2 10  
"""
import argparse as argp
from plot_result_util import PlotResultUtil

parser = argp.ArgumentParser()
parser.add_argument("data_file_name", help="The path and file name of simulated data file.", type = str)
parser.add_argument("tiles_file_name", help="The path and file name of the result tiles.", type = str)
parser.add_argument("servers_file_name", help="The path and file name of the result tile-distribution for servers.", type = str)
parser.add_argument("space_dim", help="The space dimension", type = int)
parser.add_argument("nhst_resolution", help="The histogram resolution", type=int)
parser.add_argument("--data_dir", default = "c:/temp/data/", help="The directory of the results", type=str)
parser.add_argument("--suffix", default = "", help="The suffix of sample-related output files", type=str)
parser.add_argument("--pdf_format", default = 'True', help="Would you like pdf format and high resolution for the figure output(s)?", type=str)
args = parser.parse_args()

utl = PlotResultUtil(args)
coords = utl.load_data_from_disk()
tilesCoords = utl.load_tiles_from_disk()
tiles_to_servers, nserver, hefts_of_servers = utl.load_servers_from_disk()

print tilesCoords
print '---------'
print tiles_to_servers

utl.plot_two_dimensional_data(coords, tilesCoords, nserver, tiles_to_servers, hefts_of_servers)