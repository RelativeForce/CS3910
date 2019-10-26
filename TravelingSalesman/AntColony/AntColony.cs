using System.Collections.Generic;

namespace TravelingSalesman.Ants
{
    public sealed class AntColony
    {
        private readonly AntColonyAdjacencyMatrix _matrix;

        public AntColony(AntColonyAdjacencyMatrix matrix)
        {
            _matrix = matrix;
        }

        public Route SimulateAnts(int numberOfIterations, int numberOfAnts, double speedOfAnt, int start)
        {
            var ants = new List<Ant>();
            for (int i = 0; i < numberOfAnts; i++)
            {
                ants.Add(new Ant(speedOfAnt, start, _matrix));
            }

            for (int i = 0; i < numberOfIterations; i++)
            {
                foreach (var ant in ants)
                {
                    ant.Perform();
                }

                _matrix.Decay();
            }

            return _matrix.PheromoneRoute(start);
        }
    }
}
