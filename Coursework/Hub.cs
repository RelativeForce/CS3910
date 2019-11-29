using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class Hub
    {
        private readonly int _numberOfLocations;
        private readonly List<Day> _days;
        private readonly IParticleSwarm _particleSwarm;
        private readonly Random _random;

        public Hub(int numberOfLocations, List<Day> days, IParticleSwarm particleSwarm)
        {
            _numberOfLocations = numberOfLocations;
            _days = days;
            _particleSwarm = particleSwarm;
            _random = new Random();
        }

        public double[] Simulate(int particleCount)
        {
            var particles = GenerateParticles(particleCount);

            var position = _particleSwarm.Simulate(particles);

            return position.Vector;
        }

        public double Cost(double[] estimates)
        {
            return _days.Select(d => d.Cost(estimates)).Average();
        }

        private List<Particle> GenerateParticles(int particleCount)
        {
            var particles = new List<Particle>();

            for (var index = 0; index < particleCount; index++)
            {
                var estimates = new double[_numberOfLocations];

                for (var estimateIndex = 0; estimateIndex < _numberOfLocations; estimateIndex++)
                {
                    estimates[estimateIndex] = _random.NextDouble();
                }

                var attraction = _particleSwarm.NewAttraction();

                var particle = new Particle(new Position(estimates), Cost, attraction);

                particle.EvaluateCurrentPosition();
                particle.TrackPosition();

                particles.Add(particle);
            }

            return particles;
        }
    }
}
