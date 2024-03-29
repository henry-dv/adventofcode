﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootDirectory;

            // figure out if program is being run from within Visual Studio, or from command line
            DirectoryInfo rootInfo = new(Environment.CurrentDirectory);
            if (rootInfo.Parent.Parent.Name == "bin")
                rootDirectory = rootInfo.Parent.Parent.Parent.FullName; // VS runs program from adventofcode/bin/Debug/net5.0/ or something
            else
                rootDirectory = rootInfo.FullName;

            if (args.Length < 3 || args.Length > 4)
                Exit("Required args: \n\t[year] [day] [puzzle number (1 or 2)] [Input file name (optional, must be contained in folder corresponding with # of Day)]", -1);

            if (args[0].Length == 2)
                args[0] = "20" + args[0];
            if (args[1].Length == 1)
                args[1] = '0' + args[1];

            var year = int.Parse(args[0]);
            var day = int.Parse(args[1]);
            var numOfPuzzle = int.Parse(args[2]);

            var path = 
                args.Length == 4 ? 
                    Path.Combine(rootDirectory, args[0], args[1], args[3]) :
                    Path.Combine(rootDirectory, args[0], args[1], "input.txt");

            if (!File.Exists(path))
                Exit($"File '{path}' does not exist", -1);

            var methodName = string.Empty;
            switch (numOfPuzzle)
            {
                case 1:
                    methodName = "SolveFirst";
                    break;
                case 2:
                    methodName = "SolveSecond";
                    break;
                default:
                    Exit("3rd argument (# of Puzzle) must either be 1 or 2", -1);
                    return;
            }

            Console.WriteLine("Getting solver instance...");
            Type solverType =
            (
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from type in a.GetTypes()
                where 
                    type.IsDefined(typeof(ProblemDate), false) &&
                    ((ProblemDate)type.GetCustomAttributes(typeof(ProblemDate), false)[0]).Year == year &&
                    ((ProblemDate)type.GetCustomAttributes(typeof(ProblemDate), false)[0]).Date == day
                select type
            ).Single();

            var solver = (ISolver)Activator.CreateInstance(solverType);
            MethodInfo solverMethod = solverType.GetMethod(methodName);

            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Calculating...");
            sw.Start();
            var solution = (string)solverMethod.Invoke(solver, new[] { path });
            sw.Stop();

            Console.WriteLine($"\nSolution: {solution}");
            //Console.WriteLine($"Time elapsed: {sw.ElapsedMilliseconds}ms");
            //Clipboard.SetText(solution);
            //Exit("Solution copied to clipboard."); // doesn't work with .NET core
            Exit($"Time elapsed: {sw.ElapsedMilliseconds}ms");
        }

        static void Exit(string message, int exitCode = 0)
        {
            if (exitCode == -1)
                Console.Write("Error: ");
            Console.WriteLine(message);
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            System.Environment.Exit(exitCode);
        }
    }
}
