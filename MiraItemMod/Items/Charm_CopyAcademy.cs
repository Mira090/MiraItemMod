using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_CopyAcademy : Charm_StatusInstance
    {
        public LocalizedString semanticName = new LocalizedString("ItemType_Charm_Magic");

        public string semantic = "GRIMOIRE";

        public int semanticDropWeight = 2;

        private int count;
        private int countView;
        private int countRequire = 20;

        private bool questCleared;
        private bool rewardReceived;
        public Sprite magicCharmIconSprite;

        public SkillController skillController;
        private void Awake()
        {
            effectHUD_ID = "Copy_Academy".ToSephiriaUpperId();
        }
        public void Start()
        {
            magicCharmIconSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "Academy_Charm");
            Events.OnValueRecieved += OnValueRecieved;
        }
        public void OnDestroy()
        {
            Events.OnValueRecieved -= OnValueRecieved;
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            //var maxHP = avatar.MaxHp;
            //string value = (showAllLevel ? (levelByLevel.SafeRandomAccess(0) + "→" + levelByLevel.SafeRandomAccess(maxLevel)) : levelByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());
            //string value2 = (showAllLevel ? (Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(0)) + "→" + Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(maxLevel))) : Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(LevelToIdx(level))).ToString());
            return new Loc.KeywordValue[4]
            {
            new Loc.KeywordValue("ITEM_TYPE", semanticName.ToString()),
            new Loc.KeywordValue("DROP_PERCENT", (semanticDropWeight * 50).ToString()),
                new Loc.KeywordValue("QUEST", countRequire.ToString()),
            //new Loc.KeywordValue("REWARD",value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", countView.ToString(), GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            if (NetworkAvatar is PlayerAvatar player)
            {
                skillController = player.GetSkillController();
            }
            //networkAvatar.OnMpUsedServerside += OnMpUsedServerside;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight);
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{countView}/{countRequire}");

            if (skillController != null)
            {
                skillController.OnCreateMagicServerside += OnCreateMagicServerside;
            }
        }

        private void OnValueRecieved(string command, uint netId, int value)
        {
            //Core.Logger("OnValueRecieved: " + netId + " to " + base.netId);
            if(netId == base.netId)
            {
                countView = value;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight);

            if (skillController != null)
            {
                skillController.OnCreateMagicServerside -= OnCreateMagicServerside;
            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            Inventory.UpdatePing(Item.Position);
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
                var item = inventory.FindItem(new ItemPosition(pos.x, pos.y + 1));
                if (item != null && (bool)item.Charm && item.Charm.IsEffectEnabled && !item.Charm.isUniqueEffect && item.Entity.categories.Contains(ItemCategories.Academy))
                {
                    //Core.Logger("add: " + item.Entity.aName.ToString());
                    inventory.AddItemAtPosition(new ItemMetadata(instanceID, item.Entity, 1), pos);
                }
                rewardReceived = true;
            }
        }

        private void OnCreateMagicServerside(ActiveSkill skill)
        {
            if (questCleared)
                return;
            if (NetworkAvatar.IsDead || !NetworkAvatar.IsInBattle || !IsEffectEnabled)
                return;
            if (GetConnectedCharmPositions() != null)
                return;
            count++;
            countView = count;
            Events.CommandValue(NetworkAvatar, Item, count);
            questCleared = count >= countRequire;
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count}/{countRequire}");
            NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
            SaveItemOnServer(SaveManager.CurrentRun);
        }

        private void OnMpUsedServerside(int mp)
        {
            if (questCleared)
                return;
            if (NetworkAvatar.IsDead || !NetworkAvatar.IsInBattle || !IsEffectEnabled)
                return;
            if (GetConnectedCharmPositions() != null)
                return;
            count += mp;
            questCleared = count >= countRequire;
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count}/{countRequire}");
            NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
            SaveItemOnServer(SaveManager.CurrentRun);
        }

        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_CopyAcademy_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_CopyAcademy_{Item.InstanceID}_Stack", 0);
            countView = count;
            Events.CommandValue(NetworkAvatar, Item, countView);
        }
        public override CharmConnectionData[] GetConnectedCharmPositions()
        {
            if (!IsEffectEnabled)
            {
                return null;
            }

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx, (sbyte)(Item.YIdx + 1)));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled || newItemOwnInstance.Charm.isUniqueEffect || !newItemOwnInstance.Entity.categories.Contains(ItemCategories.Academy))
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(magicCharmIconSprite, new Vector2Int(Item.XIdx, Item.YIdx + 1))
                };
            }

            return null;
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

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(pos.x, (sbyte)(pos.y + 1)));
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled && !newItemOwnInstance.Charm.isUniqueEffect && newItemOwnInstance.Entity.categories.Contains(ItemCategories.Academy))
            {
                return newItemOwnInstance.Entity.Icon;
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return new Vector2(4f, -4f);
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
