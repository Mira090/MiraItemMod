using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_Kill_Luck : Charm_StatusInstance
    {
        public int[] luckByLevel = new int[4] { 1, 1, 1, 2 };
        private int count;
        private int countView;
        private int divide = 5;
        public int max = 20;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? luckByLevel.SafeRandomAccess(0) + "→" + luckByLevel.SafeRandomAccess(maxLevel) : luckByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("LUCK", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", "+" + (showAllLevel ? GetLuck(maxLevel, countView).ToString() : GetLuck(LevelToIdx(level), countView).ToString()), GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", countView.ToString(), GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DIVIDE", divide.ToString()),
            new Loc.KeywordValue("MAX", "+" + max.ToString())
            };
        }
        public void Start()
        {
            Events.OnValueRecieved += OnValueRecieved;
        }
        public void OnDestroy()
        {
            Events.OnValueRecieved -= OnValueRecieved;
        }

        private void OnValueRecieved(string command, uint netId, int value)
        {
            if (netId == base.netId)
            {
                countView = value;
            }
        }

        private int GetLuck(int idx, int count)
        {
            return Mathf.Clamp(luckByLevel.SafeRandomAccess(idx) * (count / divide), 0, max);
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, GetLuck(CurrentLevelToIdx(), count));
            networkAvatar.OnKillUnit += OnKillUnit;
            //networkAvatar.OnStartBattle += OnStartBattle;
        }


        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -GetLuck(CurrentLevelToIdx(), count));
            networkAvatar.OnKillUnit -= OnKillUnit;
            //networkAvatar.OnStartBattle -= OnStartBattle;
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -GetLuck(LevelToIdx(oldLevel), count));
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, GetLuck(LevelToIdx(newLevel), count));
        }
        protected void OnKillUnit(UnitAvatar avatar, DamageInstance damage)
        {
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -GetLuck(CurrentLevelToIdx(), count));
            count++;
            countView = count;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, GetLuck(CurrentLevelToIdx(), count));
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        protected void OnStartBattle()
        {
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -GetLuck(CurrentLevelToIdx(), count));
            count = 0;
            countView = count;
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_KillLuck_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_KillLuck_{Item.InstanceID}_Stack", 0);
            countView = count;
        }
    }
}
