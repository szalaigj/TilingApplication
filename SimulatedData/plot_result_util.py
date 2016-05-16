"""
Created 2015-09-22 by Janos Szalai-Gindl;
"""
import datetime as dt
import numpy as np
from matplotlib.pyplot import *
import matplotlib.patches as patches
from sklearn import manifold
from sklearn.metrics import euclidean_distances

class PlotResultUtil:
  """
  This class is a helper class for depict the data and the
  result tiling and distribution for servers.
  
  It contains functions which are useful for this purpose:
      - 
  """
  
  def __init__(self, args):
    self.data_dir = args.data_dir
    self.space_dim = args.space_dim
    self.nhst_resolution = args.nhst_resolution
    self.suffix = args.suffix
    self.pdf_format = eval(args.pdf_format)
    self.direct_color = eval(args.direct_color)
    if (self.direct_color):
      self.colors_of_servers = [eval(x) for x in args.color_tuples.split()]
    rc('font', size=18)  # default for labels (not axis labels)
    rc('font', family='serif')  # default for labels (not axis labels)
    rc('figure.subplot', bottom=.14, top=.86, right=.75, left=0.04)
    # Use square shape
    rc('figure', figsize=(14, 10))
    if(self.pdf_format):
      rc('savefig', dpi=150)
    else:
      rc('savefig', dpi=100)

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

  def _determine_euclidean_distances_of_centers(self, tilesCoords, coordsMins, stepx, stepy):
    tile_centers = np.zeros(shape=(tilesCoords.shape[0],2))
    for tileCoord_idx in range(tilesCoords.shape[0]):
      tile_x = coordsMins[0] + stepx * tilesCoords[tileCoord_idx][1]
      tile_y = coordsMins[1] + stepy * tilesCoords[tileCoord_idx][3]
      width = stepx * (tilesCoords[tileCoord_idx][2] + 1 - tilesCoords[tileCoord_idx][1])
      height = stepy * (tilesCoords[tileCoord_idx][4] + 1 - tilesCoords[tileCoord_idx][3])
      tile_center_x = tile_x + (width/2.0)
      tile_center_y = tile_y + (height/2.0)
      tile_centers[tileCoord_idx] =  np.array([tile_center_x,tile_center_y])
    euclidean_distances_of_centers = euclidean_distances(tile_centers)
    return euclidean_distances_of_centers
	  
  # Based on https://en.wikipedia.org/wiki/Breadth-first_search
  def _breadth_first_search(self, neighborhoods, tileCoord_idx, graph_edge_distances):
    distance = 0.0
    graph_edge_distances[tileCoord_idx,tileCoord_idx] = distance
    queue = []
    queue.append(tileCoord_idx)
    while (len(queue)!=0):
      u = queue.pop(0)
      for neighbor_idx in neighborhoods[u]:
        if ((neighbor_idx != tileCoord_idx) and (graph_edge_distances[tileCoord_idx,neighbor_idx]==0)):
          graph_edge_distances[tileCoord_idx,neighbor_idx]=graph_edge_distances[tileCoord_idx,u] + 1
          queue.append(neighbor_idx)

  def _determine_edge_distances(self, tilesCoords):
    neighborhoods = {}
    for tileCoord_idx in range(tilesCoords.shape[0]):
      neighborhoods[tileCoord_idx] = []
    for tileCoord_idx in range(tilesCoords.shape[0]):
      for tileCoord_subidx in range(tilesCoords.shape[0]):
        tile_idx_x_low = tilesCoords[tileCoord_idx][1]
        tile_idx_x_high = tilesCoords[tileCoord_idx][2]
        tile_idx_y_low = tilesCoords[tileCoord_idx][3]
        tile_idx_y_high = tilesCoords[tileCoord_idx][4]
        tile_subidx_x_low = tilesCoords[tileCoord_subidx][1]
        tile_subidx_x_high = tilesCoords[tileCoord_subidx][2]
        tile_subidx_y_low = tilesCoords[tileCoord_subidx][3]
        tile_subidx_y_high = tilesCoords[tileCoord_subidx][4]
        if ((tileCoord_idx != tileCoord_subidx) and (((tile_idx_x_low <= tile_subidx_x_low) and (tile_subidx_x_low <= tile_idx_x_high) and ((tile_subidx_y_low == tile_idx_y_high + 1) or (tile_idx_y_low == tile_subidx_y_high + 1))) or ((tile_subidx_x_low <= tile_idx_x_low) and (tile_idx_x_low <= tile_subidx_x_high) and ((tile_subidx_y_low == tile_idx_y_high + 1) or (tile_idx_y_low == tile_subidx_y_high + 1))) or ((tile_idx_y_low <= tile_subidx_y_low) and (tile_subidx_y_low <= tile_idx_y_high) and ((tile_subidx_x_low == tile_idx_x_high + 1) or (tile_idx_x_low == tile_subidx_x_high + 1))) or ((tile_subidx_y_low <= tile_idx_y_low) and (tile_idx_y_low <= tile_subidx_y_high)) and ((tile_subidx_x_low == tile_idx_x_high + 1) or (tile_idx_x_low == tile_subidx_x_high + 1)))):
          neighborhoods[tileCoord_idx].append(tileCoord_subidx)
    print neighborhoods
    graph_edge_distances = np.zeros(shape=(tilesCoords.shape[0],tilesCoords.shape[0]))
    # navigation for determination of edge distances
    for tileCoord_idx in range(tilesCoords.shape[0]):
      self._breadth_first_search(neighborhoods, tileCoord_idx, graph_edge_distances)
    print graph_edge_distances
    return graph_edge_distances

  def _determine_color_pos(self, similarities):
    mds = manifold.MDS(n_components=3, max_iter=5000, eps=1e-12, dissimilarity="precomputed", n_jobs=1)
    # we use multidimensional scaling algorithm (see: https://en.wikipedia.org/wiki/Multidimensional_scaling) which place each color vector of tile in three dimensional RGB (color) space based on similarities of tiles:
    color_pos = mds.fit(similarities).embedding_
    color_pos_mins = color_pos.min(axis=0)
    color_pos_ranges = color_pos.max(axis=0) - color_pos_mins
    # transform positions to [0,1] because of the domain of RGB space:
    color_pos = (color_pos - color_pos_mins) / color_pos_ranges
    return color_pos

  def _determine_server_to_colors(self, nserver, tiles_to_servers, nTilesCoords, color_pos):
    server_to_colors = {}
    for server_idx in range(nserver):
      server_to_colors[server_idx] = [0.0,0.0,0.0,0.0]
    for tileCoord_idx in range(nTilesCoords):
      related_server_idx = tiles_to_servers[tileCoord_idx]
      server_to_colors[related_server_idx][0] += 1.0
      server_to_colors[related_server_idx][1] += color_pos[tileCoord_idx][0]
      server_to_colors[related_server_idx][2] += color_pos[tileCoord_idx][1]
      server_to_colors[related_server_idx][3] += color_pos[tileCoord_idx][2]
    for server_idx in range(nserver):
      server_to_colors[server_idx][1] /= server_to_colors[server_idx][0]
      server_to_colors[server_idx][2] /= server_to_colors[server_idx][0]
      server_to_colors[server_idx][3] /= server_to_colors[server_idx][0]
    return server_to_colors

  def _draw_tiles(self, ax, coordsMaxs, coordsMins, nserver, tiles_to_servers, tilesCoords):
    stepx = np.abs(coordsMaxs[0] - coordsMins[0]) / float(self.nhst_resolution)
    stepy = np.abs(coordsMaxs[1] - coordsMins[1]) / float(self.nhst_resolution)
    leg_labels_to_objs = {}
    # if the centers of tiles (and the distances between them) are appropriate representative:
    #euclidean_distances_of_centers = self._determine_euclidean_distances_of_centers(tilesCoords, coordsMins, stepx, stepy)
    #similarities = -euclidean_distances_of_centers
    # more sophisticated solution is the following where the distances are based on the neighborhoods:
    graph_edge_distances = self._determine_edge_distances(tilesCoords)
    # we would like to hand colors of tiles out such that the closer tiles are the less similar colors of them are. So we have to 'reverse' closeness for determination of similarities:
    similarities = -graph_edge_distances
    color_pos = self._determine_color_pos(similarities)
    server_to_colors = self._determine_server_to_colors(nserver, tiles_to_servers, tilesCoords.shape[0], color_pos)
    for tileCoord_idx in range(tilesCoords.shape[0]):
      tile_x = coordsMins[0] + stepx * tilesCoords[tileCoord_idx][1]
      tile_y = coordsMins[1] + stepy * tilesCoords[tileCoord_idx][3]
      width = stepx * (tilesCoords[tileCoord_idx][2] + 1 - tilesCoords[tileCoord_idx][1])
      height = stepy * (tilesCoords[tileCoord_idx][4] + 1 - tilesCoords[tileCoord_idx][3])
      tile_center_x = tile_x + (width/2.0)
      tile_center_y = tile_y + (height/2.0)
      ax.add_patch(patches.Rectangle((tile_x, tile_y),width,height,fill=False,linewidth=3,linestyle='solid',edgecolor='k',zorder = 2))
      related_server_idx = tiles_to_servers[tileCoord_idx]
      if (self.direct_color):
        related_color = self.colors_of_servers[related_server_idx]
      else:
        related_color = server_to_colors[related_server_idx][1:]
      ax.text(tile_center_x, tile_center_y, str(related_server_idx + 1), color='k',horizontalalignment='center', verticalalignment='center', fontweight='bold', fontsize=22)
      rect1 = patches.Rectangle((tile_x, tile_y),width,height,linewidth=0,alpha=0.6,facecolor=related_color,edgecolor='w',zorder = 2)
      ax.add_patch(rect1)
      if (related_server_idx + 1) not in leg_labels_to_objs:
        leg_labels_to_objs[(related_server_idx + 1)] = rect1
    return leg_labels_to_objs

  def _determine_legend(self, leg_labels_to_objs, hefts_of_servers):
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
    current_alpha = np.minimum(1.0, 1000.0 / coords.shape[0])
    ax.scatter(x_coord,y_coord, c=(0.0,0.0,1.0), marker = ".", linewidth=0, alpha=current_alpha, zorder = 3)
    self._draw_borders(ax, borders)
    leg_labels_to_objs = self._draw_tiles(ax, coordsMaxs, coordsMins, nserver, tiles_to_servers, tilesCoords)
    leg_labels_extended, leg_objs_sorted = self._determine_legend(leg_labels_to_objs, hefts_of_servers)
    xlim([coordsMins[0], coordsMaxs[0]])
    ylim([coordsMins[1], coordsMaxs[1]])
    lgd = ax.legend(leg_objs_sorted, leg_labels_extended, bbox_to_anchor=(1.075, 1.0), fontsize="small", mode="expand", borderaxespad=0.)
    #legend(leg_objs_sorted, leg_labels_extended)
    if(self.pdf_format):
      savefig(self.data_dir + 'fig_result' + self.suffix + '.pdf', bbox_extra_artists=(lgd,), format='pdf')
    else:
      savefig(self.data_dir + 'fig_result' + self.suffix + '.png', bbox_extra_artists=(lgd,))