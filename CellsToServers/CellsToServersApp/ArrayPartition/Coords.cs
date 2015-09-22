﻿using System;

namespace CellsToServersApp.ArrayPartition
{
    public class Coords
    {
        public int[] IndicesArray { get; set; }
        public int HeftOfRegion { get; set; }

        public double differenceFromDelta(double delta)
        {
            return delta - HeftOfRegion;
        }

        public void printCoords(int spaceDimension, int serialNO)
        {
            Console.Write("{0}. tile: [", serialNO);
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                Console.Write(" {0} {1} ", IndicesArray[2 * idx], IndicesArray[2 * idx + 1]);
            }
            Console.WriteLine("] : {0} heft", HeftOfRegion);
        }
    }
}
