using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Terrain
{
    public class RandomFromList<T> where T:class
    {
        private readonly List<RandomChance<T>> List;

        public RandomFromList(IEnumerable<RandomChance<T>> list)
        {
            List = list.OrderBy(o => o.Chance).ToList();
        }

        /**
         * Returns T object, never returns null
         */
        public T GetRandom()
        {
            //Iterates thru item list and performs random roll on each of them, if item is rolled then further iteration is cancelled and item is returned
            foreach (var item in List)
            {
                T result = item.GetRandom();
                if (result != null) return result;
            }

            //Return last item if non were rolled
            return List[^1].Result;
        }
    }

    public class RandomChance<T> where T:class
    {
        private readonly T result;
        private readonly float chance;

        public RandomChance(T result, float chance)
        {
            this.result = result;
            this.chance = chance;
        }

        /**
         * Returns T object with chance given in constructor
         * May return null
         */
        public T GetRandom()
        {
            return Random.value <= chance ? result : null;
        }

        public T Result => result;

        public float Chance => chance;
    }
}