using System;
using System.Linq;

namespace ParticleSwarmOptimisation
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

        public void EvaluateWith(Func<double[], double> evaluator)
        {
            Value = evaluator(Vector);
        }
    }
}