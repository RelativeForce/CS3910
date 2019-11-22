using System.Linq;

namespace ParticleSwarmOptimisation
{
    public sealed class Velocity
    {
        public double[] Vector { get; set; }

        public Velocity(double[] vector)
        {
            Vector = vector;
        }

        public Velocity Clone()
        {
            return new Velocity(Vector.ToArray());
        }
    }
}
