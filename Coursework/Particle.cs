using System;
using System.Linq;

namespace Coursework
{
    public class Particle
    {
        public Attraction Attraction { get; set; }
        public Position Position { get; private set; }
        public Position PersonalBest { get; private set; }
        public Velocity Velocity { get; private set; }

        private readonly CostEvaluator _evaluator;

        public Particle(Position position, CostEvaluator evaluator, Attraction attraction)
        {
            _evaluator = evaluator;
            Velocity = GenerateRandomVelocity(position.Vector.Length);
            Position = position;
            Attraction = attraction;
            PersonalBest = Position;
        }

        public void EvaluateCurrentPosition()
        {
            Position.EvaluateWith(_evaluator);

            if (Position.Value < PersonalBest.Value)
            {
                PersonalBest = Position;
            }
        }

        public void Move(Position globalBest)
        {
            UpdateVelocity(globalBest);

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

        private void UpdateVelocity(Position globalBest)
        {
            Velocity = Attraction.Apply(
                Velocity, 
                Position.VectorTo(PersonalBest), 
                Position.VectorTo(globalBest));
        }

        private static Velocity GenerateRandomVelocity(int length)
        {
            var random = new Random();

            var velocity = new double[length];

            for (var i = 0; i < velocity.Length; i++)
            {
                velocity[i] = random.NextDouble();
            }

            return new Velocity(velocity);
        }
    }
}