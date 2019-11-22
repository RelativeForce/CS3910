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

                foreach (var particle in particles.AsParallel())
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
            var parents = SelectParents(particles);

            var offspring = Reproduce(parents);

            var survivors = Survivors(offspring, particles.Select(p => p.Attraction).ToList());

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Attraction = survivors[i];
            }
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

        private static List<Attraction> Reproduce(List<Attraction> parents)
        {
            var children = new List<Attraction>();

            var canSelectParent = true;

            

            while (canSelectParent)
            {
                var parent1 = parents.PickRandom();
                var parent2 = parents.PickRandom();

                var child1Personal = Recombine(parent1.PersonalPullFactor, parent2.GlobalPullFactor);
                var child1Global = Recombine(parent2.PersonalPullFactor, parent1.GlobalPullFactor);
                
                var child2Personal = Recombine(parent2.PersonalPullFactor, parent1.GlobalPullFactor);
                var child2Global = Recombine(parent1.PersonalPullFactor, parent2.GlobalPullFactor);

                var child1 = new Attraction(child1Personal, child1Global);
                var child2 = new Attraction(child2Personal, child2Global);

                children.Add(child1);
                children.Add(child2);

                if (parents.Count < 2)
                    canSelectParent = false;
            }

            children.AddRange(parents);

            return children;
        }

        private static double Recombine(double factor1, double factor2)
        {
            var random = new Random();

            var value = (factor1 + factor2) * PesudoNormalDistribution() / 2;

            // 5% chance of crazy mutation
            if (random.NextDouble() < 0.05)
            {
                value = 1.0 / value;
            }

            return value;
        }

        private static double PesudoNormalDistribution()
        {
            var random = new Random();

            return (Math.Pow(random.NextDouble(), 2) * -1) + 1;
        }

        private static List<Attraction> Survivors(List<Attraction> children, List<Attraction> all)
        {
            var total = all.Count;

            while (children.Count < total)
            {
                children.Add(all.PickRandom());
            }
            
            return children;
        }
    }
}
