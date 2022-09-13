using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalCosmosHistory
{
    public static class Extensions
    {
        public static decimal RoundDown(this decimal value, int places)
        {
            var adjustment = (decimal)Math.Pow(10, places);
            return Math.Floor(value * adjustment) / adjustment;
        }
        
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
            this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count).ToArray();
        }

    }
}