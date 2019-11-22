using System;
using System.Collections.Generic;
using System.Text;

namespace ParticleSwarmOptimisation
{
    public static class ListExtensions
    {
        public static T PickRandom<T>(this List<T> list)
        {
            var random = new Random();

            var index = random.Next(list.Count);

            var element = list[index];

            list.RemoveAt(index);

            return element;
        }
    }
}
