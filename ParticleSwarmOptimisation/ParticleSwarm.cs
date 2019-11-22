using System;
using System.Collections.Generic;
using System.Linq;

namespace ParticleSwarmOptimisation
{
    public static class ParticleSwarm
    {
        private const int K = 4;

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
            var selectedAttractions = SelectParents(particles);

            var offspring = Recombine(selectedAttractions);

            Survivors(offspring);
        }

        private static List<Attraction> SelectParents(List<Particle> particles)
        {
            var attractions = particles.Select(p => p.Attraction).ToList();

            var parents = new List<Attraction>();

            var canSelectParent = true;

            while (canSelectParent)
            {
                var contenders = new List<Attraction>();
                for (int i = 0; i < K; i++)
                {
                    contenders.Add(attractions.PickRandom());
                }

                var parent = contenders.OrderByDescending(a => a.Improvement).First();

                parents.Add(parent);

                if (attractions.Count < K)
                    canSelectParent = false;
            }

            return parents;
        }

        private static List<Attraction> Recombine(List<Attraction> particles)
        {
            var random = new Random();

            var even = particles.Count % 2 == 0;



            var parents = particles.TakeLast(particles.Count / 2);

            return particles;
        }


        private static List<Attraction> Survivors(List<Attraction> particles)
        {
            var random = new Random();

            var even = particles.Count % 2 == 0;



            var parents = particles.TakeLast(particles.Count / 2);

            return particles;
        }
    }
}
