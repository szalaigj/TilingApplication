"""
Created 2015-10-03 by Janos Szalai-Gindl;
"""
import numpy as np

class HistogramCreator:
  """
  This class is a helper class for histogram creation.
  """

  def create_histogram(self, histogram_resolution, coords):
    space_dim = coords.shape[1]
    # note: e.g. [2]*3 -> [2, 2, 2]
    hst_shape = tuple([histogram_resolution]*space_dim)
    # set up histogram with initial zero values:
    hstgram = np.zeros(hst_shape,dtype=np.int32)
    coordsMaxs = coords.max(axis=0)
    coordsMins = coords.min(axis=0)
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
      hstgram[tuple(hst_idxs)] += 1
    print hstgram
    print "Maximal value of cells:", hstgram.max()
    return hstgram