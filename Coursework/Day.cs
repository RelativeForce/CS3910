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

        public double ActualDemand()
        {
            return _actualDemand;
        }

        public double Cost(double[] estimates)
        {
            var estimatedTruckCapacity = estimates.Select(Hub.ToTrucks).ToArray();

            var average = Enumerable
                .Zip(_measurements, estimatedTruckCapacity, (demand, estimate) => Math.Abs(estimate - demand))
                .Average();

            return average;
        }
    }
}
