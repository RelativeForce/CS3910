using System.Collections.Generic;
using System.Linq;

namespace Coursework
{
    public sealed class CostEvaluator
    {
        private readonly List<Day> _days;

        public CostEvaluator(List<Day> days)
        {
            _days = days;
        }

        public double Cost(double[] estimates)
        {
            return _days.Select(d => d.Cost(estimates)).Average();
        }
    }
}
