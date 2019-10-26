using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman
{
    public sealed class Ant
    {
        private static readonly Random Generator = new Random();

        private readonly double _speed;
        private readonly Node _startingNode;
        private Node _currentNode;
        private readonly IList<Node> _allNodes;
        private IList<Node> _remainingNodes;
        private double _remainingDistanceToNextNode;
        private Node _nextNode;
        private Link _currentLink;

        public Ant(double speed, Node startingNode, IList<Node> allNodes)
        {
            _speed = speed;
            _currentNode = startingNode;
            _startingNode = startingNode;
            _allNodes = allNodes;
            _remainingDistanceToNextNode = 0;
            _nextNode = null;
            _currentLink = null;

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
                    var link = _currentNode.Links.First(l => l.Same(_currentNode, _startingNode));

                    SetNextNode(link);
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
            _remainingNodes = _allNodes.Where(n => n != _startingNode).ToList();
        }

        private void SelectNextNode()
        {
            if (_nextNode != null)
            {
                _currentNode = _nextNode;
                _nextNode = null;
                _currentLink = null;
                _remainingDistanceToNextNode = 0;
            }

            Link bestLink = null;
            double desirability = 0;
            foreach (var node in _currentNode.LinkedTo.Where(n => _remainingNodes.Contains(n)))
            {
                var link = _currentNode.Links.First(l => l.Same(_currentNode, node));

                var currentLinkFactor = GetDesirabilityFactor(link);

                if (bestLink != null && !(desirability < currentLinkFactor)) 
                    continue;

                bestLink = link;
                desirability = currentLinkFactor;
            }

            SetNextNode(bestLink);
        }
        
        private static double GetDesirabilityFactor(Link link)
        {
            var randomness = (Math.Pow(Generator.NextDouble(), 2) * -1) + 1;

            return (link.PheromoneOnLink - link.Distance) * randomness;
        }

        private void SetNextNode(Link link)
        {
            _nextNode = link.A == _currentNode ? link.B : link.A;
            _currentLink = link;
            _remainingDistanceToNextNode = link.Distance;
            _remainingNodes.Remove(_nextNode);
            _currentLink.Compound();
        }

        private void Travel()
        {
            _remainingDistanceToNextNode -= _speed;
        }
    }
}
