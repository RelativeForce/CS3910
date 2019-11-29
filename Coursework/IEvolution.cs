using System.Collections.Generic;

namespace Coursework
{
    public interface IEvolution
    {
        bool ShouldEvolve(int index);

        void Evolve(List<Particle> hosts);
    }
}
