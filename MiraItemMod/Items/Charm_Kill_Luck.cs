using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_Kill_Luck : Charm_StatusInstance
    {

        public int[] luckByLevel = new int[4] { 1, 1, 1, 2 };
        private int count;
        private int countView;
        private int divide = 4;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? luckByLevel.SafeRandomAccess(0) + "→" + luckByLevel.SafeRandomAccess(maxLevel) : luckByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[4]
            {
            new Loc.KeywordValue("LUCK", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", "+" + (showAllLevel ? (countView / divide * luckByLevel.SafeRandomAccess(maxLevel)).ToString() : (countView / divide * luckByLevel.SafeRandomAccess(LevelToIdx(level))).ToString()), GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", countView.ToString(), GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DIVIDE", divide.ToString())
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

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, luckByLevel.SafeRandomAccess(CurrentLevelToIdx()) * (count / divide));
            networkAvatar.OnKillUnit += OnKillUnit;
            //networkAvatar.OnStartBattle += OnStartBattle;
        }


        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -luckByLevel.SafeRandomAccess(CurrentLevelToIdx()) * (count / divide));
            networkAvatar.OnKillUnit -= OnKillUnit;
            //networkAvatar.OnStartBattle -= OnStartBattle;
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -luckByLevel.SafeRandomAccess(LevelToIdx(oldLevel)) * (count / divide));
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, luckByLevel.SafeRandomAccess(LevelToIdx(newLevel)) * (count / divide));
        }
        protected void OnKillUnit(UnitAvatar avatar, DamageInstance damage)
        {
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -luckByLevel.SafeRandomAccess(CurrentLevelToIdx()) * (count / divide));
            count++;
            countView = count;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, luckByLevel.SafeRandomAccess(CurrentLevelToIdx()) * (count / divide));
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        protected void OnStartBattle()
        {
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -luckByLevel.SafeRandomAccess(CurrentLevelToIdx()) * (count / divide));
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
