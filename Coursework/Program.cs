using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coursework
{
    public sealed class Program
    {
        private const int NumberOfDestinations = 13;

        // Defaults
        private const string DefaultFilePath = "..\\..\\..\\cwk_train.csv";
        private const int DefaultIterationsPerGeneration = 10;
        private const int DefaultK = 10;
        private const double DefaultMutationProbability = 0.5;
        private const int DefaultIterationCount = 5000;
        private const int DefaultParticleCount = 100;

        static void Main(string[] args)
        {
            Console.Write("Please input the data set file path (enter to use default): ");

            var filePath = Console.ReadLine();

            filePath = string.IsNullOrWhiteSpace(filePath) ? DefaultFilePath : filePath;

            var days = ReadFile(filePath);

            var iterationCount = Read("Please input the number of iterations (enter to use default): ", DefaultIterationCount, int.Parse);
            var particleCount = Read("Please input the number of particle (enter to use default): ", DefaultParticleCount, int.Parse);
            var iterationsPerGeneration = Read("Please input the number of iterations per generation (enter to use default): ", DefaultIterationsPerGeneration, int.Parse);
            var mutationProbability = Read("Please input the probability of mutation (enter to use default): ", DefaultMutationProbability, double.Parse);
            var k = Read("Please input the tournament size (enter to use default): ", DefaultK, int.Parse);
            
            var evolution = new Evolution(iterationsPerGeneration, k, mutationProbability);

            var pso = new ParticleSwarm(evolution, iterationCount);

            var hub = new Hub(NumberOfDestinations, days, pso);

            var finalResult = hub.Simulate(particleCount);

            Console.WriteLine($"Best Result: {hub.Cost(finalResult)} [{finalResult.Aggregate("", (s, e) => s + e + " ")}]");
        }

        private static T Read<T>(string text, T defaultValue, Func<string, T> parser)
        {
            Console.Write(text);

            var stringValue = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(stringValue))
                return defaultValue;

            try
            {
                return parser(stringValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return defaultValue;
            }
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
