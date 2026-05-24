using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Miracles
{
    public interface IMiracleCustomGiveItems
    {
        public List<int> GetAllItems(bool generateInstanceID, MiracleController identity, int instanceID);

        public bool UseCategory => true;
        public bool UseDual => false;
        public bool UseCannotBeReward => false;
        public abstract Miracle Base { get; }
        public ItemMetadata[] GetCustomItems(bool generateInstanceID, MiracleController identity, int instanceID)
        {
            System.Random random = new System.Random(instanceID);
            List<ItemMetadata> list = new List<ItemMetadata>();

            UnitAvatar avatar = identity.GetComponent<UnitAvatar>();
            WeaponControllerSimple weapon = identity.GetComponent<WeaponControllerSimple>();
            List<int> unlockedCharms = GetAllItems(generateInstanceID, identity, instanceID);
            Dictionary<EItemRarity, List<ItemEntity>> dictionary = new Dictionary<EItemRarity, List<ItemEntity>>();
            foreach (int item in unlockedCharms)
            {
                ItemEntity itemEntity = ItemDatabase.FindItemById(item);
                if (!Base.enabled || (itemEntity.isDual && !UseDual) || (!UseCannotBeReward && itemEntity.cannotBeReward))
                {
                    continue;
                }

                bool flag = false;
                if (Base.categories.Length == 0)
                {
                    flag = true;
                }
                else
                {
                    foreach (string category in itemEntity.categories)
                    {
                        string[] array2 = Base.categories;
                        foreach (string text in array2)
                        {
                            if (category == text)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                if (!UseCategory)
                    flag = true;

                if (!flag || !itemEntity.resourcePrefab || !itemEntity.resourcePrefab.TryGetComponent<Charm_Basic>(out var component3))
                {
                    continue;
                }

                sbyte outX;
                sbyte outY;
                sbyte quantity;
                if (component3.isUniqueEffect && component3.connectedUniqueItems.Count > 0)
                {
                    bool flag2 = false;
                    foreach (ItemEntity connectedUniqueItem in component3.connectedUniqueItems)
                    {
                        if (avatar.Inventory.HasItem(connectedUniqueItem, out outX, out outY, out quantity))
                        {
                            flag2 = true;
                            break;
                        }
                    }

                    if (flag2)
                    {
                        continue;
                    }
                }

                if ((bool)avatar && avatar.Inventory.HasItem(itemEntity, out quantity, out outY, out outX) && component3.isUniqueEffect)
                {
                    continue;
                }

                bool flag3 = false;
                foreach (string category2 in itemEntity.categories)
                {
                    ItemCategoryEntity itemCategoryEntity = ItemDatabase.FindItemCategory(category2);
                    if (itemCategoryEntity.requireStartUpItem && (bool)avatar && !avatar.Inventory.HasStartUpItem(itemCategoryEntity))
                    {
                        flag3 = true;
                    }
                }

                bool num = !component3.isWeaponRelatedCharm || ((bool)weapon && component3.relatedWeapon == weapon.currentWeapon.weaponType);
                bool flag4 = !flag3 || itemEntity.itemBehaviour == ItemEntity.EItemBehaviour.StartUp;
                if (num && flag4)
                {
                    if (!dictionary.ContainsKey(itemEntity.rarity))
                    {
                        dictionary.Add(itemEntity.rarity, new List<ItemEntity>());
                    }

                    dictionary[itemEntity.rarity].Add(itemEntity);
                }
            }

            if (dictionary.Count > 0)
            {
                float luck = (avatar ? ((float)avatar.GetCustomStat(ECustomStat.Luck) / 100f) : 0f);
                float uncommonNegative = 0.33f;
                float uncommonLHS = 0.45f;
                float uncommonRHS = 0.5f;
                float rareNegative = 0.02f;
                float rareLHS = 0.1f;
                float rareRHS = 0.33f;
                float legendNegative = 0f;
                float legendLHS = 0f;
                float legendRHS = 0f;
                ItemDatabase.CalculateLuck(luck, uncommonNegative, uncommonLHS, uncommonRHS, rareNegative, rareLHS, rareRHS, legendNegative, legendLHS, legendRHS, out var uncommon, out var rare, out var legend);
                int num2 = -1;
                int num3 = Enum.GetNames(typeof(EItemRarity)).Length;
                int j = 0;
                EItemRarity eItemRarity = ItemDatabase.GetRandomRarity(random, uncommon, rare, legend);
                for (; j < num3; j++)
                {
                    if (dictionary.ContainsKey(eItemRarity))
                    {
                        num2 = random.Next(0, dictionary[eItemRarity].Count);
                        break;
                    }

                    eItemRarity = (EItemRarity)((int)(eItemRarity + 1) % num3);
                    Debug.Log("등급 상향 조정 : " + eItemRarity);
                }

                if (num2 != -1)
                {
                    if (generateInstanceID)
                    {
                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), dictionary[eItemRarity][num2], 1));
                    }
                    else
                    {
                        list.Add(new ItemMetadata(-1, dictionary[eItemRarity][num2], 1));
                    }
                }
                else
                {
                    Debug.LogError("관련 아이템을 찾을 수 없음");
                }
            }
            else
            {
                Debug.LogError("관련 아이템을 찾을 수 없음");
            }
            return list.ToArray();
        }

        [HarmonyPatch(typeof(Miracle), nameof(Miracle.GetItems), new Type[] { typeof(bool), typeof(MiracleController), typeof(int) })]
        public static class GetItemsPatch
        {
            static void Postfix(Miracle __instance, ref ItemMetadata[] __result, bool generateInstanceID, MiracleController identity, int instanceID)
            {
                Core.Logger($"GetItems: {__instance is IMiracleCustomGiveItems}");
                if(__instance is IMiracleCustomGiveItems custom)
                {
                    __result = custom.GetCustomItems(generateInstanceID, identity, instanceID);
                }
            }
        }
    }
}
