using HarmonyLib;
using Mirror;
using SephiriaMod.Registries;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Charm_StatusInstance;

namespace SephiriaMod.Items
{
    public class Charm_InventoryPower : Charm_VariableMaxLevelWhitePaper
    {
        public override int ValiableMax => 16;
        public override string StatusName => "InvLevel".ToUpperInvariant();
        private StatusInstance[] instances;
        public static StatusGroup CreateStatusGroup(string statusID, params int[] values)
        {
            return new StatusGroup { statusID = statusID, valuesByLevel = values };
        }

        public Dictionary<string, StatusGroup> status = new Dictionary<string, StatusGroup>();
        public List<StatusGroup> list = new List<StatusGroup>();
        public Dictionary<string, int> keys = new Dictionary<string, int>();
        public List<int> indexes = new List<int>();
        public List<int> indexesClient = new List<int>();

        public bool active = false;
        public string category = null;
        private void Awake()
        {
            list = new List<StatusGroup>();
            keys = new Dictionary<string, int>();
            status = new Dictionary<string, StatusGroup>();
            status[ItemCategories.Sturdy] = CreateStatusGroup("PHYSICAL_DAMAGE", 2);
            status[ItemCategories.Ember] = CreateStatusGroup("FIRE_DAMAGE", 2);
            status[ItemCategories.Glacier] = CreateStatusGroup("ICE_DAMAGE", 2);
            status[ItemCategories.Magitech] = CreateStatusGroup("LIGHTNING_DAMAGE", 2);
            status[ItemCategories.Precision] = CreateStatusGroup("CRITICAL", 400);
            status[ItemCategories.WindSong] = CreateStatusGroup("ATTACK_SPEED", 4);
            status[ItemCategories.SkySong] = CreateStatusGroup("DASH_RECOVERY_SPEED", 4);
            status[ItemCategories.Academy] = CreateStatusGroup("COOLDOWN_RECOVERY_SPEED", 4);
            status[ItemCategories.Vitality] = CreateStatusGroup("MAX_HP", 3);
            status[ItemCategories.Lake] = CreateStatusGroup("MAX_MP", 3);
            status[ItemCategories.Mystic] = CreateStatusGroup("EXP_DROP", 3);
            status[ItemCategories.Guardian] = CreateStatusGroup("DEFENSE", 3);
            status[ItemCategories.Shadow] = CreateStatusGroup("EVASION", 300);
            status[ItemCategories.Companion] = CreateStatusGroup("FOLLOWER_DAMAGE", 5);
            status[ItemCategories.Planet] = CreateStatusGroup("PLANET_DAMAGE", 5);
            status[ItemCategories.FlameSword] = CreateStatusGroup("FLAME_SWORD_DAMAGE", 4);
            status[ItemCategories.Frost] = CreateStatusGroup("FROST_RELIC_DAMAGE", 4);
            status[ItemCategories.DarkCloud] = CreateStatusGroup("DARK_CLOUD_DAMAGE", 4);
            status[ItemCategories.Stargaze] = CreateStatusGroup("INV_LEVEL", 1);
            status[ItemCategories.Drunk] = CreateStatusGroup("FINAL_DAMAGE", 1);
            status[ItemCategories.Fortune] = CreateStatusGroup("LUCK", 3);

            status[ItemCategories.Curse] = CreateStatusGroup("DEBUFF_DAMAGE", 4);
            status[ItemCategories.Savvy] = CreateStatusGroup("NEGOTIATION", 2);
            status[ItemCategories.Elemental] = CreateStatusGroup("HIGHEST_ELEMENTAL_DAMAGE", 2);
            status[ItemCategories.Alchemy] = CreateStatusGroup("POTION_SLOT", 1);
            status[ItemCategories.Weapon] = CreateStatusGroup("FINAL_WEAPONDAMAGE", 2);
            status[ItemCategories.Grimoire] = CreateStatusGroup("MAGIC_DAMAGE_BONUS", 4);

            foreach (var pair in status)
            {
                keys.Add(pair.Key, list.Count);
                list.Add(pair.Value);
                var value = pair.Value.valuesByLevel[0];
                if (pair.Value.statusID == "INV_LEVEL" || pair.Value.statusID == "POTION_SLOT")
                    pair.Value.valuesByLevel = new int[] { value };
                else
                    pair.Value.valuesByLevel = Enumerable.Range(1, ValiableMax + 1).Select(x => x * value).ToArray();
            }
        }
        private bool start = false;
        private void Start()
        {
            if (start)
                return;
            if (isOwned)
            {
                assignedCategory.Clear();
            }
            Events.OnValueRecieved += OnValueRecieved;
            start = true;
        }
        private void OnValueRecieved(string command, uint netId, int value)
        {
            //Core.Logger("OnValueRecieved: " + netId + " to " + base.netId);
            if (netId == base.netId)
            {
                if(value == -1)
                {
                    indexesClient.Clear();
                    if (Core.LogMany)
                        Core.Logger("OnValueRecieved: Clear");
                }
                else
                {
                    indexesClient.Add(value);
                    if (Core.LogMany)
                        Core.Logger("OnValueRecieved: Add " + list[value].statusID);
                }
            }
        }
        public void OnDestroy()
        {
            Events.OnValueRecieved -= OnValueRecieved;
        }
        public override int GetEffectStringCount()
        {
            if (indexesClient.Count == 0)
                return list.Count + effectsString.Length;
            else
                return indexesClient.Count + effectsString.Length;
        }

        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if(indexesClient.Count == 0)
            {
                var stats = list.ToArray();
                if (idx < effectsString.Length)
                {
                    return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
                }

                idx -= effectsString.Length;
                StatusInstance statusInstance2 = StatusDatabase.CreateStatusEntity(stats[idx].statusID, stats[idx].valuesByLevel.SafeRandomAccess(level));
                if (stats[idx].hideIfStatValueIsZero && statusInstance2.Value == 0)
                {
                    return null;
                }

                Color color2 = statusInstance2.Value < 0 ? GetNegativeColor(virtualLevelOffset) : GetPositiveColor(virtualLevelOffset);
                return statusInstance2.ToString(reverse: true, color: false, sprite: true, color2);
            }
            else
            {
                if (idx < effectsString.Length)
                {
                    if (idx == 0 || idx == 1)
                        return null;
                    return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
                }

                idx -= effectsString.Length;
                StatusInstance statusInstance2 = StatusDatabase.CreateStatusEntity(list[indexesClient[idx]].statusID, list[indexesClient[idx]].valuesByLevel.SafeRandomAccess(level));
                if (list[indexesClient[idx]].hideIfStatValueIsZero && statusInstance2.Value == 0)
                {
                    return null;
                }

                Color color2 = statusInstance2.Value < 0 ? GetNegativeColor(virtualLevelOffset) : GetPositiveColor(virtualLevelOffset);
                return statusInstance2.ToString(reverse: true, color: false, sprite: true, color2);
            }
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (!string.IsNullOrEmpty(category) && !assignedCategory.Contains(category))
                assignedCategory.Add(category);
            //NetworkAvatar.Inventory.OnInventoryStorageChangedClientside += OnInventoryStorageChangedClientside;
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }
                instances = null;
            }


            instances = new StatusInstance[indexes.Count];
            for (int j = 0; j < instances.Length; j++)
            {
                instances[j] = StatusDatabase.CreateStatusEntity(list[indexes[j]].statusID, list[indexes[j]].valuesByLevel.SafeRandomAccess(CurrentLevelToIdx()));
                instances[j].SetTarget(NetworkAvatar);
                instances[j].ApplyStatus(fromRuntime: true);
            }
        }


        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }

                instances = null;
            }
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }
                instances = null;
            }

            instances = new StatusInstance[indexes.Count];
            for (int j = 0; j < instances.Length; j++)
            {
                instances[j] = StatusDatabase.CreateStatusEntity(list[indexes[j]].statusID, list[indexes[j]].valuesByLevel.SafeRandomAccess(LevelToIdx(newLevel)));
                instances[j].SetTarget(NetworkAvatar);
                instances[j].ApplyStatus(fromRuntime: true);
            }
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!active && indexes.Count > 0)
                return;
            active = false;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    var pos = new ItemPosition(Item.XIdx + x, Item.YIdx + y);

                    NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(pos);
                    if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled || newItemOwnInstance.EntityID == Item.EntityID)
                        continue;

                    var instanceId = newItemOwnInstance.InstanceID;
                    int rarity = newItemOwnInstance.Entity.rarity switch
                    {
                        EItemRarity.Common => 1,
                        EItemRarity.Uncommon => 2,
                        EItemRarity.Rare => 3,
                        EItemRarity.Legend => 5,
                        EItemRarity.Eternal => 10,
                        _ => 1,
                    };
                    Activate(newItemOwnInstance.Charm.GetItemCategory().ToList(), rarity + UnityEngine.Random.Range(0, 3));

                    using (new GridInventory.Permission(Inventory))
                    {
                        Inventory.ForceRemoveItem(pos.x, pos.y);
                        Inventory.AddItemAtPosition(new ItemMetadata(instanceId, Item.EntityID, 1), pos);
                    }
                    return;
                }
            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (indexes.Count > 0)
                return;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    var pos = new ItemPosition(Item.XIdx + x, Item.YIdx + y);

                    NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(pos);
                    if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled || newItemOwnInstance.EntityID == Item.EntityID)
                        continue;

                    active = true;
                    return;
                }
            }
        }
        private void OnItemUpdated()
        {
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }

                instances = null;
            }

            instances = new StatusInstance[indexes.Count];
            for (int j = 0; j < instances.Length; j++)
            {
                instances[j] = StatusDatabase.CreateStatusEntity(list[indexes[j]].statusID, list[indexes[j]].valuesByLevel.SafeRandomAccess(CurrentLevelToIdx()));
                instances[j].SetTarget(NetworkAvatar);
                instances[j].ApplyStatus(fromRuntime: true);
            }
        }
        private void Activate(List<string> categories, int count)
        {
            indexes.Clear();
            Events.CommandValue(NetworkAvatar, Item, -1);
            if (categories.Count > 0)
            {
                category = categories.GetRandom();
                if (IsEffectEnabled && !assignedCategory.Contains(category))
                    assignedCategory.Add(category);
            }
            var l = Enumerable.Range(0, list.Count).ToList();
            for(int q = 0; q < count; q++)
            {
                if (l.Count <= 0)
                    break;
                if(categories.Count > q && keys.ContainsKey(categories[q]))
                {
                    var index = keys[categories[q]];
                    indexes.Add(index);
                    Events.CommandValue(NetworkAvatar, Item, index);
                    //l.Remove(index);
                }
                else
                {
                    var random = l.GetRandom();
                    indexes.Add(random);
                    Events.CommandValue(NetworkAvatar, Item, random);
                    //l.Remove(random);
                }
            }
            SaveItemOnServer(SaveManager.CurrentRun);

            OnItemUpdated();
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_InventoryPower_{Item.InstanceID}_Stack", indexes.Count);
            for(int q= 0; q < indexes.Count; q++)
            {
                saveData.SetInt($"CharmSaveData_InventoryPower_{Item.InstanceID}_Stack" + q, indexes[q]);
            }
            if(category != null)
            {
                saveData.SetString($"CharmSaveData_InventoryPower_{Item.InstanceID}_Category", category);
            }
        }
        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            if (!start)
                Start();

            var count = saveData.GetInt($"CharmSaveData_InventoryPower_{Item.InstanceID}_Stack", 0);
            indexes.Clear();
            if (NetworkClient.active)
            {
                indexesClient.Clear();
            }
            else
            {
                Events.CommandValue(NetworkAvatar, Item, -1);
            }
            for (int q = 0; q < count; q++)
            {
                indexes.Add(saveData.GetInt($"CharmSaveData_InventoryPower_{Item.InstanceID}_Stack" + q, -1));
                if (indexes[^1] == -1)
                {
                    indexes.RemoveAt(indexes.Count - 1);
                }
                else
                {
                    if (NetworkClient.active)
                    {
                        indexesClient.Add(indexes[^1]);
                    }
                    else
                    {
                        Events.CommandValue(NetworkAvatar, Item, indexes[^1]);
                    }
                }
            }
            var cate = saveData.GetString($"CharmSaveData_InventoryPower_{Item.InstanceID}_Category", null);
            if (!string.IsNullOrEmpty(cate) && !assignedCategory.Contains(cate))
            {
                category = cate;
                if (IsEffectEnabled)
                    assignedCategory.Add(cate);
            }
        }

        public override bool Weaved()
        {
            return true;
        }


        public override int GetSubIconCount()
        {
            return 1;
        }

        public override Sprite GetSubIconImage(ItemPosition pos, bool isInstance, int idx)
        {
            if (!isInstance)
            {
                return null;
            }

            if (!NetworkAvatar)
            {
                return null;
            }

            foreach(var item in assignedCategory)
            {
                foreach(var category in ItemDatabase.GetAllItemCategory())
                {
                    if(item == category.id)
                    {
                        return category.categoryIcon;
                    }
                }
            }
            if(indexesClient.Count > 0)
            {
                return CustomSpriteAsset.NoneCategoryIcon;
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return new Vector2(4f, -4f);
        }
    }
}
