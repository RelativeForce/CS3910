using System;

namespace Coursework
{
    public sealed class Attraction
    {
        public const double Inertia = 0.01;

        public readonly double CognitiveAttraction;
        public readonly double SocialAttraction;

        private double _trackedValue;
        private double _mostRecentValue;

        public Attraction(double cognitiveAttraction, double socialAttraction)
        {
            CognitiveAttraction = cognitiveAttraction;
            SocialAttraction = socialAttraction;
            _trackedValue = double.MaxValue;
            _mostRecentValue = double.MaxValue;
        }

        public void TrackPosition(double value)
        {
            _trackedValue = value;
        }

        public void SetMostRecentValue(double value)
        {
            _mostRecentValue = value;
        }

        public double Improvement => _mostRecentValue - _trackedValue;

        public Velocity Apply(Velocity currentVelocity, double[] personalBestVector, double[] globalBestVector)
        {
            var r1 = GenerateRandomFactor(currentVelocity.Vector.Length);
            var r2 = GenerateRandomFactor(currentVelocity.Vector.Length);

            var newVelocity = new double[currentVelocity.Vector.Length];

            for (var i = 0; i < newVelocity.Length; i++)
            {
                newVelocity[i] =  ((Inertia * currentVelocity.Vector[i]) + 
                                   (r1[i] * SocialAttraction * globalBestVector[i]) +
                                   (r2[i] * CognitiveAttraction * personalBestVector[i]));
            }

            return new Velocity(newVelocity);
        }

        public static double[] GenerateRandomFactor(int length)
        {
            var random = new Random();

            var velocity = new double[length];

            for (var i = 0; i < velocity.Length; i++)
            {
                velocity[i] = random.NextDouble();
            }

            return velocity;
        }
    }
}
