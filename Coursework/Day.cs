using System;
using System.Linq;

namespace Coursework
{
    public sealed class Day
    {
        private readonly double _actualDemand;
        private readonly double[] _measurements;

        public Day(double actualDemand, double[] measurements)
        {
            _actualDemand = actualDemand;
            _measurements = measurements;
        }

        public double Cost(double[] weights)
        {
            var weightArray = weights.ToArray();

            var sum = Enumerable
                .Zip(_measurements, weightArray, (measurement, weight) => weight * measurement)
                .Sum();

            return Math.Abs(_actualDemand - sum);
        }
    }
}
