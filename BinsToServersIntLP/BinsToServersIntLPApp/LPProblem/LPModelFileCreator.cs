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
            string binDefs = createBinaryVariablesDefinitionExpression(serverNO, binNO);
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

        private string createBinaryVariablesDefinitionExpression(int serverNO, int binNO)
        {
            string result = "";
            for (int idx = 1; idx <= serverNO; idx++)
            {
                for (int subIdx = 1; subIdx <= binNO; subIdx++)
                {
                    result += "x" + idx + "_" + subIdx + ",";
                }
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        private string createConstraintsExpression(int serverNO, int binNO, int pointNO, double delta, int[] binHefts)
        {
            StringBuilder sb = new StringBuilder();

            for (int idx = 1; idx <= serverNO; idx++)
            {
                string row = "d" + idx;
                for (int subIdx = 1; subIdx <= binNO; subIdx++)
                {
                    row += " +" + binHefts[subIdx - 1] + " x" + idx + "_" + subIdx;
                }
                row += " = " + delta.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + ";";
                sb.AppendLine(row);
            }
            for (int idx = 1; idx <= binNO; idx++)
            {
                string row = "";
                for (int subIdx = 1; subIdx <= serverNO; subIdx++)
                {
                    row += "x" + subIdx + "_" + idx + " +";
                }
                row = row.Substring(0, row.Length - 1);
                row += "= 1;";
                sb.AppendLine(row);
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
