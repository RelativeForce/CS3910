using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman
{
    public class AdjacencyMatrix
    {
        public readonly IList<Node> Nodes;
        public readonly int NodeCount;
        public IList<int> Indexes { get => Nodes.Select((n, i) => i).ToList(); }

        private long _searched;
        private long _total;
        private readonly double[] _matrix;

        public AdjacencyMatrix(Map map)
        {
            NodeCount = map.Nodes.Count;
            _searched = 0;
            _total = Factorial(NodeCount - 2);
            Nodes = map.Nodes;
            _matrix = new double[NodeCount * NodeCount];

            int x = 0;
            foreach(var node in map.Nodes)
            {
                int y = 0;
                foreach (var target in map.Nodes)
                {
                    _matrix[Index(x, y)] = node.Links.FirstOrDefault(l => l.Same(node, target))?.Distance ?? -1;
                    y++;
                }
                x++;
            }
        }

        public double Distance(int x, int y)
        {
            return _matrix[Index(x, y)];
        }

        public Route GenerateRandomRoute()
        {
            var remainingNodes = Indexes;
            var route = new List<int>();
            var random = new Random();

            while (remainingNodes.Any())
            {
                var nextIndex = remainingNodes[random.Next(remainingNodes.Count)];

                route.Add(nextIndex);

                remainingNodes.Remove(nextIndex);
            }

            return ToRoute(route);
        }

        public Route RandomRoute(int numberOfIterations)
        {
            Route best = null;

            for (int i = 0; i < numberOfIterations; i++)
            {
                var randomRoute = GenerateRandomRoute();

                if (best == null || best.Length > randomRoute.Length)
                    best = randomRoute;

            }

            return best;
        }

        public Route Optimise(Route best)
        {
            bool improved;
            do
            {
                improved = false;
                var optimisedRoute = OptimiseRoute(best);

                Console.WriteLine(optimisedRoute.Path + "- " + optimisedRoute.Length);

                if (best.Length > optimisedRoute.Length)
                {
                    best = optimisedRoute;
                    improved = true;
                }

            } while (improved);

            return best;
        }

        public Route ToRoute(IList<int> indexRoute)
        {
            return new Route(indexRoute, indexRoute.Select(i => Nodes[i]).ToList(), GetCost(indexRoute));
        }

        public double GetCost(IList<int> indexRoute)
        {
            double cost = 0;

            int previousIndex = -1;
            foreach (var index in indexRoute)
            {
                if (previousIndex != -1)
                {
                    cost += Distance(previousIndex, index);
                }

                previousIndex = index;
            }

            cost += Distance(previousIndex, indexRoute[0]);

            return cost;
        }

        public void TraverseAllRoutes(Route best)
        {
            var first = best.Nodes.First();

            Console.WriteLine($"Searching all combinations starting at { first.Name }\n");

            Traverse(ref best, new List<int> { best.Indexes.First() });

            Console.WriteLine("Done " + best.Path + "- " + best.Length);
        }

        protected int Index(int x, int y)
        {
            return (NodeCount * y) + x;
        }

        private void Traverse(ref Route bestRoute, IList<int> currentRoute)
        {
            if (currentRoute.Count == NodeCount)
            {
                currentRoute.Add(currentRoute.First());

                _searched++;

                if (_searched % 10000 == 0)
                {
                    Console.Write($"\r{_searched}/{_total} ({Math.Round(((double)_searched * 100) / _total, 3)}%)  ");
                }

                var route = ToRoute(currentRoute);

                if (bestRoute.Length > route.Length || bestRoute.Length == -1)
                {
                    bestRoute = route;
                    Console.WriteLine("\n" + bestRoute.Path + "- " + bestRoute.Length + "\n");
                }

                return;
            }

            foreach (var node in Indexes.Where(n => !currentRoute.Contains(n)))
            {
                var copy = currentRoute.AsEnumerable().ToList();
                copy.Add(node);

                Traverse(ref bestRoute, copy);
            }
        }

        private static int Factorial(int n)
        {
            if (n < 1)
                return 1;

            if (n == 1)
                return 1;

            return Factorial(n - 1) * n;
        }

        private Route OptimiseRoute(Route route)
        {
            var indexes = route.Indexes;
            var start = indexes[0];

            var bestInNeighborhood = route;

            foreach (var index in indexes.Skip(1))
            {
                foreach (var target in indexes.Skip(1))
                {
                    if (index == target || indexes[index] == start || indexes[target] == start)
                        continue;

                    var testIndexes = indexes.ToArray();

                    // Swap stops
                    var temp = testIndexes[index];
                    testIndexes[index] = testIndexes[target];
                    testIndexes[target] = temp;

                    var testRoute = ToRoute(testIndexes.ToList());

                    if (bestInNeighborhood.Length > testRoute.Length)
                    {
                        bestInNeighborhood = testRoute;
                    }
                }
            }

            return bestInNeighborhood;
        }
    }
}
