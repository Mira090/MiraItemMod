using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_AddStarRuby : Charm_StatusInstance
    {
        private int rewardId = 1123;

        private int count;
        private int countRequire = 3;

        private bool questCleared;

        private bool rewardReceived;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            //var maxHP = avatar.MaxHp;
            //string value = (showAllLevel ? (damageByLevel.SafeRandomAccess(0) + "→" + damageByLevel.SafeRandomAccess(maxLevel)) : damageByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());
            //string value2 = (showAllLevel ? (Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(0)) + "→" + Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(maxLevel))) : Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(LevelToIdx(level))).ToString());
            return new Loc.KeywordValue[3]
            {
                new Loc.KeywordValue("QUEST", countRequire.ToString()),
            new Loc.KeywordValue("REWARD", new LocalizedString("Item_FinalHP_Name").ToString()),
            new Loc.KeywordValue("CURRENT", count.ToString(), GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (questCleared && !rewardReceived)
            {
                ItemPosition pos = new ItemPosition(Item.XIdx, Item.YIdx);
                GridInventory inventory = Inventory;
                int instanceID = Item.InstanceID;
                var random = new System.Random(instanceID);

                using (new GridInventory.Permission(inventory))
                {
                    inventory.ForceRemoveItem(Item.XIdx, Item.YIdx);
                }

                //inventory.AddItem(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), rewardId, 1));
                //inventory.AddItem(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), rewardId, 1));
                inventory.AddItem(new ItemMetadata(instanceID, rewardId, 1));

                rewardReceived = true;
            }
        }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.OnDrinkPotion += OnDrinkPotion;
        }

        private void OnDrinkPotion(PotionEffect obj)
        {
            if (questCleared)
                return;
            count++;
            questCleared = count >= countRequire;
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count}/{countRequire}");
            NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
            SaveItemOnServer(SaveManager.CurrentRun);
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.OnDrinkPotion -= OnDrinkPotion;
        }

        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_AddStarRuby_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_AddStarRuby_{Item.InstanceID}_Stack", 0);
        }
    }
}
