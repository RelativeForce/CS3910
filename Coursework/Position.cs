using System.Linq;

namespace Coursework
{
    public class Position
    {
        public double[] Vector { get; }
        public double Value  { get; private set; }

        public Position(double[] vector)
        {
            Vector = vector;
            Value = double.MaxValue;
        }

        public double[] VectorTo(Position position)
        {
            return Enumerable.Zip(Vector, position.Vector, (v1, v2) => v2 - v1).ToArray();
        }

        public void EvaluateWith(CostEvaluator evaluator)
        {
            Value = evaluator.Cost(Vector);
        }
    }
}