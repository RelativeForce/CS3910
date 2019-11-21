using System;
using System.Collections.Generic;
using System.Linq;

namespace ParticleSwarmOptimisation
{
    public static class ParticleSwarm
    {
        public static Position Simulate(int numberOfIterations, int evolutionRate, List<Particle> particles)
        {
            particles.ForEach(p => p.EvaluateCurrentPosition());

            Particle.GlobalBest = particles.OrderBy(p => p.Position.Value).First().Position;

            for (int i = 0; i < numberOfIterations; i++)
            {
                if (i % evolutionRate == 0)
                {
                    if(i != 0)
                    {
                        Evolve(particles);
                    }

                    particles.ForEach(p => p.TrackPosition());
                }

                var parallel = particles.AsParallel();

                foreach (var particle in parallel)
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

        private static void Evolve(List<Particle> particles)
        {
            var selected = Select(particles);

            var offspring = Recombine(selected);

            particles.Clear();
            particles.AddRange(Survivors(offspring));
        }

        private static List<Particle> Select(List<Particle> particles)
        {
            var random = new Random();

            var even = particles.Count % 2 == 0;



            var parents = particles.TakeLast(particles.Count / 2);

            return particles;
        }

        private static List<Particle> Recombine(List<Particle> particles)
        {
            var random = new Random();

            var even = particles.Count % 2 == 0;



            var parents = particles.TakeLast(particles.Count / 2);

            return particles;
        }


        private static List<Particle> Survivors(List<Particle> particles)
        {
            var random = new Random();

            var even = particles.Count % 2 == 0;



            var parents = particles.TakeLast(particles.Count / 2);

            return particles;
        }
    }
}
