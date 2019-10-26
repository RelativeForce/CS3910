using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AntennaArray
{
    public class Particle
    {
        public const double GlobalPullFactor = 0.05;
        public const double PersonalPullFactor = 0.05;
        public const double Inertia = 0.01;

        public Position GlobalBest { get; set; }
        public Position Position { get; set; }

        public Position PersonalBest { get; set; }
        private readonly double[] _velocity;
        private readonly Random _random;

        public Particle([NotNull] double[] velocity, [NotNull] Position position)
        {
            _velocity = velocity;
            Position = position;
            PersonalBest = Position.Clone();
            GlobalBest = Position.Clone();
            _random = new Random();
        }

        public void Move()
        {
            UpdateVelocity();

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Position = new Position(Combine(Position.Vector, _velocity));
        }

        private void UpdateVelocity()
        {
            var randomVelocity = GenerateRandomVelocity(Position.Vector.Length);
            var newVelocity = new double[Position.Vector.Length];

            for (var i = 0; i < newVelocity.Length; i++)
            {
                newVelocity[i] = randomVelocity[i] * ((Inertia * _velocity[i]) +
                                 (GlobalPullFactor * (GlobalBest.Vector[i] - Position.Vector[i])) +
                                 (PersonalPullFactor * (PersonalBest.Vector[i] - Position.Vector[i])));
            }

            for (var i = 0; i < newVelocity.Length; i++)
            {
                _velocity[i] = newVelocity[i];
            }
        }

        private static double[] Combine(double[] a, double[] b)
        {
            var result = new double[a.Length];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = a[i] + b[i];
            }

            return result;
        }

        private double[] GenerateRandomVelocity(int length)
        {
            var velocity = new double[length];

            for (int i = 0; i < velocity.Length; i++)
            {
                velocity[i] = _random.NextDouble();
            }

            return velocity;
        }
    }
}