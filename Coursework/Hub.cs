using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class Hub
    {
        public const double InitialGlobalPullFactor = 0.1;
        public const double InitialPersonalPullFactor = 0.5;
        private readonly int _numberOfLocations;
        private readonly List<Day> _days;
        private readonly Random _random;

        public Hub(int numberOfLocations, List<Day> days)
        {
            _numberOfLocations = numberOfLocations;
            _days = days;
            _random = new Random();
        }

        public double[] Simulate(int numberOfEstimationParticles, int numberOfIterations)
        {
            var particles = GenerateParticles(numberOfEstimationParticles);

            var position = ParticleSwarm.Simulate(numberOfIterations, 10, particles);

            return position.Vector;
        }

        public double Cost(double[] estimates)
        {
            return _days.Select(d => d.Cost(estimates)).Average();
        }

        public List<Particle> GenerateParticles(int numberOfParticles)
        {
            var particles = new List<Particle>();

            for (int index = 0; index < numberOfParticles; index++)
            {
                var estimates = new double[_numberOfLocations];

                for (int estimateIndex = 0; estimateIndex < _numberOfLocations; estimateIndex++)
                {
                    estimates[estimateIndex] = _random.NextDouble();
                }

                var personalPull = InitialPersonalPullFactor * _random.NextDouble();
                var globalPull = InitialGlobalPullFactor * _random.NextDouble();


                particles.Add(new Particle(new Position(estimates), Cost, new Attraction(personalPull, globalPull)));
            }

            return particles;
        }
    }
}
