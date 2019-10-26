using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman.Ants
{
    public class AntColonyAdjacencyMatrix : AdjacencyMatrix
    {
        public const double PheromoneAmount = 0.566546;
        public const double EvaporationRate = 0.01;

        private readonly double[] _pheromone;

        public AntColonyAdjacencyMatrix(Map map) : base(map)
        {
            _pheromone = new double[NodeCount * NodeCount];
        }

        public double Pheromone(int x, int y)
        {
            return _pheromone[Index(x, y)];
        }

        public void Decay()
        {
            for (int i = 0; i < _pheromone.Length; i++)
            {
                _pheromone[i] -= _pheromone[i] * EvaporationRate;
            }
        }

        public void Compound(int x, int y)
        {
            var amount = PheromoneAmount / Distance(x, y);

            _pheromone[Index(x, y)] += amount;
            _pheromone[Index(y, x)] += amount;
        }

        public Route PheromoneRoute(int start)
        {
            var current = start;
            var route = new List<int> { start };

            for (int index = 1; index < NodeCount; index++)
            {
                current = Indexes.Where(i => i != current && !route.Contains(i)).OrderByDescending(i => Pheromone(current, i)).First();
                route.Add(current);
            }

            route.Add(start);

            return ToRoute(route);
        }
    }
}
