using System.Collections;
using System.Collections.Generic;
using Terrain.Item;
using UnityEngine;

namespace Terrain.Blocks
{
    public class RandomDropSettings
    {
        public List<RandomItemStack> items = new List<RandomItemStack>();
        public uint rollRoundCount = 1;

        public ItemStack[] Roll()
        {
            //TODO implement multiple rounds of rolls
            return new [] {items[0].Roll()};
        }
    }
    
    //TODO name
    public class RandomItemStack
    {
        private readonly ItemBase item;
        /**
         * Chance of item dropping (0 to 1)
         */
        private readonly float chance;

        private readonly uint minCount;
        private readonly uint maxCount;

        public RandomItemStack(ItemBase item, float chance, uint minCount, uint maxCount)
        {
            this.item = item;
            this.chance = chance;
            this.minCount = minCount;
            this.maxCount = maxCount;
        }

        public ItemStack Roll()
        {
            if (Random.value <= chance)
                return new ItemStack(item, (uint)Random.Range((int)minCount, (int)maxCount));
            return null;
        }
    }
}