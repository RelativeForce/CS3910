﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class Evolution
    {
        private readonly int _iterationsPerGeneration;
        private readonly int _k;
        private readonly double _mutationProbability;

        public Evolution(int iterationsPerGeneration, int k, double mutationProbability)
        {
            _iterationsPerGeneration = iterationsPerGeneration;
            _k = k;
            _mutationProbability = mutationProbability;
        }

        public bool ShouldEvolve(int index)
        {
            return index % _iterationsPerGeneration == 0 && index > 0;
        }

        public void Evolve(List<Particle> hosts)
        {
            var potentialParents = new List<Attraction>(hosts.Count);
            var currentGeneration = new List<Attraction>(hosts.Count);

            foreach (var host in hosts)
            {
                potentialParents.Add(host.Attraction);
                currentGeneration.Add(host.Attraction);
            }

            var parents = SelectParents(potentialParents);

            var children = Reproduce(parents);

            var survivors = Survivors(children, currentGeneration);

            // Redistribute the next generation back to the hosts
            for (int i = 0; i < hosts.Count; i++)
            {
                hosts[i].Attraction = survivors[i];
            }
        }

        private List<Attraction> SelectParents(List<Attraction> potentialParents)
        {
            var parents = new List<Attraction>(potentialParents.Count / _k);

            var canSelectParent = true;

            while (canSelectParent)
            {
                var contenders = new List<Attraction>();
                for (int i = 0; i < _k; i++)
                {
                    contenders.Add(potentialParents.PickRandom());
                }

                var parent = contenders.OrderByDescending(a => a.Improvement).First();

                parents.Add(parent);

                if (potentialParents.Count < _k)
                    canSelectParent = false;
            }

            return parents;
        }

        private List<Attraction> Reproduce(List<Attraction> parents)
        {
            var children = new List<Attraction>();

            var canSelectParent = true;

            while (canSelectParent)
            {
                var parent1 = parents.PickRandom();
                var parent2 = parents.PickRandom();

                var child1Cognitive = Recombine(parent1.CognitiveAttraction, parent2.SocialAttraction);
                var child1Social = Recombine(parent2.CognitiveAttraction, parent1.SocialAttraction);

                var child2Cognitive = Recombine(parent2.CognitiveAttraction, parent1.SocialAttraction);
                var child2Social = Recombine(parent1.CognitiveAttraction, parent2.SocialAttraction);

                var child1 = new Attraction(child1Cognitive, child1Social);
                var child2 = new Attraction(child2Cognitive, child2Social);

                children.Add(child1);
                children.Add(child2);

                if (parents.Count < 2)
                    canSelectParent = false;
            }

            children.AddRange(parents);

            return children;
        }

        private double Recombine(double factor1, double factor2)
        {
            var random = new Random();

            var pull = Math.Abs(factor2 - factor1) * random.NextParabolicDouble() / 2;

            var value = random.NextBool() ? factor1 + pull : factor2 - pull;

            if (random.NextDouble() < _mutationProbability)
            {
                value = 1.0 / value;
            }

            return value;
        }

        private static List<Attraction> Survivors(List<Attraction> children, List<Attraction> currentGeneration)
        {
            var total = currentGeneration.Count;

            while (children.Count < total)
            {
                children.Add(currentGeneration.PickRandom());
            }

            return children;
        }
    }
}