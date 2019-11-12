using System;
using System.Collections.Generic;
using System.Linq;

namespace ParticleSwarmOptimisation
{
    public static class ParticleSwarm
    {
        public static Position Simulate(int numberOfIterations, List<Particle> particles, Func<double[], double> evaluator, bool maximise = true)
        {
            particles.ForEach(p => p.EvaluateCurrentPositionWith(evaluator));

            var globalBestPosition = particles.OrderBy(p => p.Position.Value).First().Position;

            for (int i = 0; i < numberOfIterations; i++)
            {
                var newBestPosition = globalBestPosition;

                foreach (var particle in particles)
                {
                    particle.GlobalBest = globalBestPosition;

                    particle.Move();

                    particle.EvaluateCurrentPositionWith(evaluator);

                    if (IsBetter(particle.Position, particle.PersonalBest, maximise))
                    {
                        particle.PersonalBest = particle.Position.Clone();
                    }

                    if (IsBetter(particle.Position, newBestPosition, maximise))
                    {
                        newBestPosition = particle.Position.Clone();
                        Console.WriteLine($"New Best Found [Iteration: {i} Value: {newBestPosition.Value} Positions: {newBestPosition.Vector.Aggregate("", (str, v) => v + " " + str)}]");
                    }
                }

                globalBestPosition = newBestPosition;
            }

            return globalBestPosition;
        }

        private static bool IsBetter(Position a, Position b, bool maximise)
        {
            var aValue = a.Value;
            var bValue = b.Value;

            return maximise ? aValue < bValue : aValue > bValue;
        }
    }
}
