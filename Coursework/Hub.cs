using System;
using System.Collections.Generic;
using System.Linq;
using ParticleSwarmOptimisation;

namespace Coursework
{
    public sealed class Hub
    {
        public const double GlobalPullFactor = 0.05;
        public const double PersonalPullFactor = 0.05;
        private readonly int _numberOfLocations;
        private readonly List<Day> _days;
        private readonly double _initialMinimumEstimate;
        private readonly double _initialRangeOfEstimate;
        private readonly Random _random;

        public Hub(int numberOfLocations, List<Day> days, double initialMinimumEstimate, double initialRangeOfEstimate)
        {
            _numberOfLocations = numberOfLocations;
            _days = days;
            _initialMinimumEstimate = initialMinimumEstimate;
            _initialRangeOfEstimate = initialRangeOfEstimate;
            _random = new Random();
        }

        public double[] Simulate(int numberOfEstimationParticles, int numberOfIterations)
        {
            var particles = GenerateParticles(numberOfEstimationParticles);

            var position = ParticleSwarm.Simulate(numberOfIterations, 100, particles);

            return position.Vector;
        }

        public double Cost(double[] estimates)
        {
            return _days.Select(d => d.Cost(estimates)).Sum();
        }

        public List<Particle> GenerateParticles(int numberOfParticles)
        {
            var particles = new List<Particle>();

            for (int index = 0; index < numberOfParticles; index++)
            {
                var estimates = new double[_numberOfLocations];

                for (int estimateIndex = 0; estimateIndex < _numberOfLocations; estimateIndex++)
                {
                    estimates[estimateIndex] = _initialMinimumEstimate + (_initialRangeOfEstimate * _random.NextDouble());
                }

                particles.Add(new Particle(new Position(estimates), Cost, PersonalPullFactor, GlobalPullFactor));
            }

            return particles;
        }
    }
}
