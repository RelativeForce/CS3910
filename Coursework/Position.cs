using System;
using System.Linq;

namespace Coursework
{
    public class Position
    {
        public double[] Vector { get; set; }
        public double Value  { get; private set; }

        public Position(double[] vector)
        {
            Vector = vector;
            Value = double.MaxValue;
        }

        private Position()
        {

        }

        public double[] VectorTo(Position position)
        {
            return Enumerable.Zip(Vector, position.Vector, (v1, v2) => v2 - v1).ToArray();
        }

        public Position Clone()
        {
            return new Position
            {
                Vector = Vector.ToArray(),
                Value = Value
            };
        }

        public void EvaluateWith(ICostEvaluator evaluator)
        {
            Value = evaluator.Cost(Vector);
        }
    }
}