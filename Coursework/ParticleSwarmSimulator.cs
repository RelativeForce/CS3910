using System;
using System.Collections.Generic;

namespace Coursework
{
    public sealed class ParticleSwarmSimulator
    {
        private readonly int _weightCount;
        private readonly CostEvaluator _evaluator;
        private readonly IParticleSwarm _particleSwarm;
        private readonly Random _random;

        public ParticleSwarmSimulator(int weightCount, CostEvaluator evaluator, IParticleSwarm particleSwarm)
        {
            _weightCount = weightCount;
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
                var weights = new double[_weightCount];

                for (var weightIndex = 0; weightIndex < _weightCount; weightIndex++)
                {
                    weights[weightIndex] = _random.NextDouble();
                }

                var attraction = _particleSwarm.NewAttraction();

                var particle = new Particle(new Position(weights), _evaluator, attraction);

                particle.EvaluateCurrentPosition();
                particle.TrackPosition();

                particles.Add(particle);
            }

            return particles;
        }
    }
}
