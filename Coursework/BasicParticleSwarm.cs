﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class BasicParticleSwarm : IParticleSwarm
    {
        private readonly int _iterationCount;
        private readonly double _personalPullFactor;
        private readonly double _globalPullFactor;

        public BasicParticleSwarm(int iterationCount, double personalPullFactor, double globalPullFactor)
        {
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
                    Console.WriteLine($"New Best Found [Iteration: {i} Value: {newGlobalBestPosition.Value}]");
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