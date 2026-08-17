using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Miracles
{
    public class Miracle_GivePotion : Miracle_StatusInstance, IMiracleCustomGiveItems
    {
        public Miracle Base => this;

        public bool UseCategory => false;
        public List<int> GetAllItems(bool generateInstanceID, MiracleController identity, int instanceID)
        {
            var unlocks = identity.GetComponent<PlayerSpawner>().unlockedCharms;
            var alchemies = unlocks.Where(x => ItemDatabase.FindItemById(x) is ItemEntity entity && entity.categories.Contains(ItemCategories.Alchemy)).ToList();
            if (alchemies.Count == 0)
                return unlocks.ToList();
            return alchemies;
        }
        public ItemMetadata[] GetAdditionalItems(bool generateInstanceID, System.Random random, MiracleController identity, int instanceID)
        {
            var list = new List<ItemMetadata>();
            var potions = new List<int>() { 28, 29, 30, 31, 33, 35, 36, 38, 39, 45, 46, 47, 48 };
            var potion = potions[random.Next(0, potions.Count)];
            if (generateInstanceID)
            {
                list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), potion, 1));
            }
            else
            {
                list.Add(new ItemMetadata(-1, potion, 1));
            }
            return list.ToArray();
        }
    }
}
