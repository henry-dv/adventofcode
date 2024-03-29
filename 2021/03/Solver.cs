﻿using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._03
{
    [ProblemDate(2021, 3)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int[] report = Array.ConvertAll(lines, line => Convert.ToInt32(line, 2));
            int numBits = lines[0].Length;

            int mask = 1;
            int gammaRate = 0;
            for (int i = 0; i < numBits; i++)
            {
                int count = 0;
                foreach (int entry in report)
                    if ((entry & mask) == mask)
                        count++;

                if (count > report.Length / 2)
                    gammaRate += mask;

                mask <<= 1;
            }

            int entryMask = (1 << numBits) - 1;  // 0000111111111111
            int epsilonRate = (gammaRate ^ entryMask) & entryMask;

            return (gammaRate * epsilonRate).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            int numBits = lines[0].Length;

            List<int> oxygenReport = lines.Select(line => Convert.ToInt32(line, 2)).ToList();
            var co2Report = new List<int>(oxygenReport);

            int mask = 1 << (numBits - 1);
            do
            {
                int oneCount = oxygenReport.Count(entry => (entry & mask) == mask);
                int mostCommonBit =
                    oneCount * 2 >= oxygenReport.Count ?
                        mask : 0;

                oxygenReport = oxygenReport.Where(entry => (entry & mask) == mostCommonBit).ToList();
                mask >>= 1;
            } while (oxygenReport.Count > 1);

            mask = 1 << (numBits - 1);
            do
            {
                int oneCount = co2Report.Count(entry => (entry & mask) == mask);
                int leastCommonBit =
                    oneCount * 2 < co2Report.Count ?
                        mask : 0;

                co2Report = co2Report.Where(entry => (entry & mask) == leastCommonBit).ToList();
                mask >>= 1;
            } while (co2Report.Count > 1);

            return (oxygenReport.Single() * co2Report.Single()).ToString();
        }
    }
}
