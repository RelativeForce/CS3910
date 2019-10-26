using System;
using System.IO;
using TravelingSalesman.Ants;

namespace TravelingSalesman
{
    public sealed class Program
    {
        

        public static void Main(string[] args)
        {
            var numberOfRandomIterations = 64;
            var numberOfAntIterations = 100000;
            var numberOfAnts = 100;
            var speedOfAnt = 0.15;

            var fileText = File.ReadAllText("..\\..\\..\\ulysses16.csv").Replace("\r", "").Split('\n');

            var matrix = new AntColonyAdjacencyMatrix(new Map(fileText));

            var best = matrix.RandomRoute(numberOfRandomIterations);

            Console.WriteLine("Best Random:\n" + best.Path + "- " + best.Length  + "\n");

            best = matrix.Optimise(best);

            Console.WriteLine("Optimum from Best:\n" + best.Path + "- " + best.Length);

            Console.ReadLine();

            var antColony = new AntColony(matrix);

            var antRoute = antColony.SimulateAnts(numberOfAntIterations, numberOfAnts, speedOfAnt, best.Indexes[0]);

            Console.WriteLine("Ant route:\n" + antRoute.Path + "- " + antRoute.Length + "\n");

            antRoute = matrix.Optimise(antRoute);

            Console.WriteLine("Optimum from Ant route:\n" + antRoute.Path + "- " + antRoute.Length);

            Console.ReadLine();

            matrix.TraverseAllRoutes(best);
        }
    }
}
