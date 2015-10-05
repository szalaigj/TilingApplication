"""
Created 2015-10-03 by Janos Szalai-Gindl;
"""
import numpy as np

class DataIOUtil:
  """
  This class is a helper class for IO operations of data.
  
  It contains functions which are useful for this purpose:
      - saving simulated data and histogram to disk
      - loading simulated data from disk
      - loading tiles and servers data from disk which are
        the result of the applied method (CellsToServers)
  """
  
  def __init__(self, data_dir):
    self.data_dir = data_dir

  def save_data_to_disk(self, suffix, coords):
    np.savetxt(self.data_dir + 'data' + suffix + '.dat', coords, fmt='%10.6e')

  def save_histogram_to_disk(self, suffix, ndata, hstgram):
    cellHeftWidth = len(str(ndata))
    np.savetxt(self.data_dir + 'hstgram' + suffix + '.dat', hstgram, fmt='%-'+str(cellHeftWidth)+'u')
    np.savetxt(self.data_dir + 'hstgram_for_input' + suffix + '.dat', hstgram, fmt='%u', newline=' ')

  def load_data_from_disk(self, data_file_name, space_dim):
    coords = np.loadtxt(self.data_dir + data_file_name, delimiter=' ', usecols=tuple(range(space_dim)))
    return coords

  def load_tiles_from_disk(self, tiles_file_name, space_dim):
    return np.loadtxt(self.data_dir + tiles_file_name, dtype=np.int32, delimiter=' ', usecols=tuple(range(2 * space_dim + 1)))

  def load_servers_from_disk(self, servers_file_name):
    with open (self.data_dir + servers_file_name, "r") as server_file:
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