using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TravelingSalesman
{
    public sealed class Program
    {
        public static long Searched = 0;
        public static long Total = 0;

        public static void Main(string[] args)
        {
            var numberOfRandomIterations = 64;
            var numberOfAntIterations = 100000;
            var numberOfAnts = 100;
            var speedOfAnt = 0.15;

            var fileText = File.ReadAllText("..\\..\\..\\ulysses16.csv").Replace("\r", "").Split('\n');

            var map = new Map(fileText);
            var matrix = new AdjacencyMatrix(map);

            Total = Factorial(map.Nodes.Count - 2);

            var best = GetRandomRoute(numberOfRandomIterations, matrix);

            best = ImproveRoute(best, matrix);

            Console.WriteLine("Optimum from Best:\n" + best.Path + "- " + best.Length);

            Console.ReadLine();

            var antRoute = SimulateAnts(map, numberOfAntIterations, numberOfAnts, speedOfAnt, best.Nodes[0]);

            Console.WriteLine("Ant route:\n" + antRoute.Path + "- " + antRoute.Length);

            antRoute = ImproveRoute(antRoute, matrix);

            Console.WriteLine("Optimum from Ant route:\n" + antRoute.Path + "- " + antRoute.Length);

            Console.ReadLine();

            TraverseAllRoutes(best, map, best.Nodes[0]);
        }

        #region Normal Optimistation

        private static void TraverseAllRoutes(Route best, Map map, Node start)
        {
            Console.WriteLine($"Searching all combinations starting at {start.Name}\n");

            Traverse(map.Nodes, best, new List<Node> { start });

            Console.WriteLine("Done " + best.Path + "- " + best.Length);
        }

        private static Route GetRandomRoute(int numberOfIterations, AdjacencyMatrix matrix)
        {
            var best = new Route();

            for (int i = 0; i < numberOfIterations; i++)
            {
                var randomRoute = matrix.RandomRoute();

                //Console.WriteLine(randomRoute.Path + "- " + randomRoute.Length);

                if (best.Length > randomRoute.Length || best.Length == -1)
                    best = randomRoute;

            }

            Console.WriteLine("Best Random:\n" + best.Path + "- " + best.Length);

            return best;
        }

        private static Route ImproveRoute(Route best, AdjacencyMatrix matrix)
        {
            bool improved;
            do
            {
                improved = false;
                var optimisedRoute = matrix.Optimise(best);

                Console.WriteLine(optimisedRoute.Path + "- " + optimisedRoute.Length);

                if (best.Length > optimisedRoute.Length)
                {
                    best = optimisedRoute;
                    improved = true;
                }

            } while (improved);

            return best;
        }

        private static int Factorial(int n)
        {
            if (n < 1)
                return 1;

            if (n == 1)
                return 1;

            return Factorial(n - 1) * n;
        }

        private static void Traverse(IList<Node> allNodes, Route bestRoute, IList<Node> currentRoute)
        {
            if (currentRoute.Count == allNodes.Count)
            {
                currentRoute.Add(currentRoute.First());

                Searched++;

                if (Searched % 10000 == 0)
                {
                    Console.Write($"\r{Searched}/{Total} ({Math.Round(((double)Searched * 100) / Total, 3)}%)  ");
                }

                var cost = GetCostOfRoute(currentRoute);

                if (cost == -1)
                    return;

                if (bestRoute.Length > cost || bestRoute.Length == -1)
                {
                    bestRoute.Update(currentRoute, cost);
                    Console.WriteLine("\n" + bestRoute.Path + "- " + cost + "\n");
                }

                return;
            }

            foreach (var node in allNodes.Where(n => !currentRoute.Contains(n)))
            {
                var copy = currentRoute.AsEnumerable().ToList();
                copy.Add(node);

                Traverse(allNodes, bestRoute, copy);
            }
        }


        #endregion

        private static Route SimulateAnts(Map map, int numberOfIterations, int numberOfAnts, double speedOfAnt, Node start)
        {
            var ants = new List<Ant>();
            for (int i = 0; i < numberOfAnts; i++)
            {
                ants.Add(new Ant(speedOfAnt, start, map.Nodes));
            }

            for (int i = 0; i < numberOfIterations; i++)
            {
                foreach (var ant in ants)
                {
                    ant.Perform();
                }

                foreach (var link in map.Links)
                {
                    link.Decay();
                }
            }

            var current = start;
            var route = new List<Node> {start};
            int count = 0;
            do
            {
                var best = current.Links.Where(l => !route.Any(n => l.Same(current, n))).OrderByDescending(l => l.PheromoneOnLink).FirstOrDefault();

                if (best == null)
                {
                    current = start;
                    route.Add(current);
                }
                else
                {
                    current = best.A == current ? best.B : best.A;
                    route.Add(current);
                }

                count++;
            } while (current != start && count <= map.Nodes.Count);
            
            return new Route(route, GetCostOfRoute(route));
        }

        private static double GetCostOfRoute(IList<Node> route)
        {
            double cost = 0;

            Node previous = null;
            foreach (var node in route)
            {
                if (previous != null)
                {
                    cost += node.Links.First(l => l.Same(node, previous)).Distance;
                }
         
                previous = node;
            }

            return cost;
        }
    }
}
