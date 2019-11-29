using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class ParticleSwarm : IParticleSwarm
    {
        public const double InitialGlobalPullFactor = 0.1;
        public const double InitialPersonalPullFactor = 0.5;

        private readonly IEvolution _evolution;
        private readonly int _numberOfIterations;

        public ParticleSwarm(IEvolution evolution, int numberOfIterations)
        {
            _evolution = evolution;
            _numberOfIterations = numberOfIterations;
        }

        public Position Simulate(List<Particle> particles)
        {
            particles.ForEach(p =>
            {
                p.EvaluateCurrentPosition();
                p.TrackPosition();
            });

            Particle.GlobalBest = particles.OrderBy(p => p.Position.Value).First().Position;

            for (var i = 0; i < _numberOfIterations; i++)
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

                    particle.Move();
                }

                var newGlobalBestPosition = particles.OrderBy(p => p.Position.Value).First().Position;

                if (newGlobalBestPosition.Value < Particle.GlobalBest.Value)
                {
                    Console.WriteLine($"New Best Found [Iteration: {i} Value: {newGlobalBestPosition.Value}]");
                    Particle.GlobalBest = newGlobalBestPosition;
                }
            }

            return Particle.GlobalBest;
        }

        public Attraction NewAttraction()
        {
            var random = new Random();

            var personalPull = InitialPersonalPullFactor * random.NextDouble();
            var globalPull = InitialGlobalPullFactor * random.NextDouble();

            return new Attraction(personalPull, globalPull);
        }
    }
}
