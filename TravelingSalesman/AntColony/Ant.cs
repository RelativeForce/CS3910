using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman.AntColony
{
    public sealed class Ant
    {
        private static readonly Random Generator = new Random();

        private readonly double _speed;
        private readonly int _startingNode;
        private int _currentNode;
        private readonly IList<int> _allNodes;
        private readonly AntColonyAdjacencyMatrix _matrix;
        private IList<int> _remainingNodes;
        private double _remainingDistanceToNextNode;
        private int? _nextNode;

        public Ant(double speed, int startingNode, AntColonyAdjacencyMatrix matrix)
        {
            _speed = speed;
            _currentNode = startingNode;
            _startingNode = startingNode;
            _allNodes = matrix.Indexes;
            _matrix = matrix;
            _remainingDistanceToNextNode = 0;
            _nextNode = null;

            ResetPath();
        }

        public void Perform()
        {
            if (_remainingDistanceToNextNode <= 0)
            {
                var remainingNodes = _remainingNodes.Any();
                var completedPath = !remainingNodes && _nextNode == _startingNode;

                if (!remainingNodes && !completedPath)
                {
                    SetNextNode(_startingNode);
                }
                else
                {
                    if (completedPath)
                    {
                        ResetPath();
                    }

                    SelectNextNode();
                }
            }
            else
            {
                Travel();
            }
        }

        private void ResetPath()
        {
            _remainingNodes = _allNodes.Take(1).ToList();
        }

        private void SelectNextNode()
        {
            if (_nextNode != null)
            {
                _currentNode = _nextNode.Value;
                _nextNode = null;
                _remainingDistanceToNextNode = 0;
            }

            int? bestNode = null;
            double desirability = 0;
            foreach (var node in _remainingNodes)
            {
                var currentDesirability = GetDesirabilityFactor(node);

                if (bestNode != null && !(desirability < currentDesirability))
                    continue;

                bestNode = node;
                desirability = currentDesirability;
            }

            SetNextNode(bestNode.Value);
        }

        private double GetDesirabilityFactor(int node)
        {
            var randomness = Math.Pow(Generator.NextDouble(), 2) * -1 + 1;
            var distance = _matrix.Distance(_currentNode, node);
            var pheromoneOnLink = _matrix.Pheromone(_currentNode, node);

            return (pheromoneOnLink - distance) * randomness;
        }

        private void SetNextNode(int node)
        {
            _nextNode = node;
            _remainingDistanceToNextNode = _matrix.Distance(_currentNode, node);
            _remainingNodes.Remove(node);
            _matrix.Compound(_currentNode, node);
        }

        private void Travel()
        {
            _remainingDistanceToNextNode -= _speed;
        }
    }
}
