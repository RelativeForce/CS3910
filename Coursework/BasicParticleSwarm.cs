using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class BasicParticleSwarm : IParticleSwarm
    {
        private readonly IConsoleLogger _logger;
        private readonly int _iterationCount;
        private readonly double _personalPullFactor;
        private readonly double _globalPullFactor;

        public BasicParticleSwarm(IConsoleLogger logger, int iterationCount, double personalPullFactor, double globalPullFactor)
        {
            _logger = logger;
            _iterationCount = iterationCount;
            _personalPullFactor = personalPullFactor;
            _globalPullFactor = globalPullFactor;
        }

        public Position Simulate(List<Particle> particles)
        {
            Particle.GlobalBest = particles.OrderBy(p => p.Position.Value).First().Position;

            for (var i = 0; i < _iterationCount; i++)
            {
                foreach (var particle in particles.AsParallel())
                {
                    particle.Move();
                }

                var newGlobalBestPosition = particles.OrderBy(p => p.Position.Value).First().Position;

                if (newGlobalBestPosition.Value < Particle.GlobalBest.Value)
                {
                    _logger.LogNewBestForRunInProgress(i, newGlobalBestPosition.Value, newGlobalBestPosition.Vector);
                    Particle.GlobalBest = newGlobalBestPosition;
                }
            }

            return Particle.GlobalBest;
        }

        public Attraction NewAttraction()
        {
            return new Attraction(_personalPullFactor, _globalPullFactor);
        }
    }
}
