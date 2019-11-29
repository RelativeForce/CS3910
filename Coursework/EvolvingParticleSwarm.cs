﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class EvolvingParticleSwarm : IParticleSwarm
    {
        private readonly IEvolution _evolution;
        private readonly int _iterationCount;
        private readonly double _personalPullFactor;
        private readonly double _globalPullFactor;

        public EvolvingParticleSwarm(IEvolution evolution, int iterationCount, double personalPullFactor, double globalPullFactor)
        {
            _evolution = evolution;
            _iterationCount = iterationCount;
            _personalPullFactor = personalPullFactor;
            _globalPullFactor = globalPullFactor;
        }

        public Position Simulate(List<Particle> particles)
        {
            Particle.GlobalBest = particles.OrderBy(p => p.Position.Value).First().Position;

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

            var personalPull = _personalPullFactor * random.NextDouble();
            var globalPull = _globalPullFactor * random.NextDouble();

            return new Attraction(personalPull, globalPull);
        }
    }
}