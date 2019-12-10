using System;
using System.Collections.Generic;

namespace Coursework
{
    public sealed class ParticleSwarmSimulator
    {
        private readonly CostEvaluator _evaluator;
        private readonly IParticleSwarm _particleSwarm;
        private readonly Random _random;

        public ParticleSwarmSimulator(CostEvaluator evaluator, IParticleSwarm particleSwarm)
        {
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
                var weights = new double[_evaluator.WeightCount];

                for (var weightIndex = 0; weightIndex < _evaluator.WeightCount; weightIndex++)
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
