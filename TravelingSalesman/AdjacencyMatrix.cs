using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman
{
    public sealed class AdjacencyMatrix
    {
        private readonly double[][] _matrix;
        public readonly IList<Node> Nodes;
        public readonly int Width;
        public readonly int Height;

        public AdjacencyMatrix(Map map)
        {
            Width = map.Nodes.Count;
            Height = map.Nodes.Count;
            Nodes = map.Nodes;
            _matrix = new double[Width][];

            InitialiseMatrix();

            int x = 0;
            foreach(var node in map.Nodes)
            {
                int y = 0;
                foreach (var target in map.Nodes)
                {
                    _matrix[x][y] = node.Links.FirstOrDefault(l => l.Same(node, target))?.Distance ?? -1;
                    y++;
                }
                x++;
            }
        }

        public double ValueAt(int x, int y)
        {
            return _matrix[x][y];
        }

        private void InitialiseMatrix()
        {
            for (var x = 0; x < Width; x++)
                _matrix[x] = new double[Height];
        }

        public Route RandomRoute()
        {
            var remainingNodes = new List<int>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                remainingNodes.Add(i);
            }

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

        public List<int> ToIndexRoute(Route route)
        {
            return route.Nodes.Select(n => Nodes.IndexOf(n)).ToList();
        }

        public Route ToRoute(List<int> indexRoute)
        {
            var routeNodes = new List<Node>();
            double cost = 0;

            int previousIndex = -1;
            foreach (var index in indexRoute)
            {
                routeNodes.Add(Nodes[index]);

                if (previousIndex != -1)
                {
                    cost += ValueAt(previousIndex, index);
                }

                previousIndex = index;
            }

            cost += ValueAt(previousIndex, indexRoute[0]);

            return new Route(routeNodes, cost);
        }

        public Route Optimise(Route route)
        {
            var indexes = ToIndexRoute(route);
            var start = indexes[0];

            var bestInNeighborhood = route;

            foreach(var index in indexes.Skip(1))
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
