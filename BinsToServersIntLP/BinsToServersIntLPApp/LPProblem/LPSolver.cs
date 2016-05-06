using lpsolve55;
using System;
using System.Text;

namespace BinsToServersIntLPApp.LPProblem
{
    public class LPSolver
    {
        public void solveLP(int serverNO, int binNO, int[] binHefts, int timeoutSec, string outputFilename)
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
            for (int serverIdx = 1; serverIdx <= serverNO; serverIdx++)
            {
                printServerTiles(serverIdx, serverNO, binNO, binHefts, actualLP, vars);
                writeToStringBuilder(serverIdx, serverNO, binNO, binHefts, actualLP, vars, strBldr);
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

        private void printServerTiles(int serverIdx, int serverNO, int binNO, int[] binHefts, int actualLP, 
            double[] vars)
        {
            string outputOfRelatedTiles = "Tiles of " + serverIdx + ". server:";
            int weight = 0;
            if (serverIdx == 1)
            {
                outputOfRelatedTiles = assignBinsWithZeroHeftToFirstServer(serverNO, binNO, binHefts, 
                    outputOfRelatedTiles, true);
            }
            for (int binIdx = 1; binIdx <= binNO; binIdx++)
            {
                if (binHefts[binIdx - 1] != 0)
                {
                    int idxInVars = lpsolve.get_nameindex(actualLP, "x" + serverIdx + "_" + binIdx, false) - 1;
                    if (vars[idxInVars] == 1)
                    {
                        outputOfRelatedTiles += " " + binIdx + ".";
                        weight += binHefts[binIdx - 1];
                    }
                }
            }
            outputOfRelatedTiles += " (total heft: " + weight + ")";
            Console.WriteLine(outputOfRelatedTiles);
        }

        private string assignBinsWithZeroHeftToFirstServer(int serverNO, int binNO, int[] binHefts, 
            string outputOfRelatedTiles, bool callFromPrintMethod)
        {
            for (int binIdx = 1; binIdx <= binNO; binIdx++)
            {
                if (binHefts[binIdx - 1] == 0)
                {
                    if (callFromPrintMethod)
                    {
                        outputOfRelatedTiles += " " + binIdx + ".";
                    }
                    else
                    {
                        outputOfRelatedTiles += " " + (binIdx - 1);
                    }
                }
            }
            return outputOfRelatedTiles;
        }

        private void writeToStringBuilder(int serverIdx, int serverNO, int binNO, int[] binHefts, int actualLP, 
            double[] vars, StringBuilder strBldr)
        {
            string outputOfRelatedTiles = "";
            int weight = 0;
            if (serverIdx == 1)
            {
                outputOfRelatedTiles = assignBinsWithZeroHeftToFirstServer(serverNO, binNO, binHefts,
                    outputOfRelatedTiles, false);
            }
            for (int binIdx = 1; binIdx <= binNO; binIdx++)
            {
                if (binHefts[binIdx - 1] != 0)
                {
                    int idxInVars = lpsolve.get_nameindex(actualLP, "x" + serverIdx + "_" + binIdx, false) - 1;
                    if (vars[idxInVars] == 1)
                    {
                        outputOfRelatedTiles += " " + (binIdx - 1);
                        weight += binHefts[binIdx - 1];
                    }
                }
            }
            strBldr.Append(weight).Append(outputOfRelatedTiles);
            strBldr.AppendLine();
        }
    }
}
