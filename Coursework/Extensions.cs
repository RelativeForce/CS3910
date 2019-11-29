using System;
using System.Collections.Generic;

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
    }
}
