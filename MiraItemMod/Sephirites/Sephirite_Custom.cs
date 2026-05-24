using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Sephirites
{
    public class Sephirite_Custom : Sephirite
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            OnConnected(base.connectionToClient);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            OnDisconnected(base.connectionToClient);
        }
        protected virtual void OnConnected(NetworkConnectionToClient client)
        {

        }
        protected virtual void OnDisconnected(NetworkConnectionToClient client)
        {

        }
        protected virtual int MaxLoopCount => 1000;
        protected virtual int ModifyChoiceCount(int stat)
        {
            return stat;
        }
        protected virtual double GetCharmProbability()
        {
            return 0.9;
        }
        protected virtual double GetTabletProbability()
        {
            return 0.7;
        }
        protected virtual List<int> GetCharms(UnitAvatar avatar, PlayerSpawner player)
        {
            return new List<int>(player.unlockedCharms);
        }
        protected virtual List<int> GetTablets(UnitAvatar avatar, PlayerSpawner player)
        {
            return new List<int>(player.unlockedStoneTablets);
        }
        protected virtual EItemRarity GetRandomRarity(Type type, System.Random rand, float luck)
        {
            return RarityDice(type, rand, luck);
        }
        protected virtual List<int> GetDefaultItems(UnitAvatar avatar, PlayerSpawner player)
        {
            return new List<int>(player.unlockedCharms);
        }
        public virtual void GenerateItemsCustom(GameObject actor)
        {
            if (isGenerated || isAcquired)
            {
                return;
            }

            Debug.Log($"Sephirite Seed : {CurrentSeed}");
            System.Random random = new System.Random(CurrentSeed);
            UnitAvatar avatar = actor.GetComponent<UnitAvatar>();
            PlayerSpawner player = actor.GetComponent<PlayerSpawner>();
            int choices = 3;
            switch (type)
            {
                case Type.NORMAL:
                    choices = 5;
                    break;
                case Type.BIG:
                    choices = 5;
                    break;
                case Type.BOSS:
                    choices = 5;
                    break;
                case Type.TABLET:
                    choices = 5;
                    break;
                case Type.CHARM:
                    choices = 5;
                    break;
                case Type.TABLET_BOSS:
                    choices = 5;
                    break;
            }

            if ((bool)DungeonManager.Instance && DungeonManager.Instance.hardModeEnvironment.TryGetValue("DIZZINESS", out var value))
            {
                choices -= value;
            }

            if ((bool)avatar)
            {
                choices += avatar.GetCustomStatUnsafe("EXTRAITEMCHOICES");
            }

            choices = ModifyChoiceCount(choices);
            if(choices < 1)
                choices = 1;

            WeaponControllerSimple weaponControllerSimple = (avatar ? avatar.GetComponent<WeaponControllerSimple>() : null);
            bool useTablet = (bool)avatar && avatar.GetCustomStat(ECustomStat.TABLET) > 0;
            Dictionary<EItemRarity, WeightedItemSelector> dictionary = new Dictionary<EItemRarity, WeightedItemSelector>();
            double chance = random.NextDouble();
            if (type == Type.TABLET || type == Type.TABLET_BOSS)
            {
                chance = 1.0;
            }
            else if (!useTablet || type == Type.CHARM || type == Type.BOSS)
            {
                chance = 0.0;
            }

            float luck = (avatar ? ((float)avatar.GetCustomStat(ECustomStat.Luck) / 100f) : 0f);
            List<ItemEntity> exclusions = new List<ItemEntity>();
            if (chance < GetCharmProbability())
            {
                foreach (int item in GetCharms(avatar, player))
                {
                    ItemEntity itemEntity = ItemDatabase.FindItemById(item);
                    if (!itemEntity || appearedItems.Contains(itemEntity.id))
                    {
                        continue;
                    }

                    if (itemEntity.isDual)
                    {
                        bool flag2 = false;
                        foreach (string category in itemEntity.categories)
                        {
                            if ((bool)avatar)
                            {
                                ComboEffectBase comboEffectBase = avatar.Inventory.FindComboEffect(category);
                                if ((bool)comboEffectBase && comboEffectBase.isEnabled && comboEffectBase.lastAppliedComboEffectCount > 0)
                                {
                                    flag2 = true;
                                    break;
                                }
                            }
                        }

                        if (!flag2)
                        {
                            Debug.Log($"{itemEntity} 거름 : 연결된 카테고리 조건을 충족하지 못함");
                            continue;
                        }
                    }

                    if (!itemEntity.resourcePrefab || !itemEntity.resourcePrefab.TryGetComponent<Charm_Basic>(out var charm))
                    {
                        continue;
                    }

                    if ((bool)avatar && avatar.Inventory.HasItem(itemEntity, out var _, out var _, out var _))
                    {
                        if (charm.isUniqueEffect && charm.connectedUniqueItems.Count > 0)
                        {
                            exclusions.AddRange(charm.connectedUniqueItems);
                        }

                        if (avatar.Inventory.uniquePairCount > 0)
                        {
                            Charm_Basic charm_Basic = null;
                            foreach (Charm_Basic value3 in avatar.Inventory.charms.Values)
                            {
                                if ((bool)value3 && value3.Item != null && value3.Item.EntityID == itemEntity.id)
                                {
                                    charm_Basic = value3;
                                    break;
                                }
                            }

                            if ((bool)charm_Basic && charm_Basic.DisplayedLevel >= charm_Basic.maxLevel)
                            {
                                continue;
                            }
                        }
                        else if (charm.isUniqueEffect)
                        {
                            continue;
                        }
                    }

                    if (itemEntity.cannotBeReward)
                    {
                        continue;
                    }

                    int itemDropWeight = avatar.Inventory.GetItemDropWeight(itemEntity);
                    if (!charm.isWeaponRelatedCharm || ((bool)weaponControllerSimple && (bool)weaponControllerSimple.currentWeapon && charm.relatedWeapon == weaponControllerSimple.currentWeapon.weaponType))
                    {
                        if (!dictionary.ContainsKey(itemEntity.rarity))
                        {
                            dictionary.Add(itemEntity.rarity, new WeightedItemSelector());
                        }

                        dictionary[itemEntity.rarity].AddItem(new WeightedItem(itemEntity, itemDropWeight));
                    }
                }
            }

            foreach (ItemEntity item2 in exclusions)
            {
                Debug.LogWarning("연결된 유니크템 풀에서 안나오게 : " + item2);
                if (dictionary.ContainsKey(item2.rarity))
                {
                    dictionary[item2.rarity].RemoveItem(item2.id);
                }
            }

            if (type == Type.TABLET || type == Type.TABLET_BOSS || (chance > 1.0 - GetTabletProbability() && useTablet))
            {
                foreach (int unlockedStoneTablet in GetTablets(avatar, player))
                {
                    ItemEntity itemEntity2 = ItemDatabase.FindItemById(unlockedStoneTablet);
                    if (!itemEntity2)
                    {
                        continue;
                    }

                    if (appearedItems.Contains(itemEntity2.id))
                    {
                        Debug.Log($"{itemEntity2} 거름 : 이전 10개템 목록에 포함되기 때문에");
                        continue;
                    }

                    if (!dictionary.ContainsKey(itemEntity2.rarity))
                    {
                        dictionary.Add(itemEntity2.rarity, new WeightedItemSelector());
                    }

                    dictionary[itemEntity2.rarity].AddItem(new WeightedItem(itemEntity2, KeywordDatabase.GetConstValue("defaultItemDropWeight")));
                }
            }

            int count = 0;
            for (int i = 0; i < choices; i++)
            {
                count++;
                if(count > MaxLoopCount)
                {
                    var defaults = GetDefaultItems(avatar, player);
                    for (int q = 0; q < choices - i; q++)
                        rewards.Add(new SephiriteRewardMetadata(ItemDatabase.GenerateInstanceID(random), defaults.GetRandom()));
                    break;
                }
                int limit = appearLimit;
                EItemRarity key;
                if (type == Type.TABLET_BOSS)
                {
                    if ((bool)DungeonManager.Instance && DungeonManager.Instance.hardModeEnvironment.TryGetValue("WEAKBOSSTABLET", out var weak))
                    {
                        limit = 2;
                        key = (weak != 2) ? EItemRarity.Uncommon : EItemRarity.Common;
                    }
                    else
                    {
                        key = GetRandomRarity(type, random, luck);
                        if (!dictionary.ContainsKey(key) || !dictionary[key].HasItems())
                        {
                            i--;
                            continue;
                        }
                    }
                }
                else
                {
                    key = GetRandomRarity(type, random, luck);
                    if (!dictionary.ContainsKey(key) || !dictionary[key].HasItems())
                    {
                        i--;
                        continue;
                    }
                }

                WeightedItem random2 = dictionary[key].GetRandom(random);
                dictionary[key].RemoveItem(random2.item.id);
                rewards.Add(new SephiriteRewardMetadata(ItemDatabase.GenerateInstanceID(random), random2.item.id));
                appearedItems.Enqueue(random2.item.id);
                if (appearedItems.Count > limit)
                {
                    appearedItems.Dequeue();
                }
            }

            NetworkisGenerated = true;
        }

        [HarmonyPatch(typeof(Sephirite), nameof(Sephirite.GenerateItems))]
        public static class GenerateItemsPatch
        {
            static bool Prefix(Sephirite __instance, GameObject actor)
            {
                if(__instance is Sephirite_Custom custom)
                {
                    custom.GenerateItemsCustom(actor);
                    return false;
                }
                return true;
            }
        }
    }
}
