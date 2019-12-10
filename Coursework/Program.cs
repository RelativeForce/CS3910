using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coursework
{
    public sealed class Program
    {
        private const int NumberOfMeasurements = 13;

        // Defaults
        private const string TrainDataFilePath = "..\\..\\..\\cwk_train.csv";
        private const string TestDataFilePath = "..\\..\\..\\cwk_test.csv";
        private const string DefaultOutputFilePath = "..\\..\\..\\output.csv";
        private const bool DefaultIsEvolvingSwarm = true;
        private const int DefaultIterationsPerGeneration = 10;
        private const int DefaultRunCount = 100;
        private const int DefaultK = 10;
        private const double DefaultMutationProbability = 0.05;
        private const int DefaultIterationCount = 2000;
        private const int DefaultParticleCount = 100;
        public const double DefaultSocialAttraction = 0.1;
        public const double DefaultCognitiveAttraction = 0.5;
        public const bool ShowNewBestForRunInProgress = false;

        static void Main(string[] args)
        {
            var trainCostEvaluator = ReadDataFile(TrainDataFilePath);
            var testCostEvaluator = ReadDataFile(TestDataFilePath);
            var logger = new ConsoleLogger(ShowNewBestForRunInProgress);

            Console.WriteLine("CS3910 Coursework - Joshua Eddy");

            var runCount = Read($"Number of runs (enter to use default {DefaultRunCount}): ", DefaultRunCount, int.Parse);
            var outputFilePath = Read($"Output results file path (enter to use default '{DefaultOutputFilePath}'): ", DefaultOutputFilePath, s => s);
            var iterationCount = Read($"Number of iterations (enter to use default {DefaultIterationCount}): ", DefaultIterationCount, int.Parse);
            var particleCount = Read($"Number of particle (enter to use default {DefaultParticleCount}): ", DefaultParticleCount, int.Parse);
            var cognitiveAttraction = Read($"Cognitive attraction factor (enter to use default {DefaultCognitiveAttraction}): ", DefaultCognitiveAttraction, double.Parse);
            var socialAttraction = Read($"Social attraction factor (enter to use default {DefaultSocialAttraction}): ", DefaultSocialAttraction, double.Parse);

            var isEvolving = IsEvolvingParticleSwarm();

            var pso = isEvolving ? 
                GetEvolvingParticleSwarm(logger, iterationCount, cognitiveAttraction, socialAttraction) : 
                GetBasicParticleSwarm(logger, iterationCount, cognitiveAttraction, socialAttraction);

            RunAlgorithm(runCount, trainCostEvaluator, pso, particleCount, testCostEvaluator, logger, outputFilePath);
        }

        private static void RunAlgorithm(int runCount, CostEvaluator trainCostEvaluator, IParticleSwarm pso, int particleCount, CostEvaluator testCostEvaluator, ConsoleLogger logger, string outputFilePath)
        {
            Console.WriteLine($"Starting {runCount} runs...");

            for (var i = 0; i < runCount; i++)
            {
                var simulator = new ParticleSwarmSimulator(NumberOfMeasurements, trainCostEvaluator, pso);

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

            foreach (var line in fileText)
            {
                var data = line.Split(',').Select(double.Parse).ToArray();

                if(data.Length < NumberOfMeasurements + 1)
                    throw new FormatException("Data file does not contain an appropriate amount of data");

                var actualDemand = data[0];

                var measurements = data.Skip(1).ToArray();

                days.Add(new Day(actualDemand, measurements));
            }

            return new CostEvaluator(days);
        }
    }
}
