using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coursework
{
    public sealed class Program
    {
        private const string DefaultFilePath = "..\\..\\..\\cwk_train.csv";
        private const int NumberOfDestinations = 13;
        public const double PalletsPerTruck = 35.0;

        static void Main(string[] args)
        {
            Console.Write("Please input the data set file path (enter to use default): ");

            var filePath = Console.ReadLine();

            filePath = string.IsNullOrWhiteSpace(filePath) ? DefaultFilePath : filePath;

            var days = ReadFile(filePath);

            var hub = new Hub(NumberOfDestinations, days, 3.5 * PalletsPerTruck, 5.6 * PalletsPerTruck);

            var finalResult = hub.Simulate(50, 10000);

            Console.WriteLine($"Best Result: {hub.Cost(finalResult)} [{finalResult.Select(Hub.ToTrucks).Aggregate("", (s, e) => s + e + " ")}]");

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

                var overallDemand = data[0];

                var demandMeasurements = data.Skip(1).ToArray();

                days.Add(new Day(overallDemand, demandMeasurements));
            }

            return days;
        }
    }
}
