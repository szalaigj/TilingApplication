"""
Created 2015-09-22 by Janos Szalai-Gindl;
Executing: e.g. python data_gen 
"""
import datetime as dt
import numpy as np
from matplotlib.pyplot import *
import matplotlib.patches as patches

class PlotResultUtil:
  """
  This class is a helper class for depict the data and the
  result tiling and distribution for servers.
  
  It contains functions which are useful for this purpose:
      - 
  """
  
  def __init__(self, args):
    self.data_dir = args.data_dir
    self.data_file_name = args.data_file_name
    self.tiles_file_name = args.tiles_file_name
    self.servers_file_name = args.servers_file_name
    self.space_dim = args.space_dim
    self.nhst_resolution = args.nhst_resolution
    self.suffix = args.suffix
    self.pdf_format = args.pdf_format
    rc('font', size=18)  # default for labels (not axis labels)
    rc('font', family='serif')  # default for labels (not axis labels)
    rc('figure.subplot', bottom=.14, top=.86, right=.75, left=0.04)
    # Use square shape
    rc('figure', figsize=(14, 10))
    if(self.pdf_format):
      rc('savefig', dpi=150)
    else:
      rc('savefig', dpi=100)

  def load_data_from_disk(self):
    coords = np.loadtxt(self.data_dir + self.data_file_name, delimiter=' ', usecols=tuple(range(self.space_dim)))
    self.ndata = coords.shape[0]
    return coords

  def load_tiles_from_disk(self):
    return np.loadtxt(self.data_dir + self.tiles_file_name, dtype=np.int32, delimiter=' ', usecols=tuple(range(2 * self.space_dim + 1)))

  def load_servers_from_disk(self):
    with open (self.data_dir + self.servers_file_name, "r") as server_file:
      server_file_contents=server_file.readlines()
    tiles_to_servers = {}
    currentServer = 0
    hefts_of_servers = []
    for server_file_line in server_file_contents:
      line_to_list = map(int, server_file_line.replace('\n','').split(' '))
      # the first column is the heft of the current server
      hefts_of_servers.append(line_to_list[0])
      for idx in range(1, len(line_to_list)):
        tileIdx = line_to_list[idx]
        tiles_to_servers[tileIdx] = currentServer
      currentServer += 1
    return tiles_to_servers, currentServer, hefts_of_servers

  def _determine_borders(self, coordsMaxs, coordsMins):
      borders = []
      for current_dim in range(self.space_dim):
        # borders of current dimension:
        borders.append(np.linspace(coordsMins[current_dim],coordsMaxs[current_dim],self.nhst_resolution + 1))
      return borders

  def _draw_borders(self, ax, borders):
      x_borders = borders[0]
      for border in x_borders:
        ax.axvline(x = border, color=(0.5,0.5,0.5), linewidth=1, linestyle='-', zorder = 1)
      y_borders = borders[1]
      for border in y_borders:
        ax.axhline(y = border, color=(0.5,0.5,0.5), linewidth=1, linestyle='-', zorder = 1)

  def _draw_tiles(self, ax, coordsMaxs, coordsMins, nserver, tiles_to_servers, tilesCoords):
      stepx = np.abs(coordsMaxs[0] - coordsMins[0]) / float(self.nhst_resolution)
      stepy = np.abs(coordsMaxs[1] - coordsMins[1]) / float(self.nhst_resolution)
      server_to_colors = {}
      for server_idx in range(nserver):
        server_to_colors[server_idx] = tuple(np.random.uniform(0,1,3))
      leg_labels_to_objs = {} 
      for tileCoord_idx in range(tilesCoords.shape[0]):
        tile_x = coordsMins[0] + stepx * tilesCoords[tileCoord_idx][1]
        tile_y = coordsMins[1] + stepy * tilesCoords[tileCoord_idx][3]
        width = stepx * (tilesCoords[tileCoord_idx][2] + 1 - tilesCoords[tileCoord_idx][1])
        height = stepy * (tilesCoords[tileCoord_idx][4] + 1 - tilesCoords[tileCoord_idx][3])
        ax.add_patch(patches.Rectangle((tile_x, tile_y),width,height,fill=False,linewidth=3,linestyle='solid',edgecolor='k',zorder = 2))
        related_server_idx = tiles_to_servers[tileCoord_idx]
        related_color = server_to_colors[related_server_idx]
        rect1 = patches.Rectangle((tile_x, tile_y),width,height,linewidth=0,alpha=0.6,facecolor=related_color,edgecolor='w',zorder = 2)
        ax.add_patch(rect1)
        if (related_server_idx + 1) not in leg_labels_to_objs:
          leg_labels_to_objs[(related_server_idx + 1)] = rect1
      return leg_labels_to_objs

  def _determine_legend(self, ax, coordsMaxs, coordsMins, hefts_of_servers, nserver, tiles_to_servers, tilesCoords):
      leg_labels_to_objs = self._draw_tiles(ax, coordsMaxs, coordsMins, nserver, tiles_to_servers, tilesCoords)
      leg_labels_sorted = leg_labels_to_objs.keys()
      leg_labels_sorted.sort()
      leg_objs_sorted = []
      leg_labels_extended = []
      for key in leg_labels_sorted:
        leg_labels_extended.append("server " + str(key) + " (heft: " + str(hefts_of_servers[key-1]) + ")")
        leg_objs_sorted.append(leg_labels_to_objs[key])
      return leg_labels_extended, leg_objs_sorted

  def plot_two_dimensional_data(self, coords, tilesCoords, nserver, tiles_to_servers, hefts_of_servers):
    x_coord,y_coord = coords.T
    coordsMaxs = coords.max(axis=0)
    coordsMins = coords.min(axis=0)
    borders = self._determine_borders(coordsMaxs, coordsMins)
    fig = figure()
    ax = subplot(111, aspect='equal')
    current_alpha = np.minimum(1.0, 1000.0 / self.ndata)
    ax.scatter(x_coord,y_coord, c=(0.0,0.0,1.0), marker = ".", linewidth=0, alpha=current_alpha, zorder = 3)
    self._draw_borders(ax, borders)
    leg_labels_extended, leg_objs_sorted = self._determine_legend(ax, coordsMaxs, coordsMins, hefts_of_servers, nserver, tiles_to_servers, tilesCoords)
    xlim([coordsMins[0], coordsMaxs[0]])
    ylim([coordsMins[1], coordsMaxs[1]])
    lgd = ax.legend(leg_objs_sorted, leg_labels_extended, bbox_to_anchor=(1.075, 1.0), fontsize="small", mode="expand", borderaxespad=0.)
    #legend(leg_objs_sorted, leg_labels_extended)
    if(self.pdf_format):
      savefig(self.data_dir + 'fig_result' + self.suffix + '.pdf', bbox_extra_artists=(lgd,), format='pdf')
    else:
      savefig(self.data_dir + 'fig_result' + self.suffix + '.png', bbox_extra_artists=(lgd,))