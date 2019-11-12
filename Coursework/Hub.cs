using System;
using System.Collections.Generic;
using System.Linq;
using ParticleSwarmOptimisation;

namespace Coursework
{
    public sealed class Hub
    {
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

        public double[] Simulate(int numberOfEstimationParticles, int numberOfIterationsPerDay)
        {
            var particles = GenerateParticles(numberOfEstimationParticles);

            ParticleSwarm.Simulate(numberOfIterationsPerDay, particles, Cost, false);

            return Average(particles);
        }

        public static double ToTrucks(double estimate)
        {
            return Math.Ceiling(estimate / Program.PalletsPerTruck);
        }
        
        public double Cost(double[] estimates)
        {
            return _days.Select(d => d.Cost(estimates)).Average();
        }

        public bool IsValid(double[] position)
        {
            return !position.Any(estimate => estimate < 0);
        }

        public double[] Average(List<Particle> particles)
        {
            var positions = particles.Select(p => p.Position.Vector).Where(IsValid).ToList();

            var estimate = new double[_numberOfLocations];

            for (int locationIndex = 0; locationIndex < _numberOfLocations; locationIndex++)
            {
                estimate[locationIndex] = positions.Average(p => p[locationIndex]);
            }

            return estimate;
        }

        public List<Particle> GenerateParticles(int numberOfParticles)
        {
            var particles = new List<Particle>();

            for (int index = 0; index < numberOfParticles; index++)
            {
                var estimates = new double[_numberOfLocations];

                for (int estimateIndex = 0; estimateIndex < _numberOfLocations; estimateIndex++)
                {
                    estimates[estimateIndex] = _initialMinimumEstimate + ((_initialRangeOfEstimate - _initialMinimumEstimate) * _random.NextDouble());
                }

                particles.Add(new Particle(new Position(estimates)));
            }

            return particles;
        }
    }
}
