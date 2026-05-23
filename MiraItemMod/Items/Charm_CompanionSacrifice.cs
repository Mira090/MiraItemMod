using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_CompanionSacrifice : Charm_StatusInstance
    {
        public static LocalizedString Notice = new LocalizedString("Item_Companion_Sacrifice_Notice");
        public Sprite iconSprite;
        public int[] healPercent = new int[] { 10, 20, 30, 40, 50 };
        public bool ability = false;
        private int defense = 0;

        public int[] followerDamage = new int[] { 20, 40, 60, 80, 120 };
        public int[] followerDamage2 = new int[] { 50, 75, 100, 120, 160 };
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        public void Start()
        {
            iconSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "Companion_Charm");
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? healPercent.SafeRandomAccess(0) + "→" + healPercent.SafeRandomAccess(maxLevel) : healPercent.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? followerDamage.SafeRandomAccess(0) + "→" + followerDamage.SafeRandomAccess(maxLevel) : followerDamage.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value3 = showAllLevel ? followerDamage2.SafeRandomAccess(0) + "→" + followerDamage2.SafeRandomAccess(maxLevel) : followerDamage2.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
                new Loc.KeywordValue("HEAL", value + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("FOLLOWER1", "+" + value2 + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("FOLLOWER2", "+" + value3 + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnDamagedServerside += Avatar_OnDamagedServerside;
            defense = NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction);
            EnableDefenseEffects();
        }
        private void EnableDefenseEffects()
        {
            if (defense <= -25)
            {

            }
            if (defense <= -50)
            {
                NetworkAvatar.AddCustomStatUnsafe("FollowerDamage", followerDamage.SafeRandomAccess(CurrentLevelToIdx()));
            }
            if (defense <= -75)
            {

            }
            if (defense <= -100)
            {
                NetworkAvatar.AddCustomStatUnsafe("FollowerDamage", followerDamage2.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnDamagedServerside -= Avatar_OnDamagedServerside;
            DisableDefenseEffects();
        }
        private void DisableDefenseEffects()
        {
            if (defense <= -25)
            {

            }
            if (defense <= -50)
            {
                NetworkAvatar.AddCustomStatUnsafe("FollowerDamage", -followerDamage.SafeRandomAccess(CurrentLevelToIdx()));
            }
            if (defense <= -75)
            {

            }
            if (defense <= -100)
            {
                NetworkAvatar.AddCustomStatUnsafe("FollowerDamage", -followerDamage2.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        private void Avatar_OnDamagedServerside(DamageInstance damage)
        {
            if (NetworkAvatar.hp <= 0f && !ability)
            {

                NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx - 1, Item.YIdx));

                if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled)
                    return;
                if (newItemOwnInstance.Charm is ICompanionCharm companion)
                {
                    ability = true;
                    SaveItemOnServer(SaveManager.CurrentRun);
                    if ((bool)DungeonManager.Instance)
                    {
                        DungeonManager.Instance.Chat(NetworkAvatar as PlayerAvatar, "Mod", "/sacrifice");
                    }

                    NetworkAvatar.Networkhp = 0f;
                    NetworkAvatar.HealPercent(healPercent.SafeRandomAccess(CurrentLevelToIdx()));
                    NetworkAvatar.StartReviveInvulnerable();
                    using (new GridInventory.Permission(Inventory))
                    {
                        Inventory.ForceRemoveItem((sbyte)(xIdx - 1), yIdx);
                    }
                }
            }
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (ability && idx == 3)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetBool($"CharmSaveData_CompanionSacrifice_{Item.InstanceID}_Stack", ability);
        }
        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            ability = saveData.GetBool($"CharmSaveData_CompanionSacrifice_{Item.InstanceID}_Stack", false);
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            Inventory.UpdatePing(Item.Position);
            var d = NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction);
            if(d != defense)
            {
                DisableDefenseEffects();
                defense = d;
                EnableDefenseEffects();
            }
        }
        public override CharmConnectionData[] GetConnectedCharmPositions()
        {
            if (!IsEffectEnabled)
            {
                return null;
            }

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition((sbyte)(Item.XIdx + 1), Item.YIdx));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled)
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(iconSprite, new Vector2Int(Item.XIdx - 1, Item.YIdx))
                };
            }
            if (newItemOwnInstance.Charm is ICompanionCharm)
            {

                return null;
            }
            else
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(iconSprite, new Vector2Int(Item.XIdx - 1, Item.YIdx))
                };
            }
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

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(pos.x - 1, pos.y));
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled)
            {
                if (newItemOwnInstance.Charm is ICompanionCharm companion)
                    return companion.GetCompanionIcon(newItemOwnInstance.Charm.CurrentLevelToIdx());
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return new Vector2(4f, -4f);
        }
    }
}
