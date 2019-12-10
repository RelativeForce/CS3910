using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class BasicParticleSwarm : IParticleSwarm
    {
        private readonly ConsoleLogger _logger;
        private readonly int _iterationCount;
        private readonly double _cognitiveAttraction;
        private readonly double _socialAttraction;

        public BasicParticleSwarm(ConsoleLogger logger, int iterationCount, double cognitiveAttraction, double socialAttraction)
        {
            _logger = logger;
            _iterationCount = iterationCount;
            _cognitiveAttraction = cognitiveAttraction;
            _socialAttraction = socialAttraction;
        }

        public Position Simulate(List<Particle> particles)
        {
            var globalBest = particles.OrderBy(p => p.Position.Value).First().Position;

            for (var i = 0; i < _iterationCount; i++)
            {
                foreach (var particle in particles.AsParallel())
                {
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
            return new Attraction(_cognitiveAttraction, _socialAttraction);
        }
    }
}
