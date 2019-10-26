using System;
using System.IO;
using System.Linq;
using TravelingSalesman.Ants;

namespace TravelingSalesman
{
    public sealed class Program
    {
        private const string DefaultFilePath = "..\\..\\..\\ulysses16.csv";

        public static void Main(string[] args)
        {          
            Console.WriteLine("Traveling Salesman Problem - Joshua Eddy");

            var matrix = new AntColonyAdjacencyMatrix(ReadFile());

            CheckRoute(matrix);

            RandomSearch(matrix);

            AntColonyOptimisation(matrix);

            matrix.TraverseAllRoutes();
        }

        private static Map ReadFile()
        {
            Console.Write("Please input the data set file path (enter to use default): ");

            var filePath = Console.ReadLine();

            filePath = string.IsNullOrWhiteSpace(filePath) ? DefaultFilePath : filePath;

            var fileText = File.ReadAllText(filePath).Replace("\r", "").Split('\n');

            return new Map(fileText);
        } 

        private static void CheckRoute(AdjacencyMatrix matrix)
        {
            Console.WriteLine("\nCheck Route");
            Console.Write("Please input a route to evaluate (enter to skip): ");

            var routeString = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(routeString))
                return;

            var indexRoute = routeString.Split(' ').Select(nodeString => matrix.NameToIndex(nodeString)).ToList();

            var route = matrix.ToRoute(indexRoute);

            Console.WriteLine("Length:" + route.Length);
        }

        private static void RandomSearch(AdjacencyMatrix matrix)
        {
            Console.WriteLine("\nRandom Search");
            Console.Write("Please input the number of random searches (enter to skip): ");

            var numberString = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(numberString))
                return;

            var numberOfRandomIterations = int.Parse(numberString);

            var best = matrix.RandomRoute(numberOfRandomIterations);

            Console.WriteLine($"Best Random:\n{best.Path}- {best.Length}\n");

            Console.WriteLine("Local Search");

            best = matrix.Optimise(best);

            Console.WriteLine($"Optimum from Best:\n{best.Path}- {best.Length}");
        }

        private static void AntColonyOptimisation(AntColonyAdjacencyMatrix matrix)
        {
            const double speedOfAnt = 0.15;

            Console.WriteLine("\nAnt Colony Optimisation");

            Console.Write("Please input the number of ants (enter to skip): ");

            var numberString = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(numberString))
                return;

            var numberOfAnts = int.Parse(numberString);

            Console.Write("Please input the number of iterations: ");
            var numberOfAntIterations = int.Parse(Console.ReadLine() ?? "");

            Console.Write("Please input the starting node: ");
            var startName = Console.ReadLine() ?? "";

            var antColony = new AntColony(matrix);

            var antRoute = antColony.SimulateAnts(numberOfAntIterations, numberOfAnts, speedOfAnt, matrix.NameToIndex(startName));

            Console.WriteLine("Ant route:\n" + antRoute.Path + "- " + antRoute.Length + "\n");

            Console.WriteLine("Local Search");

            antRoute = matrix.Optimise(antRoute);

            Console.WriteLine("Optimum from Ant route:\n" + antRoute.Path + "- " + antRoute.Length);
        }
    }
}
