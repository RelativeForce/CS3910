using System;
using System.Collections.Generic;

namespace Coursework
{
    public sealed class Hub
    {
        private readonly int _numberOfMeasurements;
        private readonly CostEvaluator _evaluator;
        private readonly IParticleSwarm _particleSwarm;
        private readonly Random _random;

        public Hub(int numberOfMeasurements, CostEvaluator evaluator, IParticleSwarm particleSwarm)
        {
            _numberOfMeasurements = numberOfMeasurements;
            _evaluator = evaluator;
            _particleSwarm = particleSwarm;
            _random = new Random();
        }

        public double[] Simulate(int particleCount)
        {
            var particles = GenerateParticles(particleCount);

            var position = _particleSwarm.Simulate(particles);

            return position.Vector;
        }

        private List<Particle> GenerateParticles(int particleCount)
        {
            var particles = new List<Particle>(particleCount);

            for (var index = 0; index < particleCount; index++)
            {
                var estimates = new double[_numberOfMeasurements];

                for (var estimateIndex = 0; estimateIndex < _numberOfMeasurements; estimateIndex++)
                {
                    estimates[estimateIndex] = _random.NextDouble();
                }

                var attraction = _particleSwarm.NewAttraction();

                var particle = new Particle(new Position(estimates), _evaluator, attraction);

                particle.EvaluateCurrentPosition();
                particle.TrackPosition();

                particles.Add(particle);
            }

            return particles;
        }
    }
}
