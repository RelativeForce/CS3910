using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coursework
{
    public sealed class Program
    {
        private const bool ShowNewBestForRunInProgress = false;

        // Default parameters
        private const string DefaultTrainDataFilePath = "..\\..\\..\\cwk_train.csv";
        private const string DefaultTestDataFilePath = "..\\..\\..\\cwk_test.csv";
        private const string DefaultOutputFilePath = "..\\..\\..\\output.csv";
        private const bool DefaultIsEvolvingSwarm = true;
        private const int DefaultIterationsPerGeneration = 10;
        private const int DefaultRunCount = 100;
        private const int DefaultK = 10;
        private const double DefaultMutationProbability = 0.05;
        private const int DefaultIterationCount = 2000;
        private const int DefaultParticleCount = 100;
        private const double DefaultSocialAttraction = 0.1;
        private const double DefaultCognitiveAttraction = 0.5;

        static void Main(string[] args)
        {
            var logger = new ConsoleLogger(ShowNewBestForRunInProgress);

            Console.WriteLine("CS3910 Coursework - Joshua Eddy");

            var runCount = Read($"Number of runs (enter to use default {DefaultRunCount}): ", DefaultRunCount, int.Parse);

            var trainDataFilePath = Read($"Training data file path (enter to use default '{DefaultTrainDataFilePath}'): ", DefaultTrainDataFilePath, s => s);
            var trainCostEvaluator = ReadDataFile(trainDataFilePath);

            var testDataFilePath = Read($"Test data file path (enter to use default '{DefaultTestDataFilePath}'): ", DefaultTestDataFilePath, s => s);
            var testCostEvaluator = ReadDataFile(testDataFilePath);
            
            var outputFilePath = Read($"Output results file path (enter to use default '{DefaultOutputFilePath}'): ", DefaultOutputFilePath, s => s);
            var iterationCount = Read($"Number of iterations (enter to use default {DefaultIterationCount}): ", DefaultIterationCount, int.Parse);
            var particleCount = Read($"Number of particle (enter to use default {DefaultParticleCount}): ", DefaultParticleCount, int.Parse);
            var cognitiveAttraction = Read($"Cognitive attraction factor (enter to use default {DefaultCognitiveAttraction}): ", DefaultCognitiveAttraction, double.Parse);
            var socialAttraction = Read($"Social attraction factor (enter to use default {DefaultSocialAttraction}): ", DefaultSocialAttraction, double.Parse);

            var isEvolving = IsEvolvingParticleSwarm();

            var pso = isEvolving ? 
                GetEvolvingParticleSwarm(logger, iterationCount, cognitiveAttraction, socialAttraction) : 
                GetBasicParticleSwarm(logger, iterationCount, cognitiveAttraction, socialAttraction);

            RunAlgorithm(runCount, trainCostEvaluator, testCostEvaluator, pso, particleCount, logger, outputFilePath);
        }

        private static void RunAlgorithm(int runCount, CostEvaluator trainCostEvaluator, CostEvaluator testCostEvaluator, IParticleSwarm pso, int particleCount, ConsoleLogger logger, string outputFilePath)
        {
            Console.WriteLine($"Starting {runCount} runs...");

            for (var i = 0; i < runCount; i++)
            {
                var simulator = new ParticleSwarmSimulator(trainCostEvaluator, pso);

                var weights = simulator.Simulate(particleCount);

                var cost = testCostEvaluator.Cost(weights);

                logger.LogRunResult(i, cost, weights);

                AppendToResultsFile(outputFilePath, cost, weights);
            }
        }

        private static bool IsEvolvingParticleSwarm()
        {
            Console.Write($"Use evolutionary parameter optimisation? y/n (enter to use default {(DefaultIsEvolvingSwarm ? "y" : "n")}): ");
            var evolvingString = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(evolvingString))
                return DefaultIsEvolvingSwarm;

            return evolvingString.Trim().ToUpper().Equals("Y");
        }

        private static IParticleSwarm GetEvolvingParticleSwarm(ConsoleLogger logger, int iterationCount, double cognitiveAttraction, double socialAttraction)
        {
            var iterationsPerGeneration = Read($"Number of iterations per generation (enter to use default {DefaultIterationsPerGeneration}): ", DefaultIterationsPerGeneration, int.Parse);
            var mutationProbability = Read($"Probability of mutation (enter to use default {DefaultMutationProbability}): ", DefaultMutationProbability, double.Parse);
            var k = Read($"Tournament size (enter to use default {DefaultK}): ", DefaultK, int.Parse);

            var evolution = new Evolution(iterationsPerGeneration, k, mutationProbability);

            return new EvolvingParticleSwarm(logger, evolution, iterationCount, cognitiveAttraction, socialAttraction);
        }

        private static IParticleSwarm GetBasicParticleSwarm(ConsoleLogger logger, int iterationCount, double cognitiveAttraction, double socialAttraction)
        {
            return new BasicParticleSwarm(logger, iterationCount, cognitiveAttraction, socialAttraction);
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

        private static CostEvaluator ReadDataFile(string filePath)
        {
            var fileText = File
                .ReadAllText(filePath)
                .Replace("\r", "")
                .Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l));

            var days = new List<Day>();
            var weightCount = -1;

            foreach (var line in fileText)
            {
                var data = line.Split(',').Select(double.Parse).ToArray();

                var actualDemand = data[0];

                var measurements = data.Skip(1).ToArray();

                if (weightCount == -1)
                {
                    weightCount = measurements.Length;
                }
                else if (weightCount != measurements.Length)
                {
                    throw new FormatException("Each row must have the same amount of data");
                }

                days.Add(new Day(actualDemand, measurements));
            }

            return new CostEvaluator(days, weightCount);
        }
    }
}
