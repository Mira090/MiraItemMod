using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_ReviveHeal : Charm_StatusInstance
    {
        public int[] defense = new int[] { -5 };
        public int[] heal = new int[] { 1, 2, 3, 5 };
        private int count = 0;
        private int countView = 0;
        private bool quest = false;
        public int countMax = 3;
        public PlayerAvatar EventPlayer;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            //var maxHP = avatar.MaxHp;
            string value = showAllLevel ? defense.SafeRandomAccess(0) + "→" + defense.SafeRandomAccess(maxLevel) : defense.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? heal.SafeRandomAccess(0) + "→" + heal.SafeRandomAccess(maxLevel) : heal.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[4]
            {
            new Loc.KeywordValue("DEFENSE", value),
            new Loc.KeywordValue("HEAL", value2, GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("QUEST", countMax.ToString()),
            //new Loc.KeywordValue("REWARD",value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", countView.ToString())
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        public void Start()
        {
            if (NetworkAvatar != null && NetworkAvatar is PlayerAvatar player)
            {
                EventPlayer = player;
                EventPlayer.OnReviveFromOtherPlayerClientside += OnReviveFromOtherPlayerClientside;
            }
        }
        public void OnDestroy()
        {
            if (EventPlayer != null)
            {
                EventPlayer.OnReviveFromOtherPlayerClientside -= OnReviveFromOtherPlayerClientside;
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnRevive += OnRevive;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (quest)
            {
                ItemPosition pos = new ItemPosition(Item.XIdx, Item.YIdx);
                GridInventory inventory = Inventory;
                int instanceID = Item.InstanceID;
                var random = new System.Random(instanceID);

                using (new GridInventory.Permission(inventory))
                {
                    inventory.ForceRemoveItem(Item.XIdx, Item.YIdx);
                }
            }
        }
        private void OnRevive()
        {
            var amount = Mathf.Max(0, NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction) / defense.SafeRandomAccess(CurrentLevelToIdx()) * heal.SafeRandomAccess(CurrentLevelToIdx()));
            if (amount > 0)
            {
                count++;
                countView = count;
                quest = countMax <= count;
            }
        }

        private void OnReviveFromOtherPlayerClientside(PlayerAvatar doctor, PlayerAvatar patient)
        {
            if (patient != NetworkAvatar)
                return;
            var amount = Mathf.Max(0, NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction) / defense.SafeRandomAccess(CurrentLevelToIdx()) * heal.SafeRandomAccess(CurrentLevelToIdx()));
            if (amount > 0)
            {
                doctor.CmdHeal(amount);
                countView++;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnRevive -= OnRevive;
            if (NetworkAvatar is PlayerAvatar player)
            {
                player.OnReviveFromOtherPlayerClientside -= OnReviveFromOtherPlayerClientside;
            }
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_RiviveHeal_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_RiviveHeal_{Item.InstanceID}_Stack", 0);
            countView = count;
        }
    }
}
