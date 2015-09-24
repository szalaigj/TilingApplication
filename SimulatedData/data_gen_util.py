"""
Created 2015-09-22 by Janos Szalai-Gindl;
"""
import datetime as dt
import numpy as np
from matplotlib.pyplot import *

class DataGenUtil:
  """
  This class is a helper class for generating simulated data.
  
  It contains functions which are useful for this purpose:
      - 
  """

  def __init__(self, args):
    self.data_dir = args.data_dir
    self.suffix = args.suffix
    self.ndata = args.ndata
    self.pdf_format = args.pdf_format
    rc('font', size=24)  # default for labels (not axis labels)
    rc('font', family='serif')  # default for labels (not axis labels)
    rc('figure.subplot', bottom=.125, top=.875, right=.875, left=0.125)
    # Use square shape
    rc('figure', figsize=(10, 10))
    if(self.pdf_format):
      rc('savefig', dpi=150)
    else:
      rc('savefig', dpi=100)
  
  def multivariate_normal_data_coords(self, mean, cov):
    """
    This function returns coordinates of generated samples 
    from multivariate normal distribution
    """
    coords = np.random.multivariate_normal(mean,cov,self.ndata)
    return coords

  def multivariate_uniform_data_coords(self, space_dim, low_value, high_value):
    """
    This function returns coordinates of generated samples 
    from multivariate uniform distribution
    """
    coords = np.random.uniform(low=low_value,high=high_value,size=(self.ndata,space_dim))
    return coords

  def multivariate_chisquare_df_3_data_coords(self, space_dim):
    """
    This function returns coordinates of generated samples 
    from multivariate chi-square distribution with three degree of freedom
    """
    coords = np.random.chisquare(df=3, size=(self.ndata,space_dim))
    return coords
	
  def multivariate_exponential_scale_0_5_data_coords(self, space_dim):
    """
    This function returns coordinates of generated samples 
    from multivariate exponential distribution with scale parameter 0.5
    """
    coords = np.random.exponential(scale=0.5, size=(self.ndata,space_dim))
    return coords
	
  def save_data_to_disk(self, coords):
    np.savetxt(self.data_dir + 'data' + self.suffix + '.dat', coords, fmt='%10.6e') 

  def plot_two_dimensional_data(self, coords):
    x_coord,y_coord = coords.T
    fig, ax = subplots()
    current_alpha = np.minimum(1.0, 1000.0 / self.ndata)
    ax.scatter(x_coord,y_coord, c=(0.0,0.0,1.0), marker = ".", linewidth=0, alpha=current_alpha)
    coordsMaxs = coords.max(axis=0)
    coordsMins = coords.min(axis=0)
    xlim([coordsMins[0], coordsMaxs[0]])
    ylim([coordsMins[1], coordsMaxs[1]])
    if(self.pdf_format):
      savefig(self.data_dir + 'data' + self.suffix + '.pdf', format='pdf')
    else:
      savefig(self.data_dir + 'data' + self.suffix + '.png')

  def create_histogram(self, histogram_resolution, nserver, coords):
    space_dim = coords.shape[1]
    if(histogram_resolution == 0):
      histogram_resolution = np.ceil(np.sqrt((2*space_dim + 1)*nserver))
    # note: e.g. [2]*3 -> [2, 2, 2]
    hst_shape = tuple([histogram_resolution]*space_dim)
    # set up histogram with initial zero values:
    hstgram = np.zeros(hst_shape,dtype=np.int32)
    coordsMaxs = coords.max(axis=0)
    #print coordsMaxs
    coordsMins = coords.min(axis=0)
    #print coordsMins
    steps = []
    for current_dim in range(space_dim):
      # steps[current_dim]: 
      # The diameter of the data space along the current dimension is divided by the histogram resolution:
      steps.append(np.abs(coordsMaxs[current_dim] - coordsMins[current_dim]) / float(histogram_resolution))
    for coord in coords:
      hst_idxs = []
      for current_dim in range(space_dim):
        diff_from_min = np.abs(coord[current_dim] - coordsMins[current_dim])
        hstgram_idx = np.floor(diff_from_min / steps[current_dim])
        # the below 'if' for the maximal value of coord along the current dimension:
        if(hstgram_idx >= histogram_resolution):
          hstgram_idx = histogram_resolution - 1
        hst_idxs.append(hstgram_idx)
      #print coord
      hstgram[tuple(hst_idxs)] += 1
      #print hst_idxs
    print hstgram
    return hstgram

  def save_histogram_to_disk(self, hstgram):
    cellHeftWidth = len(str(self.ndata))
    np.savetxt(self.data_dir + 'hstgram' + self.suffix + '.dat', hstgram, fmt='%-'+str(cellHeftWidth)+'u')
    np.savetxt(self.data_dir + 'hstgram_for_input' + self.suffix + '.dat', hstgram, fmt='%u', newline=' ')