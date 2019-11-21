using System;

namespace ParticleSwarmOptimisation
{
    public class Particle
    {
        public const double Inertia = 0.01;

        public static Position GlobalBest { get; set; }
        public Position Position { get; private set; }
        public Position PersonalBest { get; set; }

        private readonly double[] _velocity;
        private readonly Func<double[], double> _evaluator;

        private double _personalPullFactor;
        private double _globalPullFactor;
        private double _trackedValue;

        public Particle(Position position, Func<double[], double> evaluator, double personalPullFactor, double globalPullFactor)
        {
            _evaluator = evaluator;
            _personalPullFactor = personalPullFactor;
            _globalPullFactor = globalPullFactor;
            _velocity = GenerateRandomVelocity(position.Vector.Length);
            _trackedValue = 0;
            Position = position;
            PersonalBest = Position.Clone();
            GlobalBest = Position.Clone();
        }

        public void EvaluateCurrentPosition()
        {
            Position.EvaluateWith(_evaluator);

            if (Position.Value < PersonalBest.Value)
            {
                PersonalBest = Position.Clone();
            }
        }

        public void Move()
        {
            UpdateVelocity();

            UpdatePosition();

            EvaluateCurrentPosition();
        }

        public void TrackPosition()
        {
            _trackedValue = Position.Value;
        }

        public double Improvement()
        {
            return Position.Value - _trackedValue;
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
                                 (_globalPullFactor * (GlobalBest.Vector[i] - Position.Vector[i])) +
                                 (_personalPullFactor * (PersonalBest.Vector[i] - Position.Vector[i])));
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

        private static double[] GenerateRandomVelocity(int length)
        {
            var random = new Random();

            var velocity = new double[length];

            for (int i = 0; i < velocity.Length; i++)
            {
                velocity[i] = random.NextDouble();
            }

            return velocity;
        }
    }
}