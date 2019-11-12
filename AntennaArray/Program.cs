using System;
using System.Collections.Generic;
using System.Linq;
using ParticleSwarmOptimisation;

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

            

            var bestPosition = ParticleSwarm.Simulate(numberOfIterations, particles, antenna.Evaluate);

            Console.WriteLine($"Best found after {numberOfIterations} iterations was {bestPosition.Value}");
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

                var particle = new Particle(velocity, new Position(positionArray));

                particles.Add(particle);
                Console.WriteLine($"Generated particle {particles.Count}");
            }

            return particles;
        }
    }
}
