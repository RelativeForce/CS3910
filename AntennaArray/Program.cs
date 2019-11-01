using System;
using System.Collections.Generic;
using System.Linq;

namespace AntennaArray
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Antenna Array Problem - Joshua Eddy");

            Console.Write("Please input the number of antenna: ");
            var numberOfAntenna = int.Parse(Console.ReadLine() ?? "");

            Console.Write("Please input the steering angle: ");
            var steeringAngle = double.Parse(Console.ReadLine() ?? "");

            Console.Write("Please input the number of particles: ");
            var numberOfParticles = int.Parse(Console.ReadLine() ?? "");

            Console.Write("Please input the number of iterations: ");
            var numberOfIterations = int.Parse(Console.ReadLine() ?? "");

            var antenna = new AntennaArray(numberOfAntenna, steeringAngle);

            var particles = GenerateParticles(numberOfParticles, numberOfAntenna, antenna.Evaluate);

            var initialBestPosition = particles.OrderBy(p => p.Position.Value).First().Position;

            var bestPosition = RunProblem(numberOfIterations, initialBestPosition, particles, antenna);

            Console.WriteLine($"Best found after {numberOfIterations} iterations was {bestPosition.Value}");
        }

        private static Position RunProblem(int numberOfIterations, Position initialBestPosition, List<Particle> particles, AntennaArray antenna)
        {
            var globalBestPosition = initialBestPosition;

            for (int i = 0; i < numberOfIterations; i++)
            {
                var newBestPosition = globalBestPosition;

                foreach (var particle in particles)
                {
                    particle.GlobalBest = globalBestPosition;

                    particle.Move();

                    particle.Position.EvaluateWith(antenna.Evaluate);

                    if (particle.Position.Value < particle.PersonalBest.Value)
                    {
                        particle.PersonalBest = particle.Position.Clone();
                    }

                    if (particle.Position.Value < newBestPosition.Value)
                    {
                        newBestPosition = particle.Position.Clone();
                        Console.WriteLine($"New Best Found [Iteration: {i} Value: {newBestPosition.Value} Positions: {newBestPosition.Vector.Aggregate("", (str, v) => v + " " + str)}]");
                    }
                }

                globalBestPosition = newBestPosition;
            }

            return globalBestPosition;
        }

        private static List<Particle> GenerateParticles(int numberOfParticles, int numberOfAntenna, Func<double[], double> evaluator)
        {
            var maxValue = ((double) numberOfAntenna) / 2;
            var interval = maxValue / (numberOfAntenna - 1);

            var random = new Random();

            var particles = new List<Particle>();

            for (int i = 0; i < numberOfParticles; i++)
            {
                var positionArray = new double[numberOfAntenna];

                for (int j = 0; j < numberOfAntenna - 1; j++)
                {
                    var start = j == 0 ? 0 : positionArray[j - 1] + AntennaArray.MinSpacing;
                    var end = interval * (j + 1);

                    var difference = end - start;

                    positionArray[j] = (random.NextDouble() * difference) + start;
                }

                positionArray[numberOfAntenna - 1] = maxValue;

                var velocity = new double[numberOfAntenna];

                for (int j = 0; j < numberOfAntenna - 1; j++)
                {
                    velocity[j] = random.NextDouble();
                }

                velocity[numberOfAntenna - 1] = 0;

                var position = new Position(positionArray);

                position.EvaluateWith(evaluator);

                var particle = new Particle(velocity, position);

                particles.Add(particle);
                Console.WriteLine($"Generated particle {particles.Count}");
            }

            return particles;
        }
    }
}
