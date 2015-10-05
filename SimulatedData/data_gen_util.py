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
    rc('savefig', dpi=150)
  
  def multivariate_normal_data_coords(self, mean, cov):
    """
    This function returns coordinates of generated samples 
    from multivariate normal distribution
    """
    coords = np.random.multivariate_normal(mean,cov,self.ndata)
    return coords

  def multivariate_normal_mixture_9_data_coords(self, space_dim, means, 
  covs, weights):
    """
    This function returns coordinates of generated samples 
    from multivariate normal mixture distribution from
    three normal distribution with weights which is a list of
    three elements
    """
    coords = np.zeros(shape=(self.ndata,space_dim))
    for idx in range(0,self.ndata):
      selector = np.random.random()
      if (selector < weights[0]):
        coords[idx] = np.random.multivariate_normal(means[0],covs[0])
      elif ((weights[0] <= selector) and (selector < sum(weights[0:2]))):
        coords[idx] = np.random.multivariate_normal(means[1],covs[1])
      elif ((sum(weights[0:2]) <= selector) and (selector < sum(weights[0:3]))):
        coords[idx] = np.random.multivariate_normal(means[2],covs[2])
      elif ((sum(weights[0:3]) <= selector) and (selector < sum(weights[0:4]))):
        coords[idx] = np.random.multivariate_normal(means[3],covs[3])
      elif ((sum(weights[0:4]) <= selector) and (selector < sum(weights[0:5]))):
        coords[idx] = np.random.multivariate_normal(means[4],covs[4])
      elif ((sum(weights[0:5]) <= selector) and (selector < sum(weights[0:6]))):
        coords[idx] = np.random.multivariate_normal(means[5],covs[5])
      elif ((sum(weights[0:6]) <= selector) and (selector < sum(weights[0:7]))):
        coords[idx] = np.random.multivariate_normal(means[6],covs[6])
      elif ((sum(weights[0:7]) <= selector) and (selector < sum(weights[0:8]))):
        coords[idx] = np.random.multivariate_normal(means[7],covs[7])
      else:
        coords[idx] = np.random.multivariate_normal(means[8],covs[8])
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

  def plot_two_dimensional_data(self, coords):
    x_coord,y_coord = coords.T
    fig, ax = subplots()
    current_alpha = np.minimum(1.0, 1000.0 / self.ndata)
    if(self.pdf_format == 'True'):
      ax.scatter(x_coord,y_coord, c=(0.0,0.0,1.0), marker = ".", linewidth=0, alpha=current_alpha)
    else:
      ax.scatter(x_coord,y_coord, c=(0.0,0.0,1.0), marker = ".", alpha=current_alpha)
    coordsMaxs = coords.max(axis=0)
    coordsMins = coords.min(axis=0)
    xlim([coordsMins[0], coordsMaxs[0]])
    ylim([coordsMins[1], coordsMaxs[1]])
    if(self.pdf_format == 'True'):
      savefig(self.data_dir + 'data' + self.suffix + '.pdf', format='pdf')
    else:
      savefig(self.data_dir + 'data' + self.suffix + '.png', format='png')