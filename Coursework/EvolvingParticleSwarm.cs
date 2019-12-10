using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class EvolvingParticleSwarm : IParticleSwarm
    {
        private readonly ConsoleLogger _logger;
        private readonly Evolution _evolution;
        private readonly int _iterationCount;
        private readonly double _cognitiveAttraction;
        private readonly double _socialAttraction;

        public EvolvingParticleSwarm(ConsoleLogger logger, Evolution evolution, int iterationCount, double cognitiveAttraction, double socialAttraction)
        {
            _logger = logger;
            _evolution = evolution;
            _iterationCount = iterationCount;
            _cognitiveAttraction = cognitiveAttraction;
            _socialAttraction = socialAttraction;
        }

        public Position Simulate(List<Particle> particles)
        {
            var globalBest = particles.OrderBy(p => p.Position.Value).First().Position;

            for (var i = 0; i < _iterationCount; i++)
            {
                var isEvolutionIteration = _evolution.ShouldEvolve(i);

                if (isEvolutionIteration)
                {
                    _evolution.Evolve(particles);
                }

                foreach (var particle in particles.AsParallel())
                {
                    if (isEvolutionIteration)
                    {
                        particle.TrackPosition();
                    }

                    particle.Move(globalBest);
                }

                var newGlobalBestPosition = particles.OrderBy(p => p.Position.Value).First().Position;

                if (newGlobalBestPosition.Value < globalBest.Value)
                { 
                    _logger.LogNewBestForRunInProgress(i, newGlobalBestPosition.Value, newGlobalBestPosition.Vector);
                    globalBest = newGlobalBestPosition;
                }
            }

            return globalBest;
        }

        public Attraction NewAttraction()
        {
            var random = new Random();

            var cognitiveAttraction = _cognitiveAttraction * random.NextDouble();
            var socialAttraction = _socialAttraction * random.NextDouble();

            return new Attraction(cognitiveAttraction, socialAttraction);
        }
    }
}
