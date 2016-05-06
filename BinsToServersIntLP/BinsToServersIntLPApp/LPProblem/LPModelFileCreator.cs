using System;
using System.Globalization;
using System.Text;

namespace BinsToServersIntLPApp.LPProblem
{
    public class LPModelFileCreator
    {
        public string createOutputLPFile(int serverNO, int binNO, int pointNO, int[] binHefts, double delta)
        {
            string objFuncExp = createObjectiveFunctionExpression(serverNO);
            string binDefs = createBinaryVariablesDefinitionExpression(serverNO, binNO, binHefts);
            string constraints = createConstraintsExpression(serverNO, binNO, pointNO, delta, binHefts);
            string output_lp = Properties.Resources.BasicLPFile;
            output_lp = output_lp.Replace(@"${obj_func}", objFuncExp);
            output_lp = output_lp.Replace(@"${bin_vars}", binDefs);
            output_lp = output_lp.Replace(@"${consts}", constraints);
            //Console.WriteLine(output_lp);
            string outputFilename = @"c:\temp\LPSolve_Models\output_"
                + serverNO + "_" + binNO + ".lp";
            System.IO.File.WriteAllText(outputFilename, output_lp);
            Console.WriteLine("The output is written out to " + outputFilename);
            return outputFilename;
        }

        private string createObjectiveFunctionExpression(int serverNO)
        {
            string result = "dAbs1";
            for (int idx = 2; idx <= serverNO; idx++)
            {
                result += (" +dAbs" + idx);
            }
            return result;
        }

        private string createBinaryVariablesDefinitionExpression(int serverNO, int binNO, int[] binHefts)
        {
            string result = "";
            for (int serverIdx = 1; serverIdx <= serverNO; serverIdx++)
            {
                for (int binIdx = 1; binIdx <= binNO; binIdx++)
                {
                    if (binHefts[binIdx - 1] != 0)
                    {
                        result += "x" + serverIdx + "_" + binIdx + ",";
                    }
                }
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        private string createConstraintsExpression(int serverNO, int binNO, int pointNO, double delta, int[] binHefts)
        {
            StringBuilder sb = new StringBuilder();

            for (int serverIdx = 1; serverIdx <= serverNO; serverIdx++)
            {
                string row = "d" + serverIdx;
                for (int binIdx = 1; binIdx <= binNO; binIdx++)
                {
                    if (binHefts[binIdx - 1] != 0)
                    {
                        row += " +" + binHefts[binIdx - 1] + " x" + serverIdx + "_" + binIdx;
                    }
                }
                row += " = " + delta.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + ";";
                sb.AppendLine(row);
            }
            for (int binIdx = 1; binIdx <= binNO; binIdx++)
            {
                if (binHefts[binIdx - 1] != 0)
                {
                    string row = "";
                    for (int serverIdx = 1; serverIdx <= serverNO; serverIdx++)
                    {
                        row += "x" + serverIdx + "_" + binIdx + " +";
                    }
                    row = row.Substring(0, row.Length - 1);
                    row += "= 1;";
                    sb.AppendLine(row);
                }
            }
            //double limit = pointNO - delta;
            for (int idx = 1; idx <= serverNO; idx++)
            {
                sb.AppendLine("d" + idx + " <= dAbs" + idx + ";");
                sb.AppendLine("-d" + idx + " <= dAbs" + idx + ";");
                //sb.AppendLine("d" + idx + " <= " + limit.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + ";");
                //sb.AppendLine("-d" + idx + " <= " + limit.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + ";");
                sb.AppendLine("d" + idx + " <= " + delta.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + ";");
                sb.AppendLine("-d" + idx + " <= " + delta.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + ";");
            }
            return sb.ToString();
        }
    }
}
