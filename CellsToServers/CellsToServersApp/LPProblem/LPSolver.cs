using lpsolve55;
using System;
using System.Text;

namespace CellsToServersApp.LPProblem
{
    public class LPSolver
    {
        public void solveLP(int serverNO, int tileNO, int[] tiles, int timeoutSec, string outputFilename)
        {
            // Please check the Debug or Release folder contains lpsolve55.dll and build on x86 platform.
            int actualLP = lpsolve.read_LP(outputFilename, 3, "");
            lpsolve.set_timeout(actualLP, timeoutSec);
            lpsolve.set_outputfile(actualLP, "result.txt");
            lpsolve.solve(actualLP);
            double objective = lpsolve.get_objective(actualLP);
            double[] vars = new double[lpsolve.get_Ncolumns(actualLP)];
            lpsolve.get_variables(actualLP, vars);
            StringBuilder strBldr = new StringBuilder();
            for (int idx = 1; idx <= serverNO; idx++)
            {
                printServerTiles(idx, tileNO, tiles, actualLP, vars);
                writeToStringBuilder(idx, tileNO, tiles, actualLP, vars, strBldr);
            }
            string serversOutput = @"c:\temp\data\servers.dat";
            System.IO.File.WriteAllText(serversOutput, strBldr.ToString());
            Console.WriteLine("The solution has " + objective + " overall difference");
            lpsolve.set_print_sol(actualLP, 1);
            lpsolve.print_objective(actualLP);
            lpsolve.print_solution(actualLP, 1);
            double elapsedTimeSecs = lpsolve.time_elapsed(actualLP);
            Console.WriteLine("Elapsed time of LP solution (secs): " + elapsedTimeSecs);
        }

        private void printServerTiles(int serverIdx, int tileNO, int[] tiles, int actualLP, double[] vars)
        {
            string outputOfRelatedTiles = "Tiles of " + serverIdx + ". server:";
            int weight = 0;
            for (int tileIdx = 1; tileIdx <= tileNO; tileIdx++)
            {
                int idxInVars = lpsolve.get_nameindex(actualLP, "x" + serverIdx + "_" + tileIdx, false) - 1;
                if (vars[idxInVars] == 1)
                {
                    outputOfRelatedTiles += " " + tileIdx + ".";
                    weight += tiles[tileIdx - 1];
                }
            }
            outputOfRelatedTiles += " (total heft: " + weight + ")";
            Console.WriteLine(outputOfRelatedTiles);
        }

        private void writeToStringBuilder(int serverIdx, int tileNO, int[] tiles, int actualLP, double[] vars,
            StringBuilder strBldr)
        {
            string outputOfRelatedTiles = "";
            int weight = 0;
            for (int tileIdx = 1; tileIdx <= tileNO; tileIdx++)
            {
                int idxInVars = lpsolve.get_nameindex(actualLP, "x" + serverIdx + "_" + tileIdx, false) - 1;
                if (vars[idxInVars] == 1)
                {
                    outputOfRelatedTiles += " " + (tileIdx - 1);
                    weight += tiles[tileIdx - 1];
                }
            }
            strBldr.Append(weight).Append(outputOfRelatedTiles);
            strBldr.AppendLine();
        }
    }
}
