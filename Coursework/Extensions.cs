using System;
using System.Collections.Generic;
using System.Text;

namespace Coursework
{
    public static class Extensions
    {
        public static T PickRandom<T>(this List<T> list)
        {
            var random = new Random();

            var index = random.Next(list.Count);

            var element = list[index];

            list.RemoveAt(index);

            return element;
        }

        public static bool NextBool(this Random r)
        {
            return r.Next(2) == 0;
        }

        public static double NextParabolicDouble(this Random r)
        {
            // An a random value between 0 and 1 that falls on an inverse parabola that
            // peaks at 1 and intercepts the x axis at -1 and 1.
            return (Math.Pow(r.NextDouble(), 2) * -1) + 1;
        }

        public static string Join<T>(this IReadOnlyCollection<T> objects, string joiner = ",")
        {
            var builder = new StringBuilder();

            var index = 0;
            foreach (var element in objects)
            {
                builder.Append(element);

                if (index < objects.Count - 1)
                {
                    builder.Append(joiner);
                }

                index++;
            }

            return builder.ToString();
        }
    }
}
