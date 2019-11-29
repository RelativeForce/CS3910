﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coursework
{
    public sealed class Program
    {
        private const int NumberOfDestinations = 13;

        // Defaults
        private const string DefaultInputFilePath = "..\\..\\..\\cwk_train.csv";
        private const string DefaultOutputFilePath = "..\\..\\..\\output.csv";
        private const bool DefaultIsEvolvingSwarm = true;
        private const int DefaultIterationsPerGeneration = 10;
        private const int DefaultRunCount = 100;
        private const int DefaultK = 10;
        private const double DefaultMutationProbability = 0.5;
        private const int DefaultIterationCount = 5000;
        private const int DefaultParticleCount = 100;
        public const double DefaultGlobalPullFactor = 0.1;
        public const double DefaultPersonalPullFactor = 0.5;

        static void Main(string[] args)
        {
            Console.Write($"Please input the data set file path (enter to use default): ");
            var inputFilePath = Console.ReadLine();
            inputFilePath = string.IsNullOrWhiteSpace(inputFilePath) ? DefaultInputFilePath : inputFilePath;
            var days = ReadFile(inputFilePath);

            var runCount = Read($"Please input the number of runs (enter to use default {DefaultRunCount}): ", DefaultRunCount, int.Parse);

            Console.Write($"Please input the output results file path (enter to use default): ");
            var outputFilePath = Console.ReadLine();
            outputFilePath = string.IsNullOrWhiteSpace(outputFilePath) ? DefaultOutputFilePath : outputFilePath;

            var iterationCount = Read($"Please input the number of iterations (enter to use default {DefaultIterationCount}): ", DefaultIterationCount, int.Parse);
            var particleCount = Read($"Please input the number of particle (enter to use default {DefaultParticleCount}): ", DefaultParticleCount, int.Parse);
            var personalPullFactor = Read($"Please input the pull factor for personal best (enter to use default {DefaultPersonalPullFactor}): ", DefaultPersonalPullFactor, double.Parse);
            var globalPullFactor = Read($"Please input the pull factor for global best (enter to use default {DefaultGlobalPullFactor}): ", DefaultGlobalPullFactor, double.Parse);

            var isEvolving = IsEvolvingParticleSwarm();

            var pso = isEvolving ? 
                GetEvolvingParticleSwarm(iterationCount, personalPullFactor, globalPullFactor) : 
                GetBasicParticleSwarm(iterationCount, personalPullFactor, globalPullFactor);

            for (var i = 0; i < runCount; i++)
            {
                var hub = new Hub(NumberOfDestinations, days, pso);

                var finalResult = hub.Simulate(particleCount);

                AppendToResultsFile(outputFilePath, hub.Cost(finalResult), finalResult);
            }
        }

        private static bool IsEvolvingParticleSwarm()
        {
            Console.Write($"Please input whether particle attraction evolves y/n (enter to use default {(DefaultIsEvolvingSwarm ? "y" : "n")}: ");
            var evolvingString = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(evolvingString))
                return DefaultIsEvolvingSwarm;

            return evolvingString.Trim().ToUpper().Equals("Y");
        }

        private static IParticleSwarm GetEvolvingParticleSwarm(int iterationCount, double personalPullFactor, double globalPullFactor)
        {
            var iterationsPerGeneration = Read($"Please input the number of iterations per generation (enter to use default {DefaultIterationsPerGeneration}): ", DefaultIterationsPerGeneration, int.Parse);
            var mutationProbability = Read($"Please input the probability of mutation (enter to use default {DefaultMutationProbability}): ", DefaultMutationProbability, double.Parse);
            var k = Read($"Please input the tournament size (enter to use default {DefaultK}): ", DefaultK, int.Parse);

            var evolution = new Evolution(iterationsPerGeneration, k, mutationProbability);

            return new EvolvingParticleSwarm(evolution, iterationCount, personalPullFactor, globalPullFactor);
        }

        private static IParticleSwarm GetBasicParticleSwarm(int iterationCount, double personalPullFactor, double globalPullFactor)
        {
            return new BasicParticleSwarm(iterationCount, personalPullFactor, globalPullFactor);
        }

        private static void AppendToResultsFile(string filePath, double cost, double[] weights)
        {
            var line = $"{cost}{weights.Aggregate("", (s, w) => s + ',' + w)}";

            File.AppendAllText(filePath, line + Environment.NewLine);
        }

        private static T Read<T>(string text, T defaultValue, Func<string, T> parser)
        {
            Console.Write(text);

            var stringValue = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(stringValue))
                return defaultValue;

            return parser(stringValue);
        }

        private static List<Day> ReadFile(string filePath)
        {
            var fileText = File.ReadAllText(filePath).Replace("\r", "").Split('\n').Where(l => !string.IsNullOrWhiteSpace(l));

            var days = new List<Day>();

            foreach (var line in fileText)
            {
                var data = line.Split(',').Select(double.Parse).ToArray();

                if(data.Length < NumberOfDestinations + 1)
                    throw new FormatException("Data file does not contain an appropriate amount of data");

                var actualDemand = data[0];

                var measurements = data.Skip(1).ToArray();

                days.Add(new Day(actualDemand, measurements));
            }

            return days;
        }
    }
}
