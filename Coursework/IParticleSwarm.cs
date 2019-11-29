using System.Collections.Generic;

namespace Coursework
{
    public interface IParticleSwarm
    {
        Position Simulate(List<Particle> particles);

        Attraction NewAttraction();
    }
}