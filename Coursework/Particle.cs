using System;
using System.Linq;

namespace Coursework
{
    public class Particle
    {
        public static Position GlobalBest { get; set; }

        public Position Position { get; private set; }
        public Attraction Attraction { get; set; }
        public Position PersonalBest { get; set; }
        public Velocity Velocity { get; private set; }

        private readonly CostEvaluator _evaluator;

        public Particle(Position position, CostEvaluator evaluator, Attraction attraction)
        {
            _evaluator = evaluator;
            Velocity = GenerateRandomVelocity(position.Vector.Length);
            Position = position;
            Attraction = attraction;
            PersonalBest = Position;
            GlobalBest = Position;
        }

        public void EvaluateCurrentPosition()
        {
            Position.EvaluateWith(_evaluator);

            if (Position.Value < PersonalBest.Value)
            {
                PersonalBest = Position;
            }
        }

        public void Move()
        {
            UpdateVelocity();

            UpdatePosition();

            EvaluateCurrentPosition();

            Attraction.SetMostRecentValue(Position.Value);
        }

        public void TrackPosition()
        {
            Attraction.TrackPosition(Position.Value);
        }

        private void UpdatePosition()
        {
            Position = new Position(Enumerable.Zip(Position.Vector, Velocity.Vector, (v1, v2) => v1 + v2).ToArray());
        }

        private void UpdateVelocity()
        {
            Velocity = Attraction.Apply(
                Velocity, 
                Position.VectorTo(PersonalBest), 
                Position.VectorTo(GlobalBest));
        }

        private static Velocity GenerateRandomVelocity(int length)
        {
            var random = new Random();

            var velocity = new double[length];

            for (int i = 0; i < velocity.Length; i++)
            {
                velocity[i] = random.NextDouble();
            }

            return new Velocity(velocity);
        }
    }
}