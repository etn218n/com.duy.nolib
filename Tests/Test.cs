using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Tests
{
    public class Test
    {
        public static void Shuffle(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                var randomIndex = Random.Range(0, array.Length);
                var temp = array[randomIndex];
                array[randomIndex] = array[i];
                array[i] = temp;
            }
        }
        
        public static List<bool> Repeat(Func<bool> test, int n)
        {
            var results = new List<bool>(n);

            for (int i = 0; i < n; i++)
                results.Add(test());

            return results;
        }

        public static int SearchHighest(IList<int> list)
        {
            if (list.Count == 0)
                return default;

            var highestValueSoFar = list[0];

            for (int i = 1; i < list.Count; i++)
            {
                if (list[i] > highestValueSoFar)
                    highestValueSoFar = list[i];
            }

            return highestValueSoFar;
        }
        
        public static int SearchLowest(IList<int> list)
        {
            if (list.Count == 0)
                return default;

            var lowestValueSoFar = list[0];

            for (int i = 1; i < list.Count; i++)
            {
                if (list[i] < lowestValueSoFar)
                    lowestValueSoFar = list[i];
            }

            return lowestValueSoFar;
        }
    }
}