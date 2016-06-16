"""
Created 2015-09-22 by Janos Szalai-Gindl;
Executing: e.g. python plot_result.py data.dat tiles.dat servers.dat 2 10
           e.g. python plot_result.py data.dat tiles.dat servers.dat 2 10 --direct_color True --color_tuples "(0.9,0.0,0.043) (0.5,0.0,0.5)"
"""
import argparse as argp
from data_io_util import DataIOUtil
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
parser.add_argument("--data_color", default = "(0.0,0.0,1.0)", help="The data color. The default color is blue.", type=str)
parser.add_argument("--data_alpha", default = 0.0, help="The alpha value for plot", type=float)
parser.add_argument("--direct_color", default = 'False', help="Would you like give the server colors?", type=str)
parser.add_argument("--color_tuples", default = "", help="The string of the server color tuples. The delimiter is the space character. So this is a good input: '(0.886,0.29,0.2) (0.46,0.46,0.46)' but this is a bad: '(0.886, 0.29, 0.2) (0.46, 0.46, 0.46)' for two servers", type=str)
args = parser.parse_args()

data_io = DataIOUtil(args.data_dir)
utl = PlotResultUtil(args)
coords = data_io.load_data_from_disk(args.data_file_name, args.space_dim)
tilesCoords = data_io.load_tiles_from_disk(args.tiles_file_name, args.space_dim)
tiles_to_servers, nserver, hefts_of_servers = data_io.load_servers_from_disk(args.servers_file_name)

print tilesCoords
print '---------'
print tiles_to_servers

utl.plot_two_dimensional_data(coords, tilesCoords, nserver, tiles_to_servers, hefts_of_servers)