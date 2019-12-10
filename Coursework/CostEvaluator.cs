using System;
using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class CostEvaluator
    {
        /// <summary>
        /// The number of weights that <see cref="Cost"/> is expecting.
        /// </summary>
        public int WeightCount { get; }

        private readonly List<Day> _days;

        public CostEvaluator(List<Day> days, int weightCount)
        {
            WeightCount = weightCount;
            _days = days;
        }

        public double Cost(double[] weights)
        {
            if(weights.Length != WeightCount)
                throw new ArgumentException("Invalid number of weights");

            return _days.Select(d => d.Cost(weights)).Average();
        }
    }
}
